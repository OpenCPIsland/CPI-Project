using System;
using UnityEngine;

namespace Fabric.MIDI
{
	[Serializable]
	public class MidiEvent
	{
		[NonSerialized]
		public double dspTime;

		[SerializeField]
		public uint deltaTime;

		[SerializeField]
		public object[] Parameters;

		[SerializeField]
		public byte parameter1;

		[SerializeField]
		public byte parameter2;

		[SerializeField]
		public byte channel;

		[SerializeField]
		public MidiHelper.MidiMetaEvent midiMetaEvent;

		[SerializeField]
		public MidiHelper.MidiChannelEvent midiChannelEvent;

		public MidiEvent()
		{
			Parameters = new object[5];
			midiMetaEvent = MidiHelper.MidiMetaEvent.None;
			midiChannelEvent = MidiHelper.MidiChannelEvent.None;
		}

		public bool isMetaEvent()
		{
			return midiChannelEvent == MidiHelper.MidiChannelEvent.None;
		}

		public bool isChannelEvent()
		{
			return midiMetaEvent == MidiHelper.MidiMetaEvent.None;
		}

		public MidiHelper.ControllerType GetControllerType()
		{
			if (midiChannelEvent != MidiHelper.MidiChannelEvent.Controller)
			{
				return MidiHelper.ControllerType.None;
			}
			switch (parameter1)
			{
			case 1:
				return MidiHelper.ControllerType.Modulation;
			case 7:
				return MidiHelper.ControllerType.MainVolume;
			case 10:
				return MidiHelper.ControllerType.Pan;
			case 64:
				return MidiHelper.ControllerType.DamperPedal;
			case 121:
				return MidiHelper.ControllerType.ResetControllers;
			case 123:
				return MidiHelper.ControllerType.AllNotesOff;
			default:
				return MidiHelper.ControllerType.Unknown;
			}
		}
	}
}
