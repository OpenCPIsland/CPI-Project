using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	public abstract class DSPComponent : MonoBehaviour
	{
		protected List<UnityEngine.Component> _dspInstances = new List<UnityEngine.Component>();

		private Dictionary<string, DSPParameter> _parameters = new Dictionary<string, DSPParameter>();

		protected string _name;

		private bool _isInitialized;

		public DSPType Type
		{
			get;
			set;
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return _isInitialized;
			}
		}

		public virtual void OnInitialise(bool addToAudioSourceGameObject = true)
		{
		}

		public virtual UnityEngine.Component CreateComponent(GameObject gameObject)
		{
			return null;
		}

		public virtual string GetTypeByName()
		{
			return "Uknown";
		}

		public void OnInitialise(string dspComponent)
		{
			AudioComponent[] array = CollectNoInstanceAudioComponents();
			for (int i = 0; i < array.Length; i++)
			{
				AudioComponent audioComponent = array[i];
				if (array[i].AudioSourceGameObject != null)
				{
					_dspInstances.Add(CreateComponent(array[i].AudioSourceGameObject));
				}
				else
				{
					audioComponent.AddDSPComponent(this);
				}
			}
			_name = dspComponent;
			_isInitialized = true;
		}

		public void AddDSPInstance(UnityEngine.Component dspComponentInstance)
		{
			if (_dspInstances == null)
			{
				DebugLog.Print("DSPComponent [ " + base.name + " ] failed to add dsp instance", DebugLevel.Error);
			}
			else
			{
				_dspInstances.Add(dspComponentInstance);
			}
		}

		public void RemoveDSPInstance(UnityEngine.Component dspComponentInstance)
		{
			if (_dspInstances == null)
			{
				DebugLog.Print("DSPComponent [ " + base.name + " ] failed to remove dsp instance", DebugLevel.Error);
			}
			else
			{
				_dspInstances.Remove(dspComponentInstance);
			}
		}

		public virtual void UpdateParameters()
		{
			foreach (KeyValuePair<string, DSPParameter> parameter in _parameters)
			{
				parameter.Value.ResetDirtyFlag();
			}
		}

		protected void AddParameter(string parameter, DSPParameter dspParameter)
		{
			if (!_parameters.ContainsKey(parameter))
			{
				_parameters.Add(parameter, dspParameter);
				dspParameter.SetValue(dspParameter.GetTargetValue(), 0f, 0.5f, true);
			}
		}

		public bool SetParameterValue(string parameter, float value, float time, float curve)
		{
			if (_parameters.ContainsKey(parameter))
			{
				DSPParameter dSPParameter = _parameters[parameter];
				dSPParameter.SetValue(value, time, curve);
				dSPParameter.GetTargetValue();
				return true;
			}
			return false;
		}

		protected float GetParameterValue(string parameter)
		{
			if (_parameters.ContainsKey(parameter))
			{
				DSPParameter dSPParameter = _parameters[parameter];
				return dSPParameter.GetValue();
			}
			return 0f;
		}

		public int GetNumberOfParameters()
		{
			return _parameters.Count;
		}

		public DSPParameter GetParameterByIndex(int index)
		{
			DSPParameter[] array = new DSPParameter[_parameters.Count];
			_parameters.Values.CopyTo(array, 0);
			return array[index];
		}

		public string GetParameterNameByIndex(int index)
		{
			string[] array = new string[_parameters.Count];
			_parameters.Keys.CopyTo(array, 0);
			return array[index];
		}

		private AudioComponent[] CollectNoInstanceAudioComponents()
		{
			List<AudioComponent> list = new List<AudioComponent>();
			int childCount = base.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Component component = base.transform.GetChild(i).GetComponent<Component>();
				if (component != null && component.name != "Instances")
				{
					AudioComponent[] componentsInChildren = component.gameObject.GetComponentsInChildren<AudioComponent>(true);
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						list.Add(componentsInChildren[j]);
					}
				}
			}
			AudioComponent component2 = base.gameObject.GetComponent<AudioComponent>();
			if (component2 != null)
			{
				list.Add(component2);
			}
			return list.ToArray();
		}
	}
}
