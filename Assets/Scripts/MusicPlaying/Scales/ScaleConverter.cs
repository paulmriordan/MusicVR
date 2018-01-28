using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicVR.Scales
{
	public enum E_ConverterType 
	{
		Chromatic, 
		Pentatonic, 
		Major, 
		Drum
	}

	/// <summary>
	/// Scale options that can be used for a musical instrument (ie, not drums)
	/// </summary>
	public enum E_Scales 
	{
		Chromatic, 
		Pentatonic, 
		Major
		//NB: keep in sync with E_ConverterType
	}; 

	public abstract class ScaleConverter
	{
		public abstract int Convert(int raw);

		public static int Convert(E_ConverterType type, int noteIn)
		{
			return Create(type).Convert(noteIn);
		}

		private static ScaleConverter Create(E_ConverterType type)
		{
			switch (type)
			{
			default:
			case E_ConverterType.Chromatic: return new ChromaticConverter();
			case E_ConverterType.Pentatonic: return new PentatonicConverter();
			case E_ConverterType.Major: return new MajorConverter();
			case E_ConverterType.Drum: return new DrumConverter();
			}
		}
	}
}
