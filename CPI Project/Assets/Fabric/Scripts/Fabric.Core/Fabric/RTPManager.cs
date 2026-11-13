using Fabric.TimelineComponent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class RTPManager
	{
		[SerializeField]
		public RTPParameterToProperty[] _parameters;

		[SerializeField]
		public Component _reference;

		private float[] _cachedValues = new float[50];

		public RTPParameterToProperty[] Parameters
		{
			get
			{
				if (_parameters == null)
				{
					_parameters = new RTPParameterToProperty[0];
				}
				return _parameters;
			}
		}

		public void Init(Component component)
		{
			if (_parameters != null)
			{
				for (int i = 0; i < _parameters.Length; i++)
				{
					_parameters[i]._parameter.Init();
				}
			}
			SetupParameterNames(component);
		}

		public void SetupParameterNames(Component component)
		{
			if (_parameters == null)
			{
				return;
			}
			for (int i = 0; i < _parameters.Length; i++)
			{
				RTPParameterToProperty rTPParameterToProperty = _parameters[i];
				bool flag = false;
				string[] array = rTPParameterToProperty._property._name.Split('/');
				if (array.Length == 1)
				{
					switch (rTPParameterToProperty._property._property)
					{
					case 0:
						rTPParameterToProperty._property._componentName = "Component";
						flag = true;
						break;
					case 1:
						rTPParameterToProperty._property._componentName = "Component";
						flag = true;
						break;
					case 2:
						rTPParameterToProperty._property._componentName = "Component";
						flag = true;
						break;
					case 3:
						rTPParameterToProperty._property._componentName = "Component";
						flag = true;
						break;
					case 4:
						rTPParameterToProperty._property._componentName = "Component";
						flag = true;
						break;
					case 5:
						rTPParameterToProperty._property._componentName = "Component";
						flag = true;
						break;
					case 6:
						rTPParameterToProperty._property._componentName = "Component";
						flag = true;
						break;
					case 7:
						rTPParameterToProperty._property._componentName = "Component";
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					for (int j = 0; j < component._dspComponents.Length; j++)
					{
						DSPComponent dSPComponent = component._dspComponents[j];
						if (!(array[0] == dSPComponent.GetTypeByName()))
						{
							continue;
						}
						for (int k = 0; k < dSPComponent.GetNumberOfParameters(); k++)
						{
							if (array[1] == dSPComponent.GetParameterNameByIndex(k))
							{
								rTPParameterToProperty._property._componentName = dSPComponent.GetTypeByName();
								rTPParameterToProperty._property._propertyName = array[1];
								flag = true;
								break;
							}
						}
					}
				}
				if (flag || component._sideChainComponents == null)
				{
					continue;
				}
				for (int l = 0; l < component._sideChainComponents.Length; l++)
				{
					SideChain exists = component._sideChainComponents[l];
					if ((bool)exists && array[0] == "SideChain")
					{
						rTPParameterToProperty._property._componentName = "SideChain";
						flag = true;
					}
				}
			}
		}

		public void Reset()
		{
			if (_parameters != null)
			{
				for (int i = 0; i < _parameters.Length; i++)
				{
					_parameters[i]._parameter.Reset((_parameters[i]._type == RTPParameterType.Random) ? true : false);
				}
			}
		}

		public RTPParameterToProperty AddParameterToProperty(float minY = 0f, float maxY = 1f, float minX = 0f, float maxX = 1f)
		{
			if (_parameters == null)
			{
				return null;
			}
			_parameters = Fabric.TimelineComponent.MyArray<RTPParameterToProperty>.Resize(_parameters, _parameters.Length + 1);
			RTPParameterToProperty rTPParameterToProperty = new RTPParameterToProperty();
			rTPParameterToProperty._envelope = new Fabric.TimelineComponent.Envelope();
			rTPParameterToProperty._envelope._points = new Fabric.TimelineComponent.Point[2];
			rTPParameterToProperty._envelope._points[0] = Fabric.TimelineComponent.Point.Alloc(minX, minY, CurveTypes.Linear);
			rTPParameterToProperty._envelope._points[1] = Fabric.TimelineComponent.Point.Alloc(maxX, maxY, CurveTypes.Linear);
			rTPParameterToProperty._parameter = new RTPParameter(0f, minX, maxX);
			rTPParameterToProperty._property = new RTPProperty();
			_parameters[_parameters.Length - 1] = rTPParameterToProperty;
			return rTPParameterToProperty;
		}

		public void InsertParameterToProperty(RTPParameterToProperty parentParameterToProperty, float minY = 0f, float maxY = 1f, float minX = 0f, float maxX = 1f)
		{
			if (_parameters == null)
			{
				return;
			}
			int index = 0;
			for (int i = 0; i < _parameters.Length; i++)
			{
				if (_parameters[i] == parentParameterToProperty)
				{
					index = i;
					break;
				}
			}
			RTPParameterToProperty rTPParameterToProperty = new RTPParameterToProperty();
			rTPParameterToProperty._envelope = new Fabric.TimelineComponent.Envelope();
			rTPParameterToProperty._envelope._points = new Fabric.TimelineComponent.Point[2];
			rTPParameterToProperty._envelope._points[0] = Fabric.TimelineComponent.Point.Alloc(minX, minY, CurveTypes.Linear);
			rTPParameterToProperty._envelope._points[1] = Fabric.TimelineComponent.Point.Alloc(maxX, maxY, CurveTypes.Linear);
			rTPParameterToProperty._parameter = new RTPParameter(1f, 0f, 1f);
			rTPParameterToProperty._property = new RTPProperty();
			_parameters = Fabric.TimelineComponent.MyArray<RTPParameterToProperty>.InsertAt(_parameters, index, rTPParameterToProperty);
		}

		public void DeleteParameterToProperty(RTPParameterToProperty parameterToProperty)
		{
			if (_parameters == null)
			{
				return;
			}
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
			_parameters = Fabric.TimelineComponent.MyArray<RTPParameterToProperty>.RemoveAt(_parameters, num);
		}

		public static void DeepCopy(RTPParameterToProperty source, RTPParameterToProperty target)
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

		private void ScanTarget(RTPParameterToProperty target, ref Dictionary<string, Serialization.IField> _targetFields)
		{
			_targetFields.Clear();
			foreach (Serialization.IField item in Serialization.EnumerateFields(target))
			{
				_targetFields[item.FieldName] = item;
			}
		}

		public void PasteParameterToProperty(RTPParameterToProperty source, RTPParameterToProperty target)
		{
			if (_parameters == null)
			{
				return;
			}
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

		public EventStatus SetParameter(Event zEvent)
		{
			EventStatus result = EventStatus.Failed_Uknown;
			if (_parameters == null)
			{
				return result;
			}
			for (int i = 0; i < _parameters.Length; i++)
			{
				RTPParameterToProperty rTPParameterToProperty = _parameters[i];
				ParameterData parameterData = (ParameterData)zEvent._parameter;
				if (rTPParameterToProperty._parameter != null && ((parameterData._index >= 0) ? (i == parameterData._index) : (rTPParameterToProperty._parameter._ID == parameterData._parameter)))
				{
					rTPParameterToProperty._parameter.SetValue(parameterData._value);
					result = EventStatus.Handled;
				}
			}
			return result;
		}

		public EventStatus SetMarker(Event zEvent)
		{
			EventStatus result = EventStatus.Failed_Uknown;
			if (_parameters == null)
			{
				return result;
			}
			for (int i = 0; i < _parameters.Length; i++)
			{
				RTPParameterToProperty rTPParameterToProperty = _parameters[i];
				string label = (string)zEvent._parameter;
				RTPMarker marker = rTPParameterToProperty._parameter._markers.GetMarker(label);
				if (marker != null)
				{
					rTPParameterToProperty._parameter.SetNormalisedValue(marker._value);
					result = EventStatus.Handled;
				}
			}
			return result;
		}

		public EventStatus KeyOffMarker(Event zEvent)
		{
			EventStatus result = EventStatus.Failed_Uknown;
			if (_parameters == null)
			{
				return result;
			}
			for (int i = 0; i < _parameters.Length; i++)
			{
				RTPParameterToProperty rTPParameterToProperty = _parameters[i];
				string text = (string)zEvent._parameter;
				rTPParameterToProperty._parameter._markers.KeyOffMarker();
				result = EventStatus.Handled;
			}
			return result;
		}

		public static float CalculateNewValueRange(float value, float newMin, float newMax, float oldMin, float oldMax)
		{
			float num = oldMax - oldMin;
			if (num == 0f)
			{
				value = newMin;
			}
			else
			{
				float num2 = newMax - newMin;
				value = (value - oldMin) * num2 / num + newMin;
			}
			return value;
		}

		private float CalculateNewValue(RTPParameterToProperty entry, float value)
		{
			value = CalculateNewValueRange(value, 0f, 1f, entry._parameter._min, entry._parameter._max);
			value = entry._envelope.Calculate_y(value);
			return CalculateNewValueRange(value, entry._property._min, entry._property._max, 0f, 1f);
		}

		public void Update(Component component)
		{
			if (_parameters == null)
			{
				return;
			}
			for (int i = 0; i < _cachedValues.Length; i++)
			{
				_cachedValues[i] = 1f;
			}
			for (int j = 0; j < _parameters.Length; j++)
			{
				RTPParameterToProperty rTPParameterToProperty = _parameters[j];
				rTPParameterToProperty._parameter.Update();
				float num = 1f;
				float min = rTPParameterToProperty._property._min;
				float max = rTPParameterToProperty._property._max;
				if (rTPParameterToProperty._type == RTPParameterType.Distance || rTPParameterToProperty._parameter.Name == "Distance")
				{
					float num2 = 1f / (rTPParameterToProperty._parameter._max - rTPParameterToProperty._parameter._min);
					if (component.ParentGameObject != null)
					{
						if (FabricManager.Instance._audioListener != null)
						{
							num = Vector3.Distance(component.ParentGameObject.transform.position, FabricManager.Instance._audioListener.transform.position);
						}
						else if (Camera.main != null)
						{
							num = Vector3.Distance(component.ParentGameObject.transform.position, Camera.main.transform.position);
						}
						rTPParameterToProperty._parameter.SetValue(num);
						num = CalculateNewValue(rTPParameterToProperty, rTPParameterToProperty._parameter.GetCurrentValue());
					}
				}
				else if (rTPParameterToProperty._type == RTPParameterType.Modulator)
				{
					RTPModulator rtpModulator = rTPParameterToProperty._rtpModulator;
					if (rtpModulator != null)
					{
						num = rtpModulator.GetValue(Time.time);
						rTPParameterToProperty._parameter.SetValue(num);
						num = CalculateNewValue(rTPParameterToProperty, rTPParameterToProperty._parameter.GetCurrentValue());
					}
				}
				else if (rTPParameterToProperty._type == RTPParameterType.Listener_Angle)
				{
					if (component.ParentGameObject != null)
					{
						Vector3 from = default(Vector3);
						Vector3 to = default(Vector3);
						if (FabricManager.Instance._audioListener != null)
						{
							from = Vector3.forward;
							to = FabricManager.Instance._audioListener.transform.forward;
						}
						else if (Camera.main != null)
						{
							from = Vector3.forward;
							to = Camera.main.transform.forward;
						}
						num = Vector3.Angle(from, to);
						rTPParameterToProperty._parameter.SetValue(num);
						num = CalculateNewValue(rTPParameterToProperty, rTPParameterToProperty._parameter.GetCurrentValue());
					}
				}
				else if (rTPParameterToProperty._type == RTPParameterType.Component_Angle)
				{
					if (component.ParentGameObject != null)
					{
						Vector3 from2 = default(Vector3);
						Vector3 to2 = default(Vector3);
						if (FabricManager.Instance._audioListener != null)
						{
							from2 = component.ParentGameObject.transform.position - FabricManager.Instance._audioListener.transform.position;
							to2 = component.ParentGameObject.transform.forward;
						}
						else if (Camera.main != null)
						{
							from2 = component.ParentGameObject.transform.position - Camera.main.transform.position;
							to2 = component.ParentGameObject.transform.forward;
						}
						num = Vector3.Angle(from2, to2);
						rTPParameterToProperty._parameter.SetValue(num);
						num = CalculateNewValue(rTPParameterToProperty, rTPParameterToProperty._parameter.GetCurrentValue());
					}
				}
				else if (rTPParameterToProperty._type == RTPParameterType.Component_Velocity)
				{
					if (component.ParentGameObject != null)
					{
						num = (component.ParentGameObject.transform.position - rTPParameterToProperty._previousPosition).magnitude / FabricTimer.GetRealtimeDelta();
						rTPParameterToProperty._parameter.SetValue(num);
						num = CalculateNewValue(rTPParameterToProperty, rTPParameterToProperty._parameter.GetCurrentValue());
						rTPParameterToProperty._previousPosition = component.ParentGameObject.transform.position;
					}
				}
				else if (rTPParameterToProperty._type == RTPParameterType.Listener_Velocity)
				{
					if (FabricManager.Instance._audioListener != null)
					{
						num = (FabricManager.Instance._audioListener.transform.position - rTPParameterToProperty._previousPosition).magnitude / FabricTimer.GetRealtimeDelta();
						rTPParameterToProperty._previousPosition = FabricManager.Instance._audioListener.transform.position;
					}
					else if (Camera.main != null)
					{
						num = (Camera.main.transform.position - rTPParameterToProperty._previousPosition).magnitude / FabricTimer.GetRealtimeDelta();
						rTPParameterToProperty._previousPosition = Camera.main.transform.position;
					}
					rTPParameterToProperty._parameter.SetValue(num);
					num = CalculateNewValue(rTPParameterToProperty, rTPParameterToProperty._parameter.GetCurrentValue());
				}
				else if (rTPParameterToProperty._type == RTPParameterType.Volume_Meter)
				{
					VolumeMeter volumeMeter = rTPParameterToProperty._volumeMeter;
					if (volumeMeter != null)
					{
						num = volumeMeter.volumeMeterState.mRMS;
						rTPParameterToProperty._parameter.SetValue(num);
						num = CalculateNewValue(rTPParameterToProperty, rTPParameterToProperty._parameter.GetCurrentValue());
					}
				}
				else if (rTPParameterToProperty._type == RTPParameterType.Global_Parameter)
				{
					GlobalParameter globalParameter = EventManager.Instance._globalParameterManager._globalRTParameters.FindItem(rTPParameterToProperty._globalParameterName);
					if (globalParameter != null)
					{
						rTPParameterToProperty._parameter._max = globalParameter._max;
						rTPParameterToProperty._parameter._min = globalParameter._min;
						rTPParameterToProperty._parameter.SetValue(globalParameter.GetCurrentValue());
						num = CalculateNewValue(rTPParameterToProperty, rTPParameterToProperty._parameter.GetCurrentValue());
					}
				}
				else if (rTPParameterToProperty._type == RTPParameterType.Custom_Parameter)
				{
					ICustomRTPParameter customRTPParameter = FabricManager.Instance._customRTPParameter;
					if (customRTPParameter != null)
					{
						num = customRTPParameter.UpdateProperty(component, rTPParameterToProperty._property, rTPParameterToProperty._propertyType);
						num = CalculateNewValue(rTPParameterToProperty, num);
						rTPParameterToProperty._parameter.SetValue(num);
					}
				}
				else if (rTPParameterToProperty._type == RTPParameterType.Max_Instances)
				{
					num = ((!(component.ComponentHolder != null)) ? 0f : ((float)component.ComponentHolder.GetNumActiveComponentInstances() / (float)component.MaxInstances));
					num = CalculateNewValue(rTPParameterToProperty, num);
					rTPParameterToProperty._parameter.SetValue(num);
				}
				else
				{
					num = CalculateNewValue(rTPParameterToProperty, rTPParameterToProperty._parameter.GetCurrentValue());
				}
				rTPParameterToProperty._property._value = num;
				if ((object)component != null)
				{
					int property = rTPParameterToProperty._property._property;
					_cachedValues[rTPParameterToProperty._property._property] *= num;
					((IRTPPropertyListener)component).UpdateProperty(rTPParameterToProperty._property, _cachedValues[rTPParameterToProperty._property._property], rTPParameterToProperty._propertyType);
				}
			}
		}
	}
}
