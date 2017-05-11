using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicVR.Scales
{
	public class DrumConverter : ScaleConverter
	{
		public override int Convert(int raw)
		{
			return DrumNoteMap.Instance.Map(raw);
		}
	}
}
