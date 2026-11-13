using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Input;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.WorldMap
{
	public class WorldMapController : MonoBehaviour
	{
		private const float ZONE_POPUP_DISABLE_DELAY = 0.5f;

		public ButtonClickListener BtnClose;

		public GameObject YouAreHereIcon;

		public GameObject FullScreenButton;

		public WorldMapZonePopup ZonePopup;

		[SerializeField]
		private WorldMapZoneButton iglooZoneButton = null;

		private Dictionary<ZoneDefinition, WorldMapZoneButton> zoneButtons;

		private List<RoomPopulation> roomPopulations;

		private ZoneDefinition currentZoneDefinition;

		private ZoneDefinition currentZonePopupZone = null;

		private Animator animator;

		private void OnValidate()
		{
		}

		private void Start()
		{
			animator = GetComponent<Animator>();
			FullScreenButton.SetActive(false);
			ZonePopup.ZonePopupClosed += onZonePopupClosed;
			ZonePopup.gameObject.SetActive(false);
			Service.Get<EventDispatcher>().DispatchEvent(new AwayFromKeyboardEvent(AwayFromKeyboardStateType.Map, true));
			zoneButtons = new Dictionary<ZoneDefinition, WorldMapZoneButton>();
			WorldMapZoneButton[] componentsInChildren = GetComponentsInChildren<WorldMapZoneButton>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				zoneButtons.Add(componentsInChildren[i].ZoneDefinition, componentsInChildren[i]);
			}
			currentZoneDefinition = Service.Get<ZoneTransitionService>().CurrentZone;
			if (zoneButtons.ContainsKey(currentZoneDefinition))
			{
				YouAreHereIcon.transform.position = zoneButtons[currentZoneDefinition].PopupPoint.position;
			}
			else
			{
				YouAreHereIcon.gameObject.SetActive(false);
			}
			getRoomPopulations();
			logMapOpen();
		}

		private void OnEnable()
		{
			BtnClose.OnClick.AddListener(onCloseButtonClicked);
		}

		private void OnDisable()
		{
			BtnClose.OnClick.RemoveListener(onCloseButtonClicked);
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new AwayFromKeyboardEvent(AwayFromKeyboardStateType.Here));
			CoroutineRunner.StopAllForOwner(this);
		}

		private void onCloseButtonClicked(ButtonClickListener.ClickType clickType)
		{
			animator.SetTrigger("Close");
		}

		public void OnZoneButtonPressed(WorldMapZoneButton zoneButton)
		{
			if (currentZonePopupZone == null || currentZonePopupZone != zoneButton.ZoneDefinition)
			{
				if (zoneButton == iglooZoneButton)
				{
					zoneButton.GetComponent<ShowIglooListButton>().OnButton();
					Service.Get<ICPSwrveService>().Action("igloo", "igloo_list", "map");
					Object.Destroy(base.gameObject);
					return;
				}
				ZonePopup.gameObject.SetActive(true);
				currentZonePopupZone = zoneButton.ZoneDefinition;
				ZonePopup.SetPosition(new Vector2(zoneButton.PopupPoint.position.x, zoneButton.PopupPoint.position.y));
				ZonePopup.SetZone(zoneButton.ZoneDefinition, zoneButton == iglooZoneButton, zoneButton != iglooZoneButton);
				ZonePopup.Open();
				logZonePressed(zoneButton.ZoneDefinition);
			}
		}

		public void OnFullScreenButtonPressed()
		{
			ZonePopup.Close();
		}

		private void onZonePopupClosed()
		{
			currentZonePopupZone = null;
			CoroutineRunner.Start(disableZonePopup(0.5f), this, "CloseZonePopup");
		}

		private IEnumerator disableZonePopup(float delay)
		{
			yield return new WaitForSeconds(delay);
			if (currentZonePopupZone == null)
			{
				ZonePopup.gameObject.SetActive(false);
			}
		}

		public void MarketplaceScreenIntroComplete()
		{
		}

		public void MarketplaceScreenOutroComplete()
		{
			Object.Destroy(base.gameObject);
		}

		public RoomPopulation GetZonePopulation(ZoneDefinition zoneDefinition)
		{
			if (roomPopulations != null)
			{
				for (int i = 0; i < roomPopulations.Count; i++)
				{
					if (roomPopulations[i].identifier.zoneId.name == zoneDefinition.SceneName)
					{
						return roomPopulations[i];
					}
				}
			}
			return new RoomPopulation(new RoomIdentifier(), RoomPopulationScale.ZERO);
		}

		private void getRoomPopulations()
		{
			PresenceData component = Service.Get<CPDataEntityCollection>().GetComponent<PresenceData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle);
			Service.Get<EventDispatcher>().AddListener<WorldServiceEvents.RoomPopulationReceivedEvent>(onRoomPopulationReceived);
			Service.Get<INetworkServicesManager>().WorldService.GetRoomPopulation(component.World, Service.Get<Localizer>().LanguageString);
		}

		private bool onRoomPopulationReceived(WorldServiceEvents.RoomPopulationReceivedEvent evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<WorldServiceEvents.RoomPopulationReceivedEvent>(onRoomPopulationReceived);
			roomPopulations = evt.RoomPopulations;
			if (ZonePopup.gameObject.activeSelf)
			{
				ZonePopup.UpdatePopulationBar();
			}
			return false;
		}

		private void logMapOpen()
		{
			Service.Get<ICPSwrveService>().Action("map", "open");
		}

		private void logZonePressed(ZoneDefinition zone)
		{
			Service.Get<ICPSwrveService>().Action("map_travel_open", zone.ZoneName);
		}
	}
}
