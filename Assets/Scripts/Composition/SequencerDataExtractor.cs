using System.Collections.Generic;
using CSharpSynth.Midi;
using CSharpSynth.Sequencer;
using MusicVR.Scales;

namespace MusicVR.Composition
{
	public class SequencerDataExtractor
	{
		CompositionData 		m_data;
		uint 					m_cumDeltaTime = 0;
		bool 					m_firstNoteAddedForColumn = false;
		List<MidiEvent> 		m_events = new List<MidiEvent>();
		List<MidiEvent> 		m_lastColumnsEvents = new List<MidiEvent>();

		public SequencerDataExtractor(CompositionData inData)
		{
			m_data = inData;
		}

		public ISequencerData GetSequencerData()
		{
			m_cumDeltaTime = 0;
			m_events.Clear();
			m_lastColumnsEvents.Clear();

			ISequencerData seqData = new ManualSequencerData();
			seqData.DeltaTiming = m_data.DeltaTiming;
			seqData.BeatsPerMinute = (uint)m_data.Tempo;
			seqData.TotalTime = (ulong)(m_data.DeltaTimeSpacing*(float)m_data.NumCols);
			seqData.Events = GetEvents();
			seqData.EventCount = seqData.Events.Length;
			return seqData;
		}

		private MidiEvent[] GetEvents()
		{
			for (int iCol = 0; iCol < m_data.NumCols; iCol++)
			{
				m_firstNoteAddedForColumn = false;

				EndPreviousNotes();

				AddNewNotes(iCol);

				m_cumDeltaTime += (uint)m_data.DeltaTimeSpacing;
			}

			return m_events.ToArray();
		}

		private void EndPreviousNotes()
		{
			int lastColCount = m_lastColumnsEvents.Count;
			for (int iEvent = 0; iEvent < lastColCount; iEvent++)
			{
				var lastEvent = m_lastColumnsEvents[iEvent];
				m_events.Add(CreateNoteOffEvent(lastEvent));
				m_cumDeltaTime = 0; //used, other events this frame are at same time 
				m_firstNoteAddedForColumn = true;
			}
			m_lastColumnsEvents.Clear();
		}

		private MidiEvent CreateNoteOffEvent(MidiEvent lastEvent)
		{
			return new MidiEvent()
			{
				deltaTime = m_firstNoteAddedForColumn ? 0 : m_cumDeltaTime,
				midiChannelEvent = MidiHelper.MidiChannelEvent.Note_Off,
				parameter1 = lastEvent.parameter1,
				parameter2 = lastEvent.parameter2,
				channel = lastEvent.channel
			};
		}

		private void AddNewNotes(int iCol)
		{
			int numInstruments = m_data.InstrumentDataList.Count;

			for (int iInst = 0; iInst < numInstruments; iInst++)
			{
				var instrument = m_data.InstrumentDataList[iInst];
				for (int iRow = 0; iRow < instrument.NumRows; iRow++)
				{
					if (instrument.IsNoteActive(iRow, iCol))
					{
						AddNewNote(iInst, iRow);
					}
				}
			}
		}

		private void AddNewNote(int iInstrument, int iRow)
		{
			var instrument = m_data.InstrumentDataList[iInstrument];
			int eventNote = ScaleConverter.Convert(instrument.Scale, iRow);
			int eventChannel = instrument.InstrumentDefinition.IsDrum ? 9 : iInstrument;
			eventNote = eventNote + instrument.InstrumentDefinition.InstrumentNoteOffset;

			var customEvent = new MidiEvent()
			{
				deltaTime = m_firstNoteAddedForColumn ? 0 : m_cumDeltaTime,
				midiChannelEvent = MidiHelper.MidiChannelEvent.Note_On,
				parameter1 = (byte)eventNote,
				parameter2 = (byte)instrument.InstrumentDefinition.NoteVelocity,
				channel = (byte)eventChannel
			};

			m_events.Add(customEvent);
			m_lastColumnsEvents.Add(customEvent);

			m_cumDeltaTime = 0; //used, other events this frame are at same time
			m_firstNoteAddedForColumn = true;
		}
	}
}

