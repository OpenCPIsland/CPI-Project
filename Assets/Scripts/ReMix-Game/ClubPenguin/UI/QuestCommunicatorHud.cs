using ClubPenguin.Adventure;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native.iOS;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class QuestCommunicatorHud : MonoBehaviour
	{
		public Text ObjectiveText;

		public GameObject SubtaskDropDownPanel;

		public GameObject SubtaskTickPanelPrefab;

		private EventChannel eventChannel;

		public bool IsOpen
		{
			get;
			private set;
		}

		public bool IsSuppressed
		{
			get;
			private set;
		}

		public bool IsPermanentlySuppressed
		{
			get;
			private set;
		}

		public int SubtaskCount
		{
			get;
			private set;
		}

		public event System.Action SubtasksChanged;

		public event System.Action CommunicatorSuppressed;

		public event System.Action CommunicatorUnsuppressed;

		private void Start()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<HudEvents.SetSubtaskText>(onSubtaskTextSet);
			eventChannel.AddListener<HudEvents.RemoveSubtaskText>(onRemoveSubtaskText);
			eventChannel.AddListener<HudEvents.DestroySubtaskText>(onDestroySubtaskText);
			eventChannel.AddListener<HudEvents.PermanentlySuppressQuestNotifier>(onPermanentlySuppressQuestNotifier);
			eventChannel.AddListener<HudEvents.SuppressQuestNotifier>(onSuppressQuestNotifier);
		}

		private void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
		}

		public void SetOpen(bool open)
		{
			IsOpen = open;
		}

		public void ToggleActive(bool active)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				base.transform.GetChild(i).gameObject.SetActive(active);
			}
		}

		public bool SetObjectiveText(string text)
		{
			bool result = false;
			if (ObjectiveText.text != text)
			{
				ObjectiveText.text = text;
				result = true;
			}
			return result;
		}

		private bool onSubtaskTextSet(HudEvents.SetSubtaskText evt)
		{
			QuestSubtask questSubtask = null;
			if (SubtaskCount > 0)
			{
				QuestSubtask[] componentsInChildren = SubtaskDropDownPanel.GetComponentsInChildren<QuestSubtask>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i].ID == evt.ID)
					{
						questSubtask = componentsInChildren[i];
						break;
					}
				}
			}
			if (questSubtask == null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(SubtaskTickPanelPrefab, SubtaskDropDownPanel.transform, false);
				questSubtask = gameObject.GetComponentInChildren<QuestSubtask>();
				questSubtask.ID = evt.ID;
				MascotDefinition definition = Service.Get<QuestService>().ActiveQuest.Mascot.Definition;
				if (definition != null)
				{
					Color navigationArrowColor = definition.NavigationArrowColor;
					questSubtask.SetTickColor(navigationArrowColor);
					questSubtask.SetTickOutlineColor(navigationArrowColor);
				}
				SubtaskCount++;
			}
			questSubtask.SubtaskText.text = evt.SubtaskText;
			if (questSubtask.TickToggle != null)
			{
				Service.Get<iOSHapticFeedback>().TriggerNotificationFeedback(iOSHapticFeedback.NotificationFeedbackType.Success);
				questSubtask.TickToggle.isOn = evt.IsComplete;
			}
			if (this.SubtasksChanged != null)
			{
				this.SubtasksChanged();
			}
			return false;
		}

		private bool onRemoveSubtaskText(HudEvents.RemoveSubtaskText evt)
		{
			if (SubtaskCount > 0)
			{
				QuestSubtask[] componentsInChildren = SubtaskDropDownPanel.GetComponentsInChildren<QuestSubtask>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i].ID == evt.ID)
					{
						SubtaskCount--;
						UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
						if (this.SubtasksChanged != null)
						{
							this.SubtasksChanged();
						}
						break;
					}
				}
			}
			return false;
		}

		private bool onDestroySubtaskText(HudEvents.DestroySubtaskText evt)
		{
			QuestSubtask[] componentsInChildren = SubtaskDropDownPanel.GetComponentsInChildren<QuestSubtask>();
			SubtaskCount = 0;
			if (componentsInChildren.Length > 0)
			{
				SubtaskDropDownPanel.SetActive(false);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].ID = "";
					UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
				}
				if (evt.PlayAnimation && this.SubtasksChanged != null)
				{
					this.SubtasksChanged();
				}
			}
			return false;
		}

		public void ShowSubtaskDropDownPanel()
		{
			SubtaskDropDownPanel.SetActive(true);
		}

		private bool onPermanentlySuppressQuestNotifier(HudEvents.PermanentlySuppressQuestNotifier evt)
		{
			IsPermanentlySuppressed = evt.Suppress;
			if (evt.AutoShow)
			{
				if (evt.Suppress && this.CommunicatorSuppressed != null)
				{
					this.CommunicatorSuppressed();
				}
				else if (!evt.Suppress && this.CommunicatorUnsuppressed != null)
				{
					this.CommunicatorUnsuppressed();
				}
			}
			return false;
		}

		private bool onSuppressQuestNotifier(HudEvents.SuppressQuestNotifier evt)
		{
			IsSuppressed = evt.Suppress;
			if (evt.AutoShow)
			{
				if (evt.Suppress && this.CommunicatorSuppressed != null)
				{
					this.CommunicatorSuppressed();
				}
				else if (!evt.Suppress && this.CommunicatorUnsuppressed != null)
				{
					this.CommunicatorUnsuppressed();
				}
			}
			return true;
		}
	}
}
