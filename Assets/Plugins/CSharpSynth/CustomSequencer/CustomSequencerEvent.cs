using System;
using System.Collections.Generic;

namespace CSharpSynth.CustomSeq
{
	public class CustomSequencerEvent
	{
		//--Variables
		public List<CustomEvent> Events; //List of Events
		//--Public Methods
		public CustomSequencerEvent()
		{
			Events = new List<CustomEvent>();
		}
	}
}

