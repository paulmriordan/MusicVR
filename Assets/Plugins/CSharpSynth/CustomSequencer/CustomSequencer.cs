using System;
using CSharpSynth.Synthesis;

namespace CSharpSynth.CustomSeq
{
	public class CustomSequencer
	{
		public struct CustomSeqData
		{
			public float TotalTime;
			public float BeatsPerMinute;
			public int EventCount;
			public CustomEvent[] Events;
			public float DeltaTiming;
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
			if (playing == true)
				return false;

			_MidiFile = seqFile;

			//				if (_MidiFile.SequencerReady == false)
			{
				try
				{
					//Combine all tracks into 1 track that is organized from lowest to highest abs time
					//						_MidiFile.CombineTracks();
					//Convert delta time to sample time
					eventIndex = 0;
					uint lastSample = 0;
					for (int x = 0; x < _MidiFile.Events.Length; x++)
					{
						_MidiFile.Events[x].deltaTime = lastSample + (uint)DeltaTimetoSamples(_MidiFile.Events[x].deltaTime);
						lastSample = _MidiFile.Events[x].deltaTime;
						//Update tempo
						//							if (_MidiFile.Events[x].midiMetaEvent == MidiHelper.MidiMetaEvent.Tempo)
						//							{
						//								_MidiFile.BeatsPerMinute = MidiHelper.MicroSecondsPerMinute / System.Convert.ToUInt32(_MidiFile.Tracks[0].MidiEvents[x].Parameters[0]);
						//							}
					}
					//Set total time to proper value
					_MidiFile.TotalTime = _MidiFile.Events[_MidiFile.Events.Length-1].deltaTime;
					//reset tempo
					_MidiFile.BeatsPerMinute = 120;
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
			//						BankManager.addBank(new InstrumentBank(synth.SampleRate, bankStr, _MidiFile.Tracks[0].Programs, _MidiFile.Tracks[0].DrumPrograms));
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
			//set bpm
			_MidiFile.BeatsPerMinute = 120;
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
		private int DeltaTimetoSamples(uint DeltaTime)
		{
			return SynthHelper.getSampleFromTime(synth.SampleRate, (DeltaTime * (60.0f / (((int)_MidiFile.BeatsPerMinute) * _MidiFile.DeltaTiming))));
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
					//set bpm
					_MidiFile.BeatsPerMinute = 120;
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
	}
}
