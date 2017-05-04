//#define DIRECT_PLAY
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CSharpSynth.CustomSeq;


public class WallMusicPlayer : MonoBehaviour 
{
	private float m_colAccum = 0;
	private int m_prevColEffect = -1;
	private MusicWallData m_data;
	private WallButtons m_wallButtons;
	private bool m_playing;
	private List<int> m_currPlaying = new List<int>();
	private bool m_refreshNotes = false;
	private CustomSequencer customSequencer;
	private Synth m_synth;
	private GameObject m_lineInstance;

	public bool IsPlaying {get { return m_playing;}}

	void Awake()
	{
		var linePrefab = Resources.Load("WallAssets/PlayLine");
		m_lineInstance = Instantiate(linePrefab) as GameObject;
		m_lineInstance.transform.SetParent(transform);
		customSequencer = new CustomSequencer(Synth.Instance.midiStreamSynthesizer);
	}

	public void Reset()
	{
		EndPreviousNotes();
		m_prevColEffect = -1;
	}

	public void Init(MusicWallData properties, WallButtons buttons, Synth synth)
	{
		m_data = properties;
		m_data.CompositionData.OnCompositionChanged += () => { m_refreshNotes = true; };
		m_data.CompositionData.OnNoteSelected += NoteSelectedEventHandler;
		m_wallButtons = buttons;
		m_synth = synth;
	}

	public void Play()
	{
		m_playing = true;

		#if !DIRECT_PLAY
		LoadSequencerData();
		customSequencer.Play ();
		SetSequencerTime();
		#endif

	}

	public void NoteSelectedEventHandler(int in_row, int in_col)
	{
		var instr = m_data.CompositionData.GetInstrumentAtLocation(in_row, in_col);

		int note = MusicScaleConverter.Get(instr.Scale).Convert(in_row - instr.StartRow);

		m_synth.NoteOn(instr.InstrumentDefintion.IsDrum ? 9 : instr.IndexInComposition, 
			note + instr.InstrumentDefintion.InstrumentNoteOffset, 
			instr.InstrumentDefintion.InstrumentInt);
		
		m_currPlaying.Add(note);
	}

	public void Stop()
	{
		m_playing = false;
		customSequencer.Stop(false);
	}

	private void LoadSequencerData()
	{
		var seqData = m_data.CompositionData.GetSequncerData();
		//adjust total time into sample time
		seqData.TotalTime = (ulong)CustomSequencer.TimetoSampleTime(
			(uint)seqData.TotalTime, 
			(int)Synth.Instance.midiStreamSynthesizer.SampleRate,
			seqData.BeatsPerMinute,
			(uint)seqData.DeltaTiming);
		
		var instruments = m_data.CompositionData.InstrumentDataList;
		for (int i = 0; i < instruments.Count; i++)
		{
			var inst = instruments[i];
			if (inst.InstrumentDefintion.IsDrum)
				customSequencer.setProgram(9, inst.InstrumentDefintion.InstrumentInt);
			else
				customSequencer.setProgram(i, inst.InstrumentDefintion.InstrumentInt);
		}
		customSequencer.Load(seqData);
	}

	private void SetSequencerTime()
	{
		float secsPerNote = 1.0f/((m_data.CompositionData.Tempo/60.0f));
		float secs = m_colAccum*secsPerNote;
		customSequencer.Time = secs;
	}

	void Update () 
	{
		UpdateRefreshNotes();
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
					#if DIRECT_PLAY
					m_synth.NoteOn(instrument.InstrumentDefintion.IsDrum ? 9 : 1, 
						note + instrument.InstrumentDefintion.InstrumentNoteOffset, 
						instrument.InstrumentDefintion.InstrumentInt);
					#endif
					m_currPlaying.Add(note);
				}
			}
			rowCum += instrument.NumRows;
		}
	}

	void UpdateRefreshNotes()
	{
		if (m_refreshNotes)
		{
			LoadSequencerData();
			SetSequencerTime();
			m_refreshNotes = false;
		}
	}

	void EndPreviousNotes()
	{
		if (m_prevColEffect != -1)
		{
			for (int iNote = 0; iNote < m_currPlaying.Count; iNote++)
			{
				int note = m_currPlaying[iNote];
				#if DIRECT_PLAY
				m_synth.NoteOff(1, note);
				#endif
			}
		}
		m_currPlaying.Clear();
	}
}
