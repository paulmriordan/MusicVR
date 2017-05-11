using System;
using System.Collections.Generic;
using CSharpSynth.Midi;

namespace CSharpSynth.Sequencer
{
	public interface ISequencerData
	{
		ulong 				TotalTime {get; set;}
		uint 				BeatsPerMinute {get; set;}
		int 				EventCount {get; set;}
		MidiEvent[] 		Events {get; set;}
		int 				DeltaTiming {get; set;}
	}
}

