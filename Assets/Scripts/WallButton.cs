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

	private bool m_selected;
	private CompositionData m_compositionData;
	private int m_row;
	private int m_col;
	private WallButtonTween m_buttonTweener;
	private WallButtonColorController m_buttonColorController;
	private bool m_mouseDown;

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
	public WallButtonColorController ColorController {get {return m_buttonColorController;}}

	void Awake()
	{
		m_buttonTweener = GetComponent<WallButtonTween>();
		m_buttonColorController = GetComponent<WallButtonColorController>();
	}

	public void SetCoord(int row, int col, CompositionData compositionRef)
	{
		m_row = row;
		m_col = col;
		m_compositionData = compositionRef;
	}

	public void SetSelected(bool selected)
	{
		m_selected = selected;
		if (m_buttonTweener != null)
			m_buttonTweener.Selected = selected;
		if (m_buttonColorController != null)
			m_buttonColorController.Selected = selected;
		m_compositionData.SetNoteActive(m_row, m_col, selected);
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
		if (m_buttonColorController != null)
			m_buttonColorController.CustomUpdate();
		if (m_buttonTweener != null)
			m_buttonTweener.CustomUpdate();
	}


}
