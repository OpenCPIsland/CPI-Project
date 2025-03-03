using System;
using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Components/RandomAudioClipComponent")]
	public class RandomAudioClipComponent : AudioComponent
	{
		private AudioClip _selectedAudioClip;

		private int[] _randomNoRepeatIndexes;

		private int _randomNoRepeatIndex = -1;

		[SerializeField]
		[HideInInspector]
		public RandomComponentPlayMode _playMode;

		[HideInInspector]
		[SerializeField]
		public int[] _randomWeights;

		[HideInInspector]
		[SerializeField]
		public bool _shareRandomNoRepeatHistory;

		[SerializeField]
		public AudioClip[] _audioClips;

		private ShuffleBag<int> _shuffleBag = new ShuffleBag<int>();

		[HideInInspector]
		[SerializeField]
		public bool _looped;

		[SerializeField]
		[HideInInspector]
		public bool _delayOnFirstPlay;

		[HideInInspector]
		[SerializeField]
		public float _loopDelay;

		[HideInInspector]
		[SerializeField]
		public float _delayRandomization;

		[HideInInspector]
		[SerializeField]
		public float _delayMaxRandomization;

		private System.Random _randomComponents
		{
			get
			{
				return Generic._random;
			}
		}

		private void OnDestroy()
		{
			for (int i = 0; i < _audioClips.Length; i++)
			{
				_audioClips[i] = null;
			}
		}

		protected override void OnInitialise(bool inPreviewMode = false)
		{
			base.OnInitialise(inPreviewMode);
			InitialiseRandomComponent(_playMode);
		}

		internal override void OnFinishPlaying(double time)
		{
			if (_componentInstance != null && _looped)
			{
				float num = _loopDelay + UnityEngine.Random.Range(_delayRandomization, _delayMaxRandomization);
				_componentInstance._instance.SetPlayScheduled(time + (double)num, 0.0);
				PlayRandomomponent(_componentInstance);
			}
			else
			{
				base.OnFinishPlaying(time);
			}
		}

		public override bool IsLooped()
		{
			if (!_looped)
			{
				return base.IsLooped();
			}
			return true;
		}

		public override void PlayInternal(ComponentInstance zComponentInstance, float target, float curve, bool dontPlayComponents)
		{
			if (CheckMIDI(zComponentInstance))
			{
				if (_delayOnFirstPlay)
				{
					float num = _loopDelay + UnityEngine.Random.Range(_delayRandomization, _delayMaxRandomization);
					zComponentInstance._instance.SetPlayScheduledAdditive(num, 0.0);
				}
				PlayRandomomponent(zComponentInstance);
			}
		}

		private void PlayRandomomponent(ComponentInstance zComponentInstance)
		{
			if (_playMode == RandomComponentPlayMode.Random)
			{
				int num = _shuffleBag.get();
				_selectedAudioClip = _audioClips[num];
			}
			else
			{
				int num2 = -1;
				if (!_shareRandomNoRepeatHistory)
				{
					num2 = GetNextRandomNoRepeatIndex();
				}
				else
				{
					RandomAudioClipComponent randomAudioClipComponent = base.ComponentHolder as RandomAudioClipComponent;
					if ((bool)randomAudioClipComponent)
					{
						num2 = randomAudioClipComponent.GetNextRandomNoRepeatIndex();
					}
				}
				if (num2 < 0)
				{
					return;
				}
				_selectedAudioClip = _audioClips[num2];
			}
			if (_selectedAudioClip != null)
			{
				base.AudioClip = _selectedAudioClip;
				base.PlayInternal(zComponentInstance, 0f, 0.5f, false);
			}
		}

		public void InitialiseRandomComponent(RandomComponentPlayMode playMode)
		{
			if (playMode == RandomComponentPlayMode.Random)
			{
				InitialiseWeights();
				UpdateWeights();
				_shareRandomNoRepeatHistory = false;
				return;
			}
			_randomNoRepeatIndexes = new int[_audioClips.Length];
			for (int i = 0; i < _randomNoRepeatIndexes.Length; i++)
			{
				_randomNoRepeatIndexes[i] = i;
			}
			MyArray<int>.Shuffle(_randomNoRepeatIndexes, _randomComponents);
		}

		private int GetNextRandomNoRepeatIndex()
		{
			int result = 0;
			if (_randomNoRepeatIndexes.Length > 1)
			{
				if (_randomNoRepeatIndex == -1 || _randomNoRepeatIndex >= _audioClips.Length)
				{
					int num = _randomNoRepeatIndexes[_randomNoRepeatIndexes.Length - 1];
					MyArray<int>.Shuffle(_randomNoRepeatIndexes, _randomComponents);
					if (num == _randomNoRepeatIndexes[0])
					{
						int num2 = _randomNoRepeatIndexes[0];
						_randomNoRepeatIndexes[0] = _randomNoRepeatIndexes[1];
						_randomNoRepeatIndexes[1] = num2;
					}
					_randomNoRepeatIndex = 0;
				}
				result = _randomNoRepeatIndexes[_randomNoRepeatIndex++];
			}
			return result;
		}

		public void InitialiseWeights()
		{
			if (_audioClips != null && _audioClips.Length != 0 && (_randomWeights == null || _randomWeights.Length != _audioClips.Length))
			{
				_randomWeights = new int[_audioClips.Length];
				for (int i = 0; i < _randomWeights.Length; i++)
				{
					_randomWeights[i] = 100;
				}
			}
		}

		public void UpdateWeights()
		{
			_shuffleBag.clear();
			if (_randomWeights != null)
			{
				for (int i = 0; i < _randomWeights.Length; i++)
				{
					_shuffleBag.addMany(i, _randomWeights[i]);
				}
			}
		}
	}
}
