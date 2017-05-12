using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using MusicVR.Wall;

namespace MusicVR.GUI
{
	/// <summary>
	/// Routing UI button events to their behaviour
	/// </summary>
	public class MusicWallUI : MonoSingleton<MusicWallUI> 
	{
		public SaveFileDialog SaveFileDialog;
		public UnityEngine.UI.Text PlayButtonText;

		public bool IsBlockingGameInput()
		{
			return SaveFileDialog.gameObject.activeSelf;
		}

		public void Save()
		{
			SaveFileDialog.Show(SaveFileDialog.E_DialogState.save);
		}

		public void Load()
		{
			SaveFileDialog.Show(SaveFileDialog.E_DialogState.load);
		}

		public void Clear()
		{
			GenericPopup.Instance.Show2ButtonPopup(
				Localization.Get("L_CLEAR_POPUP"), 
				Localization.Get("L_YES"),
				Localization.Get("L_NO"),
				() => {MusicWall.Instance.ClearWall();});
		}

		public void GenerateRandom()
		{
			GenericPopup.Instance.Show2ButtonPopup(
				Localization.Get("L_GENERATE_RANDOM"), 
				Localization.Get("L_YES"),
				Localization.Get("L_NO"),
				() => {MusicWall.Instance.GenerateRandom();});
		}

		public void PausePlay()
		{
			MusicWall.Instance.TogglePlayPause();
			PlayButtonText.text = Localization.Get(MusicWall.Instance.IsPlaying ? "L_PLAY_BUTTON_STOP" : "L_PLAY_BUTTON_PLAY");

		}
	}
}
