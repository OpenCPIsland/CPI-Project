using ClubPenguin.Analytics;
using ClubPenguin.Input;
using ClubPenguin.Net.Domain;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.WorldMap
{
	public class WorldMapZonePopup : MonoBehaviour
	{
		private const int POPULATION_ICON_ON_INDEX = 0;

		private const int POPULATION_ICON_OFF_INDEX = 1;

		[SerializeField]
		private ButtonClickListener btnZonePromptYes = null;

		[SerializeField]
		private ButtonClickListener btnZonePromptNo = null;

		public Text ZoneText;

		public RectTransform Arrow;

		public GameObject PopulationPanel;

		public TintSelector[] PopulationBarIcons;

		public float ScreenEdgeBuffer = 30f;

		public GameObject ZonePanel;

		public GameObject IglooPanel;

		private ZoneDefinition zoneDefinition;

		private Animator animator;

		private MyIglooTransitionButton iglooTransitionButton;

		private ShowIglooListButton showIglooListButton;

		public event Action ZonePopupClosed;

		private void OnValidate()
		{
		}

		private void Awake()
		{
			animator = GetComponent<Animator>();
			iglooTransitionButton = GetComponent<MyIglooTransitionButton>();
			showIglooListButton = GetComponent<ShowIglooListButton>();
		}

		private void OnDisable()
		{
			btnZonePromptYes.OnClick.RemoveListener(onZonePromptYesClicked);
			btnZonePromptNo.OnClick.RemoveListener(onZonePromptNoClicked);
		}

		public void SetZone(ZoneDefinition zoneDefinition, bool zoneIsIgloo = false, bool showPopulationBar = true)
		{
			ZoneText.text = Service.Get<Localizer>().GetTokenTranslation(zoneDefinition.ZoneToken);
			this.zoneDefinition = zoneDefinition;
			updateZonePanel(!zoneIsIgloo);
			IglooPanel.SetActive(zoneIsIgloo);
			if (showPopulationBar)
			{
				PopulationPanel.SetActive(true);
				UpdatePopulationBar();
			}
			else
			{
				PopulationPanel.SetActive(false);
			}
		}

		private void updateZonePanel(bool visible)
		{
			btnZonePromptYes.OnClick.RemoveListener(onZonePromptYesClicked);
			btnZonePromptNo.OnClick.RemoveListener(onZonePromptNoClicked);
			ZonePanel.SetActive(visible);
			if (visible)
			{
				btnZonePromptYes.OnClick.AddListener(onZonePromptYesClicked);
				btnZonePromptNo.OnClick.AddListener(onZonePromptNoClicked);
			}
		}

		public void SetPosition(Vector2 targetPosition)
		{
			base.transform.position = new Vector3(targetPosition.x, targetPosition.y, base.transform.position.z);
			float num = ((RectTransform)base.transform).rect.width * 0.5f;
			float width = ((RectTransform)base.transform.parent).rect.width;
			Vector2 anchoredPosition = base.transform.GetComponent<RectTransform>().anchoredPosition;
			Vector2 anchoredPosition2 = anchoredPosition;
			if (anchoredPosition2.x + num > width - ScreenEdgeBuffer)
			{
				anchoredPosition2.x = width - num - ScreenEdgeBuffer;
			}
			else if (anchoredPosition2.x - num < ScreenEdgeBuffer)
			{
				anchoredPosition2.x = num + ScreenEdgeBuffer;
			}
			((RectTransform)base.transform).anchoredPosition = anchoredPosition2;
			Arrow.anchoredPosition = new Vector2(anchoredPosition.x - anchoredPosition2.x, Arrow.anchoredPosition.y);
		}

		private void onZonePromptNoClicked(ButtonClickListener.ClickType clickType)
		{
			Close();
		}

		private void onZonePromptYesClicked(ButtonClickListener.ClickType clickType)
		{
			logTravelToZone();
			Service.Get<ZoneTransitionService>().LoadZone(zoneDefinition);
		}

		public void OnIglooListButtonPressed()
		{
			UnityEngine.Object.Destroy(GetComponentInParent<WorldMapController>().gameObject);
			showIglooListButton.OnButton();
			Service.Get<ICPSwrveService>().Action("igloo", "igloo_list", "map");
		}

		private IEnumerator showIglooList()
		{
			UnityEngine.Object.Destroy(GetComponentInParent<WorldMapController>().gameObject);
			yield return new WaitForEndOfFrame();
			showIglooListButton.OnButton();
		}

		public void OnMyIglooButtonPressed()
		{
			iglooTransitionButton.OnButton();
		}

		public void Open()
		{
			if (!animator.GetBool("IsOpen"))
			{
				animator.SetBool("IsOpen", true);
			}
			else
			{
				animator.SetTrigger("Open");
			}
		}

		public void Close()
		{
			if (this.ZonePopupClosed != null)
			{
				this.ZonePopupClosed();
			}
			animator.SetBool("IsOpen", false);
		}

		public void UpdatePopulationBar()
		{
			RoomPopulation zonePopulation = GetComponentInParent<WorldMapController>().GetZonePopulation(zoneDefinition);
			int populationScaled = (int)zonePopulation.populationScaled;
			for (int i = 0; i < PopulationBarIcons.Length; i++)
			{
				if (i < populationScaled)
				{
					PopulationBarIcons[i].SelectColor(0);
				}
				else
				{
					PopulationBarIcons[i].SelectColor(1);
				}
			}
		}

		public void OnTooltipOpenAnimationComplete()
		{
		}

		public void OnTooltipCloseAnimationComplete()
		{
		}

		private void logTravelToZone()
		{
			RoomPopulation zonePopulation = GetComponentInParent<WorldMapController>().GetZonePopulation(zoneDefinition);
			Service.Get<ICPSwrveService>().Action("map_travel", zoneDefinition.ZoneName, zonePopulation.populationScaled.ToString());
		}
	}
}
