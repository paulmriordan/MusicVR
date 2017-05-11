using System.Collections;

namespace CompositionCommands
{
	/// <summary>
	/// All edits to composition must be executed as a command via this manager.
	/// Facilitates undo-ing of any edit to the composition by storing all commands on stack
	/// Based on command design pattern. 
	/// </summary>
	public class CompositionCommandManager
	{
		private CompositionData m_compositionData;
		private Stack commandStack = new Stack();

		public CompositionCommandManager(CompositionData compositionData)
		{
			m_compositionData = compositionData;
		}

		public void ExecuteCommand(Command cmd)
		{
			cmd.Execute();
			if (cmd is UndoableCommand)
			{
				commandStack.Push(cmd);
			}
			m_compositionData.CompositionChanged();
		}

		public void Undo()
		{
			if (commandStack.Count > 0)
			{
				UndoableCommand cmd = (UndoableCommand)commandStack.Pop();
				cmd.Undo();
				m_compositionData.CompositionChanged();
			}
		}
	}
}

