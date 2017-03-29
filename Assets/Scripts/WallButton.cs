using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WallButton : MonoBehaviour 
{
	public class WallButtonInputConsumer : InputConsumerBase
	{
		public override bool TryConsumeInput() 
		{
			return false;
		}
	}

	public class WallButtonInputState
	{
		public enum E_SelectState {none, selecting, unselecting}

		private bool m_selectionEnabled = true;

		public E_SelectState InputSelectType {get;set;}
		public WallButton LastHitButton {get;set;}
		public bool IsSelectionEnabled { get { return m_selectionEnabled;}}

		public bool WallInputActive()
		{
			return InputSelectType != E_SelectState.none;
		}

		public void SelectionEnabled(bool enabled)
		{
			m_selectionEnabled = enabled;	
		}

		public void Clear()
		{
			LastHitButton = null;
			InputSelectType = E_SelectState.none;
		}
	}

	static WallButtonInputConsumer m_wallButtonInput = new WallButtonInputConsumer();
	public static WallButtonInputState s_wallButtonInputState = new WallButtonInputState();

	public Material SelectedMaterial;
	public Material UnselectedMaterial;
	public float ScaleTime = 0.2f;
	public Vector3 ScaleSelected = new Vector3(1.0f,1.0f,0.8f);
	public Vector3 ScaleMouseDown = new Vector3(1.2f,1.2f,0.6f);
	public Vector3 ScaleUnselected = new Vector3(1.0f,1.0f,1.0f);
	[ColorUsageAttribute(true, true, 0, 8, 0.125f, 3)]
	public Color PlayingColor;
	[ColorUsageAttribute(true, true, 0, 8, 0.125f, 3)]
	public Color SelectedColor;
	public float PlayingFadeTime = 0.1f;
	public MeshRenderer MeshRenderer;

	private bool m_selected;
	private float m_playingFadeTarget = 0;
	private float m_playingFadeProg = 0;
	private float m_playingFadeVel = 0;
	private Material m_playingMaterial;
	private Vector3 m_scaleVelocity;

	public bool MouseDown {get; private set;}
	public bool IsSelected {get { return m_selected;}}

	void Awake()
	{
		m_playingMaterial = new Material(SelectedMaterial);
		m_playingMaterial.name = "playing material";
	}


	public void SetSelected(bool selected)
	{
		m_selected = selected;
	}

	public void SetPlaying(bool playing)
	{
		m_playingFadeTarget = playing ? 1.0f : 0;
	}

	public void OnTouchStay()
	{
		if (!Input.GetMouseButton(0))
			return;
		Debug.Log("OnMouseOver");
		bool allowStart = false;
		{
			allowStart = s_wallButtonInputState.InputSelectType == WallButtonInputState.E_SelectState.none;
			allowStart &= !InputManager.Instance.IsCameraDragOccuring();
			allowStart &= Time.time - InputManager.Instance.inputDownTime > InputManager.Instance.HoldTime;
			allowStart &= s_wallButtonInputState.IsSelectionEnabled && !EventSystem.current.IsPointerOverGameObject();
		}
		bool allowSubsequent = false;
		{
			allowSubsequent = s_wallButtonInputState.InputSelectType != WallButtonInputState.E_SelectState.none;
			allowSubsequent &= this != s_wallButtonInputState.LastHitButton;
			allowSubsequent &= s_wallButtonInputState.IsSelectionEnabled && !EventSystem.current.IsPointerOverGameObject();
		}
		if (allowStart)
		{
			Debug.Log("allow start");
			MouseDown = true;
			s_wallButtonInputState.InputSelectType = m_selected 
				? WallButtonInputState.E_SelectState.unselecting 
				: WallButtonInputState.E_SelectState.selecting;
			SetSelected(s_wallButtonInputState.InputSelectType == WallButtonInputState.E_SelectState.selecting);
		}
		else if (allowSubsequent)
		{
			Debug.Log("allow subsequent");
			MouseDown = true;
			SetSelected(s_wallButtonInputState.InputSelectType == WallButtonInputState.E_SelectState.selecting);
			s_wallButtonInputState.LastHitButton = this;
		}
	}

	public void OnTouchExit()
	{
		Debug.Log("OnMouseExit");
		MouseDown = false;
	}

	public void OnTouchUp()
	{
		Debug.Log("OnMouseUp");
		// Click button if no camera drag occurred & no button selecting occurred
		if (s_wallButtonInputState.InputSelectType == WallButtonInputState.E_SelectState.none)
		{
			s_wallButtonInputState.InputSelectType = m_selected 
				? WallButtonInputState.E_SelectState.unselecting 
				: WallButtonInputState.E_SelectState.selecting;
			
			SetSelected(s_wallButtonInputState.InputSelectType == WallButtonInputState.E_SelectState.selecting);
		}

		MouseDown = false;
		if (s_wallButtonInputState.LastHitButton)
			s_wallButtonInputState.LastHitButton.MouseDown = false;
		s_wallButtonInputState.Clear();
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
