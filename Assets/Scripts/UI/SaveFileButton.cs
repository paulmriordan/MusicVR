using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveFileButton : MonoBehaviour {

	public SaveFileDialog SaveFileDialog {get;set;}
	public Text Text;
	public Image Image;
	public Color EmptyColor;
	public Color UsedColor;

	public void Setup(string text, bool empty)
	{
		Text.text = text;
		Image.color = empty ? EmptyColor : UsedColor;
	}

	public void Clicked()
	{
		SaveFileDialog.Clicked(this);
	}
}
