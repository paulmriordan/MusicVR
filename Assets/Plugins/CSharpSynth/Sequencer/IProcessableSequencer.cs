using System;
using System.Collections.Generic;
using CSharpSynth.Midi;

namespace CSharpSynth.Sequencer
{
	public interface IProcessableSequencer
	{
		bool isPlaying {get; }
		int SampleTime {get; }

		void Lock();
		void Unlock();
		SequencerEventList Process(int samplesPerBuffer);
		void ProcessEvent(MidiEvent customEvent);
		void IncrementSampleCounter(int samplesPerBuffer);
	}
}

