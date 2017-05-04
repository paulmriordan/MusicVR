using UnityEngine;

public class WallButtonInputConsumer : InputConsumerBase
{
	public override bool TryConsumeInput(InputManager.InputState state) 
	{
		if (Input.GetMouseButtonUp(0))
			return true; //always consume button up events
		if (!Input.GetMouseButton(0))
			return false;
		// Always consume, regardless of position, if held for long enough
		return Time.time - state.InputDownTime > state.HoldTime;
	}

	public override bool IsFinished()
	{
		return !Input.GetMouseButton(0);
	}
}
