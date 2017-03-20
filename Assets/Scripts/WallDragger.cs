using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WallDragger : MonoBehaviour 
{
	public KeyCode WallDraggerKey = KeyCode.LeftShift;
	public BoundedDrag BoundedDrag;
	public event Action OnDragModeEnabled = () => {};
	public event Action OnDragModeDisabled = () => {};

	private Vector3 m_dragStart;
	private bool m_dragModeEnabled;

	void Update()
	{
		if (Input.GetKeyDown(WallDraggerKey))
		{
			OnDragModeEnabled();
			m_dragModeEnabled = true;
		}
		if (Input.GetKeyUp(WallDraggerKey))
		{
			OnDragModeDisabled();
			m_dragModeEnabled = false;
		}

		if (Input.GetMouseButtonDown(0) && Input.GetKey(WallDraggerKey))
		{
//			Camera.main.ScreenPointToRay
		}

		if (Input.GetMouseButtonUp(0) && m_dragModeEnabled)
		{
		}

		BoundedDrag.SetDragAllowedFuncPtr(() => {return m_dragModeEnabled;});
		transform.position = new Vector3(0, BoundedDrag.GetCurrentPos(), 0);
	}
}
