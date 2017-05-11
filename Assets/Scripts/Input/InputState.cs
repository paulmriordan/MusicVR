using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicVR.WallInput
{
	public class InputState
	{
		public float 		HoldTime = 0.5f;
		public float 		HoldMoveLimit = 10.0f;
		public Vector2 		EdgeDistPan = new Vector2(0.15f, 0.15f);
		public float 		ThresholdStartEdgePan = 10.0f;

		public float 		InputDownTime {get;set;}
		public Vector3 		InputDownPos {get;set;}
	}
}