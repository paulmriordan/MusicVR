using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CSharpSynth.Midi;
using CSharpSynth.Sequencer;
using MusicVR.Scales;

namespace MusicVR.Composition
{
	[System.Serializable]
	public class InstrumentData
	{
		public int NumRows = 100;
		[SerializeField] E_ConverterType m_scale = E_ConverterType.Pentatonic;
		[SerializeField] int InstrumentDefinitionIndex = 0;
		private bool[] m_buttonData;

		public int StartRow { get; private set;}
		public int IndexInComposition { get; private set;}

		public E_ConverterType Scale { 
			get {
				if (InstrumentDefintion.IsDrum)
					return E_ConverterType.Drum;
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
}