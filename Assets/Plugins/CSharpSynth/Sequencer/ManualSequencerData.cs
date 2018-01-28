using System;
using CSharpSynth.Synthesis;
using CSharpSynth.Midi;

namespace CSharpSynth.Sequencer
{
	/// <summary>
	/// Manual Sequencer Data. Simplified version of MidiFile
	/// </summary>
	public struct ManualSequencerData : ISequencerData
	{
		public ulong 				TotalTime {get;set;}
		public uint 				BeatsPerMinute {get;set;}
		public int 					EventCount {get;set;}
		public MidiEvent[] 			Events {get;set;}
		public int 					DeltaTiming {get;set;}
	}
}
