using Fabric.MIDI;
using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Components/MIDIComponent")]
	public class MIDIComponent : Component
	{
		[HideInInspector]
		[SerializeField]
		public MidiSequencer midiSequencer = new MidiSequencer();

		[SerializeField]
		[HideInInspector]
		public string midiFilePath;

		[HideInInspector]
		[SerializeField]
		public bool _loop;

		[SerializeField]
		[HideInInspector]
		public bool _ignoreNoteOff;

		[HideInInspector]
		[SerializeField]
		public bool _controlTargetComponents;

		protected override void OnInitialise(bool inPreviewMode = false)
		{
			base.OnInitialise(inPreviewMode);
			midiSequencer.MidiEvent += OnMidiHandler;
		}

		public void Awake()
		{
			if (!_controlTargetComponents)
			{
				return;
			}
			for (int i = 0; i < midiSequencer.seqEvt.Events.Count; i++)
			{
				if (midiSequencer.seqEvt.Events[i].component != null)
				{
					midiSequencer.seqEvt.Events[i].component.MoveToComponent(this);
				}
			}
		}

		public void LoadMidi(string filename)
		{
			midiSequencer.LoadMidi(filename);
			midiFilePath = filename;
		}

		public void ReloadMidi()
		{
			midiSequencer.ReloadMidi(midiFilePath);
		}

		public void UnloadMidi()
		{
			midiSequencer.UnloadMidi();
			midiFilePath = "";
		}

		public override void PlayInternal(ComponentInstance zComponentInstance, float target, float curve, bool dontPlayComponents)
		{
			if (CheckMIDI(zComponentInstance))
			{
				double offset = 0.0;
				if (_activeMusicTimeSettings != null)
				{
					offset = _activeMusicTimeSettings.GetDelay();
					midiSequencer.SetTempo((uint)_activeMusicTimeSettings._bpm);
					midiSequencer.Looping = _loop;
				}
				midiSequencer.Play(offset);
				base.PlayInternal(zComponentInstance, _fadeInTime, _fadeInCurve, true);
			}
		}

		public override void StopInternal(bool stopInstances, bool forceStop, float target, float curve, double scheduleEnd = 0.0)
		{
			midiSequencer.Stop(forceStop);
			base.StopInternal(stopInstances, forceStop, target, curve, scheduleEnd);
		}

		public override bool IsPlaying()
		{
			return midiSequencer.isPlaying;
		}

		public override bool IsComponentActive()
		{
			return midiSequencer.isPlaying;
		}

		private void FixedUpdate()
		{
			if (midiSequencer.isPlaying)
			{
				midiSequencer.SetTempo((uint)_activeMusicTimeSettings._bpm);
				midiSequencer.Process(AudioSettings.dspTime);
			}
		}

		public void OnSequencerHandler()
		{
		}

		public void OnMidiHandler(Component component, MidiEvent midiEvent, double offset)
		{
			if (component != null)
			{
				if (midiEvent.midiChannelEvent == MidiHelper.MidiChannelEvent.Note_On)
				{
					_componentInstance._instance.SetPlayScheduled(offset, 0.0);
					_componentInstance._instance.SetMIDIEvent(midiEvent);
					component.PlayInternal(_componentInstance, 0f, 0.5f);
				}
				else if (midiEvent.midiChannelEvent == MidiHelper.MidiChannelEvent.Note_Off && !_ignoreNoteOff)
				{
					component.StopInternal(false, false, 0f, 0.5f, offset);
				}
			}
		}
	}
}
