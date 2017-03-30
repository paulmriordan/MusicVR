﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CompositionData
{
	[System.Serializable]
	public class InstrumentData
	{
		public int NumRows = 100;
		[SerializeField] int InstrumentDefinitionIndex = 0;
		public MusicScaleConverter.E_ConverterType Scale = MusicScaleConverter.E_ConverterType.Pentatonic;
		public InstrumentDefinitions.Instrument InstrumentDefintion { get {
				return InstrumentDefinitions.Instance.Get(InstrumentDefinitionIndex);
			}
		}

		private bool[] m_buttonData;

		public void Init(int numCols)
		{
			m_buttonData = new bool[numCols*NumRows];
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
	}

	public List<InstrumentData> InstrumentDataList;
	public int NumCols = 20;
	public float Tempo = 120.0f;
	public int NumRows { get; private set;}
	public int Size { get {
			return NumRows * NumCols;
		}}

	public void Init()
	{
		NumRows = 0;
		for (int i = 0; i < InstrumentDataList.Count; i++)
		{
			InstrumentDataList[i].Init(NumCols);
			NumRows += InstrumentDataList[i].NumRows;
		}
	}
	public void Clear()
	{
		for (int i = 0; i < InstrumentDataList.Count; i++)
			InstrumentDataList[i].Clear();
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

}