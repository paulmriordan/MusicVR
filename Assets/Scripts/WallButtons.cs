using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallButtons
{
	private WallButton[] m_wallButtons = new WallButton[0];
	private WallProperties m_properties;

	public void Create(WallProperties args)
	{
		DestroyAll();
		m_properties = args;
		m_wallButtons = new WallButton[args.NumRows*args.NumCols];
		InstantiateButtons();
	}

	public void LoadMusicData(WallMusicData musicData)
	{
		if (m_wallButtons.Length != musicData.Size)
		{
			Debug.LogError("lengths don't match??  wallbuttons: " + m_wallButtons.Length + " musicData: " +  musicData.Size);
			return;
		}
		for (int iCol = 0; iCol < m_properties.NumCols; iCol++) 
		{
			for (int iRow = 0; iRow < m_properties.NumRows; iRow++) 
			{
				m_wallButtons[iRow + iCol*m_properties.NumRows].SetSelected(musicData.IsNoteActive(iRow, iCol));
			}
		}
	}

	public WallButton GetButton(int row, int col)
	{
		return m_wallButtons[row + col*m_properties.NumRows];
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
		float colAngle = (2*Mathf.PI)/(float)m_properties.NumCols;
		float buttonWidth = m_properties.GetButtonWidth();
		float buttonPadding = m_properties.ButtonPaddingFrac * buttonWidth;

		for (int iCol = 0; iCol < m_properties.NumCols; iCol++) 
		{
			float x0 = Mathf.Sin(iCol * colAngle) * m_properties.Radius;
			float z0 = Mathf.Cos(iCol * colAngle) * m_properties.Radius;
			float x1 = Mathf.Sin((iCol + 1) * colAngle) * m_properties.Radius;
			float z1 = Mathf.Cos((iCol + 1) * colAngle) * m_properties.Radius;
			float x = (x0 + x1) * 0.5f;
			float z = (z0 + z1) * 0.5f;

			for (int iRow = 0; iRow < m_properties.NumRows; iRow++) 
			{
				float y = iRow * (buttonPadding + buttonWidth) + buttonWidth * 0.5f + buttonPadding;
				var pos = new Vector3(x, y, z);
				var posRot = new Vector3(x, 0, z);
				var inst = GameObject.Instantiate(m_properties.Prefab, pos, Quaternion.LookRotation(-posRot));
				inst.transform.SetParent(m_properties.Parent, false);
				inst.transform.localScale = new Vector3(buttonWidth, buttonWidth, buttonWidth);
				m_wallButtons[iRow + iCol*m_properties.NumRows] = inst.GetComponentInChildren<WallButton>();
			}
		}
	}

}
