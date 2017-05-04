using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SequencerButtonInputHander : ButtonInputHandler
{
	public class WallButtonInputState
	{
		public event System.Action<Vector2> OnPanRequested = (p) => {};

		public enum E_SelectState {none, selecting, unselecting}

		private bool m_selectionEnabled = true;
		private WallButtonInputConsumer m_buttonInputConsumer = new WallButtonInputConsumer();

		public E_SelectState InputSelectType {get;set;}
		public SequencerWallButton LastHitButton {get;set;}
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

	private SequencerWallButton m_parent;

	protected override WallButtonAbstract Parent { get {return m_parent;}}

	public void InitWithSequencerButton(SequencerWallButton parent)
	{
		m_parent = parent;
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
			s_wallButtonInputState.InputSelectType = m_parent.IsSelected 
				? WallButtonInputState.E_SelectState.unselecting 
				: WallButtonInputState.E_SelectState.selecting;

			TryClick();
		}

		MouseDown = false;
		if (s_wallButtonInputState.LastHitButton)
			s_wallButtonInputState.LastHitButton.InputHandler.MouseDown = false;
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
				s_wallButtonInputState.InputSelectType = m_parent.IsSelected 
					? WallButtonInputState.E_SelectState.unselecting 
					: WallButtonInputState.E_SelectState.selecting;
				
				TryClick();
			}
		}
	}

	void TrySubsequentSelection()
	{
		if (s_wallButtonInputState.InputSelectType != WallButtonInputState.E_SelectState.none)
		{
			bool allowSubsequent = this.m_parent != s_wallButtonInputState.LastHitButton;
			allowSubsequent &= s_wallButtonInputState.IsSelectionEnabled && !EventSystem.current.IsPointerOverGameObject();
			if (allowSubsequent)
			{
				MouseDown = true;

				TryClick();
				
				s_wallButtonInputState.LastHitButton = this.m_parent;
			}
		}
	}

	private void TryClick()
	{
		//Don't toggle if already at desired state
		var selected = s_wallButtonInputState.InputSelectType == WallButtonInputState.E_SelectState.selecting;
		if (m_parent.IsSelected != selected)
			m_parent.Clicked();
	}
}

