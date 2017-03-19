using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallButton : MonoBehaviour 
{
	public Material SelectedMaterial;
	public Material UnselectedMaterial;

	private MeshRenderer m_meshRenderer;

	void Awake()
	{
		m_meshRenderer = GetComponent<MeshRenderer>();
	}

	public void SetSelected(bool selected)
	{
		m_meshRenderer.material = selected ? SelectedMaterial: UnselectedMaterial;
	}	
}
