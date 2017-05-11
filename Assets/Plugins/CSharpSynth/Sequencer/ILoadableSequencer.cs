using System;
using System.Collections.Generic;

namespace CSharpSynth.Sequencer
{
	public interface ILoadableSequencer
	{
		float Time {get; set;}

		void Play();
		void Stop(bool immediate);
		void SetProgram(int channel, int program);
		bool Load(ISequencerData seqFile);
	}
}

