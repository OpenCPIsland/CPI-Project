using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric.TimelineComponent
{
	[AddComponentMenu("")]
	public class TimelineLayer : Component
	{
		[HideInInspector]
		[SerializeField]
		public TimelineRegion[] _regions;

		[SerializeField]
		[HideInInspector]
		public ParameterToProperty[] _parameters;

		[HideInInspector]
		[SerializeField]
		public TimelineParameter _controlParameter;

		private bool _layerIsActive;

		public ParameterToProperty[] Parameters
		{
			get
			{
				if (_parameters == null)
				{
					_parameters = new ParameterToProperty[0];
				}
				return _parameters;
			}
		}

		public TimelineRegion[] Regions
		{
			get
			{
				if (_regions == null)
				{
					_regions = base.gameObject.GetComponentsInChildren<TimelineRegion>();
				}
				return _regions;
			}
		}

		protected override void OnInitialise(bool inPreviewMode = false)
		{
			_regions = base.gameObject.GetComponentsInChildren<TimelineRegion>();
			UpdateRegionEnvelopes();
		}

		public override void PlayInternal(ComponentInstance zComponentInstance, float target, float curve, bool dontPlayComponents)
		{
			base.PlayInternal(zComponentInstance, target, curve, true);
			_layerIsActive = true;
			UpdateRegionEnvelopes();
		}

		public override void StopInternal(bool stopInstances, bool forceStop, float target, float curve, double scheduleEnd = 0.0)
		{
			base.StopInternal(stopInstances, forceStop, target, curve, scheduleEnd);
			_layerIsActive = false;
		}

		public override bool IsPlaying()
		{
			if (_layerIsActive || base.IsPlaying())
			{
				return true;
			}
			return false;
		}

		public override bool IsComponentActive()
		{
			if (_layerIsActive || _isComponentActive)
			{
				return true;
			}
			return false;
		}

		public override bool UpdateInternal(ref Context context)
		{
			if (_controlParameter == null || _parameters == null || _regions == null)
			{
				return false;
			}
			float num = 0f;
			if ((bool)_controlParameter)
			{
				num = _controlParameter.GetNormalisedCurrentValue();
			}
			float num2 = 1f;
			float num3 = 1f;
			for (int i = 0; i < _parameters.Length; i++)
			{
				ParameterToProperty parameterToProperty = _parameters[i];
				if (parameterToProperty != null && parameterToProperty._parameter != null)
				{
					if (parameterToProperty._property == Property.Volume)
					{
						num2 *= parameterToProperty._envelope.Calculate_y(parameterToProperty._parameter.GetNormalisedCurrentValue());
					}
					else if (parameterToProperty._property == Property.Pitch)
					{
						float num4 = parameterToProperty._envelope.Calculate_y(parameterToProperty._parameter.GetNormalisedCurrentValue());
						num3 *= Mathf.Pow(2f, num4 * 8f - 4f);
					}
				}
			}
			for (int j = 0; j < _regions.Length; j++)
			{
				TimelineRegion timelineRegion = _regions[j];
				float num5 = Mathf.Round(num * _controlParameter._max * 100f) / 100f;
				float num6 = Mathf.Round(timelineRegion._x * _controlParameter._max * 100f) / 100f;
				float num7 = num6 + Mathf.Round(timelineRegion._width * _controlParameter._max * 100f) / 100f;
				if (_componentInstance != null && num5 >= num6 && num5 <= num7)
				{
					float num8 = 1f;
					if (num != 0f && num != 1f)
					{
						num8 = timelineRegion._volumeEnvelope.Calculate_y(num);
					}
					timelineRegion.Volume = num8 * num2 * timelineRegion._regionVolume;
					float num9 = 1f;
					if (timelineRegion._autopitchenabled)
					{
						num9 = 0f + num / timelineRegion._autopitchreference * 1f;
					}
					timelineRegion.Pitch = num9 * num3;
					if (!timelineRegion.IsPlaying() && _layerIsActive)
					{
						timelineRegion.PlayInternal(_componentInstance, 0f, 0.5f);
					}
					else if (!IsOneShot() && timelineRegion._loopMode == RegionLoopMode.PlayToEnd && timelineRegion.HasReachedEnd())
					{
						timelineRegion.StopInternal(false, false, 0f, 0.5f);
					}
				}
				else if (timelineRegion.IsPlaying())
				{
					if (timelineRegion._loopMode == RegionLoopMode.Cutoff)
					{
						timelineRegion.StopInternal(false, true, 0f, 0.5f);
					}
					else if (timelineRegion._loopMode == RegionLoopMode.PlayToEnd)
					{
						timelineRegion._regionIsActive = false;
					}
				}
				if (timelineRegion._loopMode == RegionLoopMode.PlayToEnd && !timelineRegion.IsOneShot() && timelineRegion.HasReachedEnd())
				{
					timelineRegion.StopInternal(false, false, 0f, 0.5f);
				}
			}
			bool flag = false;
			flag = base.UpdateInternal(ref context);
			if (((TimelineComponent)_parentComponent)._isOneShot || !_layerIsActive)
			{
				_isComponentActive = flag;
				if (!_isComponentActive)
				{
					_layerIsActive = false;
				}
			}
			else
			{
				_isComponentActive = _layerIsActive;
			}
			return _isComponentActive;
		}

		public CurveTypes ConvertEnvelopeTypes(EnvelopPointTypes types)
		{
			switch (types)
			{
			case EnvelopPointTypes.Bezier:
				return CurveTypes.Bezier;
			case EnvelopPointTypes.Linear:
				return CurveTypes.Linear;
			case EnvelopPointTypes.Flat:
				return CurveTypes.Flat;
			case EnvelopPointTypes.Log:
				return CurveTypes.Log;
			default:
				return CurveTypes.Linear;
			}
		}

		public void SetControlParameter(TimelineParameter controlParameter)
		{
			if (controlParameter != null)
			{
				_controlParameter = controlParameter;
			}
		}

		public void UpdateRegionEnvelopes()
		{
			if (_regions.Length <= 0)
			{
				return;
			}
			Array.Sort(_regions, (TimelineRegion r1, TimelineRegion r2) => r1._x.CompareTo(r2._x));
			for (int i = 0; i < _regions.Length; i++)
			{
				_regions[i].ResetVolumeEnvelope();
			}
			for (int j = 0; j < _regions.Length - 1; j++)
			{
				TimelineRegion timelineRegion = _regions[j];
				TimelineRegion timelineRegion2 = _regions[j + 1];
				float num = timelineRegion._x + timelineRegion._width;
				if (num > timelineRegion2._x)
				{
					float num2 = num - timelineRegion2._x;
					timelineRegion._volumeEnvelope._points[2]._x -= num2;
					timelineRegion2._volumeEnvelope._points[1]._x += num2;
				}
			}
		}

		public void DeleteParameterToProperty(ParameterToProperty parameterToProperty)
		{
			int num = 0;
			while (true)
			{
				if (num < _parameters.Length)
				{
					if (_parameters[num] == parameterToProperty)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			_parameters = MyArray<ParameterToProperty>.RemoveAt(_parameters, num);
		}

		public static void DeepCopy(ParameterToProperty source, ParameterToProperty target)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (Serialization.IField item in Serialization.EnumerateFields(source))
			{
				dictionary[item.FieldName] = item.GetValue();
			}
			foreach (Serialization.IField item2 in Serialization.EnumerateFields(target))
			{
				item2.SetValue(dictionary[item2.FieldName]);
			}
		}

		public void PasteParameterToProperty(ParameterToProperty source, ParameterToProperty target)
		{
			int num = 0;
			while (true)
			{
				if (num < _parameters.Length)
				{
					if (_parameters[num] == target)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			DeepCopy(source, target);
		}

		public void AddParameterToProperty()
		{
			_parameters = MyArray<ParameterToProperty>.Resize(_parameters, _parameters.Length + 1);
			ParameterToProperty parameterToProperty = new ParameterToProperty();
			parameterToProperty._envelope = new Envelope();
			parameterToProperty._envelope._points = new Point[2];
			parameterToProperty._envelope._points[0] = Point.Alloc(0f, 0f, CurveTypes.Linear);
			parameterToProperty._envelope._points[1] = Point.Alloc(1f, 1f, CurveTypes.Linear);
			_parameters[_parameters.Length - 1] = parameterToProperty;
		}

		public TimelineRegion AddRegion(string regionName)
		{
			GameObject gameObject = new GameObject(regionName);
			gameObject.transform.parent = base.transform;
			TimelineRegion timelineRegion = gameObject.AddComponent<TimelineRegion>();
			timelineRegion.ResetVolumeEnvelope();
			_regions = base.gameObject.GetComponentsInChildren<TimelineRegion>();
			return timelineRegion;
		}

		public void RemoveRegion(TimelineRegion region)
		{
			UnityEngine.Object.DestroyImmediate(region.gameObject);
			_regions = base.gameObject.GetComponentsInChildren<TimelineRegion>();
			UpdateRegionEnvelopes();
		}
	}
}
