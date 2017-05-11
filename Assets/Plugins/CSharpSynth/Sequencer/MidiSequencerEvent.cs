using System.Collections.Generic;
using CSharpSynth.Midi;
using CSharpSynth.CustomSeq;

namespace CSharpSynth.Sequencer 
{
	public class MidiSequencerEvent : ISequencerEventList
    {
        //--Variables
		public List<ISequencerEvent> Events {get;set;} //List of Events
        //--Public Methods
        public MidiSequencerEvent()
        {
			Events = new List<ISequencerEvent>();
        }
    }
}
