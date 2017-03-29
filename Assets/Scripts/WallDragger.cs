using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WallDragger : MonoBehaviour 
{
	public KeyCode WallDraggerKey = KeyCode.LeftShift;
	public BoundedDrag BoundedDrag;
	public BoundedDrag HorizontalDrag;

	public event Action OnDraggingActive = () => {};
	public event Action OnDragModeDisabled = () => {};

	private Vector3 m_dragStart;

	public bool DraggingEnabled {get; set;}

	void Start()
	{
		BoundedDrag.SetDragAllowedFuncPtr(() => {return DraggingEnabled;});
		HorizontalDrag.SetDragAllowedFuncPtr(() => {return DraggingEnabled;});
	}

	public void Reset(float maxLimit, float minLimit)
	{
		BoundedDrag.SetDragLimit(maxLimit, minLimit);
		BoundedDrag.ForceToPositionImmediately((maxLimit + minLimit) * 0.5f);
	}

	void Update()
	{
		transform.position = new Vector3(0, BoundedDrag.GetCurrentPos(), 0);

		var euler = transform.localRotation.eulerAngles;
		transform.localRotation = Quaternion.Euler(euler.x, HorizontalDrag.GetCurrentPos(), euler.z);
		 
	}
}
