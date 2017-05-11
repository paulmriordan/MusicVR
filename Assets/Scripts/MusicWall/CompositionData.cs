using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CSharpSynth.Midi;
using CSharpSynth.Sequencer;
using CompositionCommands;

[System.Serializable]
public class CompositionData
{
	[System.Serializable]
	public class InstrumentData
	{
		public int NumRows = 100;
		[SerializeField] MusicScaleConverter.E_ConverterType m_scale = MusicScaleConverter.E_ConverterType.Pentatonic;
		[SerializeField] int InstrumentDefinitionIndex = 0;
		private bool[] m_buttonData;

		public int StartRow { get; private set;}
		public int IndexInComposition { get; private set;}

		public MusicScaleConverter.E_ConverterType Scale { 
			get {
				if (InstrumentDefintion.IsDrum)
					return MusicScaleConverter.E_ConverterType.Drum;
				return m_scale;
			}
			set {
				if (!InstrumentDefintion.IsDrum)
					m_scale = value;
			}
		}

		public InstrumentDefinitions.Instrument InstrumentDefintion { 
			get {
				return InstrumentDefinitions.Instance.Get(InstrumentDefinitionIndex);
			}
			set {
				InstrumentDefinitionIndex = InstrumentDefinitions.Instance.Instruments.FindIndex((i) => i == value);
			}
		}

		public void Init(int in_numCols, int in_startRow, int in_index)
		{
			m_buttonData = new bool[in_numCols*NumRows];
			StartRow = in_startRow;
			IndexInComposition = in_index;
		}

		public void Clear()
		{
			System.Array.Clear(m_buttonData, 0, m_buttonData.Length);
		}

		public void CreateDummyButtonData(float ProbInitSelected, int numCols)
		{
			m_buttonData = new bool[NumRows*numCols];
			for (int i = 0; i < m_buttonData.Length; i++)
				m_buttonData[i] = UnityEngine.Random.value < ProbInitSelected;
		}

		public bool IsNoteActive(int row, int col)
		{
			return m_buttonData[row + col*NumRows];
		}

		public void SetNoteActive(int row, int col, bool active)
		{
			m_buttonData[row + col*NumRows] = active;
		}
	}


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
						int eventNote = MusicScaleConverter.Get(instrument.Scale).Convert(iRow);
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