﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

class tinyfd
{

	// Straight From the c++ Dll (unmanaged)
	[DllImport("TestCPPLibrary", EntryPoint="TestDivide")]
	public static extern float StraightFromDllTestDivide(float a, float b);

	// cross platform utf8
	[DllImport("tinyfiledialogs64.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
	public static extern int tinyfd_messageBox(string aTitle, string aMessage, string aDialogTyle, string aIconType, int aDefaultButton);
	[DllImport("tinyfiledialogs64.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr tinyfd_inputBox(string aTitle, string aMessage, string aDefaultInput);
	[DllImport("tinyfiledialogs64.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr tinyfd_saveFileDialog(string aTitle, string aDefaultPathAndFile, int aNumOfFilterPatterns, string[] aFilterPatterns, string aSingleFilterDescription);
	[DllImport("tinyfiledialogs.dll", EntryPoint = "tinyfd_openFileDialog", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr tinyfd_openFileDialog(string aTitle, string aDefaultPathAndFile, int aNumOfFilterPatterns, string[] aFilterPatterns, string aSingleFilterDescription, int aAllowMultipleSelects);
	[DllImport("tinyfiledialogs64.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr tinyfd_selectFolderDialog(string aTitle, string aDefaultPathAndFile);
	[DllImport("tinyfiledialogs64.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr tinyfd_colorChooser(string aTitle, string aDefaultHexRGB, byte[] aDefaultRGB, byte[] aoResultRGB);

	// windows only utf16
	[DllImport("tinyfiledialogs64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
	public static extern int tinyfd_messageBoxW(string aTitle, string aMessage, string aDialogTyle, string aIconType, int aDefaultButton);
	[DllImport("tinyfiledialogs64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
	public static extern string tinyfd_saveFileDialogW(string aTitle, string aDefaultPathAndFile, int aNumOfFilterPatterns, string[] aFilterPatterns, string aSingleFilterDescription);
	[DllImport("tinyfiledialogs64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
	public static extern string tinyfd_openFileDialogW(string aTitle, string aDefaultPathAndFile, int aNumOfFilterPatterns, string[] aFilterPatterns, string aSingleFilterDescription, int aAllowMultipleSelects);
	[DllImport("tinyfiledialogs64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
	public static extern string tinyfd_selectFolderDialogW(string aTitle, string aDefaultPathAndFile);
	[DllImport("tinyfiledialogs64.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
	public static extern string tinyfd_colorChooserW(string aTitle, string aDefaultHexRGB, byte[] aDefaultRGB, byte[] aoResultRGB);

}

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
//		Debug.Log(tinyfd.StraightFromDllTestDivide(4.0f,2.0f));
//		tinyfd.tinyfd_openFileDialog("", "", 0, new string[] {}, "", 0);
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
