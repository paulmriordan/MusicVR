using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WallDragger : MonoBehaviour 
{
	public KeyCode WallDraggerKey = KeyCode.LeftShift;
	public BoundedDrag BoundedDrag;
	public event Action OnDraggingActive = () => {};
	public event Action OnDragModeDisabled = () => {};

	private Vector3 m_dragStart;
	private bool m_draggingActive;

	public bool IsDraggingActive()
	{
		return m_draggingActive;
	}

	void Update()
	{
		if (Input.GetKeyDown(WallDraggerKey))
		{
			OnDraggingActive();
			m_draggingActive = true;
		}
		if (Input.GetKeyUp(WallDraggerKey))
		{
			OnDragModeDisabled();
			m_draggingActive = false;
		}

		if (Input.GetMouseButtonDown(0) && Input.GetKey(WallDraggerKey))
		{
//			Camera.main.ScreenPointToRay
		}

		if (Input.GetMouseButtonUp(0) && m_draggingActive)
		{
		}

		BoundedDrag.SetDragAllowedFuncPtr(() => {return m_draggingActive;});
		transform.position = new Vector3(0, BoundedDrag.GetCurrentPos(), 0);
	}
}
