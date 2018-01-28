using System.Collections;
using UnityEngine;

namespace MusicVR.Composition
{
	public abstract class UndoableCommand : Command
	{
		public abstract void Undo();
	}
}