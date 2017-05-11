using System;
using System.Collections.Generic;

namespace CSharpSynth.CustomSeq
{
	public class CustomSequencerEvents : ISequencerEventList
	{
		//--Variables
		public List<ISequencerEvent> Events {get; set;} //List of Events

		//--Public Methods
		public CustomSequencerEvents()
		{
			Events = new List<ISequencerEvent>();
		}
	}
}

