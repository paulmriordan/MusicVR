using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using CSharpSynth.Effects;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using CSharpSynth.Midi;

public class Synth : MonoSingleton<Synth> {

	public int bufferSize = 1024;
	public int midiNoteVolume = 100;
//	public int midiInstrument = 1;
	public string bankFilePath = "GM Bank/gm";
//	public int NoteOffset = 80;
	[Range(0,1.0f)]
	public float MasterVolume = 1;
	public const int SAMPLE_RATE = 44100;
	public StreamSynthesizer midiStreamSynthesizer;
	private float[] sampleBuffer;
	private float gain = 1f;

//	public CustomSequencer customSequencer;

	protected override void _Awake()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		midiStreamSynthesizer = new StreamSynthesizer(SAMPLE_RATE, 1, bufferSize, 40);
		#else
		midiStreamSynthesizer = new StreamSynthesizer(SAMPLE_RATE, 2, bufferSize, 40);
		#endif
		sampleBuffer = new float[midiStreamSynthesizer.BufferSize];		

		midiStreamSynthesizer.MasterVolume = MasterVolume;
		midiStreamSynthesizer.LoadBank (bankFilePath);
	}

	public void NoteOn(int channel, int note, int instrument)
	{
		midiStreamSynthesizer.NoteOn(channel, note, midiNoteVolume, instrument);
	}

	public void NoteOff(int channel, int note)
	{
		midiStreamSynthesizer.NoteOff(channel, note);
	}

	// See http://unity3d.com/support/documentation/ScriptReference/MonoBehaviour.OnAudioFilterRead.html for reference code
	//	If OnAudioFilterRead is implemented, Unity will insert a custom filter into the audio DSP chain.
	//
	//	The filter is inserted in the same order as the MonoBehaviour script is shown in the inspector. 	
	//	OnAudioFilterRead is called everytime a chunk of audio is routed thru the filter (this happens frequently, every ~20ms depending on the samplerate and platform). 
	//	The audio data is an array of floats ranging from [-1.0f;1.0f] and contains audio from the previous filter in the chain or the AudioClip on the AudioSource. 
	//	If this is the first filter in the chain and a clip isn't attached to the audio source this filter will be 'played'. 
	//	That way you can use the filter as the audio clip, procedurally generating audio.
	//
	//	If OnAudioFilterRead is implemented a VU meter will show up in the inspector showing the outgoing samples level. 
	//	The process time of the filter is also measured and the spent milliseconds will show up next to the VU Meter 
	//	(it turns red if the filter is taking up too much time, so the mixer will starv audio data). 
	//	Also note, that OnAudioFilterRead is called on a different thread from the main thread (namely the audio thread) 
	//	so calling into many Unity functions from this function is not allowed ( a warning will show up ). 	
	private void OnAudioFilterRead (float[] data, int channels)
	{

		//This uses the Unity specific float method we added to get the buffer
		midiStreamSynthesizer.GetNext (sampleBuffer);

		for (int i = 0; i < data.Length; i++) {
			data [i] = sampleBuffer [i] * gain;
		}
	}
}
