﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMusicPlayer : MonoBehaviour 
{
	private float m_colAccum = 0;
	private int m_prevColEffect = -1;
	private WallProperties m_wallProperties;
	private WallButtons m_wallButtons;
	private bool m_playing;
	private List<int> m_currPlaying = new List<int>();
	private Synth m_synth;

	public void Play(WallProperties properties, WallButtons buttons, Synth synth)
	{
		m_wallProperties = properties;
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
			float notesPerSec = (m_wallProperties.Tempo/60.0f);
			m_colAccum += Time.deltaTime*notesPerSec;
			m_colAccum = Mathf.Repeat(m_colAccum, (float)m_wallProperties.NumCols);
		}
	}

	void UpdateNewColEffects()
	{
		int currCol = (int)m_colAccum;

		if (m_prevColEffect != -1)
		{
			for (int iRow = 0; iRow < m_wallProperties.NumRows; iRow++)
			{
				var button = m_wallButtons.GetButton(iRow, m_prevColEffect);
				button.SetPlaying(false);
			}
		}
		for (int iRow = 0; iRow < m_wallProperties.NumRows; iRow++)
		{
			var button = m_wallButtons.GetButton(iRow, currCol);
			button.SetPlaying(true);
		}
		m_prevColEffect = currCol;
	}

	void UpdateNewColAudio()
	{
		int currCol = (int)m_colAccum;
		if (m_prevColEffect != -1)
		{
			for (int iNote = 0; iNote < m_currPlaying.Count; iNote++)
			{
				int note = m_currPlaying[iNote];
				m_synth.NoteOff(1, note);
			}
		}

		m_currPlaying.Clear();

		for (int iRow = 0; iRow < m_wallProperties.NumRows; iRow++)
		{
			var button = m_wallButtons.GetButton(iRow, currCol);
			if (button.IsSelected)
			{
				int note = MusicScaleConverter.Get(m_wallProperties.Scale).Convert(iRow);
				m_synth.NoteOn(1, note);
				m_currPlaying.Add(note);
			}
		}
	}
}