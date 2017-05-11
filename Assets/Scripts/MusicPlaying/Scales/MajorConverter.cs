using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicVR.Scales
{
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
