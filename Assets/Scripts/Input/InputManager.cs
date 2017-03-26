using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

	public SmoothMouseLook m_mouseLook;
	public WallDragger m_wallDragger;

	void Start () 
	{
		m_wallDragger.OnDraggingActive += () => m_mouseLook.EnableLook(false);
		m_wallDragger.OnDragModeDisabled += () => m_mouseLook.EnableLook(true);
		m_wallDragger.OnDraggingActive += () => WallButton.SelectionEnabled(false);
		m_wallDragger.OnDragModeDisabled += () => WallButton.SelectionEnabled(true);
	}

	void Update()
	{
		UpdatingObject.Check();

		// Disable input if required
		{
			bool inputBlocked = MusicWallUI.Instance.IsBlockingGameInput();
			inputBlocked |= m_wallDragger.IsDraggingActive();

			m_mouseLook.EnableLook(!inputBlocked);
			WallButton.SelectionEnabled(!inputBlocked);
		}
	}

}
