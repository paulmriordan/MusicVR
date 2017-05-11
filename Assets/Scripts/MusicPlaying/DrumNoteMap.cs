using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object for mapping wall button index to drum note.
/// The drum sfx files have ~100 drums, most of which are not required.
/// This script allows us to choose which drum note to include on the wall
/// </summary>
public class DrumNoteMap : ScriptableObject {

	public static string Active = "DrumNoteMapDB";//"DrumNoteMapDB_BigMono"; 

	private static DrumNoteMap s_internalInstance;

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

	[System.Serializable]
	public class NoteOutSet
	{
		public string Name;
		public bool Included = true;
		public List<int> Notes;
	}
}
