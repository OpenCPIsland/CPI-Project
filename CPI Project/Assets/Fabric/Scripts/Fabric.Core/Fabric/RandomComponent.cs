using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Components/RandomComponent")]
	public class RandomComponent : Component, IRTPPropertyListener
	{
		private Component _selectedComponent;

		private int[] _randomNoRepeatIndexes;

		private int _randomNoRepeatIndex = -1;

		[HideInInspector]
		[SerializeField]
		public RandomComponentPlayMode _playMode;

		[SerializeField]
		[HideInInspector]
		public int[] _randomWeights;

		[HideInInspector]
		[SerializeField]
		public bool _looped;

		[SerializeField]
		[HideInInspector]
		public bool _delayOnFirstPlay;

		[SerializeField]
		[HideInInspector]
		public RandomComponentTriggerMode _triggerMode;

		[SerializeField]
		[HideInInspector]
		public float _delay;

		[NonSerialized]
		private float _retriggerTimer;

		[NonSerialized]
		public float _retriggerTime;

		[HideInInspector]
		[SerializeField]
		public float _delayRandomization;

		[SerializeField]
		[HideInInspector]
		public float _delayMaxRandomization;

		[SerializeField]
		[HideInInspector]
		public bool _shareRandomNoRepeatHistory;

		private ShuffleBag<int> _shuffleBag = new ShuffleBag<int>();

		[NonSerialized]
		[HideInInspector]
		private ComponentStatus _status = ComponentStatus.Stopped;

		private System.Random _randomComponents
		{
			get
			{
				return Generic._random;
			}
		}

		protected override void OnInitialise(bool inPreviewMode = false)
		{
			base.OnInitialise(inPreviewMode);
			InitialiseRandomComponent(_playMode);
		}

		private bool PlayNextRandom(double time)
		{
			if (_componentInstance != null && _looped)
			{
				float num = _delay + UnityEngine.Random.Range(_delayRandomization, _delayMaxRandomization);
				_componentInstance._instance.SetPlayScheduled(time + (double)num, 0.0);
				PlayRandomomponent(_componentInstance);
				return true;
			}
			return false;
		}

		internal override bool OnMarker(double time)
		{
			if (_triggerMode == RandomComponentTriggerMode.WaitOnMarker && !PlayNextRandom(time))
			{
				return base.OnMarker(time);
			}
			return false;
		}

		internal override void OnFinishPlaying(double time)
		{
			if (_triggerMode == RandomComponentTriggerMode.WaitToFinish && !PlayNextRandom(time))
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

		public override bool IsComponentActive()
		{
			if (_looped && _playMode == RandomComponentPlayMode.RandomNoRepeat && _triggerMode == RandomComponentTriggerMode.Retrigger)
			{
				if (_status == ComponentStatus.Stopped)
				{
					return false;
				}
				return true;
			}
			return base.IsComponentActive();
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
			_randomNoRepeatIndexes = new int[_components.Count];
			for (int i = 0; i < _randomNoRepeatIndexes.Length; i++)
			{
				_randomNoRepeatIndexes[i] = i;
			}
			MyArray<int>.Shuffle(_randomNoRepeatIndexes, _randomComponents);
		}

		public override void PlayInternal(ComponentInstance zComponentInstance, float target, float curve, bool dontPlayComponents)
		{
			if (CheckMIDI(zComponentInstance) && _components.Count != 0)
			{
				if (_selectedComponent != null)
				{
					_selectedComponent.Stop();
				}
				base.PlayInternal(zComponentInstance, target, curve, true);
				float num = 0f;
				if (_delayOnFirstPlay)
				{
					num = _delay + UnityEngine.Random.Range(_delayRandomization, _delayMaxRandomization);
					_componentInstance._instance.SetPlayScheduledAdditive(num, 0.0);
				}
				PlayRandomomponent(zComponentInstance);
				if (_looped && _playMode == RandomComponentPlayMode.RandomNoRepeat && _triggerMode == RandomComponentTriggerMode.Retrigger)
				{
					_retriggerTime = 0f;
					_retriggerTime = num;
				}
				_status = ComponentStatus.Playing;
			}
		}

		public override void StopInternal(bool stopInstances, bool forceStop, float target, float curve, double scheduleEnd = 0.0)
		{
			if (_looped && _playMode == RandomComponentPlayMode.RandomNoRepeat && _triggerMode == RandomComponentTriggerMode.Retrigger)
			{
				_retriggerTime = 0f;
				_retriggerTimer = 0f;
			}
			base.StopInternal(stopInstances, forceStop, target, curve, scheduleEnd);
			_status = ComponentStatus.Stopped;
		}

		private void RetriggerPlayFunction()
		{
			if (_status != ComponentStatus.Paused)
			{
				PlayRandomomponent(_componentInstance);
			}
		}

		private int GetNextRandomNoRepeatIndex()
		{
			int result = 0;
			if (_randomNoRepeatIndexes.Length > 1)
			{
				if (_randomNoRepeatIndex == -1 || _randomNoRepeatIndex >= _components.Count)
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

		private void PlayRandomomponent(ComponentInstance zComponentInstance)
		{
			if (_playMode == RandomComponentPlayMode.Random)
			{
				int index = _shuffleBag.get();
				_selectedComponent = _components[index];
			}
			else
			{
				int num = -1;
				if (!_shareRandomNoRepeatHistory)
				{
					num = GetNextRandomNoRepeatIndex();
				}
				else
				{
					RandomComponent randomComponent = base.ComponentHolder as RandomComponent;
					if ((bool)randomComponent)
					{
						num = randomComponent.GetNextRandomNoRepeatIndex();
					}
				}
				if (num < 0)
				{
					return;
				}
				_selectedComponent = _components[num];
			}
			if (_selectedComponent != null)
			{
				_selectedComponent.PlayInternal(zComponentInstance, 0f, 0.5f);
			}
		}

		public void InitialiseWeights()
		{
			Component[] childComponents = GetChildComponents();
			if (childComponents != null && childComponents.Length != 0 && (_randomWeights == null || _randomWeights.Length != childComponents.Length))
			{
				_randomWeights = new int[childComponents.Length];
				for (int i = 0; i < _randomWeights.Length; i++)
				{
					_randomWeights[i] = 100;
				}
			}
		}

		public override bool UpdateInternal(ref Context context)
		{
			if (_looped && _playMode == RandomComponentPlayMode.RandomNoRepeat && _triggerMode == RandomComponentTriggerMode.Retrigger)
			{
				float realtimeDelta = FabricTimer.GetRealtimeDelta();
				_retriggerTimer += realtimeDelta;
				if (_retriggerTimer >= _retriggerTime)
				{
					_componentInstance._instance.SetPlayScheduled(_retriggerTimer - _retriggerTime, 0.0);
					PlayRandomomponent(_componentInstance);
					_retriggerTime = _delay + UnityEngine.Random.Range(_delayRandomization, _delayMaxRandomization);
					_retriggerTimer = 0f;
				}
			}
			return base.UpdateInternal(ref context);
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

		List<RTPProperty> IRTPPropertyListener.CollectProperties()
		{
			List<RTPProperty> list = CollectProperties();
			list.Add(new RTPProperty(8, "Delay", 0f, 60f));
			list.Add(new RTPProperty(9, "Delay Randomization", 0f, 60f));
			return list;
		}

		bool IRTPPropertyListener.UpdateProperty(RTPProperty property, float value, RTPPropertyType type)
		{
			if (UpdateProperty(property, value, type))
			{
				return true;
			}
			if (property._property == 8)
			{
				if (property._name == "Delay")
				{
					_delay = RTPParameterToProperty.SetValueByType(_delay, value, type);
					if (_triggerMode == RandomComponentTriggerMode.Retrigger)
					{
						_retriggerTime = _delay;
					}
					return true;
				}
				if (property._name == "Delay Randomization")
				{
					_delayMaxRandomization = RTPParameterToProperty.SetValueByType(_delayMaxRandomization, value, type);
					return true;
				}
			}
			return false;
		}
	}
}
