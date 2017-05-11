using System;
using System.Collections.Generic;
using CSharpSynth.Midi;

namespace CSharpSynth.Sequencer
{
	public class SequencerEventList
	{
		//--Variables
		public List<MidiEvent> Events {get; set;} //List of Events

		//--Public Methods
		public SequencerEventList()
		{
			Events = new List<MidiEvent>();
		}
	}
}

