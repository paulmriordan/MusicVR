using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class BoundedDrag : UpdatingObject
{
	public float m_Elasticity = 0.1f; // Only used for MovementType.Elastic
	public bool m_Inertia = true;
	public float m_DecelerationRate = 0.135f; // Only used when inertia is enabled
	public float m_DragSensitivity = 0.05f;
	public float m_minMovementToDrag = 0.005f;
	public float m_minMovementToSettle = 0.05f;
	public float m_overstretching = 0.55f;
	public bool m_ClampBounds = false;
	public float MinLimit = 0;
	public float MaxLimit = 1.0f;
	public float MaxFixedDTPerFrame = 0.1f;

	public Vector3 m_DragDirection = Vector3.up;

	bool m_Dragging ;
	float m_Velocity;
	float m_PrevPosition ;
	float m_dragStartPosition ;
	float m_dragStartCursorPosition ;
	float m_dragStartTime;
	float m_targetZ = 0;
	float m_currentZ ;
	Func<bool> m_IsDragAllowed = () => true;

	public bool IsDragging { get { return m_Dragging;}}

	public float TimeDragging()
	{
		if (m_dragStartTime > 0)
			return Time.time - m_dragStartTime;
		return -1.0f;
	}

	public void ForceToPositionImmediately(float pos)
	{
		SetTargetPos(pos);
		m_currentZ = pos;
	}

	public float GetCurrentPos()
	{
		return m_targetZ;
	}

	public void SetTargetPos(float pos)
	{
		m_targetZ = pos;
		m_Velocity = 0;
	}

	public void SetDragLimit(float minLimit, float maxLimit)
	{
		MinLimit = minLimit;
		MaxLimit = maxLimit;
	}

	public void SetDragAllowedFuncPtr(Func<bool> dragAllowed)
	{
		m_IsDragAllowed = dragAllowed;
	}

	protected override void Update()
	{
		UpdateDragInput();
		UpdateDragMovement();
	}

	private float DragValue {
		get {
			return Vector3.Dot(Input.mousePosition, m_DragDirection);
		}
	}

	private void UpdateDragInput()
	{
		if (m_IsDragAllowed())
		{
			if(Input.GetMouseButtonDown(0)) 
			{
				m_dragStartPosition = m_targetZ;
				m_dragStartCursorPosition = DragValue;
				m_dragStartTime = Time.time;
			}
			else if (Input.GetMouseButton(0) /*&& m_IsDragAllowed()*/)
			{
				if(!m_Dragging) {
					var totalMov = Mathf.Abs(DragValue - m_dragStartCursorPosition)/Screen.height;
					if(totalMov > m_minMovementToDrag) {
						m_dragStartPosition = m_targetZ;
						m_dragStartCursorPosition = DragValue;
						m_Dragging = true;
					}
				} else {
					var pointerDelta = (DragValue - m_dragStartCursorPosition) * m_DragSensitivity;
					var position = m_dragStartPosition + pointerDelta;

					// Offset to get content into place in the view.
					var offset = CalculateOffset(position - m_targetZ);
					position += offset;
					var range = Mathf.Abs(MaxLimit - MinLimit);
					if (range != 0)
					{
						if (offset > 0f)
							position -= RubberDelta(offset, Mathf.Abs(MaxLimit - MinLimit));
						else if (offset < 0)
							position += RubberDelta(Mathf.Abs(offset), Mathf.Abs(MinLimit - MaxLimit));
					}
					else 
						position = MaxLimit; //PR 2016-Aug-02 TRAIL-2685: prevent div by zero

					m_targetZ = position;
				}
			} else if(Input.GetMouseButtonUp(0)) {
				m_Dragging = false;
				m_dragStartTime = -1.0f;
			}
		}
	}

	private void UpdateDragMovement()
	{	
		float deltaTime = Mathf.Min(MaxFixedDTPerFrame,Time.unscaledDeltaTime); // GL - yeh, I know this is for fixedDT but it applies in this case too
		var offset0 = CalculateOffset(0f);
		if (!m_Dragging && (offset0 != 0f || m_Velocity != 0f)) {
			var position = m_targetZ;
			// Apply spring physics if movement is elastic and content has an offset from the view.
			if (offset0 != 0f)
				position = Mathf.SmoothDamp(m_targetZ, m_targetZ + offset0, ref m_Velocity, m_ClampBounds ? 0 : m_Elasticity, Mathf.Infinity, deltaTime);
			// Else move content according to velocity with deceleration applied.
			else if (m_Inertia) {
				m_Velocity *= Mathf.Pow(m_DecelerationRate, deltaTime);
				if (Mathf.Abs(m_Velocity) < m_minMovementToSettle)
					m_Velocity = 0;
				position += m_Velocity * deltaTime;
			} // If we have neither elaticity or friction, there shouldn't be any velocity.
			else
				m_Velocity = 0f;

			if(m_Velocity != 0f)
				m_targetZ = position;
		}

		var offset0_2 = CalculateOffset(0f);
		if (offset0_2 != 0 && m_ClampBounds)
		{
			m_targetZ += offset0_2;
		}

		if (m_Dragging && m_Inertia) {
			var newVelocity = (m_targetZ - m_PrevPosition) / deltaTime;
			var t = deltaTime * 10f;
			m_Velocity = m_Velocity * t + newVelocity * (1-t);
		}
		m_currentZ = (m_targetZ - m_currentZ) * .5f;

		m_PrevPosition = m_targetZ;
	}

	private float CalculateOffset(float delta)
	{
		var cmp = m_targetZ + delta;

		if(cmp < MaxLimit)
			return MaxLimit - cmp;
		else if(cmp > MinLimit)
			return MinLimit - cmp;
		return 0f;
	}

	private float RubberDelta(float overStretching, float viewSize)
	{
		return (1 - (1 / ((Mathf.Abs(overStretching) * (m_ClampBounds ? 0 : m_overstretching) / viewSize) + 1))) * viewSize * Mathf.Sign(overStretching);
	}

}
