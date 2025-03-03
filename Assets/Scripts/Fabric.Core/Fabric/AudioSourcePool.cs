using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[ExecuteInEditMode]
	public class AudioSourcePool : MonoBehaviour
	{
		public interface IAudioSourcePoolListener
		{
			bool AudioSourceStolen();
		}

		private Queue<AudioVoice> _audioVoicePool;

		private List<AudioVoice> _allocatedList;

		private List<AudioVoice> _freeingVoicesList;

		private float _fadeInTime;

		private float _fadeOutTime = 0.5f;

		private static AudioSourcePool _instance;

		private void OnEnable()
		{
			AudioSourcePool audioSourcePool = _instance = FabricManager.Instance.AudioSourcePoolManager;
		}

		public static AudioSourcePool Create()
		{
			GameObject gameObject = new GameObject("AudioSourcePool");
			gameObject.transform.parent = FabricManager.Instance.gameObject.transform;
			return gameObject.AddComponent<AudioSourcePool>();
		}

		public static void Destroy()
		{
			FabricManager.Instance.AudioSourcePoolManager.Shutdown();
			Object.DestroyImmediate(FabricManager.Instance.AudioSourcePoolManager.gameObject);
		}

		public int Size()
		{
			if (_audioVoicePool == null)
			{
				return 0;
			}
			return _audioVoicePool.Count;
		}

		public void Refresh()
		{
			AudioVoice[] componentsInChildren = base.gameObject.GetComponentsInChildren<AudioVoice>(true);
			_audioVoicePool = new Queue<AudioVoice>(componentsInChildren);
			_allocatedList = new List<AudioVoice>(_audioVoicePool.Count);
			_freeingVoicesList = new List<AudioVoice>(_audioVoicePool.Count);
		}

		public void Resize(int count)
		{
			if (count > 0 && count != _audioVoicePool.Count)
			{
				AudioVoice[] componentsInChildren = GetComponentsInChildren<AudioVoice>(true);
				AudioVoice[] array = componentsInChildren;
				foreach (AudioVoice audioVoice in array)
				{
					Object.DestroyImmediate(audioVoice.gameObject);
				}
				CreateVoicePool(count);
			}
		}

		public void Initialise(int count, float fadeInTime, float fadeOutTime)
		{
			if (count != 0)
			{
				_audioVoicePool = new Queue<AudioVoice>();
				_allocatedList = new List<AudioVoice>(count);
				_freeingVoicesList = new List<AudioVoice>(count);
				_fadeInTime = fadeInTime;
				_fadeOutTime = fadeOutTime;
				CreateVoicePool(count);
			}
		}

		public void CreateVoicePool(int count)
		{
			int num = 0;
			while (true)
			{
				if (num >= count)
				{
					return;
				}
				GameObject gameObject = null;
				AudioVoice audioVoice = null;
				bool addAudioSource = true;
				if (FabricManager.Instance._VRAudioManager != null && FabricManager.Instance._VRAudioManager._vrSolutions.Count > 0)
				{
					gameObject = FabricManager.Instance._VRAudioManager.GetAudioSource();
					if ((bool)gameObject)
					{
						addAudioSource = false;
					}
					else
					{
						gameObject = new GameObject();
					}
				}
				else
				{
					gameObject = new GameObject();
				}
				gameObject.name = "AudioVoice_" + num;
				audioVoice = gameObject.AddComponent<AudioVoice>();
				if (audioVoice == null || gameObject == null)
				{
					break;
				}
				audioVoice.Init(addAudioSource);
				audioVoice.transform.parent = base.transform;
				Generic.SetGameObjectActive(audioVoice.gameObject, false);
				_audioVoicePool.Enqueue(audioVoice);
				num++;
			}
			DebugLog.Print("Failed to allocate audio source in the pool!", DebugLevel.Error);
		}

		public void Shutdown()
		{
			if (_audioVoicePool != null)
			{
				for (int i = 0; i < _audioVoicePool.Count; i++)
				{
					Object.DestroyImmediate(_audioVoicePool.Dequeue());
				}
			}
			if (_allocatedList != null)
			{
				for (int j = 0; j < _allocatedList.Count; j++)
				{
					Object.DestroyImmediate(_allocatedList[j]);
				}
			}
			if (_freeingVoicesList != null)
			{
				for (int k = 0; k < _freeingVoicesList.Count; k++)
				{
					Object.DestroyImmediate(_allocatedList[k]);
				}
			}
		}

		public AudioSource Alloc(Component component)
		{
			if (_audioVoicePool == null || _audioVoicePool.Count == 0)
			{
				return null;
			}
			AudioVoice audioVoice = _audioVoicePool.Dequeue();
			if (audioVoice == null)
			{
				return null;
			}
			_allocatedList.Add(audioVoice);
			Generic.SetGameObjectActive(audioVoice.gameObject, true);
			audioVoice.Set(component, _fadeInTime);
			return audioVoice._audioSource;
		}

		public void Free(AudioSource audioSource, bool callStop = false)
		{
			if (audioSource != null)
			{
				AudioVoice audioVoice = FindAudioVoiceFromSource(audioSource);
				if (audioVoice != null && _allocatedList.Remove(audioVoice))
				{
					audioVoice.Stop(_fadeOutTime, callStop);
					_freeingVoicesList.Add(audioVoice);
				}
			}
		}

		public void Update()
		{
			if (_freeingVoicesList != null)
			{
				for (int i = 0; i < _freeingVoicesList.Count; i++)
				{
					AudioVoice audioVoice = _freeingVoicesList[i];
					audioVoice.UpdateInternal();
					if (!audioVoice.IsPlaying())
					{
						Generic.SetGameObjectActive(audioVoice.gameObject, false);
						_freeingVoicesList.Remove(audioVoice);
						_audioVoicePool.Enqueue(audioVoice);
					}
				}
			}
			if (_allocatedList != null)
			{
				for (int j = 0; j < _allocatedList.Count; j++)
				{
					_allocatedList[j].UpdateInternal();
				}
			}
		}

		public void FreeAll()
		{
			AudioVoice[] array = _allocatedList.ToArray();
			AudioVoice[] array2 = array;
			foreach (AudioVoice audioVoice in array2)
			{
				Free(audioVoice._audioSource);
			}
		}

		public void GetInfo(ref int numAllocatedVoices, ref int numToBeRemovedVoices)
		{
			if (_allocatedList != null)
			{
				numAllocatedVoices = _allocatedList.Count;
			}
			if (_freeingVoicesList != null)
			{
				numToBeRemovedVoices = _freeingVoicesList.Count;
			}
		}

		public AudioVoice[] GetAllocatedAudioVoices()
		{
			return _allocatedList.ToArray();
		}

		public void SetFadeInTime(float fadeInTime)
		{
			_fadeInTime = fadeInTime;
		}

		public void SetFadeOutTime(float fadeOutTime)
		{
			_fadeOutTime = fadeOutTime;
		}

		private AudioVoice FindAudioVoiceFromSource(AudioSource audioSource)
		{
			for (int i = 0; i < _allocatedList.Count; i++)
			{
				if (_allocatedList[i]._audioSource == audioSource)
				{
					return _allocatedList[i];
				}
			}
			return null;
		}

		private AudioSource FindAudioSource(AudioVoice audioVoice)
		{
			int num = _allocatedList.IndexOf(audioVoice);
			if (num >= 0)
			{
				return _allocatedList[num]._audioSource;
			}
			return null;
		}
	}
}
