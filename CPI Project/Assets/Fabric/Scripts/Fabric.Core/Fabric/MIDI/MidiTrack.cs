using System;
using UnityEngine;

namespace Fabric.MIDI
{
	[Serializable]
	public class MidiTrack
	{
		[NonSerialized]
		public uint NotesPlayed;

		[SerializeField]
		public ulong TotalTime;

		[SerializeField]
		public byte[] Programs;

		[SerializeField]
		public byte[] DrumPrograms;

		[SerializeField]
		public MidiEvent[] MidiEvents;

		[SerializeField]
		public Component component;

		[SerializeField]
		public bool Mute;

		[NonSerialized]
		public int eventIndex;

		[NonSerialized]
		public double eventDeltaTime;

		public int EventCount
		{
			get
			{
				return MidiEvents.Length;
			}
		}

		public MidiTrack()
		{
			NotesPlayed = 0u;
			TotalTime = 0uL;
			eventIndex = 0;
		}

		public bool ContainsProgram(byte program)
		{
			for (int i = 0; i < Programs.Length; i++)
			{
				if (Programs[i] == program)
				{
					return true;
				}
			}
			return false;
		}

		public bool ContainsDrumProgram(byte drumprogram)
		{
			for (int i = 0; i < DrumPrograms.Length; i++)
			{
				if (DrumPrograms[i] == drumprogram)
				{
					return true;
				}
			}
			return false;
		}
	}
}
