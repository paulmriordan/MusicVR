using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallButton : MonoBehaviour 
{
	public Material SelectedMaterial;
	public Material UnselectedMaterial;
	public float ScaleTime = 0.2f;
	public Vector3 ScaleSelected = new Vector3(1.0f,1.0f,0.8f);
	public Vector3 ScaleMouseDown = new Vector3(1.2f,1.2f,0.6f);
	public Vector3 ScaleUnselected = new Vector3(1.0f,1.0f,1.0f);
	public Color PlayingColor;
	public Color SelectedColor;
	public float PlayingFadeTime = 0.1f;

	public MeshRenderer MeshRenderer;
	private bool m_selected;
	private float m_playingFadeTarget = 0;
	private float m_playingFadeProg = 0;
	private float m_playingFadeVel = 0;
	private Material m_playingMaterial;

	public bool MouseDown {get; private set;}

	enum E_InputSelectType {none, selecting, unselecting}
	private static E_InputSelectType s_inputSelectType = E_InputSelectType.none;
	private static WallButton s_lastHitButton = null;
	private static bool s_selectionEnabled = true;

	private Vector3 m_scaleVelocity;

	void Awake()
	{
		m_playingMaterial = new Material(SelectedMaterial);
		m_playingMaterial.name = "playing material";
	}

	public static void SelectionEnabled(bool enabled)
	{
		s_selectionEnabled = enabled;	
	}

	public void SetSelected(bool selected)
	{
		m_selected = selected;
	}	

	public void SetPlaying(bool playing)
	{
		m_playingFadeTarget = playing ? 1.0f : 0;
	}

	public void OnMouseDown()
	{
		if (s_selectionEnabled)
		{
			MouseDown = true;
			s_inputSelectType = m_selected ? E_InputSelectType.unselecting : E_InputSelectType.selecting;
			SetSelected(s_inputSelectType == E_InputSelectType.selecting);
		}
	}

	public void OnMouseEnter()
	{
		if (Input.GetMouseButton(0) && s_selectionEnabled)
		{
			if (s_inputSelectType == E_InputSelectType.none)
				s_inputSelectType = m_selected ? E_InputSelectType.unselecting : E_InputSelectType.selecting;
			MouseDown = true;
			SetSelected(s_inputSelectType == E_InputSelectType.selecting);
			s_lastHitButton = this;
		}
	}

	public void OnMouseExit()
	{
		MouseDown = false;
	}

	public void OnMouseUp()
	{
		MouseDown = false;
		if (s_lastHitButton)
			s_lastHitButton.MouseDown = false;
		s_inputSelectType = E_InputSelectType.none;
	}

	public void CustomUpdate()
	{
		UpdateColor();
		UpdateSetMaterial();
		UpdateScale();
	}

	private void UpdateColor()
	{
		if (m_selected && MeshRenderer.sharedMaterial == m_playingMaterial)
		{
			m_playingFadeProg = Mathf.SmoothDamp(m_playingFadeProg, m_playingFadeTarget, ref m_playingFadeVel, PlayingFadeTime);
			if (m_playingFadeProg < 0.001f)
				m_playingFadeProg = 0;
			Color targetColor = PlayingColor * (m_playingFadeProg) + SelectedColor * (1.0f - m_playingFadeProg);
			MeshRenderer.sharedMaterial.SetColor("_EmissionColor", targetColor);
		}
	}

	private void UpdateSetMaterial()
	{
		if (!m_selected)
		{
			MeshRenderer.sharedMaterial = UnselectedMaterial;
		}
		else 
		{
			if (m_playingFadeProg != 0 || m_playingFadeTarget == 1.0f)
				MeshRenderer.sharedMaterial = m_playingMaterial;
			else 
				MeshRenderer.sharedMaterial = SelectedMaterial;
		}
	}

	private void UpdateScale()
	{
		Vector3 targetScale = ScaleUnselected;
		if (MouseDown)
			targetScale = ScaleMouseDown;
		else if (m_selected)
			targetScale = ScaleSelected;
		if ((targetScale - MeshRenderer.transform.localScale).sqrMagnitude < 0.001f)
			MeshRenderer.transform.localScale = targetScale;
		else
			MeshRenderer.transform.localScale = Vector3.SmoothDamp(MeshRenderer.transform.localScale, 
				targetScale, 
				ref m_scaleVelocity,
				ScaleTime);
	}
}
