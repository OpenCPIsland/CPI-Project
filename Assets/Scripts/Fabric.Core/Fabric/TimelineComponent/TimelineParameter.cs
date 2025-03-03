using System;
using UnityEngine;

namespace Fabric.TimelineComponent
{
	[Serializable]
	[AddComponentMenu("")]
	public class TimelineParameter : MonoBehaviour
	{
		private float MAX_NORMALISED_RANGE = 1f;

		private float MIN_NORMALISED_RANGE;

		[HideInInspector]
		[SerializeField]
		public string _name;

		[NonSerialized]
		[HideInInspector]
		public int _ID;

		[SerializeField]
		[HideInInspector]
		public float _min;

		[HideInInspector]
		[SerializeField]
		public float _max = 1f;

		[HideInInspector]
		[SerializeField]
		public ParameterLoopBehaviour _loopBehaviour;

		[SerializeField]
		[HideInInspector]
		public float _velocity;

		[SerializeField]
		[HideInInspector]
		public float _seekSpeed;

		[HideInInspector]
		[SerializeField]
		private float _seekTarget;

		[SerializeField]
		[HideInInspector]
		private float _value;

		[SerializeField]
		[HideInInspector]
		public bool _resetToDefaultValue;

		[HideInInspector]
		[SerializeField]
		public RTPMarkers _markers = new RTPMarkers();

		private float _defaultValue;

		private float _direction = 1f;

		public void Awake()
		{
			_value = _seekTarget;
			_defaultValue = _value;
			_ID = _name.GetHashCode();
		}

		public void Reset()
		{
			if (_resetToDefaultValue)
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
			float num = _max - _min;
			return _seekTarget * num;
		}

		public float GetNormalisedValue()
		{
			return _seekTarget;
		}

		public void SetValueFromMarker(string label)
		{
			RTPMarker marker = _markers.GetMarker(label);
			if (marker != null)
			{
				SetValue(marker._value);
			}
		}

		public void SetValue(float value)
		{
			AudioTools.Limit(ref value, _min, _max);
			float num = 1f / (_max - _min);
			value *= num;
			_seekTarget = value;
		}

		public float CalculateNormalisedValue(float value)
		{
			AudioTools.Limit(ref value, _min, _max);
			float num = 1f / (_max - _min);
			value *= num;
			return value;
		}

		public void SetNormalisedValue(float value)
		{
			_seekTarget = value;
		}

		public float GetCurrentValue()
		{
			float num = _max - _min;
			return _value * num;
		}

		public float GetNormalisedCurrentValue()
		{
			return _value;
		}

		public void UpdateInternal()
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
