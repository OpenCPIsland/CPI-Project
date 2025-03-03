using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;

namespace ClubPenguin
{
	public class UIElementDisablerManager
	{
		private Dictionary<string, UIElementDisabler> disablerMap;

		private Dictionary<string, UIElementDisablerGroup> disablerGroupMap;

		private Dictionary<string, bool> disabledElements;

		private Dictionary<string, bool> disabledGroups;

		private bool isShowingDebugLabels = false;

		public bool IsShowingDebugLabels
		{
			get
			{
				return isShowingDebugLabels;
			}
		}

		public UIElementDisablerManager()
		{
			disablerMap = new Dictionary<string, UIElementDisabler>();
			disablerGroupMap = new Dictionary<string, UIElementDisablerGroup>();
			disabledElements = new Dictionary<string, bool>();
			disabledGroups = new Dictionary<string, bool>();
			Service.Get<EventDispatcher>().AddListener<UIDisablerEvents.EnableUIElement>(onUIElementEnabled);
			Service.Get<EventDispatcher>().AddListener<UIDisablerEvents.DisableUIElement>(onUIElementDisabled);
			Service.Get<EventDispatcher>().AddListener<UIDisablerEvents.EnableUIElementGroup>(onUIElementGroupEnabled);
			Service.Get<EventDispatcher>().AddListener<UIDisablerEvents.DisableUIElementGroup>(onUIElementGroupDisabled);
			Service.Get<EventDispatcher>().AddListener<UIDisablerEvents.DisableAllUIElements>(onDisableAllUIElements);
			Service.Get<EventDispatcher>().AddListener<UIDisablerEvents.EnableAllUIElements>(onEnableAllUIElements);
			Service.Get<EventDispatcher>().AddListener<SceneTransitionEvents.TransitionStart>(onSceneTransition);
		}

		public bool IsUIElementDisabled(string UIElementID)
		{
			return disabledElements.ContainsKey(UIElementID);
		}

		public bool IsUIElementGroupDisabled(string UIElementGroupID)
		{
			return disabledGroups.ContainsKey(UIElementGroupID);
		}

		public bool DoesUIElementExist(string UIElementID)
		{
			return disablerMap.ContainsKey(UIElementID);
		}

		public bool DoesUIElementGroupExist(string UIElementGroupID)
		{
			return disablerGroupMap.ContainsKey(UIElementGroupID);
		}

		public bool RegisterDisabler(UIElementDisabler disabler)
		{
			bool result = false;
			if (!string.IsNullOrEmpty(disabler.UIElementID) && (!disablerMap.ContainsKey(disabler.UIElementID) || disabler.GiveUniqueID))
			{
				if (disablerMap.ContainsKey(disabler.UIElementID))
				{
					int num = 0;
					string text;
					do
					{
						text = string.Format("{0}_{1}", disabler.UIElementID, num);
						num++;
					}
					while (disablerMap.ContainsKey(text));
					disabler.UIElementID = text;
				}
				disablerMap[disabler.UIElementID] = disabler;
				result = true;
				if (disabledElements.ContainsKey(disabler.UIElementID))
				{
					disabler.DisableElement(disabledElements[disabler.UIElementID]);
				}
				else
				{
					foreach (string key in disabledGroups.Keys)
					{
						if (disablerGroupMap.ContainsKey(key) && disablerGroupMap[key].ContainsElement(disabler))
						{
							disabler.DisableElement(disabledGroups[key]);
						}
					}
				}
				if (isShowingDebugLabels)
				{
					disabler.ShowDebugIDLabel();
				}
			}
			return result;
		}

		public bool RegisterDisablerGroup(UIElementDisablerGroup disablerGroup)
		{
			bool result = false;
			if (!string.IsNullOrEmpty(disablerGroup.UIElementGroupID) && !disablerMap.ContainsKey(disablerGroup.UIElementGroupID))
			{
				disablerGroupMap[disablerGroup.UIElementGroupID] = disablerGroup;
				result = true;
				if (disabledGroups.ContainsKey(disablerGroup.UIElementGroupID))
				{
					disablerGroup.DisableElementGroup(disabledGroups[disablerGroup.UIElementGroupID]);
				}
				if (isShowingDebugLabels)
				{
					disablerGroup.ShowDebugIDLabel();
				}
			}
			return result;
		}

		public bool UnregisterDisabler(string UIElementID)
		{
			if (disablerMap.ContainsKey(UIElementID))
			{
				disablerMap.Remove(UIElementID);
				return true;
			}
			return false;
		}

		public bool UnregisterDisablerGroup(string UIElementGroupID)
		{
			if (disablerGroupMap.ContainsKey(UIElementGroupID))
			{
				disablerGroupMap.Remove(UIElementGroupID);
				return true;
			}
			return false;
		}

		private bool onUIElementEnabled(UIDisablerEvents.EnableUIElement evt)
		{
			if (disablerMap.ContainsKey(evt.ElementID))
			{
				disablerMap[evt.ElementID].EnableElement();
			}
			if (disabledElements.ContainsKey(evt.ElementID))
			{
				disabledElements.Remove(evt.ElementID);
			}
			return false;
		}

		private bool onUIElementDisabled(UIDisablerEvents.DisableUIElement evt)
		{
			if (disablerMap.ContainsKey(evt.ElementID))
			{
				disablerMap[evt.ElementID].DisableElement(evt.HideElement);
			}
			if (!disabledElements.ContainsKey(evt.ElementID))
			{
				disabledElements[evt.ElementID] = evt.HideElement;
			}
			return false;
		}

		private bool onUIElementGroupEnabled(UIDisablerEvents.EnableUIElementGroup evt)
		{
			if (disablerGroupMap.ContainsKey(evt.ElementGroupID))
			{
				disablerGroupMap[evt.ElementGroupID].EnableElementGroup();
			}
			if (disabledGroups.ContainsKey(evt.ElementGroupID))
			{
				disabledGroups.Remove(evt.ElementGroupID);
			}
			return false;
		}

		private bool onUIElementGroupDisabled(UIDisablerEvents.DisableUIElementGroup evt)
		{
			if (disablerGroupMap.ContainsKey(evt.ElementGroupID))
			{
				disablerGroupMap[evt.ElementGroupID].DisableElementGroup(evt.HideElements);
			}
			if (!disabledGroups.ContainsKey(evt.ElementGroupID))
			{
				disabledGroups[evt.ElementGroupID] = evt.HideElements;
			}
			return false;
		}

		private bool onDisableAllUIElements(UIDisablerEvents.DisableAllUIElements evt)
		{
			foreach (UIElementDisabler value in disablerMap.Values)
			{
				value.DisableElement(false);
			}
			return false;
		}

		private bool onEnableAllUIElements(UIDisablerEvents.EnableAllUIElements evt)
		{
			foreach (UIElementDisabler value in disablerMap.Values)
			{
				value.EnableElement();
			}
			disabledElements.Clear();
			disabledGroups.Clear();
			return false;
		}

		private bool onSceneTransition(SceneTransitionEvents.TransitionStart evt)
		{
			if (evt.SceneName == "Home")
			{
				disabledElements.Clear();
				disabledGroups.Clear();
			}
			return false;
		}

		public void OnApplicationPause(bool isPaused)
		{
			if (!isPaused)
			{
				disabledElements.Clear();
				disabledGroups.Clear();
			}
		}
	}
}
