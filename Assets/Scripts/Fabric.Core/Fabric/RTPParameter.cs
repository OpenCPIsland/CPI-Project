using Fabric.TimelineComponent;
using System;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class RTPParameter
	{
		private static float MAX_NORMALISED_RANGE = 1f;

		private static float MIN_NORMALISED_RANGE = 0f;

		[HideInInspector]
		[SerializeField]
		public string Name = "";

		[NonSerialized]
		[HideInInspector]
		public int _ID;

		[SerializeField]
		public float _value;

		[SerializeField]
		[HideInInspector]
		public float _min;

		[HideInInspector]
		[SerializeField]
		public float _max = 1f;

		[HideInInspector]
		[SerializeField]
		public ParameterLoopBehaviour _loopBehaviour;

		[HideInInspector]
		[SerializeField]
		public float _velocity;

		[SerializeField]
		[HideInInspector]
		public float _seekSpeed;

		[SerializeField]
		private float _seekTarget;

		[SerializeField]
		[HideInInspector]
		public bool _resetToDefaultValue = true;

		private float _defaultValue;

		private float _direction = 1f;

		[HideInInspector]
		[SerializeField]
		public RTPMarkers _markers = new RTPMarkers();

		public RTPParameter()
		{
		}

		public RTPParameter(float value, float min, float max)
		{
			_value = value;
			_min = min;
			_max = max;
		}

		public void Init()
		{
			_value = _seekTarget;
			_defaultValue = _value;
			_direction = 1f;
			_ID = Name.GetHashCode();
		}

		public void Reset(bool randomise = false)
		{
			if (randomise)
			{
				_value = (_seekTarget = UnityEngine.Random.Range(0f, 1f));
			}
			else if (_resetToDefaultValue)
			{
				_value = (_seekTarget = _defaultValue);
			}
			_markers.Reset();
			_direction = 1f;
		}

		public void SetSeekSpeed(float value)
		{
			_seekSpeed = value;
		}

		public float GetSeekSpeed()
		{
			return _seekSpeed;
		}

		public float GetValue()
		{
			return RTPManager.CalculateNewValueRange(_seekTarget, _min, _max, 0f, 1f);
		}

		public float GetNormalisedValue()
		{
			return _seekTarget;
		}

		public void SetValue(float value)
		{
			AudioTools.Limit(ref value, _min, _max);
			_seekTarget = RTPManager.CalculateNewValueRange(value, 0f, 1f, _min, _max);
		}

		public void SetValueFromMarker(string label)
		{
			RTPMarker marker = _markers.GetMarker(label);
			if (marker != null)
			{
				SetValue(marker._value);
			}
		}

		public void SetNormalisedValue(float value)
		{
			_seekTarget = value;
		}

		public float GetCurrentValue()
		{
			return RTPManager.CalculateNewValueRange(_value, _min, _max, 0f, 1f);
		}

		public float GetNormalisedCurrentValue()
		{
			return _value;
		}

		public void Update()
		{
			float realtimeDelta = FabricTimer.GetRealtimeDelta();
			if (_direction > 0f)
			{
				_seekTarget += _velocity * realtimeDelta;
			}
			else
			{
				_seekTarget -= _velocity * realtimeDelta;
			}
			_markers.Update(_seekTarget, _direction);
			if (_markers.IsMarkerKeyOff())
			{
				_seekTarget = _markers._keyOffMarker._value;
			}
			if (_velocity != 0f)
			{
				if (_seekTarget > MAX_NORMALISED_RANGE)
				{
					if (_loopBehaviour == ParameterLoopBehaviour.Loop)
					{
						_seekTarget -= MAX_NORMALISED_RANGE - MIN_NORMALISED_RANGE;
					}
					else if (_loopBehaviour == ParameterLoopBehaviour.OneShot)
					{
						_seekTarget = MAX_NORMALISED_RANGE;
					}
					else if (_loopBehaviour == ParameterLoopBehaviour.PingPong)
					{
						_direction = -1f;
					}
					else
					{
						_seekTarget = MAX_NORMALISED_RANGE;
					}
					_markers.Reset();
				}
				else if (_seekTarget < MIN_NORMALISED_RANGE)
				{
					if (_loopBehaviour == ParameterLoopBehaviour.Loop)
					{
						_seekTarget += MAX_NORMALISED_RANGE - MIN_NORMALISED_RANGE;
					}
					else if (_loopBehaviour == ParameterLoopBehaviour.OneShot)
					{
						_seekTarget = MIN_NORMALISED_RANGE;
					}
					else if (_loopBehaviour == ParameterLoopBehaviour.PingPong)
					{
						_direction = 1f;
					}
					else
					{
						_seekTarget = MIN_NORMALISED_RANGE;
					}
					_markers.Reset();
				}
				if (_seekTarget > MAX_NORMALISED_RANGE)
				{
					_seekTarget = MAX_NORMALISED_RANGE;
				}
				else if (_seekTarget < MIN_NORMALISED_RANGE)
				{
					_seekTarget = MIN_NORMALISED_RANGE;
				}
			}
			if (_seekSpeed != 0f)
			{
				float num = _seekSpeed * realtimeDelta;
				if (_seekTarget > _value)
				{
					_value += num;
					if (_value > _seekTarget)
					{
						_value = _seekTarget;
					}
				}
				else if (_seekTarget < _value)
				{
					_value -= num;
					if (_value < _seekTarget)
					{
						_value = _seekTarget;
					}
				}
			}
			else
			{
				_value = _seekTarget;
			}
			AudioTools.Limit(ref _value, 0f, 1f);
		}
	}
}
