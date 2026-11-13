using System;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class DSPParameter
	{
		[NonSerialized]
		private bool _isDirty;

		[SerializeField]
		private float _value = 1f;

		[SerializeField]
		private float _min;

		[SerializeField]
		private float _max = 1f;

		private InterpolatedParameter _interpolatedParameter = new InterpolatedParameter();

		public float Min
		{
			get
			{
				return _min;
			}
		}

		public float Max
		{
			get
			{
				return _max;
			}
		}

		public DSPParameter()
		{
		}

		public DSPParameter(float value, float min, float max)
		{
			_value = value;
			_isDirty = false;
			_min = min;
			_max = max;
		}

		public void SetValue(float value, float time = 0f, float curve = 0.5f, bool forceDirtyFlag = false)
		{
			if (value != _value || forceDirtyFlag)
			{
				_value = value;
				if (_value >= _max)
				{
					_value = _max;
				}
				if (_value <= _min)
				{
					_value = _min;
				}
				_interpolatedParameter.SetTarget(FabricTimer.Get(), value, time, curve);
				_isDirty = true;
			}
		}

		public float GetTargetValue()
		{
			return _value;
		}

		public float GetValue()
		{
			return _interpolatedParameter.Get(FabricTimer.Get());
		}

		public void ResetDirtyFlag()
		{
			_isDirty = false;
		}

		public bool HasReachedTarget()
		{
			if (!_isDirty)
			{
				return _interpolatedParameter.HasReachedTarget();
			}
			return false;
		}
	}
}
