using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MusicVR.Synth;

public class MusicWall : MonoSingleton<MusicWall> 
{
	public event Action<MusicWallData> OnWallDataUpdated = (p) => {};

	public MusicWallData 			WallProperties;
	public bool 					HasWall = true;
	public float					ProbInitSelected = 0.2f;
	public Synth					Synth;

	private WallButtonManager 		m_wallButtonManager = new WallButtonManager();
	private WallMesh 				m_wallMesh;
	private WallMusicPlayer			m_musicPlayer;

	public bool		 				NeedsUpdate {get; set;}
	public bool 					IsPlaying {get { return m_musicPlayer.IsPlaying;}}

	public void ClearWall()
	{
		WallProperties.CompositionData.Clear();
		NeedsUpdate = true;
	}

	public void GenerateRandom()
	{
		WallProperties.CompositionData.CreateDummyButtonData(ProbInitSelected);
		NeedsUpdate = true;
	}

	public void TogglePlayPause()
	{
		if (m_musicPlayer.IsPlaying)
			m_musicPlayer.Stop();
		else
			m_musicPlayer.Play();
	}

	protected override void _Awake()
	{
		if (HasWall)
			m_wallMesh = gameObject.AddComponent<WallMesh>();
		m_musicPlayer = gameObject.AddComponent<WallMusicPlayer>();
		WallProperties.Init();
		m_musicPlayer.Init(WallProperties, m_wallButtonManager, Synth);
		GenerateRandom();
		NeedsUpdate = true;
	}

	void OnValidate()
	{
		NeedsUpdate = true;
	}

	void Update()
	{
		if (NeedsUpdate)
		{
			m_wallButtonManager.Create(WallProperties);
			if (HasWall)
				m_wallMesh.Create(WallProperties);
			m_musicPlayer.Init(WallProperties, m_wallButtonManager, Synth);
			m_musicPlayer.Reset();
			m_musicPlayer.Play();
			OnWallDataUpdated(WallProperties);

		}
		m_wallButtonManager.Update();
		NeedsUpdate = false;
	}
}
