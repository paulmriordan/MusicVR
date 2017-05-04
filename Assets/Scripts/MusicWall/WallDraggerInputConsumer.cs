﻿using UnityEngine;
using System;

public class WallDraggerInputConsumer : InputConsumerBase
{
	public Func<bool> IsFinishedFunc;

	public override bool TryConsumeInput(InputManager.InputState state) 
	{
		if (!Input.GetMouseButton(0))
			return false;
		// If dragged over threshold distance (and nothing else has consumed input 
		// in the mean time), start wall panning
		var d2 = (Input.mousePosition - state.inputDownPos).sqrMagnitude;
		return d2 > state.HoldMoveLimit * state.HoldMoveLimit;
	}

	public override bool IsFinished()
	{
		return IsFinishedFunc();
	}
}
