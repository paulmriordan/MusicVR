using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveFileDialog : MonoBehaviour {

	public int NumSlots = 0;
	public GameObject SaveFileButtonPrefab;
	public GridLayoutGroup Grid;

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
		GenericPopup.Instance.Show2ButtonPopup(Localization.Get("L_OVERWRITE_POPUP"), Localization.Get("L_YES"), Localization.Get("L_NO"));
	}

	public void Show()
	{
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}
}
