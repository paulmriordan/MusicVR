using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScroller : MonoBehaviour {

	public float Tempo = 120.0f;

	private int m_currentCol = 0;
	private float m_colAccum = 0;
	private int m_prevCol = -1;
	private WallProperties m_wallProperties;
	private WallButtons m_wallButtons;
	private bool m_playing;

	public void Play(WallProperties properties, WallButtons buttons)
	{
		m_wallProperties = properties;
		m_wallButtons = buttons;
		m_playing = true;
	}

	public void Stop()
	{
		m_playing = false;
	}

	// Update is called once per frame
	void Update () 
	{
		UpdatePosition();
		UpdateButtonEffects();
	}

	void UpdatePosition()
	{
		if (m_playing)
		{
			float secsPerNotes = 1.0f/(Tempo/60.0f);
			float notesPerSec = (Tempo/60.0f);
			m_colAccum += Time.deltaTime*notesPerSec;
			m_colAccum = Mathf.Repeat(m_colAccum, (float)m_wallProperties.NumCols);
		}
	}

	void UpdateButtonEffects()
	{
		int currCol = (int)m_colAccum;
		if (currCol != m_prevCol)
		{
			if (m_prevCol != -1)
			{
				for (int iRow = 0; iRow < m_wallProperties.NumRows; iRow++)
				{
					var button = m_wallButtons.GetButton(iRow, m_prevCol);
					button.SetPlaying(false);
				}
			}
			for (int iRow = 0; iRow < m_wallProperties.NumRows; iRow++)
			{
				var button = m_wallButtons.GetButton(iRow, currCol);
				button.SetPlaying(true);
			}
			m_prevCol = currCol;
		}
	}
}
