using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallButtons
{
	private WallButton[] m_wallButtons = new WallButton[0];
	private MusicWallData m_data;

	public void Create(MusicWallData data)
	{
		DestroyAll();
		m_data = data;
		m_wallButtons = new WallButton[data.CompositionData.NumRows * data.CompositionData.NumCols];
		InstantiateButtons();
		LoadMusicData(data.CompositionData);
	}

	public void LoadMusicData(CompositionData musicData)
	{
		if (m_wallButtons.Length != musicData.Size)
		{
			Debug.LogError("lengths don't match??  wallbuttons: " + m_wallButtons.Length + " musicData: " +  musicData.Size);
			return;
		}
		for (int iCol = 0; iCol < musicData.NumCols; iCol++) 
		{
			for (int iRow = 0; iRow < musicData.NumRows; iRow++) 
			{
				var btn = m_wallButtons[iRow + iCol*musicData.NumRows];
				btn.SetSelected(musicData.IsNoteActive(iRow, iCol));

				var instrumentData = musicData.GetInstrumentAtLocation(iRow, iCol);
				btn.SelectedMaterial = instrumentData.InstrumentDefintion.SelectedButtonMaterial;
				btn.UnselectedMaterial = instrumentData.InstrumentDefintion.UnselectedButtonMaterial;
				btn.RegeneratePlayingMaterial();
			}
		}
	}

	public WallButton GetButton(int row, int col)
	{
		return m_wallButtons[row + col*m_data.CompositionData.NumRows];
	}

	public void DestroyAll()
	{
		for (int i = m_wallButtons.Length - 1; i > 0; i--)
		{
			GameObject.Destroy(m_wallButtons[i].transform.parent.gameObject);
		}
	}

	public void Update()
	{
		for (int i = 0; i < m_wallButtons.Length; i++)
			m_wallButtons[i].CustomUpdate();
	}

	private void InstantiateButtons()
	{
		float colAngle = (2*Mathf.PI)/(float)m_data.CompositionData.NumCols;
		float buttonWidth = m_data.GetButtonWidth();
		float buttonPadding = m_data.ButtonPaddingFrac * buttonWidth;

		for (int iCol = 0; iCol < m_data.CompositionData.NumCols; iCol++) 
		{
			float x0 = Mathf.Sin(iCol * colAngle) * m_data.Radius;
			float z0 = Mathf.Cos(iCol * colAngle) * m_data.Radius;
			float x1 = Mathf.Sin((iCol + 1) * colAngle) * m_data.Radius;
			float z1 = Mathf.Cos((iCol + 1) * colAngle) * m_data.Radius;
			float x = (x0 + x1) * 0.5f;
			float z = (z0 + z1) * 0.5f;

			for (int iRow = 0; iRow < m_data.CompositionData.NumRows; iRow++) 
			{
				float y = iRow * (buttonPadding + buttonWidth) + buttonWidth * 0.5f + buttonPadding;
				var pos = new Vector3(x, y, z);
				var posRot = new Vector3(x, 0, z);
				var inst = GameObject.Instantiate(m_data.Prefab, pos, Quaternion.LookRotation(-posRot));
				inst.transform.SetParent(m_data.Parent, false);
				inst.transform.localScale = new Vector3(buttonWidth, buttonWidth, buttonWidth);
				m_wallButtons[iRow + iCol*m_data.CompositionData.NumRows] = inst.GetComponentInChildren<WallButton>();
			}
		}
	}

}
