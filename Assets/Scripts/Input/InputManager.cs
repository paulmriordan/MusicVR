using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

	public SmoothMouseLook m_mouseLook;
	public WallDragger m_wallDragger;

	void Start () 
	{
		m_wallDragger.OnDragModeEnabled += () => m_mouseLook.EnableLook(false);
		m_wallDragger.OnDragModeDisabled += () => m_mouseLook.EnableLook(true);
		m_wallDragger.OnDragModeEnabled += () => WallButton.SelectionEnabled(false);
		m_wallDragger.OnDragModeDisabled += () => WallButton.SelectionEnabled(true);
	}

	void Update()
	{
		UpdatingObject.Check();
	}

}
