using System;
using System.Collections.Generic;
using CSharpSynth.Midi;

namespace CSharpSynth.Sequencer
{
	public interface IProcessableSequencer
	{
		bool isPlaying {get; }
		int SampleTime {get; }

		SequencerEventList Process(int samplesPerBuffer);
		void ProcessEvent(MidiEvent customEvent);
		void IncrementSampleCounter(int samplesPerBuffer);
	}
}

