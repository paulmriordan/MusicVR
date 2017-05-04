using System.Collections;
using UnityEngine;

namespace CompositionCommands
{
	public enum E_CommandType {toggleScale, toggleInstrument, toggleSequencerNote};

	public abstract class Command
	{
		public abstract void Execute();
	}

	public abstract class UndoableCommand : Command
	{
		public abstract void Undo();
	}

	public class ToggleScaleCommand : UndoableCommand
	{
		private CompositionData.InstrumentData m_instrument;
		public ToggleScaleCommand(CompositionData.InstrumentData instrument)
		{
			m_instrument = instrument;
		}

		public override void Execute()
		{
			if (!m_instrument.InstrumentDefintion.IsDrum)
			{
				var scaleNames = System.Enum.GetNames(typeof(MusicScaleConverter.E_Scales));
				int newScale = MathHelper.Mod((int)m_instrument.Scale + 1, scaleNames.Length);
				m_instrument.Scale = (MusicScaleConverter.E_ConverterType)newScale;
			}
		}

		public override void Undo()
		{
			if (!m_instrument.InstrumentDefintion.IsDrum)
			{
				var scaleNames = System.Enum.GetNames(typeof(MusicScaleConverter.E_Scales));
				int newScale = MathHelper.Mod(((int)m_instrument.Scale + -1), scaleNames.Length);
				m_instrument.Scale = (MusicScaleConverter.E_ConverterType)newScale;
			}
		}
	}

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