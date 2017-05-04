using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SequencerButtonInputHander : ButtonInputHandler
{
	public static SequencerButtonDrag s_sequencerButtonDrag = new SequencerButtonDrag();

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
		if (s_sequencerButtonDrag.IsSelectionEnabled && !s_sequencerButtonDrag.WallInputActive())
		{
			s_sequencerButtonDrag.InputSelectType = m_parent.IsSelected 
				? SequencerButtonDrag.E_SelectState.unselecting 
				: SequencerButtonDrag.E_SelectState.selecting;

			TryClick();
		}

		MouseDown = false;
		if (s_sequencerButtonDrag.LastHitButton)
			s_sequencerButtonDrag.LastHitButton.InputHandler.MouseDown = false;
		s_sequencerButtonDrag.Clear();
	}
	#endregion


	void TryStartSelection()
	{
		if (s_sequencerButtonDrag.InputSelectType == SequencerButtonDrag.E_SelectState.none)
		{
			bool allowStart = s_sequencerButtonDrag.IsSelectionEnabled;
			allowStart &= !EventSystem.current.IsPointerOverGameObject();
			if (allowStart)
			{
				MouseDown = true;
				s_sequencerButtonDrag.InputSelectType = m_parent.IsSelected 
					? SequencerButtonDrag.E_SelectState.unselecting 
					: SequencerButtonDrag.E_SelectState.selecting;
				
				TryClick();
			}
		}
	}

	void TrySubsequentSelection()
	{
		if (s_sequencerButtonDrag.InputSelectType != SequencerButtonDrag.E_SelectState.none)
		{
			bool allowSubsequent = this.m_parent != s_sequencerButtonDrag.LastHitButton;
			allowSubsequent &= s_sequencerButtonDrag.IsSelectionEnabled && !EventSystem.current.IsPointerOverGameObject();
			if (allowSubsequent)
			{
				MouseDown = true;

				TryClick();
				
				s_sequencerButtonDrag.LastHitButton = this.m_parent;
			}
		}
	}

	private void TryClick()
	{
		//Don't toggle if already at desired state
		var selected = s_sequencerButtonDrag.InputSelectType == SequencerButtonDrag.E_SelectState.selecting;
		if (m_parent.IsSelected != selected)
			m_parent.Clicked();
	}
}

