using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSkyBox : MonoBehaviour 
{
	public float Rate = 1.0f;
	void Update () 
	{
		RenderSettings.skybox.SetFloat("_Rotation", Time.time*Rate); //To set the speed, just multiply the Time.time with whatever amount you want.
	}
}
