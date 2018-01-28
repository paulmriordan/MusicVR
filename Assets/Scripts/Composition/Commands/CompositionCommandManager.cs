using System.Collections;

namespace MusicVR.Composition
{
	/// <summary>
	/// All edits to composition must be executed as a command via this manager.
	/// Facilitates undo-ing of any edit to the composition by storing all commands on stack
	/// Based on command design pattern. 
	/// </summary>
	public class CompositionCommandManager
	{
		private static CompositionCommandManager instance = null;

		private CompositionData m_compositionData;
		private Stack commandStack = new Stack();

		private CompositionCommandManager()
		{
		}

		public static CompositionCommandManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new CompositionCommandManager();
				}
				return instance;
			}
		}

		public void Init(CompositionData compositionData)
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

