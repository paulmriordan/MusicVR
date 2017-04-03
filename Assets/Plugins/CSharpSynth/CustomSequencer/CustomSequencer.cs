using System;
using CSharpSynth.Synthesis;

namespace CSharpSynth.CustomSeq
{
	public class CustomSequencer
	{
		public struct CustomSeqData
		{
			public ulong TotalTime;
			public uint BeatsPerMinute;
			public int EventCount;
			public CustomEvent[] Events;
			public int DeltaTiming;
		}
		private CustomSeqData _MidiFile;
		private StreamSynthesizer synth;
		private int[] currentPrograms; //current instruments
		//			private List<byte> blockList;
		//			private double PitchWheelSemitoneRange = 2.0;
		private bool playing = false;
		private bool looping = true;
		private CustomSequencerEvent seqEvt;
		private int sampleTime;
		private int eventIndex;
		//			//--Events
		//			public delegate void NoteOnEventHandler(int channel, int note, int velocity);
		//			public event NoteOnEventHandler NoteOnEvent;
		//			public delegate void NoteOffEventHandler(int channel, int note);
		//			public event NoteOffEventHandler NoteOffEvent;

		//--Public Properties
		public bool isPlaying
		{
			get { return playing; }
		}
		public int SampleTime
		{
			get { return sampleTime; }
		}
		public float Time
		{
			get { return (int)SynthHelper.getTimeFromSample(synth.SampleRate, sampleTime); }
			set { SetTime(value); }
		}
		public CustomSequencer(StreamSynthesizer synth)
		{
			currentPrograms = new int[16]; //16 channels
			this.synth = synth;
			this.synth.setSequencer(this);
			//				blockList = new List<byte>();
			seqEvt = new CustomSequencerEvent();
		}
		public bool Load(CustomSeqData seqFile)
		{
//			if (playing == true)
//				return false;
//
			_MidiFile = seqFile;

//				if (_MidiFile.SequencerReady == false)
			{
				try
				{
					//Convert delta time to sample time
					eventIndex = 0;
					uint lastSample = 0;
					for (int x = 0; x < _MidiFile.Events.Length; x++)
					{
						_MidiFile.Events[x].deltaTime = lastSample + (uint)DeltaTimetoSamples(_MidiFile.Events[x].deltaTime);
						lastSample = _MidiFile.Events[x].deltaTime;
					}
					//mark midi as ready for sequencing
					//						_MidiFile.SequencerReady = true;
				}
				catch (Exception ex)
				{
					//UnitySynth
					UnityEngine.Debug.Log("Error Loading Custom Seq:\n" + ex.Message);
					return false;
				}
			}
			//				blockList.Clear();
			//				if (UnloadUnusedInstruments == true)
			//				{
			//					if (synth.SoundBank == null)
			//					{//If there is no bank warn the developer =)
			//						Debug.Log("No Soundbank loaded !");
			//					}
			//					else
			//					{
			//						string bankStr = synth.SoundBank.BankPath;
			//						//Remove old bank being used by synth
			//						synth.UnloadBank();
			//						//Add the bank and switch to it with the synth
			//						BankManager.addBank(new InstrumentBank(synth.SampleRate, bankStr, _MidiFile.Programs, _MidiFile.DrumPrograms));
			//						synth.SwitchBank(BankManager.Count - 1);
			//					}
			//				}
			return true;
		}
		public void setProgram(int channel, int program)
		{
			currentPrograms[channel] = program;
		}
		public void Play()
		{
			if (playing == true)
				return;
			//Clear the current programs for the channels.
			Array.Clear(currentPrograms, 0, currentPrograms.Length);
			//Clear vol, pan, and tune
			ResetControllers();
			//Let the synth know that the sequencer is ready.
			eventIndex = 0;
			playing = true;
		}
		public void Stop(bool immediate)
		{
			playing = false;
			sampleTime = 0;
			if (immediate)
				synth.NoteOffAll(true);
			else
				synth.NoteOffAll(false);
		}
		public static int TimetoSampleTime(uint DeltaTime, int sampleRate, uint BBM, uint fileDeltaTiming)
		{
			return SynthHelper.getSampleFromTime(sampleRate, ((float)DeltaTime * (60.0f / ((float)((int)BBM) * fileDeltaTiming))));
		}
		private int DeltaTimetoSamples(uint DeltaTime)
		{
			return SynthHelper.getSampleFromTime(synth.SampleRate, ((float)DeltaTime * (60.0f / (float)(((int)_MidiFile.BeatsPerMinute) * _MidiFile.DeltaTiming))));
		}

