using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MusicVR.Scales;

namespace MusicVR.Composition
{
	[System.Serializable]
	public class CompositionData
	{
		[field: System.NonSerialized]
		public event System.Action 					OnCompositionChanged = () => {};
		public event System.Action<int, int, bool> 	OnNoteStateChanged = (r,c,s) => {};

		public List<InstrumentData> 				InstrumentDataList;
		public int 									NumCols = 20;
		public int 									Tempo = 120;
		public int 									DeltaTiming = 500;
		public int 									DeltaTimeSpacing = 500;

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


	}
}