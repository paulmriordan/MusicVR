using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicVR.Scales
{
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
}
