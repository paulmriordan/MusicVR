using System.Collections;
using UnityEngine;

namespace CompositionCommands
{	
	/// <summary>
	/// Command to change scale (major, minor, pentatonic) of instrument
	/// </summary>
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
}