using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Fabric.MIDI
{
	[Serializable]
	public class MidiSequencer
	{
		public delegate void OnMidiEventHandler(Component component, MidiEvent midiEvent, double offser);

		[SerializeField]
		public MidiHeader midiHeader;

		[SerializeField]
		public List<MidiTrack> tracks = new List<MidiTrack>();

		[SerializeField]
		public uint BPM = 120u;

		[SerializeField]
		public uint MPQN = 500000u;

		[SerializeField]
		public MidiSequencerEvents seqEvt = new MidiSequencerEvents();

		private bool playing;

		private bool looping;

		private double dspTriggerTime;

		public uint BeatsPerMinute
		{
			get
			{
				return BPM;
			}
			set
			{
				BPM = value;
				MPQN = 60000000u / BPM;
			}
		}

		public bool isPlaying
		{
			get
			{
				return playing;
			}
		}

		public bool Looping
		{
			get
			{
				return looping;
			}
			set
			{
				looping = value;
			}
		}

		public event OnMidiEventHandler MidiEvent;

		public bool LoadMidi(string filename)
		{
			if (playing)
			{
				return false;
			}
			Stream stream = null;
			stream = File.Open(filename, FileMode.Open);
			loadStream(stream);
			if (stream != null)
			{
				stream.Close();
			}
			return true;
		}

		public bool ReloadMidi(string filename)
		{
			if (playing)
			{
				return false;
			}
			Stream stream = null;
			stream = File.Open(filename, FileMode.Open);
			loadStream(stream, true);
			if (stream != null)
			{
				stream.Close();
			}
			return true;
		}

		public void UnloadMidi()
		{
			tracks.Clear();
			seqEvt.Events.Clear();
		}

		public void Play(double offset = 0.0)
		{
			if (!playing)
			{
				dspTriggerTime = AudioSettings.dspTime + offset;
				for (int i = 0; i < tracks.Count; i++)
				{
					tracks[i].eventDeltaTime = AudioSettings.dspTime + offset;
					tracks[i].eventIndex = 0;
				}
				playing = true;
			}
		}

		public void Stop(bool immediate)
		{
			for (int i = 0; i < tracks.Count; i++)
			{
				tracks[i].eventIndex = 0;
			}
			playing = false;
		}

		public void SetTempo(uint tempo)
		{
			BeatsPerMinute = tempo;
		}

		public void Process(double frame)
		{
			int num = 0;
			for (int i = 0; i < tracks.Count; i++)
			{
				if (!tracks[i].Mute)
				{
					num++;
				}
			}
			for (int j = 0; j < tracks.Count; j++)
			{
				MidiTrack midiTrack = tracks[j];
				if (midiTrack.Mute)
				{
					continue;
				}
				double num2 = dspTriggerTime + DeltaTimetoMS((uint)midiTrack.TotalTime);
				if (frame > num2 && midiTrack.eventIndex == midiTrack.MidiEvents.Length)
				{
					num--;
				}
				if (num == 0)
				{
					if (!looping)
					{
						playing = false;
						break;
					}
					for (int k = 0; k < tracks.Count; k++)
					{
						tracks[k].eventIndex = 0;
					}
				}
				seqEvt.Events[j].Events.Clear();
				if (midiTrack.eventIndex < midiTrack.MidiEvents.Length)
				{
					MidiEvent midiEvent = midiTrack.MidiEvents[midiTrack.eventIndex];
					double num3 = DeltaTimetoMS(midiEvent.deltaTime);
					double num4 = midiTrack.eventDeltaTime + num3;
					double num5 = 0.0099999997764825821;
					double num6 = AudioSettings.dspTime + num5;
					if (num6 > num4)
					{
						midiEvent.dspTime = num4 - AudioSettings.dspTime;
						midiTrack.eventDeltaTime = num4;
						midiTrack.eventIndex++;
						seqEvt.Events[j].Events.Add(midiEvent);
					}
				}
			}
			for (int l = 0; l < seqEvt.Events.Count; l++)
			{
				ProcessMidiEvent(seqEvt.Events[l]);
			}
		}

		public void ProcessMidiEvent(MidiSequencerEvent seqEvent)
		{
			for (int i = 0; i < seqEvent.Events.Count; i++)
			{
				MidiEvent midiEvent = seqEvent.Events[i];
				if (midiEvent.midiChannelEvent != 0)
				{
					if (this.MidiEvent != null)
					{
						this.MidiEvent(seqEvent.component, midiEvent, midiEvent.dspTime);
					}
					continue;
				}
				MidiHelper.MidiMetaEvent midiMetaEvent = midiEvent.midiMetaEvent;
				if (midiMetaEvent == MidiHelper.MidiMetaEvent.Tempo && midiEvent.Parameters[0] != null)
				{
					BeatsPerMinute = 60000000u / Convert.ToUInt32(midiEvent.Parameters[0]);
				}
			}
		}

		public void Dispose()
		{
			Stop(true);
			seqEvt = null;
		}

		private double DeltaTimetoMS(uint DeltaTime)
		{
			return (double)DeltaTime * (60.0 / (double)((int)BeatsPerMinute * midiHeader.DeltaTiming));
		}

		private void loadStream(Stream stream, bool reloading = false)
		{
			byte[] array = new byte[4];
			stream.Read(array, 0, 4);
			if (Encoding.UTF8.GetString(array, 0, array.Length) != "MThd")
			{
				return;
			}
			midiHeader = new MidiHeader();
			stream.Read(array, 0, 4);
			Array.Reverse(array);
			BitConverter.ToInt32(array, 0);
			array = new byte[2];
			stream.Read(array, 0, 2);
			Array.Reverse(array);
			midiHeader.setMidiFormat(BitConverter.ToInt16(array, 0));
			stream.Read(array, 0, 2);
			Array.Reverse(array);
			int num = BitConverter.ToInt16(array, 0);
			stream.Read(array, 0, 2);
			Array.Reverse(array);
			int num2 = BitConverter.ToInt16(array, 0);
			midiHeader.DeltaTiming = (num2 & 0x7FFF);
			midiHeader.TimeFormat = (((num2 & 0x8000) > 0) ? MidiHelper.MidiTimeFormat.FamesPerSecond : MidiHelper.MidiTimeFormat.TicksPerBeat);
			bool flag = (!reloading || tracks.Count != num) ? true : false;
			if (flag)
			{
				tracks.Clear();
				seqEvt.Events.Clear();
			}
			int num3 = 0;
			while (true)
			{
				if (num3 >= num)
				{
					return;
				}
				MidiTrack midiTrack = null;
				MidiSequencerEvent midiSequencerEvent = null;
				if (flag)
				{
					midiTrack = new MidiTrack();
					tracks.Add(midiTrack);
					midiSequencerEvent = new MidiSequencerEvent();
					seqEvt.Events.Add(midiSequencerEvent);
				}
				else
				{
					midiTrack = tracks[num3];
					midiTrack.TotalTime = 0uL;
					midiSequencerEvent = seqEvt.Events[num3];
				}
				List<byte> list = new List<byte>();
				List<byte> list2 = new List<byte>();
				List<MidiEvent> list3 = new List<MidiEvent>();
				list.Add(0);
				list2.Add(0);
				array = new byte[4];
				stream.Read(array, 0, 4);
				if (Encoding.UTF8.GetString(array, 0, array.Length) != "MTrk")
				{
					break;
				}
				stream.Read(array, 0, 4);
				Array.Reverse(array);
				int num4 = BitConverter.ToInt32(array, 0);
				array = new byte[num4];
				stream.Read(array, 0, num4);
				int i = 0;
				byte b = 0;
				int num5 = 0;
				MidiEvent midiEvent;
				for (; i < array.Length; list3.Add(midiEvent), tracks[num3].TotalTime = tracks[num3].TotalTime + midiEvent.deltaTime)
				{
					ushort numOfBytes = 0;
					uint data = BitConverter.ToUInt32(array, i);
					midiEvent = new MidiEvent();
					midiEvent.deltaTime = GetTime(data, ref numOfBytes);
					i += 4 - (4 - numOfBytes);
					byte b2 = array[i];
					int num6 = GetChannel(b2);
					if (b2 < 128)
					{
						b2 = b;
						num6 = num5;
						i--;
					}
					if (b2 != byte.MaxValue)
					{
						b2 = (byte)(b2 & 0xF0);
					}
					b = b2;
					num5 = num6;
					switch (b2)
					{
					case 128:
						midiEvent.midiChannelEvent = MidiHelper.MidiChannelEvent.Note_Off;
						i++;
						midiEvent.channel = (byte)num6;
						midiEvent.Parameters[0] = midiEvent.channel;
						midiEvent.parameter1 = array[i++];
						midiEvent.parameter2 = array[i++];
						midiEvent.Parameters[1] = midiEvent.parameter1;
						midiEvent.Parameters[2] = midiEvent.parameter2;
						continue;
					case 144:
						midiEvent.midiChannelEvent = MidiHelper.MidiChannelEvent.Note_On;
						i++;
						midiEvent.channel = (byte)num6;
						midiEvent.Parameters[0] = midiEvent.channel;
						midiEvent.parameter1 = array[i++];
						midiEvent.parameter2 = array[i++];
						midiEvent.Parameters[1] = midiEvent.parameter1;
						midiEvent.Parameters[2] = midiEvent.parameter2;
						if (midiEvent.parameter2 == 0)
						{
							midiEvent.midiChannelEvent = MidiHelper.MidiChannelEvent.Note_Off;
						}
						tracks[num3].NotesPlayed++;
						continue;
					case 160:
						midiEvent.midiChannelEvent = MidiHelper.MidiChannelEvent.Note_Aftertouch;
						midiEvent.channel = (byte)num6;
						midiEvent.Parameters[0] = midiEvent.channel;
						i++;
						midiEvent.parameter1 = array[++i];
						midiEvent.parameter2 = array[++i];
						continue;
					case 176:
						midiEvent.midiChannelEvent = MidiHelper.MidiChannelEvent.Controller;
						midiEvent.channel = (byte)num6;
						midiEvent.Parameters[0] = midiEvent.channel;
						i++;
						midiEvent.parameter1 = array[i++];
						midiEvent.parameter2 = array[i++];
						midiEvent.Parameters[1] = midiEvent.parameter1;
						midiEvent.Parameters[2] = midiEvent.parameter2;
						continue;
					case 192:
						midiEvent.midiChannelEvent = MidiHelper.MidiChannelEvent.Program_Change;
						midiEvent.channel = (byte)num6;
						midiEvent.Parameters[0] = midiEvent.channel;
						i++;
						midiEvent.parameter1 = array[i++];
						midiEvent.Parameters[1] = midiEvent.parameter1;
						if (midiEvent.channel != 9)
						{
							if (!list.Contains(midiEvent.parameter1))
							{
								list.Add(midiEvent.parameter1);
							}
						}
						else if (!list2.Contains(midiEvent.parameter1))
						{
							list2.Add(midiEvent.parameter1);
						}
						continue;
					case 208:
						midiEvent.midiChannelEvent = MidiHelper.MidiChannelEvent.Channel_Aftertouch;
						midiEvent.channel = (byte)num6;
						midiEvent.Parameters[0] = midiEvent.channel;
						i++;
						midiEvent.parameter1 = array[++i];
						continue;
					case 224:
					{
						midiEvent.midiChannelEvent = MidiHelper.MidiChannelEvent.Pitch_Bend;
						midiEvent.channel = (byte)num6;
						midiEvent.Parameters[0] = midiEvent.channel;
						i++;
						midiEvent.parameter1 = array[++i];
						midiEvent.parameter2 = array[++i];
						ushort parameter = midiEvent.parameter1;
						parameter = (ushort)(parameter << 7);
						parameter = (ushort)(parameter | midiEvent.parameter2);
						midiEvent.Parameters[1] = ((double)(int)parameter - 8192.0) / 8192.0;
						continue;
					}
					case byte.MaxValue:
						switch (array[++i])
						{
						case 0:
							midiEvent.midiMetaEvent = MidiHelper.MidiMetaEvent.Sequence_Number;
							i++;
							break;
						case 1:
							midiEvent.midiMetaEvent = MidiHelper.MidiMetaEvent.Text_Event;
							i++;
							midiEvent.parameter1 = array[i++];
							midiEvent.Parameters[0] = midiEvent.parameter1;
							midiEvent.Parameters[1] = Encoding.UTF8.GetString(array, i, array[i - 1]);
							i += array[i - 1];
							break;
						case 2:
							midiEvent.midiMetaEvent = MidiHelper.MidiMetaEvent.Copyright_Notice;
							i++;
							midiEvent.parameter1 = array[i++];
							midiEvent.Parameters[0] = midiEvent.parameter1;
							midiEvent.Parameters[1] = Encoding.UTF8.GetString(array, i, array[i - 1]);
							i += array[i - 1];
							break;
						case 3:
							midiEvent.midiMetaEvent = MidiHelper.MidiMetaEvent.Sequence_Or_Track_Name;
							i++;
							midiEvent.parameter1 = array[i++];
							midiEvent.Parameters[0] = midiEvent.parameter1;
							midiEvent.Parameters[1] = Encoding.UTF8.GetString(array, i, array[i - 1]);
							i += array[i - 1];
							break;
						case 4:
							midiEvent.midiMetaEvent = MidiHelper.MidiMetaEvent.Instrument_Name;
							i++;
							midiEvent.Parameters[0] = Encoding.UTF8.GetString(array, i + 1, array[i]);
							i += array[i] + 1;
							break;
						case 5:
							midiEvent.midiMetaEvent = MidiHelper.MidiMetaEvent.Lyric_Text;
							i++;
							midiEvent.Parameters[0] = Encoding.UTF8.GetString(array, i + 1, array[i]);
							i += array[i] + 1;
							break;
						case 6:
							midiEvent.midiMetaEvent = MidiHelper.MidiMetaEvent.Marker_Text;
							i++;
							midiEvent.Parameters[0] = Encoding.UTF8.GetString(array, i + 1, array[i]);
							i += array[i] + 1;
							break;
						case 7:
							midiEvent.midiMetaEvent = MidiHelper.MidiMetaEvent.Cue_Point;
							i++;
							midiEvent.Parameters[0] = Encoding.UTF8.GetString(array, i + 1, array[i]);
							i += array[i] + 1;
							break;
						case 32:
							midiEvent.midiMetaEvent = MidiHelper.MidiMetaEvent.Midi_Channel_Prefix_Assignment;
							i++;
							midiEvent.parameter1 = array[i++];
							midiEvent.Parameters[0] = midiEvent.parameter1;
							midiEvent.Parameters[1] = array[i++];
							break;
						case 47:
							midiEvent.midiMetaEvent = MidiHelper.MidiMetaEvent.End_of_Track;
							i += 2;
							break;
						case 81:
						{
							midiEvent.midiMetaEvent = MidiHelper.MidiMetaEvent.Tempo;
							i++;
							midiEvent.Parameters[4] = array[i++];
							byte[] array3 = new byte[4];
							for (int num15 = 0; num15 < 3; num15++)
							{
								array3[num15 + 1] = array[num15 + i];
							}
							i += 3;
							byte[] array4 = new byte[4];
							for (int num16 = 0; num16 < 4; num16++)
							{
								array4[3 - num16] = array3[num16];
							}
							uint num17 = BitConverter.ToUInt32(array4, 0);
							midiEvent.Parameters[0] = num17;
							break;
						}
						case 84:
						{
							midiEvent.midiMetaEvent = MidiHelper.MidiMetaEvent.Smpte_Offset;
							i++;
							int num8 = array[i++];
							if (num8 >= 4)
							{
								for (int n = 0; n < 4; n++)
								{
									midiEvent.Parameters[n] = array[i++];
								}
							}
							else
							{
								for (int num9 = 0; num9 < num8; num9++)
								{
									midiEvent.Parameters[num9] = array[i++];
								}
							}
							for (int num10 = 4; num10 < num8; num10++)
							{
								i++;
							}
							break;
						}
						case 88:
						{
							midiEvent.midiMetaEvent = MidiHelper.MidiMetaEvent.Time_Signature;
							i++;
							int num11 = array[i++];
							if (num11 >= 4)
							{
								for (int num12 = 0; num12 < 4; num12++)
								{
									midiEvent.Parameters[num12] = array[i++];
								}
							}
							else
							{
								for (int num13 = 0; num13 < num11; num13++)
								{
									midiEvent.Parameters[num13] = array[i++];
								}
							}
							for (int num14 = 4; num14 < num11; num14++)
							{
								i++;
							}
							break;
						}
						case 89:
						{
							midiEvent.midiMetaEvent = MidiHelper.MidiMetaEvent.Key_Signature;
							i++;
							int num7 = array[i++];
							if (num7 >= 4)
							{
								for (int k = 0; k < 4; k++)
								{
									midiEvent.Parameters[k] = array[i++];
								}
							}
							else
							{
								for (int l = 0; l < num7; l++)
								{
									midiEvent.Parameters[l] = array[i++];
								}
							}
							for (int m = 4; m < num7; m++)
							{
								i++;
							}
							break;
						}
						case 127:
						{
							midiEvent.midiMetaEvent = MidiHelper.MidiMetaEvent.Sequencer_Specific_Event;
							i++;
							midiEvent.Parameters[4] = array[i++];
							byte[] array2 = new byte[(byte)midiEvent.Parameters[4]];
							for (int j = 0; j < array2.Length; j++)
							{
								array2[j] = array[i++];
							}
							midiEvent.Parameters[0] = array2;
							break;
						}
						}
						continue;
					case 240:
						break;
					default:
						continue;
					}
					for (; array[i] != 247; i++)
					{
					}
					i++;
				}
				tracks[num3].Programs = list.ToArray();
				tracks[num3].DrumPrograms = list2.ToArray();
				tracks[num3].MidiEvents = list3.ToArray();
				num3++;
			}
			throw new Exception("Invalid track!");
		}

		private int GetChannel(byte statusbyte)
		{
			statusbyte = (byte)(statusbyte << 4);
			return statusbyte >> 4;
		}

		private uint GetTime(uint data, ref ushort numOfBytes)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			numOfBytes++;
			for (int i = 0; i < bytes.Length && (bytes[i] & 0x80) > 0; i++)
			{
				numOfBytes++;
			}
			for (int j = numOfBytes; j < 4; j++)
			{
				bytes[j] = 0;
			}
			Array.Reverse(bytes);
			data = BitConverter.ToUInt32(bytes, 0);
			data >>= 32 - numOfBytes * 8;
			uint num = data & 0x7F;
			int num2 = 1;
			while ((data >>= 8) != 0)
			{
				num |= (data & 0x7F) << 7 * num2;
				num2++;
			}
			return num;
		}
	}
}
