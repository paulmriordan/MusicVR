using System;

namespace CSharpSynth.CustomSeq
{
	public class CustomEvent
	{    
		public enum CustomEventType
		{
			None,
			Note_On,
			Note_Off,
			Unknown
		}

		public uint deltaTime;
		//			public MidiHelper.MidiMetaEvent midiMetaEvent; //copy right, etc
		public CustomEventType midiChannelEvent; //note on, note off, etc
		public object[] Parameters;
		public byte note;
		public byte velocity;
		public byte channel;
	}
}

