using UnityEngine;
using System.Collections;
using CompositionCommands;

public class WallButtonAbstract : MonoBehaviour
{
	public InstrumentUIData.InstrumentUIButton m_UIButtonData;
	protected CompositionData.InstrumentData m_instrumentData;
	protected int m_row = -1;
	protected int m_col = -1;
	protected WallButtonTween m_buttonTweener;
	protected WallButtonColorController m_buttonColorController;
	protected SequencerButtonInputHander m_inputHander;

	public WallButtonColorController ColorController {get {return m_buttonColorController;}}
	public SequencerButtonInputHander InputHandler {get {return m_inputHander;}}
	public WallButtonTween Tweener {get {return m_buttonTweener;}}

	public void Clicked()
	{
		switch ( m_UIButtonData.CommandType)
		{
		case E_CommandType.toggleScale:
			MusicWall.Instance.WallProperties.CompositionData.CommandManager.ExecuteCommand(new ToggleScaleCommand(m_instrumentData));
			break;
		case E_CommandType.toggleInstrument:
			MusicWall.Instance.WallProperties.CompositionData.CommandManager.ExecuteCommand(new ToggleInstrumentCommand(m_instrumentData));
			break;
		case E_CommandType.toggleSequencerNote:
			var compositionData = MusicWall.Instance.WallProperties.CompositionData;
			compositionData.CommandManager.ExecuteCommand(new ToggleSequencerNoteCommand(compositionData, m_row, m_col));

			RefreshVisualState();
			break;
		}
	}

	public void RefreshVisualState()
	{
		var compositionData = MusicWall.Instance.WallProperties.CompositionData;
		var selected = compositionData.IsNoteActive(m_row, m_col);
		if (m_buttonTweener != null)
			m_buttonTweener.Selected = selected;
		if (m_buttonColorController != null)
			m_buttonColorController.Selected = selected;
	}
}

