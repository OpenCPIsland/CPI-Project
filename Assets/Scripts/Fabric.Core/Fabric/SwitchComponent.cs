using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Components/SwitchComponent")]
	public class SwitchComponent : Component, GlobalSwitch.IListener, IRTPPropertyListener
	{
		[Serializable]
		public class GlobalSwitchContainer
		{
			[SerializeField]
			public string _switchName;

			[SerializeField]
			public List<Component> _components = new List<Component>();
		}

		[SerializeField]
		[HideInInspector]
		public Component _selectedComponent;

		private Component _defferedSelectedComponent;

		[SerializeField]
		[HideInInspector]
		public bool _startOnSwitch = true;

		[SerializeField]
		[HideInInspector]
		public bool _syncToMusicOnFirstPlay = true;

		[HideInInspector]
		[SerializeField]
		public SwitchComponentSwitchType _switchComponentSwitchType = SwitchComponentSwitchType.OnSwitch;

		[SerializeField]
		[HideInInspector]
		public bool _useGlobalSwitch;

		[HideInInspector]
		[SerializeField]
		public string _globalSwitch;

		[SerializeField]
		[HideInInspector]
		public List<GlobalSwitchContainer> _globalSwitchMap = new List<GlobalSwitchContainer>();

		protected override void OnInitialise(bool inPreviewMode = false)
		{
			if (_useGlobalSwitch && _globalSwitch != null)
			{
				EventManager.Instance._globalParameterManager.RegisterGlobalSwitchListener(_globalSwitch, this);
			}
			base.OnInitialise(inPreviewMode);
		}

		private GlobalSwitchContainer GetGlobalSwitchContainer(GlobalSwitch.Switch globalSwitch)
		{
			for (int i = 0; i < _globalSwitchMap.Count; i++)
			{
				GlobalSwitchContainer globalSwitchContainer = _globalSwitchMap[i];
				if (globalSwitch._name == globalSwitchContainer._switchName)
				{
					return globalSwitchContainer;
				}
			}
			return null;
		}

		private void OnDestroy()
		{
			if (_useGlobalSwitch && _globalSwitch != null && EventManager.Instance != null)
			{
				EventManager.Instance._globalParameterManager.UnregisterGlobalSwitchListener(_globalSwitch, this);
			}
		}

		bool GlobalSwitch.IListener.OnSwitch(GlobalSwitch.Switch _switch)
		{
			for (int i = 0; i < _componentInstances.Length; i++)
			{
				ComponentInstance componentInstance = _componentInstances[i];
				if (componentInstance == null)
				{
					continue;
				}
				GlobalSwitchContainer globalSwitchContainer = GetGlobalSwitchContainer(_switch);
				if (globalSwitchContainer == null)
				{
					continue;
				}
				Component component = globalSwitchContainer._components[0];
				if (!(component != null) || !(component != _selectedComponent))
				{
					continue;
				}
				bool isComponentActive = _isComponentActive;
				if (_selectedComponent != null)
				{
					_selectedComponent.StopInternal(false, false, 0f, 0.5f);
				}
				_selectedComponent = component;
				if (_startOnSwitch && isComponentActive && !IsMusicSyncEnabled())
				{
					_componentInstance._instance.ResetPlayScheduled();
					_selectedComponent.PlayInternal(_componentInstance, 0f, 0.5f);
					if (_componentStatus == ComponentStatus.Stopping)
					{
						StopInternal(false, false, _fadeParameter.GetTimeRemaining(FabricTimer.Get()), _fadeOutCurve);
					}
				}
				if (HasValidEventNotifier())
				{
					NotifyEvent(EventNotificationType.OnSwitch, _selectedComponent);
				}
			}
			return true;
		}

		public bool IsMusicSyncEnabled()
		{
			if (_activeMusicTimeSettings != null)
			{
				return _switchComponentSwitchType == SwitchComponentSwitchType.OnMusicSync;
			}
			return false;
		}

		private void SetSwitch(string name, bool ignoreActiveFlag = false)
		{
			int num = 0;
			Component component;
			while (true)
			{
				if (num < _components.Count)
				{
					component = _components[num];
					if (component != null && component.Name == name && component != _selectedComponent)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			bool flag = ignoreActiveFlag || _isComponentActive;
			if (_selectedComponent != null)
			{
				_selectedComponent.StopInternal(false, false, 0f, 0.5f);
			}
			_selectedComponent = component;
			if (_startOnSwitch && flag && !IsMusicSyncEnabled())
			{
				_componentInstance._instance.ResetPlayScheduled();
				_selectedComponent.PlayInternal(_componentInstance, 0f, 0.5f);
				if (_componentStatus == ComponentStatus.Stopping)
				{
					StopInternal(false, false, _fadeParameter.GetTimeRemaining(FabricTimer.Get()), _fadeOutCurve);
				}
			}
			if (HasValidEventNotifier())
			{
				NotifyEvent(EventNotificationType.OnSwitch, _selectedComponent);
			}
		}

		private void SetDefferedSwitch(string name)
		{
			int num = 0;
			Component component;
			while (true)
			{
				if (num < _components.Count)
				{
					component = _components[num];
					if (component.name == name)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			_defferedSelectedComponent = component;
		}

		public override EventStatus OnProcessEvent(Event zEvent, ComponentInstance zInstance)
		{
			EventStatus result = EventStatus.Failed_Uknown;
			if (zEvent.EventAction == EventAction.SetSwitch)
			{
				if (_switchComponentSwitchType == SwitchComponentSwitchType.OnSwitch)
				{
					List<ComponentInstance> list = FindInstances(zEvent.parentGameObject, false);
					if (list != null && list.Count > 0)
					{
						for (int i = 0; i < list.Count; i++)
						{
							ComponentInstance componentInstance = list[i];
							((SwitchComponent)componentInstance._instance).SetSwitch((string)zEvent._parameter);
							result = EventStatus.Handled;
						}
					}
					else
					{
						SetSwitch((string)zEvent._parameter);
						result = EventStatus.Handled;
					}
				}
				else
				{
					List<ComponentInstance> list2 = FindInstances(zEvent.parentGameObject, false);
					if (list2 != null && list2.Count > 0)
					{
						for (int j = 0; j < list2.Count; j++)
						{
							ComponentInstance componentInstance2 = list2[j];
							((SwitchComponent)componentInstance2._instance).SetDefferedSwitch((string)zEvent._parameter);
							result = EventStatus.Handled;
						}
					}
					else
					{
						SetDefferedSwitch((string)zEvent._parameter);
						result = EventStatus.Handled;
					}
				}
			}
			return result;
		}

		public override void PlayInternal(ComponentInstance zComponentInstance, float target, float curve, bool dontPlayComponents)
		{
			if (!CheckMIDI(zComponentInstance))
			{
				return;
			}
			base.PlayInternal(zComponentInstance, target, curve, true);
			if (_switchComponentSwitchType == SwitchComponentSwitchType.OnPlay && _defferedSelectedComponent != null)
			{
				_selectedComponent = _defferedSelectedComponent;
				_defferedSelectedComponent = null;
			}
			if (_useGlobalSwitch && _globalSwitch != null)
			{
				GlobalSwitch globalSwitch = EventManager.Instance._globalParameterManager._globalSwitches.FindItem(_globalSwitch);
				if (globalSwitch != null)
				{
					GlobalSwitchContainer globalSwitchContainer = GetGlobalSwitchContainer(globalSwitch.GetActiveSwitch());
					if (globalSwitchContainer != null)
					{
						_selectedComponent = globalSwitchContainer._components[0];
					}
				}
			}
			if (_selectedComponent != null)
			{
				if (_activeMusicTimeSettings != null && IsMusicSyncEnabled() && _syncToMusicOnFirstPlay && !_musicTimeResetOnPlay)
				{
					_componentInstance._instance.SetPlayScheduledAdditive(_activeMusicTimeSettings.GetDelay(this), 0.0);
				}
				_selectedComponent.PlayInternal(zComponentInstance, 0f, 0.5f);
			}
		}

		public override void StopInternal(bool stopInstances, bool forceStop, float target, float curve, double scheduleEnd = 0.0)
		{
			if (IsMusicSyncEnabled())
			{
				base.StopInternal(stopInstances, forceStop, target, curve, _activeMusicTimeSettings.GetDelay());
			}
			else
			{
				base.StopInternal(stopInstances, forceStop, target, curve, scheduleEnd);
			}
		}

		private void DoSwitch(double time)
		{
			if (_defferedSelectedComponent != null)
			{
				if (_selectedComponent != null)
				{
					_selectedComponent.StopInternal(false, false, 0f, 0.5f, time);
				}
				_componentInstance._instance.SetPlayScheduled(time, 0.0);
				_defferedSelectedComponent.PlayInternal(_componentInstance, 0f, 0.5f);
				_selectedComponent = _defferedSelectedComponent;
				_defferedSelectedComponent = null;
			}
		}

		internal override void OnFinishPlaying(double time)
		{
			if (_switchComponentSwitchType == SwitchComponentSwitchType.OnEnd)
			{
				DoSwitch(time);
			}
			else
			{
				base.OnFinishPlaying(time);
			}
		}

		internal override bool OnMarker(double time)
		{
			if (_switchComponentSwitchType == SwitchComponentSwitchType.OnMarker)
			{
				DoSwitch(time);
				return true;
			}
			return base.OnMarker(time);
		}

		public override bool UpdateInternal(ref Context context)
		{
			if (IsMusicSyncEnabled() && _activeMusicTimeSettings != null && (bool)_defferedSelectedComponent && _componentInstance != null)
			{
				double offset = 0.0;
				if (_activeMusicTimeSettings.CheckIfNextEventIsWithinRange(ref offset))
				{
					DoSwitch(offset);
				}
			}
			return base.UpdateInternal(ref context);
		}

		List<RTPProperty> IRTPPropertyListener.CollectProperties()
		{
			List<RTPProperty> list = CollectProperties();
			list.Add(new RTPProperty(8, "Switch", 0f, 1f));
			return list;
		}

		bool IRTPPropertyListener.UpdateProperty(RTPProperty property, float value, RTPPropertyType type)
		{
			if (UpdateProperty(property, value, type))
			{
				return true;
			}
			if (property._property == 8 && property._name == "Switch" && _components.Count > 0)
			{
				int index = (int)(value * (float)_components.Count);
				Component component = _components[index];
				if (component != _selectedComponent)
				{
					SetSwitch(component.Name, (_componentInstance != null) ? true : false);
				}
				return true;
			}
			return false;
		}
	}
}
