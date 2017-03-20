using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WallProperties
{
	public Transform Parent;
	public GameObject Prefab;
	public int NumRows = 100;
	public int NumCols = 20;
	public float Radius = 0.1f;
	public float ButtonPaddingFrac = 0.5f;
	public float ButtonWidthFac = 0.75f;
	public float Tempo = 120.0f;
	public MusicScaleConverter.E_ConverterType Scale = MusicScaleConverter.E_ConverterType.Pentatonic;

	public float GetButtonWidth()
	{
		float colAngle = (2*Mathf.PI)/(float)NumCols;
		return Radius * Mathf.Sin(colAngle) / (Mathf.Sin((Mathf.PI - colAngle)*0.5f)) * ButtonWidthFac;	
	}

	public float GetTotalHeight()
	{
		float buttonWidth = GetButtonWidth();
		float buttonPadding = ButtonPaddingFrac * buttonWidth;
		return buttonWidth + buttonPadding + NumRows*(buttonWidth + buttonPadding);
	}
}

