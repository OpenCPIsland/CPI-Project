using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class UIElementDisablerGroup : MonoBehaviour
	{
		public string UIElementGroupID;

		public bool IncludeAllChildren = false;

		public UIElementDisabler[] ElementsInGroup;

		private List<UIElementDisabler> groupElementsList;

		private UIElementDisablerManager disablerManager;

		public bool IsEnabled
		{
			get;
			private set;
		}

		private void Awake()
		{
			groupElementsList = new List<UIElementDisabler>();
		}

		private void Start()
		{
			disablerManager = Service.Get<UIElementDisablerManager>();
			disablerManager.RegisterDisablerGroup(this);
			groupElementsList.AddRange(ElementsInGroup);
			if (IncludeAllChildren)
			{
				UIElementDisabler[] componentsInChildren = GetComponentsInChildren<UIElementDisabler>();
				groupElementsList.AddRange(componentsInChildren);
			}
		}

		private void OnDestroy()
		{
			if (disablerManager != null)
			{
				disablerManager.UnregisterDisablerGroup(UIElementGroupID);
			}
		}

		public void DisableElementGroup(bool hide)
		{
			for (int i = 0; i < groupElementsList.Count; i++)
			{
				if (groupElementsList[i] != null)
				{
					groupElementsList[i].DisableElement(hide);
				}
			}
			IsEnabled = false;
		}

		public void EnableElementGroup()
		{
			for (int i = 0; i < groupElementsList.Count; i++)
			{
				if (groupElementsList[i] != null && !disablerManager.IsUIElementDisabled(groupElementsList[i].UIElementID))
				{
					groupElementsList[i].EnableElement();
				}
			}
			IsEnabled = true;
		}

		public bool ContainsElement(UIElementDisabler disabler)
		{
			return groupElementsList.Contains(disabler);
		}

		public bool ContainsElementID(string elementID)
		{
			for (int i = 0; i < groupElementsList.Count; i++)
			{
				if (groupElementsList[i].UIElementID == elementID)
				{
					return true;
				}
			}
			return false;
		}

		public void ShowDebugIDLabel()
		{
		}

		public void HideDebugIDLabel()
		{
		}
	}
}
