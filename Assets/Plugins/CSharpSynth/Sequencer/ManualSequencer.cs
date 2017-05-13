using System;
using CSharpSynth.Synthesis;
using CSharpSynth.Midi;
using System.Threading;

namespace CSharpSynth.Sequencer
{
	/// <summary>
	/// A manually loadable version of MidiSequencer. 
	/// </summary>
	public class ManualSequencer : ILoadableSequencer, IProcessableSequencer
	{
		private ISequencerData m_sequencerData;
		private StreamSynthesizer synth;
		private int[] currentPrograms; //current instruments
		private bool playing = false;
		private bool looping = true;
		private SequencerEventList seqEvt;
		private int sampleTime;
		private int eventIndex;
		private object _lock = new object();

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
			set 
			{ 
				lock (_lock)
				{
					SetTime(value); 
				}
			}
		}
		public ManualSequencer(StreamSynthesizer synth)
		{
			currentPrograms = new int[16]; //16 channels
			this.synth = synth;
			this.synth.setSequencer(this);
			seqEvt = new SequencerEventList();
		}
		public void Lock()
		{
			Monitor.Enter(_lock);
		}
		public void Unlock()
		{
			Monitor.Exit(_lock);
		}
		public bool Load(ISequencerData seqFile)
		{
			lock (_lock)
			{
				m_sequencerData = seqFile;
				{
					try
					{
						//Convert delta time to sample time
						eventIndex = 0;
						sampleTime = 0;
						uint lastSample = 0;
						for (int x = 0; x < m_sequencerData.Events.Length; x++)
						{
							m_sequencerData.Events[x].deltaTime = lastSample + (uint)DeltaTimetoSamples(m_sequencerData.Events[x].deltaTime);
							lastSample = m_sequencerData.Events[x].deltaTime;
						}
					}
					catch (Exception ex)
					{
						//UnitySynth
						UnityEngine.Debug.Log("Error Loading Custom Seq:\n" + ex.Message);
						return false;
					}
				}
			}

			return true;
		}
		public void SetProgram(int channel, int program)
		{
			lock (_lock)
			{
				currentPrograms[channel] = program;
			}
		}
		public void Play()
		{
			lock (_lock)
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
		}
		public void Stop(bool immediate)
		{
			lock (_lock)
			{
				playing = false;
				sampleTime = 0;
				if (immediate)
					synth.NoteOffAll(true);
				else
					synth.NoteOffAll(false);
			}
		}
		public static int TimetoSampleTime(uint DeltaTime, int sampleRate, uint BBM, uint fileDeltaTiming)
		{
			return SynthHelper.getSampleFromTime(sampleRate, ((float)DeltaTime * (60.0f / ((float)((int)BBM) * fileDeltaTiming))));
		}
		private int DeltaTimetoSamples(uint DeltaTime)
		{
			return SynthHelper.getSampleFromTime(synth.SampleRate, ((float)DeltaTime * (60.0f / (float)(((int)m_sequencerData.BeatsPerMinute) * m_sequencerData.DeltaTiming))));
		}

		public SequencerEventList Process(int frame)
		{
			lock (_lock)
			{
				seqEvt.Events.Clear();
				//stop or loop
				if (sampleTime >= (int)m_sequencerData.TotalTime)
				{
					sampleTime = 0;
					if (looping == true)
					{
						//DONT Clear the current programs for the channels. (only set once in Wall Music Player)
	//					Array.Clear(currentPrograms, 0, currentPrograms.Length);

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
				while (eventIndex < m_sequencerData.EventCount && m_sequencerData.Events[eventIndex].deltaTime < (sampleTime + frame))
				{
					seqEvt.Events.Add(m_sequencerData.Events[eventIndex]);
					eventIndex++;
				}
			}
			return seqEvt;
		}

		public void ResetControllers()
		{
			lock (_lock)
			{
				//Reset Pan Positions back to 0.0f
				Array.Clear(synth.PanPositions, 0, synth.PanPositions.Length);
				//Set Tuning Positions back to 0.0f
				Array.Clear(synth.TunePositions, 0, synth.TunePositions.Length);
				//Reset Vol Positions back to 1.00f
				for (int x = 0; x < synth.VolPositions.Length; x++)
					synth.VolPositions[x] = 1.00f;
			}
		}

		public void IncrementSampleCounter(int amount)
		{
			lock (_lock)
			{
				sampleTime = sampleTime + amount;
			}
		}

		public void ProcessEvent(MidiEvent customEvent)
		{
			lock (_lock)
			{
				if (customEvent.midiChannelEvent != MidiHelper.MidiChannelEvent.None)
				{
					switch (customEvent.midiChannelEvent)
					{
					case MidiHelper.MidiChannelEvent.Note_On:
						synth.NoteOn(customEvent.channel, 
							customEvent.parameter1,
							customEvent.parameter2,
							currentPrograms[customEvent.channel]);
						break;
					case MidiHelper.MidiChannelEvent.Note_Off:
						synth.NoteOff(customEvent.channel, customEvent.parameter1);
						break;
					default:
						break;
					}
				}
			}
		}
		private void SetTime(float timeSecs)
		{
			int _stime = SynthHelper.getSampleFromTime(synth.SampleRate, timeSecs);
			if (_stime > sampleTime)
			{
				SilentProcess(_stime - sampleTime);
			}
		}
		private void SilentProcess(int amount)
		{
			while (eventIndex < m_sequencerData.EventCount && m_sequencerData.Events[eventIndex].deltaTime < (sampleTime + amount))
			{
				if (m_sequencerData.Events[eventIndex].midiChannelEvent != MidiHelper.MidiChannelEvent.Note_On)
					ProcessEvent(m_sequencerData.Events[eventIndex]);               
				eventIndex++;
			}

			sampleTime = sampleTime + amount;
		}
	}
}
