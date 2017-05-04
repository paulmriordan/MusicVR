using System.Collections;

namespace CompositionCommands
{
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

