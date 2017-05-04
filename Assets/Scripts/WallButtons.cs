using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CompositionCommands;

public class WallButtons
{
	private WallButton[] m_wallButtons = new WallButton[0];
	private WallUIButton[] m_wallUIButtons = new WallUIButton[0];

	private MusicWallData m_data;

	public void Create(MusicWallData data)
	{
		DestroyAll();

		m_data = data;
		m_wallButtons = new WallButton[data.CompositionData.NumRows * data.CompositionData.NumCols];
		m_wallUIButtons = new WallUIButton[data.CompositionData.InstrumentDataList.Count * InstrumentUIData.Instance.Buttons.Count];

		CreateUIButtons();
		CreateSequencerButtons();

		LoadMusicData(data.CompositionData);
	}

	public void LoadMusicData(CompositionData musicData)
	{
		if (m_wallButtons.Length != musicData.Size)
		{
			Debug.LogError("lengths don't match?  wallbuttons: " + m_wallButtons.Length + " musicData: " +  musicData.Size);
			return;
		}
		for (int iCol = 0; iCol < musicData.NumCols; iCol++) 
		{
			for (int iRow = 0; iRow < musicData.NumRows; iRow++) 
			{
				var button = m_wallButtons[iRow + iCol*musicData.NumRows];
				var noteActive = musicData.IsNoteActive(iRow, iCol);
				var instrument = musicData.GetInstrumentAtLocation(iRow, iCol);
				LoadMusicDataForButton(button, noteActive, instrument);
			}
		}
	}

	public WallButton GetSequencerButton(int row, int col)
	{
		return m_wallButtons[row + col*m_data.CompositionData.NumRows];
	}

	public void DestroyAll()
	{
		for (int i = m_wallButtons.Length - 1; i > 0; i--)
			GameObject.Destroy(m_wallButtons[i].transform.parent.gameObject);
		
		for (int i = m_wallUIButtons.Length - 1; i > 0; i--)
			GameObject.Destroy(m_wallUIButtons[i].transform.parent.gameObject);
	}

	public void Update()
	{
		for (int i = 0; i < m_wallButtons.Length; i++)
			m_wallButtons[i].CustomUpdate();
		for (int i = 0; i < m_wallUIButtons.Length; i++)
			m_wallUIButtons[i].CustomUpdate();
	}

	private void LoadMusicDataForButton(WallButton btn, bool noteActive, CompositionData.InstrumentData instrumentData)
	{
		btn.SetSelected(noteActive);
		btn.ColorController.SetMaterials(
			instrumentData.InstrumentDefintion.SelectedButtonMaterial,
			instrumentData.InstrumentDefintion.UnselectedButtonMaterial);
	}

	private void CreateUIButtons()
	{
		int prevRows = 0;
		for (int iInstrument = 0; iInstrument < m_data.CompositionData.InstrumentDataList.Count; iInstrument++)
		{
			prevRows += m_data.CompositionData.InstrumentDataList[iInstrument].NumRows;
			CreateUIButtonsForInstrument(prevRows, iInstrument);
		}
	}

	private void CreateUIButtonsForInstrument(int rowOffset, int iInstrument)
	{
		float buttonWidth = m_data.GetButtonWidth();
		float buttonPadding = m_data.ButtonPaddingFrac * buttonWidth;
		float unitColAngle = (2*Mathf.PI)/(float)m_data.CompositionData.NumCols;

		int startCol = 0;

		int numButtons = InstrumentUIData.Instance.Buttons.Count;
		for (int i_btn = 0; i_btn < numButtons; i_btn++)
		{
			var buttonData = InstrumentUIData.Instance.Buttons[i_btn];

			var pos = GetXZPosition(startCol, startCol + buttonData.Width, unitColAngle, m_data.Radius);
			pos.y = GetButtonYPos(rowOffset, iInstrument, buttonWidth, buttonPadding);

			var inst = CreateButtonInstance(m_data.UIButtonPrefab, pos, buttonWidth);
			SetupUIButton(inst, i_btn, iInstrument);

			startCol += buttonData.Width; // next UI button is placed on following column
		}
	}

	private void SetupUIButton(GameObject buttonInstance, int btnIndex, int instrumentIndex)
	{
		var uiBtn = buttonInstance.GetComponentInChildren<WallUIButton>();
		var buttonData = InstrumentUIData.Instance.Buttons[btnIndex];
		var instrumentData = m_data.CompositionData.InstrumentDataList[instrumentIndex];

		uiBtn.Init(buttonData, m_data.CompositionData, instrumentData);

		// assign ref to button
		m_wallUIButtons[instrumentIndex *  InstrumentUIData.Instance.Buttons.Count + btnIndex] = uiBtn;
	}

	private void CreateSequencerButtons()
	{
		float unitColAngle = (2*Mathf.PI)/(float)m_data.CompositionData.NumCols;

		for (int iCol = 0; iCol < m_data.CompositionData.NumCols; iCol++) 
		{
			var pos = GetXZPosition(iCol, iCol + 1, unitColAngle, m_data.Radius);
			CreateSequencerButtonsForCol(iCol, pos);
		}
	}

	private Vector3 GetXZPosition(int startCol, int endCol, float unitColAngle, float radius)
	{
		float x0 = Mathf.Sin(startCol * unitColAngle) * radius;
		float z0 = Mathf.Cos(startCol * unitColAngle) * radius;
		float x1 = Mathf.Sin(endCol * unitColAngle) * radius;
		float z1 = Mathf.Cos(endCol * unitColAngle) * radius;
		float x = (x0 + x1) * 0.5f;
		float z = (z0 + z1) * 0.5f;
		return new Vector3(x, 0, z);
	}

	private void CreateSequencerButtonsForCol(int iCol, Vector3 colPos)
	{
		float buttonWidth = m_data.GetButtonWidth();
		float buttonPadding = m_data.ButtonPaddingFrac * buttonWidth;

		int prevRows = 0;
		for (int iInstrument = 0; iInstrument < m_data.CompositionData.InstrumentDataList.Count; iInstrument++)
		{
			for (int iRow = 0; iRow < m_data.CompositionData.InstrumentDataList[iInstrument].NumRows; iRow++) 
			{
				int currRow = iRow + prevRows;

				colPos.y = GetButtonYPos(currRow, iInstrument, buttonWidth, buttonPadding);
				var instance = CreateButtonInstance(m_data.ButtonPrefab, colPos, buttonWidth);

				SetupButton(currRow, iCol, instance);
			}
			prevRows += m_data.CompositionData.InstrumentDataList[iInstrument].NumRows;
		}
	}

	private float GetButtonYPos(int sequencerRowIndex, int instrumentIndex, float buttonWidth, float buttonPadding)
	{
		return (sequencerRowIndex + instrumentIndex) * (buttonPadding + buttonWidth) + buttonWidth * 0.5f + buttonPadding;
	}

	private GameObject CreateButtonInstance(GameObject prefab, Vector3 pos, float sclae)
	{
		var posRot = new Vector3(pos.x, 0, pos.z);
		var inst = GameObject.Instantiate(prefab, pos, Quaternion.LookRotation(-posRot));
		inst.transform.SetParent(m_data.Parent, false);
		inst.transform.localScale = new Vector3(sclae, sclae, sclae);
		return inst;
	}

	private void SetupButton(int row, int col, GameObject button)
	{
		var wallButton = button.GetComponentInChildren<WallButton>();
		wallButton.SetCoord(row, col, m_data.CompositionData);
		m_wallButtons[row + col * m_data.CompositionData.NumRows] = wallButton;
	}

}
