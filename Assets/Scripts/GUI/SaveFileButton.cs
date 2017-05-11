using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MusicVR.GUI
{
	public class SaveFileButton : MonoBehaviour {

		public SaveFileDialog SaveFileDialog {get;set;}
		public Text TextUsed;
		public Text TextEmpty;
		public Image EmptyImage;
		public Image UsedImage;

		public void Setup(string text, bool empty)
		{
			TextUsed.text = text;
			TextEmpty.text = text;
			TextEmpty.gameObject.SetActive(empty);
			TextUsed.gameObject.SetActive(!empty);
			EmptyImage.gameObject.SetActive(empty);
			UsedImage.gameObject.SetActive(!empty);
		}

		public void Clicked()
		{
			SaveFileDialog.Clicked(this);
		}
	}
}
