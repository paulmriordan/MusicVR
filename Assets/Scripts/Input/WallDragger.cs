using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MusicVR.WallInput
{
	/// <summary>
	/// This script translates and rotates it's transform (ie, the wall) via to it's BoundedDrag components.
	/// BoundedDrag objects provide current rotation and position for current user input drags.
	/// </summary>
	public class WallDragger : MonoBehaviour 
	{	
		public KeyCode 				WallDraggerKey = KeyCode.LeftShift;
		[UnityEngine.Serialization.FormerlySerializedAs("BoundedDrag")]
		public ElasticBoundedDrag 	VerticalDrag;
		public ElasticBoundedDrag 	HorizontalDrag;
		public Vector2 				ScreeenPanScale = new Vector2(0.1f,0.1f);
		public float 				QuantizeVelocity = 1.0f;

		private Vector3	 			m_dragStart;
		private float 				m_numCols;

		public WallDraggerInputConsumer InputConsumer {get; private set;}

		void Start()
		{
			InputConsumer = new WallDraggerInputConsumer();
			// Stop consuming input once the panning is no longer active
			InputConsumer.IsFinishedFunc = () => {return !VerticalDrag.IsDragging && !HorizontalDrag.IsDragging;};
			// Allow dragging if input has been consumed
			VerticalDrag.SetDraggingEnabledFuncPtr(() => {return InputConsumer.IsActive();});
			HorizontalDrag.SetDraggingEnabledFuncPtr(() => {return InputConsumer.IsActive();});
		}

		public void Reset(float maxLimit, float minLimit, float numCols)
		{
			VerticalDrag.SetDragLimit(maxLimit, minLimit);
			VerticalDrag.SetTargetPos((maxLimit + minLimit) * 0.5f);
			m_numCols = numCols;
		}

		public void PerformPan(Vector2 pan)
		{
			VerticalDrag.SetTargetPos(VerticalDrag.GetCurrentPos() + pan.y * ScreeenPanScale.y);
			HorizontalDrag.SetTargetPos(HorizontalDrag.GetCurrentPos() + pan.x * ScreeenPanScale.x);
		}

		void Update()
		{
			transform.position = new Vector3(0, VerticalDrag.GetCurrentPos(), 0);

			var euler = transform.localRotation.eulerAngles;
			var newY = HorizontalDrag.GetCurrentPos();

			//quantize when past a certain speed; it's very disorientating otherwise
			if (Mathf.Abs(HorizontalDrag.Velocity) > QuantizeVelocity)
			{
				var oneColRotation = 360.0f / m_numCols;
				newY = Mathf.Round(newY/oneColRotation)*oneColRotation;
			}

			transform.localRotation = Quaternion.Euler(euler.x, newY, euler.z);
		}
	}
}