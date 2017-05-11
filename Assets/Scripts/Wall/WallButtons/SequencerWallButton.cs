using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MusicVR.Composition;

public class SequencerWallButton : WallButtonAbstract 
{
	private int m_row = -1;
	private int m_col = -1;
	private CompositionData m_compositionData;

	public bool IsSelected {get { return m_compositionData.IsNoteActive(m_row, m_col);}}

	protected override void Awake()
	{
		base.Awake();
		SequencerButtonInputHander seqInputHandler;
		m_inputHander = seqInputHandler = GetComponent<SequencerButtonInputHander>();
		seqInputHandler.InitWithSequencerButton(this);
	}

	public override void Clicked()
	{
		var compositionData = MusicWall.Instance.WallProperties.CompositionData;
		compositionData.CommandManager.ExecuteCommand(new ToggleSequencerNoteCommand(compositionData, m_row, m_col));

		RefreshVisualState();
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

	public void SetCoord(int row, int col, CompositionData compositionRef)
	{
		m_row = row;
		m_col = col;
		m_compositionData = compositionRef;
	}

	public void CustomUpdate()
	{
		if (m_buttonColorController != null)
			m_buttonColorController.CustomUpdate();
		if (m_buttonTweener != null)
			m_buttonTweener.CustomUpdate();
	}
}
