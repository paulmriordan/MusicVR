using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMusicData 
{
	private bool[] m_buttonData;
	private WallProperties m_properties;

	public int Size { get { return m_buttonData.Length;}}

	public void CreateDummyButtonData(WallProperties properties, float ProbInitSelected)
	{
		m_properties = properties;

		m_buttonData = new bool[m_properties.NumRows*m_properties.NumCols];
		UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
		for (int i = 0; i < m_buttonData.Length; i++)
			m_buttonData[i] = UnityEngine.Random.value < ProbInitSelected;
	}

	public bool IsNoteActive(int row, int col)
	{
		return m_buttonData[row + col*m_properties.NumRows];
	}
}
