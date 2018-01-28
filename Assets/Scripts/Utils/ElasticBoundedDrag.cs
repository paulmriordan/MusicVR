using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class ElasticBoundedDrag : AutoUpdatingObject
{
	public float 		Elasticity = 0.1f;
	public bool 		Inertia = true;
	public float 		DecelRate = 0.135f;
	public float 		DragSensitivity = 0.05f;
	public float 		DragStartThresholdDist = 0.005f;
	public float 		MinVelToSettle = 0.05f;
	public float 		Overstretching = 0.55f;
	public bool 		ClampBounds = false;
	public float 		MinLimit = 0;
	public float 		MaxLimit = 1.0f;
	public Vector3 		DragDirection = Vector3.up;

	private float 		m_vel;
	private float 		m_prevPosition;
	private float 		m_startPosition;
	private float 		m_startCursorPosition;
	private float 		m_target = 0;
	Func<bool> 			m_draggingEnabled = () => true;

	public bool IsDragging {get; private set;}
	public float Velocity {get { return m_vel;}}

	public float GetCurrentPos()
	{
		return m_target;
	}

	public void SetTargetPos(float pos)
	{
		m_target = pos;
		m_vel = 0;
	}

	public void SetDragLimit(float min, float max)
	{
		MinLimit = min;
		MaxLimit = max;
	}

	public void SetDraggingEnabledFuncPtr(Func<bool> dragAllowed)
	{
		m_draggingEnabled = dragAllowed;
	}

	protected override void Update()
	{
		if (m_draggingEnabled())
			ManuallyFireDragEvents();
		
		UpdateCurrentPosition();
	}

	private void ManuallyFireDragEvents()
	{
		if (Input.GetMouseButtonDown(0)) 
			OnBeginDrag();
		else if (Input.GetMouseButton(0))
			OnDrag();
		else if (Input.GetMouseButtonUp(0)) 
			OnEndDrag();
	}

	private void OnBeginDrag()
	{
		m_startPosition = m_target;
		m_startCursorPosition = CursorPosProjDragDir();
	}

	private void OnDrag()
	{
		if (!IsDragging) 
		{
			TryStartDrag();
		} 
		else 
		{
			var pointerDelta = (CursorPosProjDragDir() - m_startCursorPosition) * DragSensitivity;
			var position = m_startPosition + pointerDelta;

			// Offset to get content into place in the view.
			var offset = CalculateOffsetOutsideLimits(position - m_target);
			position += offset;

			// Apply rubber delta to drag if dragged beyond limits
			var range = Mathf.Abs(MaxLimit - MinLimit);
			if (range != 0)
				position += (offset < 0 ? 1.0f : -1.0f) * RubberDelta(Mathf.Abs(offset), range);
			else 
				position = MaxLimit;

			m_target = position;
		}
	}

	private void OnEndDrag()
	{
		IsDragging = false;
	}
		
	private float CursorPosProjDragDir()
	{
		return Vector3.Dot(Input.mousePosition, DragDirection);
	}

	/// <summary>
	/// Starts the drag once treshold drag distance has been reached
	/// </summary>
	private void TryStartDrag()
	{
		var totalMov = Mathf.Abs(CursorPosProjDragDir() - m_startCursorPosition)/Screen.dpi;
		if(totalMov > DragStartThresholdDist) 
		{
			m_startPosition = m_target;
			m_startCursorPosition = CursorPosProjDragDir();
			IsDragging = true;
		}
	}

	private void UpdateCurrentPosition()
	{	
		float deltaTime = Time.unscaledDeltaTime; 

		if (!IsDragging)
			UpdateUndraggedPosition(deltaTime);

		if (ClampBounds)
			ApplyClampToBounds();

		if (IsDragging && Inertia) 
			ApplyInertia(deltaTime);

		m_prevPosition = m_target;
	}

	private float CalculateOffsetOutsideLimits(float delta)
	{
		var newPos = m_target + delta;

		if (newPos < MaxLimit)
		{
			return MaxLimit - newPos;
		}
		else if (newPos > MinLimit)
		{
			return MinLimit - newPos;
		}
		return 0;
	}

	private void UpdateUndraggedPosition(float deltaTime)
	{
		var offset = CalculateOffsetOutsideLimits(0);

		// if moving or if outside bounds (ie needs to spring back), update position
		if (offset != 0 || m_vel != 0)
		{
			var newPosition = m_target;

			// Apply spring physics if content has an offset from the view.
			if (offset != 0)
			{
				newPosition = Mathf.SmoothDamp(m_target,
					m_target + offset, 
					ref m_vel,
					ClampBounds ? 0 : Elasticity,
					Mathf.Infinity,
					deltaTime);
			}
			// Else move content according to velocity with deceleration applied.
			else if (Inertia) 
			{
				m_vel *= Mathf.Pow(DecelRate, deltaTime);
				if (Mathf.Abs(m_vel) < MinVelToSettle)
					m_vel = 0;
				newPosition += m_vel * deltaTime;
			} 
			// If we have neither elaticity or friction, there shouldn't be any velocity.
			else
			{
				m_vel = 0;
			}

			if(m_vel != 0)
			{
				m_target = newPosition;
			}
		}
	}

	private void ApplyClampToBounds()
	{
		var offset = CalculateOffsetOutsideLimits(0);
		if (offset != 0)
		{
			m_target += offset;
		}
	}

	private void ApplyInertia(float deltaTime)
	{
		var newVelocity = (m_target - m_prevPosition) / deltaTime;
		m_vel = Mathf.Lerp(m_vel, newVelocity, deltaTime * 10f);
	}

	private float RubberDelta(float overStretching, float viewSize)
	{
		return (1.0f - (1.0f / ((Mathf.Abs(overStretching) * (ClampBounds ? 0 : Overstretching) / viewSize) + 1.0f))) * viewSize * Mathf.Sign(overStretching);
	}

}
