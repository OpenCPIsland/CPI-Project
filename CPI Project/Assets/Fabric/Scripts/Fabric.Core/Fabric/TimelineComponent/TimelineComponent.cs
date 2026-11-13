using System.Collections.Generic;
using UnityEngine;

namespace Fabric.TimelineComponent
{
	[AddComponentMenu("Fabric/Components/TimelineComponent")]
	public class TimelineComponent : Component, IRTPPropertyListener
	{
		private TimelineParameter[] _parametersList;

		[SerializeField]
		[HideInInspector]
		public bool _isOneShot;

		protected override void OnInitialise(bool inPreviewMode = false)
		{
			if (_parametersList == null)
			{
				_parametersList = GetParameterList(true);
			}
			base.OnInitialise(inPreviewMode);
		}

		public override void PlayInternal(ComponentInstance zComponentInstance, float target, float curve, bool dontPlayComponents)
		{
			if (CheckMIDI(zComponentInstance))
			{
				ResetParameters();
				base.PlayInternal(zComponentInstance, _fadeInTime, _fadeInCurve, dontPlayComponents);
			}
		}

		public TimelineParameter[] GetParameterList(bool update = false)
		{
			if (update || _parametersList == null)
			{
				_parametersList = GetComponents<TimelineParameter>();
			}
			return _parametersList;
		}

		public void SetMarker(string label)
		{
			for (int i = 0; i < _parametersList.Length; i++)
			{
				TimelineParameter timelineParameter = _parametersList[i];
				RTPMarker marker = timelineParameter._markers.GetMarker(label);
				if (marker != null)
				{
					timelineParameter.SetNormalisedValue(marker._value);
				}
			}
		}

		public void KeyOffMarker()
		{
			for (int i = 0; i < _parametersList.Length; i++)
			{
				TimelineParameter timelineParameter = _parametersList[i];
				timelineParameter._markers.KeyOffMarker();
			}
		}

		public TimelineParameter GetParameterByIndex(int index)
		{
			for (int i = 0; i < _parametersList.Length; i++)
			{
				if (i == index)
				{
					return _parametersList[i];
				}
			}
			return null;
		}

		public int GetIndexByParameter(TimelineParameter parameter)
		{
			for (int i = 0; i < _parametersList.Length; i++)
			{
				if (parameter == _parametersList[i])
				{
					return i;
				}
			}
			return -1;
		}

		public TimelineParameter AddParameter(string parameterName)
		{
			TimelineParameter timelineParameter = base.gameObject.AddComponent<TimelineParameter>();
			timelineParameter._name = parameterName;
			_parametersList = base.gameObject.GetComponentsInChildren<TimelineParameter>();
			return timelineParameter;
		}

		public void DeleteParameter(TimelineParameter parameter)
		{
			Object.DestroyImmediate(parameter);
			GetParameterList(true);
		}

		public void SetOneShot(bool isOneShot)
		{
			if (_isOneShot == isOneShot)
			{
				return;
			}
			_isOneShot = isOneShot;
			if (_componentInstances == null)
			{
				return;
			}
			for (int i = 0; i < _componentInstances.Length; i++)
			{
				TimelineComponent timelineComponent = _componentInstances[i]._instance as TimelineComponent;
				if (timelineComponent != null && timelineComponent.IsInstance)
				{
					timelineComponent.SetOneShot(isOneShot);
				}
			}
		}

		public TimelineLayer AddLayer(string layerName)
		{
			GameObject gameObject = new GameObject(base.name);
			gameObject.transform.parent = base.transform;
			return gameObject.AddComponent<TimelineLayer>();
		}

		private void ResetParameters()
		{
			if (_parametersList != null)
			{
				for (int i = 0; i < _parametersList.Length; i++)
				{
					_parametersList[i].Reset();
				}
			}
		}

		public override bool UpdateInternal(ref Context context)
		{
			bool isComponentActive = _isComponentActive;
			if (_parametersList != null)
			{
				for (int i = 0; i < _parametersList.Length; i++)
				{
					_parametersList[i].UpdateInternal();
				}
			}
			_isComponentActive = base.UpdateInternal(ref context);
			if (!_isComponentActive)
			{
				ResetParameters();
			}
			return _isComponentActive;
		}

		public override EventStatus OnProcessEvent(Event zEvent, ComponentInstance zInstance)
		{
			EventStatus result = EventStatus.Failed_Uknown;
			if (zInstance == null)
			{
				DebugLog.Print("Timeline" + base.name + "doesn't have a valid instance", DebugLevel.Error);
				return EventStatus.Failed_Invalid_Instance;
			}
			if (zEvent.EventAction == EventAction.SetParameter)
			{
				ParameterData parameter = (ParameterData)zEvent._parameter;
				List<ComponentInstance> list = FindInstances(zEvent.parentGameObject, false);
				if (list != null && list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						ComponentInstance componentInstance = list[i];
						((TimelineComponent)componentInstance._instance).SetParameter(parameter);
						result = EventStatus.Handled;
					}
				}
				else
				{
					SetParameter(parameter);
					result = EventStatus.Handled;
				}
			}
			else if (zEvent.EventAction == EventAction.SetMarker)
			{
				List<ComponentInstance> list2 = FindInstances(zEvent.parentGameObject, false);
				if (list2 != null && list2.Count > 0)
				{
					for (int j = 0; j < list2.Count; j++)
					{
						ComponentInstance componentInstance2 = list2[j];
						((TimelineComponent)componentInstance2._instance).SetMarker((string)zEvent._parameter);
						result = EventStatus.Handled;
					}
				}
				else
				{
					SetMarker((string)zEvent._parameter);
					result = EventStatus.Handled;
				}
			}
			else if (zEvent.EventAction == EventAction.KeyOffMarker)
			{
				List<ComponentInstance> list3 = FindInstances(zEvent.parentGameObject, false);
				if (list3 != null && list3.Count > 0)
				{
					for (int k = 0; k < list3.Count; k++)
					{
						ComponentInstance componentInstance3 = list3[k];
						((TimelineComponent)componentInstance3._instance).KeyOffMarker();
						result = EventStatus.Handled;
					}
				}
				else
				{
					KeyOffMarker();
					result = EventStatus.Handled;
				}
			}
			return result;
		}

		private void SetParameter(ParameterData parameterData)
		{
			for (int i = 0; i < _parametersList.Length; i++)
			{
				TimelineParameter timelineParameter = _parametersList[i];
				if ((parameterData._index >= 0) ? (i == parameterData._index) : (timelineParameter._ID == parameterData._parameter))
				{
					timelineParameter.SetValue(parameterData._value);
				}
			}
		}

		List<RTPProperty> IRTPPropertyListener.CollectProperties()
		{
			if (_parametersList == null)
			{
				_parametersList = GetParameterList(true);
			}
			List<RTPProperty> list = CollectProperties();
			if (_parametersList != null)
			{
				for (int i = 0; i < _parametersList.Length; i++)
				{
					TimelineParameter timelineParameter = _parametersList[i];
					list.Add(new RTPProperty(8, timelineParameter._name, timelineParameter._min, timelineParameter._max));
				}
			}
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
				for (int i = 0; i < _parametersList.Length; i++)
				{
					TimelineParameter timelineParameter = _parametersList[i];
					if (timelineParameter._name == property._name)
					{
						timelineParameter.SetValue(value);
					}
				}
			}
			return false;
		}
	}
}
