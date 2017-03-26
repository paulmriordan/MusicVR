using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MusicWallData
{

	public Transform Parent;
	public GameObject Prefab;
	public float Radius = 0.1f;
	public float ButtonPaddingFrac = 0.5f;
	public float ButtonWidthFac = 0.75f;
	public CompositionData CompositionData;

	public float GetButtonWidth()
	{
		float colAngle = (2*Mathf.PI)/(float)CompositionData.NumCols;
		return Radius * Mathf.Sin(colAngle) / (Mathf.Sin((Mathf.PI - colAngle)*0.5f)) * ButtonWidthFac;	
	}

	public float GetTotalHeight()
	{
		float buttonWidth = GetButtonWidth();
		float buttonPadding = ButtonPaddingFrac * buttonWidth;
		return buttonWidth + buttonPadding + CompositionData.NumRows * (buttonWidth + buttonPadding);
	}

	public Vector3 GetPositionAtAngle(float radAngle)
	{
		return new Vector3(Mathf.Sin(radAngle) * Radius, 0, Mathf.Cos(radAngle) * Radius);
	}
}

