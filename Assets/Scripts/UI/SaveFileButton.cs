using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveFileButton : MonoBehaviour {

	public SaveFileDialog SaveFileDialog {get;set;}

	public void Clicked()
	{
		SaveFileDialog.Clicked(this);
	}
}
