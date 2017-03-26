using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CompositionData
{
	public int NumRows = 100;
	public int NumCols = 20;
	public float Tempo = 120.0f;
	public MusicScaleConverter.E_ConverterType Scale = MusicScaleConverter.E_ConverterType.Pentatonic;
	private bool[] m_buttonData;

	public int Size { get {
			if (m_buttonData == null)
				m_buttonData = new bool[NumCols * NumRows];
			return m_buttonData.Length;
		}}

	public void CreateDummyButtonData(float ProbInitSelected)
	{
		m_buttonData = new bool[NumRows*NumCols];
		UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
		for (int i = 0; i < m_buttonData.Length; i++)
			m_buttonData[i] = UnityEngine.Random.value < ProbInitSelected;
	}

	public bool IsNoteActive(int row, int col)
	{
		return m_buttonData[row + col*NumRows];
	}
}