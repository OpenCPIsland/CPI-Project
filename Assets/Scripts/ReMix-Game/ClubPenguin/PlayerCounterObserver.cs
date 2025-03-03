#define UNITY_ASSERTIONS
using ClubPenguin.Net;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin
{
	public class PlayerCounterObserver : MonoBehaviour
	{
		[Tooltip("The settings for each given count observed. Lowest to highest!!")]
		public CountChangeSettings[] Settings;

		public string ResetTriggerName = "Reset";

		public TextMesh CounterText;

		public Animator Animator;

		public void Awake()
		{
			Array.Sort(Settings, (CountChangeSettings setting1, CountChangeSettings setting2) => setting1.ObservingCount.CompareTo(setting2.ObservingCount));
			Service.Get<EventDispatcher>().AddListener<PlayerSpawnedEvents.RoomPopulationChanged>(onRoomPopulationChanged);
		}

		public void Start()
		{
			setCountText(Service.Get<INetworkServicesManager>().ServerUserCount);
		}

		public void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<PlayerSpawnedEvents.RoomPopulationChanged>(onRoomPopulationChanged);
		}

		public void OnValidate()
		{
			for (int i = 0; i < Settings.Length; i++)
			{
				Assert.IsNotNull(Settings[i], "Setting cannot be null for index " + i);
				Assert.IsNotNull(GetComponentInChildren<Animator>(), "Missing an animator!");
				Assert.IsTrue(Settings[i].ObservingCount >= 0, "Setting count must be positive for setting index " + i);
			}
		}

		private bool onRoomPopulationChanged(PlayerSpawnedEvents.RoomPopulationChanged evt)
		{
			checkCountAgainstObservationSettings(evt.Count);
			return false;
		}

		private void checkCountAgainstObservationSettings(int count)
		{
			bool flag = false;
			for (int num = Settings.Length - 1; num >= 0; num--)
			{
				if (count >= Settings[num].ObservingCount)
				{
					flag = true;
					if (Settings[num].GameObjectToEnable != null)
					{
						Settings[num].GameObjectToEnable.SetActive(true);
					}
					setTrigger(Settings[num].Triggername);
					break;
				}
			}
			if (!flag)
			{
				setTrigger(ResetTriggerName);
			}
			setCountText(count);
		}

		private void setTrigger(string triggerName)
		{
			Animator.SetTrigger(triggerName);
		}

		private void setCountText(int count)
		{
			if (count <= 0)
			{
				CounterText.text = "";
			}
			else
			{
				CounterText.text = count.ToString();
			}
		}
	}
}
