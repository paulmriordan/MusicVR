using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WallDragger : MonoBehaviour 
{	
	public KeyCode WallDraggerKey = KeyCode.LeftShift;
	public BoundedDrag BoundedDrag;
	public BoundedDrag HorizontalDrag;
	public Vector2 ScreeenPanScale = new Vector2(0.1f,0.1f);
	public float QuantizeVelocity = 1.0f;

	private Vector3 m_dragStart;
	private float m_numCols;

	public WallDraggerInputConsumer InputConsumer {get; private set;}

	void Start()
	{
		InputConsumer = new WallDraggerInputConsumer();
		InputConsumer.IsFinishedFunc = () => {return !BoundedDrag.IsDragging && !HorizontalDrag.IsDragging;};
		BoundedDrag.SetDragAllowedFuncPtr(() => {return InputConsumer.IsActive();});
		HorizontalDrag.SetDragAllowedFuncPtr(() => {return InputConsumer.IsActive();});
	}

	public void Reset(float maxLimit, float minLimit, float numCols)
	{
		BoundedDrag.SetDragLimit(maxLimit, minLimit);
		BoundedDrag.ForceToPositionImmediately((maxLimit + minLimit) * 0.5f);
		m_numCols = numCols;
	}

	public void PerformPan(Vector2 pan)
	{
		BoundedDrag.SetTargetPos(BoundedDrag.GetCurrentPos() + pan.y * ScreeenPanScale.y);
		HorizontalDrag.SetTargetPos(HorizontalDrag.GetCurrentPos() + pan.x * ScreeenPanScale.x);
	}

	void Update()
	{
		transform.position = new Vector3(0, BoundedDrag.GetCurrentPos(), 0);

		var euler = transform.localRotation.eulerAngles;
		var newY = HorizontalDrag.GetCurrentPos();
		//quantize
		if (Mathf.Abs(HorizontalDrag.Velocity) > QuantizeVelocity)
		{
			var oneColRotation = 360.0f / m_numCols;
			newY = Mathf.Round(newY/oneColRotation)*oneColRotation;
		}
		transform.localRotation = Quaternion.Euler(euler.x, newY, euler.z);
	}
}