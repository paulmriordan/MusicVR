using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MusicScaleConverter
{
	public enum E_ConverterType { Chromatic, Pentatonic, Major}

	public static ScaleConverter Get(E_ConverterType type)
	{
		switch (type)
		{
		default:
		case E_ConverterType.Chromatic: return new ChromaticConverter();
		case E_ConverterType.Pentatonic: return new PentatonicConverter();
		case E_ConverterType.Major: return new MajorConverter();
		}
	}

	public abstract class ScaleConverter
	{
		public abstract int Convert(int raw);
	}

	public class ChromaticConverter : ScaleConverter
	{
		public override int Convert(int raw)
		{
			return raw;
		}
	}

	public class PentatonicConverter : ScaleConverter
	{
		public override int Convert(int raw)
		{
			int octaves = raw/5;
			int remainder = raw%5;
			switch (remainder)
			{
			default:
			case 0: return octaves*12 + remainder;
			case 1: return octaves*12 + remainder + 1;
			case 2: return octaves*12 + remainder + 2;
			case 3:	return octaves*12 + remainder + 4;
			case 4:	return octaves*12 + remainder + 5;
			}
		}
	}

	public class MajorConverter : ScaleConverter
	{
		public override int Convert(int raw)
		{
			int octaves = raw/8;
			int remainder = raw%8;
			switch (remainder)
			{
			default:
			case 0: return octaves*12 + remainder;
			case 1: return octaves*12 + remainder + 1;
			case 2: return octaves*12 + remainder + 2;
			case 3: return octaves*12 + remainder + 2;
			case 4:	return octaves*12 + remainder + 3;
			case 5:	return octaves*12 + remainder + 4;
			case 6:	return octaves*12 + remainder + 5;
			}
		}
	}
}
