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

	private Animator m_animator;
	private MeshRenderer m_meshRenderer;
	private bool m_selected;
	public bool MouseDown {get; private set;}

	enum E_InputSelectType {none, selecting, unselecting}
	private static E_InputSelectType s_inputSelectType = E_InputSelectType.none;
	private static WallButton s_lastHitButton = null;

	private Vector3 m_scaleVelocity;

	void Awake()
	{
		m_animator = GetComponent<Animator>();
		m_meshRenderer = GetComponent<MeshRenderer>();
	}

	public void SetSelected(bool selected)
	{
		m_selected = selected;
		m_animator.SetBool("MouseDown", false);
		m_animator.SetBool("Selected", m_selected);
		m_meshRenderer.material = m_selected ? SelectedMaterial : UnselectedMaterial;
	}	

	public void OnMouseDown()
	{
		MouseDown = true;
		s_inputSelectType = m_selected ? E_InputSelectType.unselecting : E_InputSelectType.selecting;
		SetSelected(s_inputSelectType == E_InputSelectType.selecting);
		m_animator.SetBool("MouseDown", true);
	}

	public void OnMouseEnter()
	{
		if (Input.GetMouseButton(0))
		{
			if (s_inputSelectType == E_InputSelectType.none)
				s_inputSelectType = m_selected ? E_InputSelectType.unselecting : E_InputSelectType.selecting;
			MouseDown = true;
			SetSelected(s_inputSelectType == E_InputSelectType.selecting);
			m_animator.SetBool("MouseDown", true);
			s_lastHitButton = this;
		}
	}

	public void OnMouseExit()
	{
		MouseDown = false;
		m_animator.SetBool("MouseDown", false);
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
		Vector3 targetScale = ScaleUnselected;
		if (MouseDown)
			targetScale = ScaleMouseDown;
		else if (m_selected)
			targetScale = ScaleSelected;
		transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale, ref m_scaleVelocity, ScaleTime);
	}
}
