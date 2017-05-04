using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls whether the buttons color and materials. In normal state, all buttons share
/// selected and unselected materials. When the sequencer is playing, they temporarily
/// create a unique 'playingMaterial' instance to fade from the selected color to the playing color.
/// After playing fade has finished, this unique material is dicarded.
/// </summary>
public class WallButtonColorController : MonoBehaviour
{
	[ColorUsageAttribute(true, true, 0, 8, 0.125f, 3)]
	public Color PlayingColor;
	public float PlayingFadeTime = 0.1f;
	public MeshRenderer MeshRenderer;

	private float m_playingFadeTarget = 0;
	private float m_playingFadeProg = 0;
	private float m_playingFadeVel = 0;
	private Material m_playingMaterial;
	private Color m_selectedColor;
	private Material m_selectedMaterial;
	private Material m_unselectedMaterial;

	public bool Selected {get; set;}

	public void SetPlaying(bool playing)
	{
		m_playingFadeTarget = playing ? 1.0f : 0;
	}

	public void SetMaterials(Material selectedMaterial, Material unselectedMaterial)
	{
		m_selectedMaterial = selectedMaterial;
		m_unselectedMaterial = unselectedMaterial;
		RegeneratePlayingMaterial();
	}

	public void CustomUpdate()
	{
		UpdateSetMaterial();
		UpdateColor();
	}

	private void UpdateColor()
	{
		if (Selected && MeshRenderer.sharedMaterial == m_playingMaterial)
		{
			m_playingFadeProg = Mathf.SmoothDamp(m_playingFadeProg, m_playingFadeTarget, ref m_playingFadeVel, PlayingFadeTime);
			if (m_playingFadeProg < 0.001f)
				m_playingFadeProg = 0;
			Color targetColor = PlayingColor * (m_playingFadeProg) + m_selectedColor * (1.0f - m_playingFadeProg);
			MeshRenderer.sharedMaterial.SetColor("_EmissionColor", targetColor);
		}
	}

	private void RegeneratePlayingMaterial()
	{
		if (m_selectedMaterial == null)
			return;
		if (m_playingMaterial != null)
			Destroy(m_playingMaterial);
		m_playingMaterial = new Material(m_selectedMaterial);
		m_playingMaterial.name = "playing material";
		m_selectedColor = m_selectedMaterial.GetColor("_EmissionColor");
	}

	private void UpdateSetMaterial()
	{
		if (!Selected)
		{
			MeshRenderer.sharedMaterial = m_unselectedMaterial;
		}
		else 
		{
			if (m_playingFadeProg != 0 || m_playingFadeTarget == 1.0f)
				MeshRenderer.sharedMaterial = m_playingMaterial;
			else 
				MeshRenderer.sharedMaterial = m_selectedMaterial;
		}
	}
}

