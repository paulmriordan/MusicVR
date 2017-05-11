using System.Collections;
using UnityEngine;

namespace CompositionCommands
{
	public abstract class UndoableCommand : Command
	{
		public abstract void Undo();
	}
}