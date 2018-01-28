using UnityEngine;
using MusicVR.WallInput;

namespace MusicVR.Wall
{
	public class WallButtonInputConsumer : InputConsumerBase
	{
		public override bool TryConsumeInput(InputState state) 
		{
			if (Input.GetMouseButtonUp(0))
				return true; //always consume button up events
			if (!Input.GetMouseButton(0))
				return false;

			// if not hitting ui, and input down is over button, consume
			if (!InputManager.Instance.InputBlockedByUI())
			{
				RaycastHit hit;
				if (Physics.Raycast(Camera.main.ScreenPointToRay(state.InputDownPos), out hit) 
					&& hit.collider.GetComponent<SequencerWallButton>() != null)
				{
					return Time.time - state.InputDownTime > state.HoldTime;
				}
			}
			return false;
		}

		public override bool IsFinished()
		{
			return !Input.GetMouseButton(0);
		}
	}
}
