using System.Collections;
using UnityEngine;

namespace MusicVR.Composition
{
	/// <summary>
	/// Command to change active state of note on wall when clicked
	/// </summary>
	public class ToggleSequencerNoteCommand : UndoableCommand
	{
		private CompositionData m_compositionData;
		private int m_row;
		private int m_col;

		public ToggleSequencerNoteCommand(CompositionData compositionData, int row, int col)
		{
			m_compositionData = compositionData;
			m_row = row;
			m_col = col;
		}

		public override void Execute()
		{
			m_compositionData.SetNoteActive(m_row, m_col, !m_compositionData.IsNoteActive(m_row, m_col));
		}

		public override void Undo()
		{
			m_compositionData.SetNoteActive(m_row, m_col, !m_compositionData.IsNoteActive(m_row, m_col));
		}
	}
}