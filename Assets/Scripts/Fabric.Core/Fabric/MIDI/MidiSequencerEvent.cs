using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric.MIDI
{
	[Serializable]
	public class MidiSequencerEvent
	{
		[SerializeField]
		public Component component;

		[NonSerialized]
		public List<MidiEvent> Events = new List<MidiEvent>();
	}
}
