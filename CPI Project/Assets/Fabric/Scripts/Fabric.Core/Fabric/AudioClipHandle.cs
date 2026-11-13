using System;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class AudioClipHandle
	{
		[SerializeField]
		private string _audioClipPath = "";

		[SerializeField]
		private bool _useAudioClipPath = true;

		[NonSerialized]
		private int _refCount;

		[NonSerialized]
		private AudioClip _audioClip;

		[NonSerialized]
		private bool _loadAudioDataEventAction;

		public AudioClip AudioClip
		{
			get
			{
				return _audioClip;
			}
			set
			{
				_audioClip = value;
			}
		}

		public bool UseAudioClipPath
		{
			get
			{
				return _useAudioClipPath;
			}
			set
			{
				_useAudioClipPath = value;
			}
		}

		public int RefCount
		{
			get
			{
				return _refCount;
			}
		}

		public void LoadAudioData()
		{
			if (_audioClip != null && _audioClip.loadState != AudioDataLoadState.Loaded)
			{
				_audioClip.LoadAudioData();
			}
			_loadAudioDataEventAction = true;
		}

		public void UnloadAudioData()
		{
			if (_audioClip != null)
			{
				_audioClip.UnloadAudioData();
			}
			_loadAudioDataEventAction = false;
		}

		public void SetAudioClipPath(string path)
		{
			_audioClipPath = path;
		}

		public bool IsAudioClipPathSet()
		{
			if (_audioClipPath.Length <= 0)
			{
				return false;
			}
			return true;
		}

		public string GetAudioClipPath()
		{
			return _audioClipPath;
		}

		public AudioClip IncRef(bool loadAudioClip = true)
		{
			if (_refCount == 0 && loadAudioClip)
			{
				if (_audioClip == null && IsAudioClipPathSet())
				{
					_audioClip = (Resources.Load(_audioClipPath) as AudioClip);
				}
				else if (_audioClip != null)
				{
					if (!_loadAudioDataEventAction && _audioClip.loadState != AudioDataLoadState.Loaded)
					{
						_audioClip.LoadAudioData();
					}
				}
				else
				{
					DebugLog.Print("AudioClipHandle Failed to load audio clip [" + _audioClipPath + "]", DebugLevel.Error);
				}
			}
			_refCount++;
			return _audioClip;
		}

		public void DecRef(bool unloadAudioClip = true)
		{
			_refCount--;
			if (_refCount != 0)
			{
				return;
			}
			if (unloadAudioClip)
			{
				if (_audioClip != null && IsAudioClipPathSet())
				{
					Resources.UnloadAsset(_audioClip);
					_audioClip = null;
				}
				else if (_audioClip != null)
				{
					if (!_loadAudioDataEventAction)
					{
						_audioClip.UnloadAudioData();
					}
				}
				else
				{
					DebugLog.Print("AudioClipHandle Failed to unload audio clip [" + _audioClipPath + "]", DebugLevel.Error);
				}
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(_audioClip);
			}
		}
	}
}
