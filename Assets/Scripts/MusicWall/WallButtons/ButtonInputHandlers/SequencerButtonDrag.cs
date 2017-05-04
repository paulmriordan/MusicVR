using UnityEngine;

public class SequencerButtonDrag
{
	public event System.Action<Vector2> OnPanRequested = (p) => {};

	public enum E_SelectState {none, selecting, unselecting}

	private bool 						m_selectionEnabled = true;
	private WallButtonInputConsumer 	m_buttonInputConsumer = new WallButtonInputConsumer();

	public E_SelectState 				InputSelectType {get;set;}
	public SequencerWallButton 			LastHitButton {get;set;}
	public bool 						IsSelectionEnabled { get { return m_selectionEnabled && m_buttonInputConsumer.IsActive();}}

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
			&& EdgePanningUtil.DragTreshReached((Input.mousePosition - state.InputDownPos).magnitude,
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

