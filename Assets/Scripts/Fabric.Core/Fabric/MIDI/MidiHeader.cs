using System;
using UnityEngine;

namespace Fabric.MIDI
{
	[Serializable]
	public class MidiHeader
	{
		[SerializeField]
		public int DeltaTiming;

		[NonSerialized]
		public MidiHelper.MidiFormat MidiFormat;

		[NonSerialized]
		public MidiHelper.MidiTimeFormat TimeFormat;

		public void setMidiFormat(int format)
		{
			switch (format)
			{
			case 0:
				MidiFormat = MidiHelper.MidiFormat.SingleTrack;
				break;
			case 1:
				MidiFormat = MidiHelper.MidiFormat.MultiTrack;
				break;
			case 2:
				MidiFormat = MidiHelper.MidiFormat.MultiSong;
				break;
			}
		}
	}
}
