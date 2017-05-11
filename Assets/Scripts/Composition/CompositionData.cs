using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CSharpSynth.Midi;
using CSharpSynth.Sequencer;
using MusicVR.Scales;

namespace MusicVR.Composition
{
	[System.Serializable]
	public class CompositionData
	{
		[field: System.NonSerialized]
		public event System.Action OnCompositionChanged = () => {};
		public event System.Action<int,int, bool> OnNoteStateChanged = (r,c,s) => {};

		public List<InstrumentData> InstrumentDataList;
		public int NumCols = 20;
		public int Tempo = 120;
		public int DeltaTiming = 500;
		public int DeltaTimeSpacing = 500;

		public CompositionCommandManager CommandManager {get; private set;}
		public int NumRows { get; private set;}
		public int Size { get {
				return NumRows * NumCols;
			}}

		public void Init()
		{
			CommandManager = new CompositionCommandManager(this);

			NumRows = 0;
			for (int i = 0; i < InstrumentDataList.Count; i++)
			{
				InstrumentDataList[i].Init(NumCols, NumRows, i);
				NumRows += InstrumentDataList[i].NumRows;
			}
		}
		public void Clear()
		{
			for (int i = 0; i < InstrumentDataList.Count; i++)
				InstrumentDataList[i].Clear();
		}

		public void CompositionChanged()
		{
			OnCompositionChanged();
		}

		public void CreateDummyButtonData(float ProbInitSelected)
		{
			UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
			for (int i = 0; i < InstrumentDataList.Count; i++)
				InstrumentDataList[i].CreateDummyButtonData(ProbInitSelected, NumCols);
		}

		public bool IsNoteActive(int row, int col)
		{
			int rowCum = 0;
			for (int i = 0; i < InstrumentDataList.Count; i++)
			{
				if (row < rowCum + InstrumentDataList[i].NumRows)
					return InstrumentDataList[i].IsNoteActive(row - rowCum, col);
				rowCum += InstrumentDataList[i].NumRows;
			}
			Debug.LogError("row " + row + " not in music data");
			return false;
		}

		public void SetNoteActive(int row, int col, bool active)
		{
			int rowCum = 0;
			for (int i = 0; i < InstrumentDataList.Count; i++)
			{
				if (row < rowCum + InstrumentDataList[i].NumRows)
				{
					if (InstrumentDataList[i].IsNoteActive(row - rowCum, col) != active)
					{
						InstrumentDataList[i].SetNoteActive(row - rowCum, col, active);
						OnCompositionChanged();
						OnNoteStateChanged(row, col, active);
					}
					return;
				}
				rowCum += InstrumentDataList[i].NumRows;
			}
			Debug.LogError("row " + row + " not in music data");
			return;
		}

		public InstrumentData GetInstrumentAtLocation(int row, int col)
		{
			int rowCum = 0;
			for (int i = 0; i < InstrumentDataList.Count; i++)
			{
				if (row < rowCum + InstrumentDataList[i].NumRows)
					return InstrumentDataList[i];
				rowCum += InstrumentDataList[i].NumRows;
			}
			Debug.LogError("row " + row + " not in music data");
			return null;
		}


		public ISequencerData GetSequencerData()
		{
			ISequencerData seqData = new ManualSequencerData();
			seqData.DeltaTiming = DeltaTiming;
			seqData.BeatsPerMinute = (uint)Tempo;
			seqData.TotalTime = (ulong)(DeltaTimeSpacing*(float)NumCols);

			List<MidiEvent> events = new List<MidiEvent>();
			List<MidiEvent> lastColEvents = new List<MidiEvent>();
			int numInstruments = InstrumentDataList.Count;
			uint cumDeltaTime = 0;
			for (int iCol = 0; iCol < NumCols; iCol++)
			{
				bool first = true;

				int lastColCount = lastColEvents.Count;
				for (int iEvent = 0; iEvent < lastColCount; iEvent++)
				{
					var lastEvent = lastColEvents[iEvent];
					var customEvent = new MidiEvent()
					{
						deltaTime = first ? cumDeltaTime : 0,
						midiChannelEvent = MidiHelper.MidiChannelEvent.Note_Off,
						parameter1 = lastEvent.parameter1,
						parameter2 = lastEvent.parameter2,
						channel = lastEvent.channel
					};
					events.Add(customEvent);
					cumDeltaTime = 0; //used, other events this frame are at same time 
					first = false;
				}
				lastColEvents.Clear();

				for (int iInst = 0; iInst < numInstruments; iInst++)
				{
					var instrument = InstrumentDataList[iInst];
					for (int iRow = 0; iRow < instrument.NumRows; iRow++)
					{
						if (instrument.IsNoteActive(iRow, iCol))
						{
							int eventNote = ScaleConverter.Convert(instrument.Scale, iRow);
							int eventChannel = instrument.InstrumentDefintion.IsDrum ? 9 : iInst;
							eventNote = eventNote + instrument.InstrumentDefintion.InstrumentNoteOffset;
							
							var customEvent = new MidiEvent()
							{
								deltaTime = first ? cumDeltaTime : 0,
								midiChannelEvent = MidiHelper.MidiChannelEvent.Note_On,
								parameter1 = (byte)eventNote,
								parameter2 = (byte)instrument.InstrumentDefintion.NoteVelocity,
								channel = (byte)eventChannel
							};

							events.Add(customEvent);
							lastColEvents.Add(customEvent);

							cumDeltaTime = 0; //used, other events this frame are at same time
							first = false;
						}
					}
				}


				cumDeltaTime += (uint)DeltaTimeSpacing;
			}
			seqData.Events = events.ToArray();
			seqData.EventCount = seqData.Events.Length;
			return seqData;
		}
	}
}