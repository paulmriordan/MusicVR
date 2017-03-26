using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicWall : MonoBehaviour 
{
	public MusicWallData 			WallProperties;
	public bool 					HasWall = true;
	public float					ProbInitSelected = 0.2f;
	public Synth					Synth;

	private WallButtons 			m_wallButtons = new WallButtons();
	private WallMesh 				m_wallMesh;
	private WallMusicPlayer			m_wallScroller;

	public bool		 				m_NeedUpdate {get; set;}

	//______________________________________________________________________________________
	void Awake()
	{
		if (HasWall)
			m_wallMesh = gameObject.AddComponent<WallMesh>();
		m_wallScroller = gameObject.AddComponent<WallMusicPlayer>();
		m_NeedUpdate = true;
	}

	//______________________________________________________________________________________
	void OnValidate()
	{
		m_NeedUpdate = true;
	}
	
	//______________________________________________________________________________________
	void Update()
	{
		if (m_NeedUpdate)
		{
			WallProperties.CompositionData.CreateDummyButtonData(ProbInitSelected);
			m_wallButtons.Create(WallProperties);
			if (HasWall)
				m_wallMesh.Create(WallProperties);
			m_wallScroller.Play(WallProperties, m_wallButtons, Synth);

		}
		m_wallButtons.Update();
		m_NeedUpdate = false;
	}
}
