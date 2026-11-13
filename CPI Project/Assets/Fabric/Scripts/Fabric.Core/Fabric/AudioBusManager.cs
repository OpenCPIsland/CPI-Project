using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Fabric
{
	[Serializable]
	public class AudioBusManager
	{
		[SerializeField]
		public List<AudioBus> _audioBuses = new List<AudioBus>();

		public UnityEngine.Audio.AudioMixer GetAudioBusMixerGroup(string name)
		{
			AudioBus audioBus = FindAudioBusByName(name);
			if (audioBus != null)
			{
				return audioBus._audioMixerGroup.audioMixer;
			}
			return null;
		}

		public bool IncrementAudioBusInstance(string name)
		{
			AudioBus audioBus = FindAudioBusByName(name);
			if (audioBus != null)
			{
				return audioBus.IncrementInstance();
			}
			return false;
		}

		public bool DecrementAudioBusInstance(string name)
		{
			AudioBus audioBus = FindAudioBusByName(name);
			if (audioBus != null)
			{
				return audioBus.DecrementInstance();
			}
			return false;
		}

		public bool IncrementAudioBusAudioComponent(string name)
		{
			AudioBus audioBus = FindAudioBusByName(name);
			if (audioBus != null)
			{
				return audioBus.IncrementAudioComponent();
			}
			return false;
		}

		public bool DecrementAudioBusAudioComponent(string name)
		{
			AudioBus audioBus = FindAudioBusByName(name);
			if (audioBus != null)
			{
				return audioBus.DecrementAudioComponent();
			}
			return false;
		}

		public List<string> GetAudioBusNames()
		{
			List<string> list = new List<string>();
			list.Add("None");
			for (int i = 0; i < _audioBuses.Count; i++)
			{
				list.Add(_audioBuses[i]._name);
			}
			return list;
		}

		public int GetAudioBusIndexByName(string name)
		{
			for (int i = 0; i < _audioBuses.Count; i++)
			{
				if (_audioBuses[i]._name == name)
				{
					return i + 1;
				}
			}
			return 0;
		}

		public AudioBus FindAudioBusByName(string name)
		{
			if (name.Length == 0)
			{
				return null;
			}
			for (int i = 0; i < _audioBuses.Count; i++)
			{
				if (_audioBuses[i]._name == name)
				{
					return _audioBuses[i];
				}
			}
			return null;
		}

		public bool CreateAudioBus(string name)
		{
			if (FindAudioBusByName(name) != null)
			{
				return false;
			}
			AudioBus audioBus = new AudioBus();
			audioBus._name = name;
			_audioBuses.Add(audioBus);
			return true;
		}

		public void DestroyAudioBus(string name)
		{
			AudioBus audioBus = FindAudioBusByName(name);
			if (audioBus != null)
			{
				_audioBuses.Remove(audioBus);
			}
		}

		public void SetAudioBusParameter(string busName, string parameterName, float value)
		{
			AudioBus audioBus = FindAudioBusByName(busName);
			if (audioBus != null)
			{
				audioBus._audioMixerGroup.audioMixer.SetFloat(parameterName, value);
			}
		}
	}
}
