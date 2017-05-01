using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumNoteMap : ScriptableObject {
	
	private static DrumNoteMap s_internalInstance;

	[System.Serializable]
	public class NoteOutSet
	{
		public string Name;
		public bool Included = true;
		public List<int> Notes;
	}
	public static string Active = "DrumNoteMapDB";//"DrumNoteMapDB_BigMono";
	public List<NoteOutSet> NoteMap;
	protected System.Random m_rnd;

	public static DrumNoteMap Instance
	{
		get{
			if (s_internalInstance == null)
			{
				s_internalInstance = Resources.Load<DrumNoteMap>(Active);
				if(s_internalInstance == null)
				{
					Debug.LogError("DrumNoteMapDB could not be loaded");
					return null;
				}
				s_internalInstance.m_rnd = new System.Random(System.DateTime.Now.Millisecond);
			}
			return s_internalInstance;
		}
	}

	public int GetNumTotalNotes()
	{
		int total = 0;
		int len = NoteMap.Count;
		for (int i = 0; i < len; i++)
		{
			total += NoteMap[i].Included ? 1 : 0;
		}
		return total;
	}

	private NoteOutSet GetNoteSet(int in_note)
	{
		int len = GetNumTotalNotes();
		in_note = in_note % len;

		int mapTotal = NoteMap.Count;
		for (int i = 0; i < mapTotal; i++)
		{
			if (NoteMap[i].Included)
				if (in_note-- == 0)
					return NoteMap[i];
		}
		return null;
	}

	public int Map(int in_note)
	{
		var noteSet = GetNoteSet(in_note);
		if (noteSet != null)
		{
			return noteSet.Notes[0];
//			return noteSet.Notes.GetRandomNoRepeat(m_rnd);
		}
		return in_note;
	}
}
