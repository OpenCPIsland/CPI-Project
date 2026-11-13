using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.LOD
{
	public class GroupCulling : MonoBehaviour
	{
		[SerializeField]
		private List<GameObject> groups;

		[SerializeField]
		private List<Switch> switches;

		[SerializeField]
		private List<int> disabledState;

		private List<string> groupOverrides;

		public void Awake()
		{
			groupOverrides = new List<string>();
		}

		public void OnEnable()
		{
			EventDispatcher eventDispatcher = Service.Get<EventDispatcher>();
			eventDispatcher.AddListener<SwitchEvents.SwitchChange>(onSwitchChange);
			eventDispatcher.AddListener<CinematographyEvents.SetGroupCullingOverride>(onSetGroupCullingOverride);
			eventDispatcher.AddListener<CinematographyEvents.ClearGroupCullingOverride>(onClearGroupCullingOverride);
			updateGroups();
		}

		public void OnDisable()
		{
			EventDispatcher eventDispatcher = Service.Get<EventDispatcher>();
			eventDispatcher.RemoveListener<SwitchEvents.SwitchChange>(onSwitchChange);
			eventDispatcher.RemoveListener<CinematographyEvents.SetGroupCullingOverride>(onSetGroupCullingOverride);
			eventDispatcher.RemoveListener<CinematographyEvents.ClearGroupCullingOverride>(onClearGroupCullingOverride);
			for (int i = 0; i < groups.Count; i++)
			{
				if (groups[i] != null)
				{
					groups[i].SetActive(true);
				}
			}
		}

		private void updateGroups()
		{
			int num = int.MaxValue;
			bool flag = false;
			int count = switches.Count;
			for (int i = 0; i < count; i++)
			{
				if (switches[i].OnOff)
				{
					flag = true;
					num &= disabledState[i];
				}
			}
			if (!flag)
			{
				num = 0;
			}
			int count2 = groups.Count;
			for (int i = 0; i < count2; i++)
			{
				if (groupOverrides.Contains(groups[i].name))
				{
					groups[i].SetActive(true);
				}
				else
				{
					groups[i].SetActive((num & 1) == 0);
				}
				num >>= 1;
			}
		}

		private bool onSwitchChange(SwitchEvents.SwitchChange evt)
		{
			int count = switches.Count;
			for (int i = 0; i < count; i++)
			{
				if (evt.Owner == switches[i].transform)
				{
					updateGroups();
					break;
				}
			}
			return false;
		}

		private bool onSetGroupCullingOverride(CinematographyEvents.SetGroupCullingOverride evt)
		{
			groupOverrides.Clear();
			groupOverrides.AddRange(evt.GroupNames);
			updateGroups();
			return false;
		}

		private bool onClearGroupCullingOverride(CinematographyEvents.ClearGroupCullingOverride evt)
		{
			groupOverrides.Clear();
			updateGroups();
			return false;
		}
	}
}
