#if false
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WallButtons
{
	private WallButton[] m_wallButtons = new WallButton[0];
	private WallUIButton[] m_wallUIButtons = new WallUIButton[0];
    private GameObject[] m_wallDragColliders = new GameObject[0];
    private const int UIbuttonsPerRow = 3;

	private MusicWallData m_data;

	public void Create(MusicWallData data)
	{
		DestroyAll();
		m_data = data;
		m_wallButtons = new WallButton[data.CompositionData.NumRows * data.CompositionData.NumCols];
		m_wallUIButtons = new WallUIButton[data.CompositionData.InstrumentDataList.Count * UIbuttonsPerRow];
        m_wallDragColliders = new GameObject[data.CompositionData.NumCols];

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

	private void InstantiateButtons()
	{
		float colAngle = (2*Mathf.PI)/(float)m_data.CompositionData.NumCols;
		float buttonWidth = m_data.GetButtonWidth();
		float buttonPadding = m_data.ButtonPaddingFrac * buttonWidth;

		InstantiateUIButtons();
        InstantiateWallDragColliders();

        for (int iCol = 0; iCol < m_data.CompositionData.NumCols; iCol++) 
		{
			float x0 = Mathf.Sin(iCol * colAngle) * m_data.Radius;
			float z0 = Mathf.Cos(iCol * colAngle) * m_data.Radius;
			float x1 = Mathf.Sin((iCol + 1) * colAngle) * m_data.Radius;
			float z1 = Mathf.Cos((iCol + 1) * colAngle) * m_data.Radius;
			float x = (x0 + x1) * 0.5f;
			float z = (z0 + z1) * 0.5f;

			int prevRows = 0;
			for (int iInstrument = 0; iInstrument < m_data.CompositionData.InstrumentDataList.Count; iInstrument++)
			{
				for (int iRow = 0; iRow < m_data.CompositionData.InstrumentDataList[iInstrument].NumRows; iRow++) 
				{
					int currRow = iRow + prevRows;
					int rowOffsetForUI = iInstrument; 
					float y = (currRow + rowOffsetForUI) * (buttonPadding + buttonWidth) + buttonWidth * 0.5f + buttonPadding;
					var pos = new Vector3(x, y, z);
					var inst = CreateButton(m_data.ButtonPrefab, pos, buttonWidth);
					m_wallButtons[currRow + iCol*m_data.CompositionData.NumRows] = inst.GetComponentInChildren<WallButton>();
					m_wallButtons[currRow + iCol*m_data.CompositionData.NumRows].SetCoord(currRow, iCol, m_data.CompositionData);
				}
				prevRows += m_data.CompositionData.InstrumentDataList[iInstrument].NumRows;
			}
		}
	}

	private void InstantiateUIButtons()
	{
		float unitColAngle = (2*Mathf.PI)/(float)m_data.CompositionData.NumCols;
		float buttonWidth = m_data.GetButtonWidth();
		float buttonPadding = m_data.ButtonPaddingFrac * buttonWidth;

		int prevRows = 0;
		for (int iInstrument = 0; iInstrument < m_data.CompositionData.InstrumentDataList.Count; iInstrument++)
		{
			var instrument = m_data.CompositionData.InstrumentDataList[iInstrument];
			prevRows += instrument.NumRows;
			int startCol = 0;
			for (int i_btn = 0; i_btn < UIbuttonsPerRow; i_btn++)
			{
				int buttonCols = 2;
				float x0 = Mathf.Sin(startCol * unitColAngle) * m_data.Radius;
				float z0 = Mathf.Cos(startCol * unitColAngle) * m_data.Radius;
				float x1 = Mathf.Sin((startCol + buttonCols) * unitColAngle) * m_data.Radius;
				float z1 = Mathf.Cos((startCol + buttonCols) * unitColAngle) * m_data.Radius;
				float x = (x0 + x1) * 0.5f;
				float z = (z0 + z1) * 0.5f;

				int rowOffsetForUI = (iInstrument); 
				float y = (prevRows + rowOffsetForUI) * (buttonPadding + buttonWidth) + buttonWidth * 0.5f + buttonPadding;
				var pos = new Vector3(x, y, z);
				var inst = CreateButton(m_data.UIButtonPrefab, pos, buttonWidth);
				var uiBtn = inst.GetComponentInChildren<WallUIButton>();
				m_wallUIButtons[iInstrument + i_btn*UIbuttonsPerRow] = uiBtn;
				uiBtn.SetWidth(buttonCols);
				uiBtn.SetInstrument(instrument);
				uiBtn.SetButtonAction((i) => 
					{
						var scaleNames = System.Enum.GetNames(typeof(MusicScaleConverter.E_Scales));
						int newScale = ((int)i.Scale + 1) % scaleNames.Length;
						i.Scale = (MusicScaleConverter.E_ConverterType)newScale;
						uiBtn.Text.GetComponent<TextMesh>().text = scaleNames[newScale];
						m_data.CompositionData.CompositionChanged();
					});

				startCol += buttonCols;
			}
		}
    }

    private void InstantiateWallDragColliders()
    {
        float buttonWidth = m_data.GetButtonWidth();

        float colAngle = (2 * Mathf.PI) / (float)m_data.CompositionData.NumCols;
        //float buttonWidth = m_data.GetButtonWidth();
        for (int iCol = 0; iCol < m_data.CompositionData.NumCols; iCol++)
        {
            const float RADIUS_FAC = 1.2f;
            const float WIDTH_FAC = 1.5f;
            float x0 = Mathf.Sin(iCol * colAngle) * m_data.Radius * RADIUS_FAC;
            float z0 = Mathf.Cos(iCol * colAngle) * m_data.Radius * RADIUS_FAC;
            float x1 = Mathf.Sin((iCol + 1) * colAngle) * m_data.Radius * RADIUS_FAC;
            float z1 = Mathf.Cos((iCol + 1) * colAngle) * m_data.Radius * RADIUS_FAC;
            float x = (x0 + x1) * 0.5f;
            float z = (z0 + z1) * 0.5f;
            var pos = new Vector3(x, 0, z);
            m_wallDragColliders[iCol] = CreateWallDragCollider(pos, buttonWidth * WIDTH_FAC);
        }
    
    }


    private GameObject CreateButton(GameObject prefab, Vector3 pos, float buttonWidth)
	{
		var posRot = new Vector3(pos.x, 0, pos.z);
		var inst = GameObject.Instantiate(prefab, pos, Quaternion.LookRotation(-posRot));
		inst.transform.SetParent(m_data.Parent, false);
		inst.transform.localScale = new Vector3(buttonWidth, buttonWidth, buttonWidth);
		return inst;
	}

    private GameObject CreateWallDragCollider(Vector3 pos, float buttonWidth)
    {
        var h = m_data.GetTotalHeight();
        var posRot = new Vector3(pos.x, 0, pos.z);
        pos.y += h * 0.5f;
        var inst = GameObject.Instantiate(m_data.GrabbableWallCollider, pos, Quaternion.LookRotation(-posRot));
        inst.transform.SetParent(m_data.Parent, false);
        const float Z_THICKNESS = 0.1f;
        inst.transform.localScale = new Vector3(buttonWidth, h, Z_THICKNESS);
        inst.AddComponent<BoxCollider>();
        return inst;
    }
}

#endif