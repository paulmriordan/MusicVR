using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

namespace MusicVR.GUI
{
	public class GenericPopup : MonoSingleton<GenericPopup> {

		[SerializeField] GameObject OneButtonPopup;
		[SerializeField] GameObject TwoButtonPopup;
		[SerializeField] Text OneButtonMessage;
		[SerializeField] Text TwoButtonMessage;
		[SerializeField] Text Button1Text1;
		[SerializeField] Text Button2Text1;
		[SerializeField] Text Button2Text2;
		Action confirmAction = () => {};
		Action cancelAction = () => {};

		protected override void _Awake()
		{
			Hide();
		}

		public void Show2ButtonPopup(string message, string buttonText, string buttonText2, Action onConfirm = null, Action onCancel = null)
		{
			TwoButtonMessage.text = message;
			confirmAction = onConfirm;
			cancelAction = onCancel;
			Button2Text1.text = buttonText;
			Button2Text2.text = buttonText2;
			TwoButtonPopup.SetActive(true);
		}

		public void Show1ButtonPopup(string message, string buttonText, Action onConfirm = null, Action onCancel = null)
		{
			OneButtonMessage.text = message;
			confirmAction = onConfirm;
			cancelAction = onCancel;
			Button1Text1.text = buttonText;
			OneButtonPopup.SetActive(true);
		}

		public void ConfirmClicked()
		{
			Hide();
			if (confirmAction != null)
				confirmAction();
		}

		public void CancelClicked()
		{
			Hide();
			if (cancelAction != null)
				cancelAction();
		}

		public void Hide()
		{
			OneButtonPopup.SetActive(false);
			TwoButtonPopup.SetActive(false);
		}
	}
}
