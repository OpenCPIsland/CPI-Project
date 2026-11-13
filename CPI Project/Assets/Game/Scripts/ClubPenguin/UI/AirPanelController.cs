using ClubPenguin.World.Activities.Diving;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class AirPanelController : MonoBehaviour
	{
		private const float MAX_AIR_SUPPLY = 10f;

		private const float AIR_ANIM_TOTAL_NUM_FRAMES = 960f;

		private const float AIR_ANIM_FRAMES_PER_INCREMENT = 100f;

		public GameObject AirSupplyUI;

		private EventDispatcher dispatcher;

		private bool isVisibleState;

		private bool isAnimatingVisibility;

		private Animator anim;

		private Animator airAnimation;

		private void Start()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<DivingEvents.ShowHud>(showHudRequested);
			dispatcher.AddListener<DivingEvents.HideHud>(hideHudRequested);
			dispatcher.AddListener<DivingEvents.AirSupplyUpdated>(onAirSupplyUpdated);
			anim = base.gameObject.GetComponent<Animator>();
			airAnimation = AirSupplyUI.gameObject.GetComponent<Animator>();
		}

		private void Update()
		{
			if (isAnimatingVisibility)
			{
				return;
			}
			if (isVisibleState)
			{
				if (!base.gameObject.activeInHierarchy)
				{
					showHud();
				}
			}
			else if (base.gameObject.activeInHierarchy)
			{
				hideHud();
			}
		}

		public void OnDestroy()
		{
			dispatcher.RemoveListener<DivingEvents.AirSupplyUpdated>(onAirSupplyUpdated);
			dispatcher.RemoveListener<DivingEvents.ShowHud>(showHudRequested);
			dispatcher.RemoveListener<DivingEvents.HideHud>(hideHudRequested);
		}

		private bool onAirSupplyUpdated(DivingEvents.AirSupplyUpdated evt)
		{
			updateAirSupplyGauge(evt.AirSupply);
			return false;
		}

		private void updateAirSupplyGauge(float airSupply)
		{
			if (airAnimation != null)
			{
				airAnimation.Play("AirBubbleColorMeter", 0, 0.00104166672f * (10f - airSupply) * 100f);
			}
		}

		private void show()
		{
			base.gameObject.SetActive(true);
			isAnimatingVisibility = false;
		}

		private void hide()
		{
			base.gameObject.SetActive(false);
			isAnimatingVisibility = false;
		}

		private bool showHudRequested(DivingEvents.ShowHud e)
		{
			showHud();
			return false;
		}

		private void showHud()
		{
			isVisibleState = true;
			isAnimatingVisibility = true;
			anim.SetTrigger("showUI");
		}

		private bool hideHudRequested(DivingEvents.HideHud e)
		{
			hideHud();
			return false;
		}

		private void hideHud()
		{
			isVisibleState = false;
			isAnimatingVisibility = true;
			anim.SetTrigger("hideUI");
		}
	}
}
