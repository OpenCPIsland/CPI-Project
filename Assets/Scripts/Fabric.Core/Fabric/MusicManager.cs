using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class MusicManager
	{
		[SerializeField]
		public List<MusicTimeSittings> _musicTimeSignatures = new List<MusicTimeSittings>();

		[NonSerialized]
		internal List<MusicTimeSittings> _componentMusicTimeSettings = new List<MusicTimeSittings>();

		internal void RegisterComponentMusicTimeSettings(Component component, MusicTimeSittings musicTimeSettings)
		{
			_componentMusicTimeSettings.Add(musicTimeSettings);
		}

		private void Update()
		{
			for (int i = 0; i < _musicTimeSignatures.Count; i++)
			{
				_musicTimeSignatures[i].Update();
			}
			for (int j = 0; j < _componentMusicTimeSettings.Count; j++)
			{
				_componentMusicTimeSettings[j].Update();
			}
		}

		public MusicTimeSittings GetMusicSettingByName(string name)
		{
			for (int i = 0; i < _musicTimeSignatures.Count; i++)
			{
				if (_musicTimeSignatures[i]._name == name)
				{
					return _musicTimeSignatures[i];
				}
			}
			return null;
		}

		public MusicTimeSittings GetMusicSettingByIndex(int index)
		{
			index--;
			if (index < 0 || index >= _musicTimeSignatures.Count)
			{
				return null;
			}
			return _musicTimeSignatures[index];
		}

		public void RegisterMusicSyncNotifications(string name, MusicTimeSittings.OnBarHandler onBarCallback, MusicTimeSittings.OnBeatHandler onBeatCallback)
		{
			MusicTimeSittings musicSettingByName = GetMusicSettingByName(name);
			if (musicSettingByName != null)
			{
				musicSettingByName._onBeatDetected += onBeatCallback;
				musicSettingByName._onBarDetected += onBarCallback;
			}
		}
	}
}
