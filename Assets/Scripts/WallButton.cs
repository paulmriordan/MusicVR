//#def MOUSE_INPUT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WallButton : MonoBehaviour 
{
	public class WallButtonInputConsumer : InputConsumerBase
	{
		public override bool TryConsumeInput(InputManager.InputState state) 
		{
			if (Input.GetMouseButtonUp(0))
				return true; //always consume button up events
			if (!Input.GetMouseButton(0))
				return false;
			// Always consume, regardless of position, if held for long enough
			return Time.time - state.inputDownTime > state.HoldTime;
		}

		public override bool IsFinished()
		{
			return !Input.GetMouseButton(0);
		}
	}

	public class WallButtonInputState
	{
		public event System.Action<Vector2> OnPanRequested = (p) => {};

		public enum E_SelectState {none, selecting, unselecting}

		private bool m_selectionEnabled = true;
		private WallButtonInputConsumer m_buttonInputConsumer = new WallButtonInputConsumer();

		public E_SelectState InputSelectType {get;set;}
		public WallButton LastHitButton {get;set;}
		public bool IsSelectionEnabled { get { return m_selectionEnabled && m_buttonInputConsumer.IsActive();}}

		public bool WallInputActive()
		{
			return InputSelectType != E_SelectState.none;
		}

		public void SelectionEnabled(bool enabled)
		{
			m_selectionEnabled = enabled;	
		}

		public void Update(InputManager.InputState state)
		{
			
			if (IsSelectionEnabled 
				&& EdgePanningUtil.DragTreshReached((Input.mousePosition - state.inputDownPos).magnitude,
					state.ThresholdStartEdgePan))
			{
				var pan = EdgePanningUtil.EdgeDragPanAmount(Input.mousePosition, 
															state.EdgeDistPan.x,
															state.EdgeDistPan.y);	
				if (pan.sqrMagnitude > 0)
					OnPanRequested(pan);
			}
		}

		public void Clear()
		{
			LastHitButton = null;
			InputSelectType = E_SelectState.none;
		}
	}

	public static WallButtonInputState s_wallButtonInputState = new WallButtonInputState();

	[ColorUsageAttribute(true, true, 0, 8, 0.125f, 3)]
	public Color PlayingColor;
	public float PlayingFadeTime = 0.1f;
	public MeshRenderer MeshRenderer;

	private bool m_selected;
	private float m_playingFadeTarget = 0;
	private float m_playingFadeProg = 0;
	private float m_playingFadeVel = 0;
	private Material m_playingMaterial;
	private Color m_selectedColor;
	private CompositionData m_compositionData;
	private int m_row;
	private int m_col;
	private WallButtonTween m_buttonTweener;
	private bool m_mouseDown;

	public Material SelectedMaterial {get;set;}
	public Material UnselectedMaterial {get;set;}
	public bool MouseDown { 
		get {
			return m_mouseDown;
		}
		set {
			m_mouseDown = value;
			if (m_buttonTweener != null)
				m_buttonTweener.Held = value;
		}
	}
	public bool IsSelected {get { return m_selected;}}

	void Awake()
	{
		m_buttonTweener = GetComponent<WallButtonTween>();
	}

	public void SetCoord(int row, int col, CompositionData compositionRef)
	{
		m_row = row;
		m_col = col;
		m_compositionData = compositionRef;
	}

	public void RegeneratePlayingMaterial()
	{
		if (SelectedMaterial == null)
			return;
		if (m_playingMaterial != null)
			Destroy(m_playingMaterial);
		m_playingMaterial = new Material(SelectedMaterial);
		m_playingMaterial.name = "playing material";
		m_selectedColor = SelectedMaterial.GetColor("_EmissionColor");
	}

	public void SetSelected(bool selected)
	{
		m_selected = selected;
		if (m_buttonTweener != null)
			m_buttonTweener.Selected = selected;
		m_compositionData.SetNoteActive(m_row, m_col, selected);
	}

	public void SetPlaying(bool playing)
	{
		m_playingFadeTarget = playing ? 1.0f : 0;
	}

#region Input Events
	public void OnTouchStay()
	{
        if (!Input.GetMouseButton(0))
			return;

		TryStartSelection();
		TrySubsequentSelection();
	}

	public void OnTouchExit()
	{
		MouseDown = false;
	}

	public void OnTouchUp()
	{
		// Click button if no camera drag occurred & no button selecting occurred
		if (s_wallButtonInputState.IsSelectionEnabled && !s_wallButtonInputState.WallInputActive())
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
    #endregion

    #region VR Input Events
    public void OnPointerSelectedButtonPressed()
    {
        TryStartVRSelection();
        TrySubsequenVRtSelection();
    }

    //public void OnTouchExit()
    //{
    //    MouseDown = false;
    //}

    public void OnPointerSelectedButtonReleased()
    {
        // Click button if no camera drag occurred & no button selecting occurred
        if (!s_wallButtonInputState.WallInputActive())
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

    void TryStartVRSelection()
    {
        if (s_wallButtonInputState.InputSelectType == WallButtonInputState.E_SelectState.none)
        {
            //bool allowStart = s_wallButtonInputState.IsSelectionEnabled;
            //allowStart &= !EventSystem.current.IsPointerOverGameObject();
            //if (allowStart)
            {
                MouseDown = true;
                s_wallButtonInputState.InputSelectType = m_selected
                    ? WallButtonInputState.E_SelectState.unselecting
                    : WallButtonInputState.E_SelectState.selecting;
                SetSelected(s_wallButtonInputState.InputSelectType == WallButtonInputState.E_SelectState.selecting);
            }
        }
    }


    void TrySubsequenVRtSelection()
    {
        if (s_wallButtonInputState.InputSelectType != WallButtonInputState.E_SelectState.none)
        {
            bool allowSubsequent = this != s_wallButtonInputState.LastHitButton;
            //allowSubsequent &= s_wallButtonInputState.IsSelectionEnabled && !EventSystem.current.IsPointerOverGameObject();
            if (allowSubsequent)
            {
                MouseDown = true;
                SetSelected(s_wallButtonInputState.InputSelectType == WallButtonInputState.E_SelectState.selecting);
                s_wallButtonInputState.LastHitButton = this;
            }
        }
    }

    #endregion



    void TryStartSelection()
	{
		if (s_wallButtonInputState.InputSelectType == WallButtonInputState.E_SelectState.none)
		{
			bool allowStart = s_wallButtonInputState.IsSelectionEnabled;
			allowStart &= !EventSystem.current.IsPointerOverGameObject();
			if (allowStart)
			{
				MouseDown = true;
				s_wallButtonInputState.InputSelectType = m_selected 
					? WallButtonInputState.E_SelectState.unselecting 
					: WallButtonInputState.E_SelectState.selecting;
				SetSelected(s_wallButtonInputState.InputSelectType == WallButtonInputState.E_SelectState.selecting);
			}
		}
	}

	void TrySubsequentSelection()
	{
		if (s_wallButtonInputState.InputSelectType != WallButtonInputState.E_SelectState.none)
		{
			bool allowSubsequent = this != s_wallButtonInputState.LastHitButton;
			allowSubsequent &= s_wallButtonInputState.IsSelectionEnabled && !EventSystem.current.IsPointerOverGameObject();
			if (allowSubsequent)
			{
				MouseDown = true;
				SetSelected(s_wallButtonInputState.InputSelectType == WallButtonInputState.E_SelectState.selecting);
				s_wallButtonInputState.LastHitButton = this;
			}
		}
	}

	public void CustomUpdate()
	{
		UpdateColor();
		UpdateSetMaterial();
		if (m_buttonTweener != null)
			m_buttonTweener.CustomUpdate();
	}

	private void UpdateColor()
	{
		if (m_selected && MeshRenderer.sharedMaterial == m_playingMaterial)
		{
			m_playingFadeProg = Mathf.SmoothDamp(m_playingFadeProg, m_playingFadeTarget, ref m_playingFadeVel, PlayingFadeTime);
			if (m_playingFadeProg < 0.001f)
				m_playingFadeProg = 0;
			Color targetColor = PlayingColor * (m_playingFadeProg) + m_selectedColor * (1.0f - m_playingFadeProg);
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


}