		public CustomSequencerEvent Process(int frame)
		{
			seqEvt.Events.Clear();
			//stop or loop
			if (sampleTime >= (int)_MidiFile.TotalTime)
			{
				sampleTime = 0;
				if (looping == true)
				{
					//Clear the current programs for the channels.
					Array.Clear(currentPrograms, 0, currentPrograms.Length);
					//Clear vol, pan, and tune
					ResetControllers();
//					//set bpm
//					_MidiFile.BeatsPerMinute = 120;
					//Let the synth know that the sequencer is ready.
					eventIndex = 0;
				}
				else
				{
					playing = false;
					synth.NoteOffAll(true);
					return null;
				}
			}
			while (eventIndex < _MidiFile.EventCount && _MidiFile.Events[eventIndex].deltaTime < (sampleTime + frame))
			{
				seqEvt.Events.Add(_MidiFile.Events[eventIndex]);
				eventIndex++;
			}
			return seqEvt;
		}

		public void ResetControllers()
		{
			//Reset Pan Positions back to 0.0f
			Array.Clear(synth.PanPositions, 0, synth.PanPositions.Length);
			//Set Tuning Positions back to 0.0f
			Array.Clear(synth.TunePositions, 0, synth.TunePositions.Length);
			//Reset Vol Positions back to 1.00f
			for (int x = 0; x < synth.VolPositions.Length; x++)
				synth.VolPositions[x] = 1.00f;
		}

		public void IncrementSampleCounter(int amount)
		{
			sampleTime = sampleTime + amount;
		}

		public void ProcessCustomEvent(CustomEvent customEvent)
		{
			if (customEvent.midiChannelEvent != CustomEvent.CustomEventType.None)
			{
				switch (customEvent.midiChannelEvent)
				{
				case CustomEvent.CustomEventType.Note_On:
					synth.NoteOn(customEvent.channel, 
						customEvent.note,
						customEvent.velocity,
						currentPrograms[customEvent.channel]);
					break;
				case CustomEvent.CustomEventType.Note_Off:
					synth.NoteOff(customEvent.channel, customEvent.note);
					break;
				default:
					break;
				}
			}
		}
		private void SetTime(float timeSecs)
		{
			int _stime = SynthHelper.getSampleFromTime(synth.SampleRate, timeSecs);
//			if (_stime > sampleTime)
			{
				SilentProcess(_stime - sampleTime);
			}
//			else if (_stime < sampleTime)
//			{//we have to restart the midi to make sure we get the right temp, instrument, etc
//				synth.Stop();
//				sampleTime = 0;
//				Array.Clear(currentPrograms, 0, currentPrograms.Length);
//				ResetControllers();
//				_MidiFile.BeatsPerMinute = 120;
//				eventIndex = 0;
//				SilentProcess(_stime);
//			}
		}
		private void SilentProcess(int amount)
		{
			while (eventIndex < _MidiFile.EventCount && _MidiFile.Events[eventIndex].deltaTime < (sampleTime + amount))
			{
				if (_MidiFile.Events[eventIndex].midiChannelEvent != CustomEvent.CustomEventType.Note_On)
					ProcessCustomEvent(_MidiFile.Events[eventIndex]);               
				eventIndex++;
			}
			sampleTime = sampleTime + amount;
		}
	}
}
