using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WallButtonTween : MonoBehaviour
{
	public float ScaleTime = 0.2f;
	public Vector3 ScaleSelected = new Vector3(1.0f,1.0f,0.2f);
	public Vector3 ScaleMouseDown = new Vector3(1.1f,1.1f,0.1f);
	public Vector3 ScaleUnselected = new Vector3(1.0f,1.0f,0.7f);
	public MeshRenderer Target;

	private Vector3 m_scaleVelocity;

	public bool Held {get;set;}
	public bool Selected {get;set;}

	public void CustomUpdate()
	{
		UpdateScale();
	}

	private void UpdateScale()
	{
		Vector3 targetScale = ScaleUnselected;
		if (Held)
			targetScale = ScaleMouseDown;
		else if (Selected)
			targetScale = ScaleSelected;
		if ((targetScale - Target.transform.localScale).sqrMagnitude < 0.001f)
			Target.transform.localScale = targetScale;
		else
			Target.transform.localScale = Vector3.SmoothDamp(Target.transform.localScale, 
				targetScale, 
				ref m_scaleVelocity,
				ScaleTime);
	}
}

