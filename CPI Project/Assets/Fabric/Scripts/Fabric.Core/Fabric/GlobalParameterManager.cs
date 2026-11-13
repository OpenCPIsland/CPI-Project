using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class GlobalParameterManager : IEventListener
	{
		[Serializable]
		public class GlobalParametersFastList : FastList<string, GlobalParameter>
		{
		}

		[Serializable]
		public class GlobalSwitchFastList : FastList<string, GlobalSwitch>
		{
		}

		[SerializeField]
		public GlobalParametersFastList _globalRTParameters = new GlobalParametersFastList();

		[SerializeField]
		public GlobalSwitchFastList _globalSwitches = new GlobalSwitchFastList();

		bool IEventListener.IsDestroyed
		{
			get
			{
				return this == null;
			}
		}

		public void Init()
		{
			_globalRTParameters.BuildFast();
			_globalSwitches.BuildFast();
			EventManager.Instance.RegisterListener(this, "GlobalParameter");
		}

		public void Shutdown()
		{
			EventManager.Instance.UnregisterListener(this, "GlobalParameter");
		}

		public bool SetGlobalParameter(string parameterName, float value)
		{
			GlobalParameter globalParameter = _globalRTParameters.FindItem(parameterName);
			if (globalParameter != null)
			{
				globalParameter.SetValue(value);
				return true;
			}
			return false;
		}

		public float GetGlobalParameter(string parameterName)
		{
			GlobalParameter globalParameter = _globalRTParameters.FindItem(parameterName);
			if (globalParameter != null)
			{
				return globalParameter.GetCurrentValue();
			}
			return 0f;
		}

		public bool SetGlobalSwitch(string globalSwitchName, string switchName)
		{
			GlobalSwitch globalSwitch = _globalSwitches.FindItem(globalSwitchName);
			if (globalSwitch != null)
			{
				return globalSwitch.SetActiveSwitch(switchName);
			}
			return false;
		}

		public bool RegisterGlobalSwitchListener(string globalSwitchName, GlobalSwitch.IListener listener)
		{
			GlobalSwitch globalSwitch = _globalSwitches.FindItem(globalSwitchName);
			if (globalSwitch != null)
			{
				return globalSwitch.RegisterListener(listener);
			}
			return false;
		}

		public bool UnregisterGlobalSwitchListener(string globalSwitchName, GlobalSwitch.IListener listener)
		{
			GlobalSwitch globalSwitch = _globalSwitches.FindItem(globalSwitchName);
			if (globalSwitch != null)
			{
				return globalSwitch.UnregisterListener(listener);
			}
			return false;
		}

		public void Update()
		{
			for (int i = 0; i < _globalRTParameters.GetCount(); i++)
			{
				GlobalParameter globalParameter = _globalRTParameters.FindItemByIndex(i);
				if (globalParameter != null)
				{
					globalParameter.Update();
				}
			}
		}

		EventStatus IEventListener.Process(Event zEvent)
		{
			EventStatus result = EventStatus.Not_Handled;
			switch (zEvent.EventAction)
			{
			case EventAction.SetGlobalParameter:
			{
				GlobalParameterData globalParameterData = (GlobalParameterData)zEvent._parameter;
				if (globalParameterData != null)
				{
					SetGlobalParameter(globalParameterData._name, globalParameterData._value);
					result = EventStatus.Handled;
				}
				break;
			}
			case EventAction.SetGlobalSwitch:
			{
				GlobalSwitchParameterData globalSwitchParameterData = (GlobalSwitchParameterData)zEvent._parameter;
				if (globalSwitchParameterData != null)
				{
					SetGlobalSwitch(globalSwitchParameterData._name, globalSwitchParameterData._switch);
					result = EventStatus.Handled;
				}
				break;
			}
			}
			return result;
		}

		bool IEventListener.IsActive(GameObject parentGameObject)
		{
			return false;
		}

		bool IEventListener.GetEventInfo(GameObject parentGameObject, ref EventInfo eventInfo)
		{
			return false;
		}

		bool IEventListener.GetEventListeners(string eventName, List<EventListener> listeners)
		{
			return false;
		}

		bool IEventListener.GetEventListeners(int eventID, List<EventListener> listeners)
		{
			return false;
		}
	}
}
