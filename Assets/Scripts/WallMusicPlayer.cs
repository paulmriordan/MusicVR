using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMusicPlayer : MonoBehaviour 
{
	private float m_colAccum = 0;
	private int m_prevColEffect = -1;
	private MusicWallData m_data;
	private WallButtons m_wallButtons;
	private bool m_playing;
	private List<int> m_currPlaying = new List<int>();
	private Synth m_synth;
	private GameObject m_lineInstance;

	public bool IsPlaying {get { return m_playing;}}

	void Awake()
	{
		var linePrefab = Resources.Load("WallAssets/PlayLine");
		m_lineInstance = Instantiate(linePrefab) as GameObject;
		m_lineInstance.transform.SetParent(transform);
	}

	public void Reset()
	{
		EndPreviousNotes();
		m_prevColEffect = -1;
	}

	public void Play(MusicWallData properties, WallButtons buttons, Synth synth)
	{
		m_data = properties;
		m_wallButtons = buttons;
		m_playing = true;
		m_synth = synth;
	}

	public void Stop()
	{
		m_playing = false;
	}

	void Update () 
	{
		UpdatePosition();

		int currCol = (int)m_colAccum;
		if (currCol != m_prevColEffect)
		{
			UpdateNewColEffects();
			UpdateNewColAudio();
		}
	}

	void UpdatePosition()
	{
		if (m_playing)
		{
			float notesPerSec = (m_data.CompositionData.Tempo/60.0f);
			m_colAccum += Time.deltaTime*notesPerSec;
			m_colAccum = Mathf.Repeat(m_colAccum, (float)m_data.CompositionData.NumCols);
			float frac = m_colAccum/(float)m_data.CompositionData.NumCols;
			Vector3 pos;
			pos = m_data.GetPositionAtAngle(frac * 2.0f * Mathf.PI);
			var h = m_data.GetTotalHeight();
			pos.y = h * 0.5f - m_data.GetButtonWidth()*0.5f;
			m_lineInstance.transform.localPosition = pos;
			m_lineInstance.transform.localScale = new Vector3(m_lineInstance.transform.localScale.x, h - 2.0f * m_data.GetButtonWidth(), 0.01f);

			Vector3 forward;
			forward = m_data.GetPositionAtAngle(frac * 2.0f * Mathf.PI + transform.localRotation.eulerAngles.y * Mathf.Deg2Rad);
			m_lineInstance.transform.forward = forward;
		}
	}

	void UpdateNewColEffects()
	{
		int currCol = (int)m_colAccum;

		if (m_prevColEffect != -1)
		{
			for (int iRow = 0; iRow < m_data.CompositionData.NumRows; iRow++)
			{
				var button = m_wallButtons.GetButton(iRow, m_prevColEffect);
				button.SetPlaying(false);
			}
		}
		for (int iRow = 0; iRow < m_data.CompositionData.NumRows; iRow++)
		{
			var button = m_wallButtons.GetButton(iRow, currCol);
			button.SetPlaying(true);
		}
		m_prevColEffect = currCol;
	}

	void UpdateNewColAudio()
	{
		int currCol = (int)m_colAccum;
		EndPreviousNotes();

		int numInstruments = m_data.CompositionData.InstrumentDataList.Count;
		int rowCum = 0;
		for (int iInst = 0; iInst < numInstruments; iInst++)
		{
			var instrument = m_data.CompositionData.InstrumentDataList[iInst];
			for (int iRow = 0; iRow < instrument.NumRows; iRow++)
			{
				var button = m_wallButtons.GetButton(iRow + rowCum, currCol);
				if (button.IsSelected)
				{
					int note = MusicScaleConverter.Get(instrument.Scale).Convert(iRow);
					m_synth.NoteOn(1, note + instrument.InstrumentNoteOffset, instrument.Instrument);
					m_currPlaying.Add(note);
				}
			}
			rowCum += instrument.NumRows;
		}
	}

	void EndPreviousNotes()
	{
		if (m_prevColEffect != -1)
		{
			for (int iNote = 0; iNote < m_currPlaying.Count; iNote++)
			{
				int note = m_currPlaying[iNote];
				m_synth.NoteOff(1, note);
			}
		}
		m_currPlaying.Clear();
	}
}
