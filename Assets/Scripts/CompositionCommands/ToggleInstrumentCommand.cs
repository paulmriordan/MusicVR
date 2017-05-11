using System.Collections;
using UnityEngine;

namespace CompositionCommands
{	
	/// <summary>
	/// Command to change chosen instrument type (eg, marimba, drums)
	/// </summary>
	public class ToggleInstrumentCommand : UndoableCommand
	{
		private CompositionData.InstrumentData m_instrument;
		public ToggleInstrumentCommand(CompositionData.InstrumentData instrument)
		{
			m_instrument = instrument;
		}

		public override void Execute()
		{
			var instruments = InstrumentDefinitions.Instance.Instruments;
			var currInstrument = instruments.FindIndex((i) => i == m_instrument.InstrumentDefintion);
			int newInstrument = MathHelper.Mod(((int)currInstrument + 1), instruments.Count);
			m_instrument.InstrumentDefintion = InstrumentDefinitions.Instance.Instruments[newInstrument];
		}

		public override void Undo()
		{
			var instruments = InstrumentDefinitions.Instance.Instruments;
			var currInstrument = instruments.FindIndex((i) => i == m_instrument.InstrumentDefintion);
			int newInstrument = MathHelper.Mod(((int)currInstrument - 1), instruments.Count);
			m_instrument.InstrumentDefintion = InstrumentDefinitions.Instance.Instruments[newInstrument];
		}
	}
}