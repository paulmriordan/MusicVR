using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallButton : MonoBehaviour 
{
	public Material SelectedMaterial;
	public Material UnselectedMaterial;

	private MeshRenderer m_meshRenderer;
	private bool m_selected;

	void Awake()
	{
		m_meshRenderer = GetComponent<MeshRenderer>();
	}

	public void SetSelected(bool selected)
	{
		m_selected = selected;
		m_meshRenderer.material = m_selected ? SelectedMaterial: UnselectedMaterial;
	}	

	public void OnMouseDown()
	{
//		SetSelected(!m_selected);
	}

	public void OnMouseUp()
	{
		SetSelected(!m_selected);
	}
}
