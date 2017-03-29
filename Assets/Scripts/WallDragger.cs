using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WallDragger : MonoBehaviour 
{
	public class WallDraggerInputConsumer : InputConsumerBase
	{
		public Func<bool> IsFinishedFunc;

		public override bool TryConsumeInput(InputManager.InputState state) 
		{
			if (!Input.GetMouseButton(0))
				return false;
			var d2 = (Input.mousePosition - state.inputDownPos).sqrMagnitude;
			return d2 > state.HoldMoveLimit * state.HoldMoveLimit;
		}

		public override bool IsFinished()
		{
			return IsFinishedFunc();
		}
	}

	public KeyCode WallDraggerKey = KeyCode.LeftShift;
	public BoundedDrag BoundedDrag;
	public BoundedDrag HorizontalDrag;
	public Vector2 ScreeenPanScale = new Vector2(0.1f,0.1f);

	private Vector3 m_dragStart;

	public WallDraggerInputConsumer InputConsumer {get; private set;}

	void Start()
	{
		InputConsumer = new WallDraggerInputConsumer();
		InputConsumer.IsFinishedFunc = () => {return !BoundedDrag.IsDragging && !HorizontalDrag.IsDragging;};
		BoundedDrag.SetDragAllowedFuncPtr(() => {return InputConsumer.IsActive();});
		HorizontalDrag.SetDragAllowedFuncPtr(() => {return InputConsumer.IsActive();});
	}

	public void Reset(float maxLimit, float minLimit)
	{
		BoundedDrag.SetDragLimit(maxLimit, minLimit);
		BoundedDrag.ForceToPositionImmediately((maxLimit + minLimit) * 0.5f);
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
		transform.localRotation = Quaternion.Euler(euler.x, HorizontalDrag.GetCurrentPos(), euler.z);
		 
	}
}
