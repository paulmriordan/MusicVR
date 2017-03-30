using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveFileDialog : MonoBehaviour {

	public enum E_DialogState {save, load};
	
	public int NumSlots = 0;
	public GameObject SaveFileButtonPrefab;
	public GridLayoutGroup Grid;

	private E_DialogState m_activeState;

	void Awake() 
	{
		for (int i = 0; i < NumSlots; i++)
		{
			var newObj = (GameObject.Instantiate(SaveFileButtonPrefab) as GameObject).GetComponent<SaveFileButton>();
			newObj.transform.SetParent(Grid.transform);
			newObj.SaveFileDialog = this;
		}
		Hide();
	}

	public void Clicked(SaveFileButton clicked)
	{
		int index = GetButtonIndexInGrid(clicked);
		if (index == -1)
		{
			Debug.LogError("Could not find button in grid");
			return;
		}

		switch (m_activeState)
		{
		case E_DialogState.save:
			if(SaveFileExists(index))
			{
				GenericPopup.Instance.Show2ButtonPopup(
					Localization.Get("L_OVERWRITE_POPUP"), 
					Localization.Get("L_YES"),
					Localization.Get("L_NO"),
					() => {SaveToFile(index);});
			}
			else 
				SaveToFile(index);
			break;
		case E_DialogState.load:
			if(SaveFileExists(index))
			{
				GenericPopup.Instance.Show2ButtonPopup(
					Localization.Get("L_LOAD_POPUP"),
					Localization.Get("L_YES"),
					Localization.Get("L_NO"),
					() => {LoadFromFile(index);});
			}
			break;
		}
	}

	public void Show(E_DialogState state)
	{
		m_activeState = state;
		gameObject.SetActive(true);

		var info = new DirectoryInfo(Application.persistentDataPath);
		var fileInfo = info.GetFiles();

		//set all buttons as empty
		{
			for (int i = 0; i < Grid.transform.childCount; i++) 
			{	
				var button = Grid.transform.GetChild(i);
				if (button != null)
				{
					var saveFileButton = button.GetComponent<SaveFileButton>();
					saveFileButton.Setup(Localization.Get("L_SAVE_EMPTY"), true);
				}
			}
		}

		//Check files and set buttons as occupied
		{
			for (int i = 0; i < fileInfo.Length; i++) 
			{
				var file = fileInfo [i];
				var split = file.Name.Split('_');
				int number;
				if (split.Length > 2 && int.TryParse(split[split.Length - 2], out number))
				{
					var button = Grid.transform.GetChild(number);
					if (button != null)
					{
						var saveFileButton = button.GetComponent<SaveFileButton>();
						saveFileButton.Setup(file.LastWriteTime.ToString(), false);
					}
				}
			}
		}
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	int GetButtonIndexInGrid(SaveFileButton button)
	{
		var buttonTransform = button.transform;
		for (int i = 0; i < Grid.transform.childCount; i++) 
		{	
			if (Grid.transform.GetChild(i) == buttonTransform)
			{
				return i;
			}
		}
		return -1;
	}

	void SaveToFile(int number)
	{
		BinaryFormatter bf = new BinaryFormatter();
		using (FileStream file = File.Create (Application.persistentDataPath + "/save_" + number + "_.mwd"))
		{
			bf.Serialize(file, MusicWall.Instance.WallProperties.CompositionData);
		}
		// file.Close();
		Hide();
	}

	bool SaveFileExists(int number)
	{
		return (File.Exists(Application.persistentDataPath + "/save_" + number + "_.mwd"));
	}

	void LoadFromFile(int number) 
	{
		if(SaveFileExists(number))
		{
			BinaryFormatter bf = new BinaryFormatter();
			using (FileStream file = File.Open(Application.persistentDataPath + "/save_" + number + "_.mwd", FileMode.Open))
			{
				MusicWall.Instance.WallProperties.CompositionData = (CompositionData)bf.Deserialize(file);
				MusicWall.Instance.NeedsUpdate = true;
			}
			Hide();
		}
	}
}
