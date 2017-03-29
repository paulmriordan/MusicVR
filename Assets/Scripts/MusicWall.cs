using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MusicWall : MonoSingleton<MusicWall> 
{
	public event Action<MusicWallData> OnWallDataUpdated = (p) => {};

	public MusicWallData 			WallProperties;
	public bool 					HasWall = true;
	public float					ProbInitSelected = 0.2f;
	public Synth					Synth;

	private WallButtons 			m_wallButtons = new WallButtons();
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
			m_musicPlayer.Play(WallProperties, m_wallButtons, Synth);
	}

	//______________________________________________________________________________________
	protected override void _Awake()
	{
		if (HasWall)
			m_wallMesh = gameObject.AddComponent<WallMesh>();
		m_musicPlayer = gameObject.AddComponent<WallMusicPlayer>();
		NeedsUpdate = true;
	}

	//______________________________________________________________________________________
	void OnValidate()
	{
		NeedsUpdate = true;
	}
	
	//______________________________________________________________________________________
	void Update()
	{
		if (NeedsUpdate)
		{
			m_wallButtons.Create(WallProperties);
			if (HasWall)
				m_wallMesh.Create(WallProperties);
			m_musicPlayer.Reset();
			m_musicPlayer.Play(WallProperties, m_wallButtons, Synth);
			OnWallDataUpdated(WallProperties);

		}
		m_wallButtons.Update();
		NeedsUpdate = false;
	}
}
