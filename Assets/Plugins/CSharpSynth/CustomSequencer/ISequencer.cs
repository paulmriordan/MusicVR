using System;
using System.Collections.Generic;
using CSharpSynth.Midi;

namespace CSharpSynth.CustomSeq
{
	public interface ISequencerEvent
	{
		uint 							deltaTime {get; set;}
		MidiHelper.MidiChannelEvent 	midiChannelEvent {get; set;} //note on, note off, etc
		MidiHelper.MidiMetaEvent 		midiMetaEvent {get;set;}
		object[] 						Parameters {get; set;}
		byte 							parameter1 {get; set;}
		byte 							parameter2 {get; set;}
		byte 							channel {get; set;}

		MidiHelper.ControllerType GetControllerType();
	}

	public interface ISequencerEventList
	{
		List<ISequencerEvent> Events {get; set;}
	}

	public interface ISequencerData
	{
		ulong 				TotalTime {get; set;}
		uint 				BeatsPerMinute {get; set;}
		int 				EventCount {get; set;}
		ISequencerEvent[] 	Events {get; set;}
		int 				DeltaTiming {get; set;}
	}

	public interface ILoadableSequencer
	{

		float Time {get; set;}

		void Play();
		void Stop(bool immediate);
		void SetProgram(int channel, int program);
		bool Load(ISequencerData seqFile);
	}

	public interface IProcessableSequencer
	{
		bool isPlaying {get; }
		int SampleTime {get; }

		ISequencerEventList Process(int samplesPerBuffer);
		void ProcessEvent(ISequencerEvent customEvent);
		void IncrementSampleCounter(int samplesPerBuffer);
	}
}

