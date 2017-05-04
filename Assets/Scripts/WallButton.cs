using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WallButton : WallButtonAbstract 
{
	private CompositionData m_compositionData;

	public bool IsSelected {get { return m_compositionData.IsNoteActive(m_row, m_col);}}


	void Awake()
	{
		m_buttonTweener = GetComponent<WallButtonTween>();
		m_buttonColorController = GetComponent<WallButtonColorController>();
		m_inputHander = GetComponent<SequencerButtonInputHander>();
		m_inputHander.Init(this);
		m_UIButtonData.CommandType = CompositionCommands.E_CommandType.toggleSequencerNote;
		m_UIButtonData.Width = 1;
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
