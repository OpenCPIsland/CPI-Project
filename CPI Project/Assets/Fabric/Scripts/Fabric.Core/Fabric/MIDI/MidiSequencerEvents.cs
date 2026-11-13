using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric.MIDI
{
	[Serializable]
	public class MidiSequencerEvents
	{
		[SerializeField]
		public List<MidiSequencerEvent> Events = new List<MidiSequencerEvent>();
	}
}
