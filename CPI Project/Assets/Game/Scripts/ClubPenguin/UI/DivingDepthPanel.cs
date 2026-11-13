using ClubPenguin.World.Activities.Diving;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class DivingDepthPanel : MonoBehaviour
	{
		public const string STATE_COLOR_CAUTION = "#FF9028FF";

		public const string STATE_COLOR_PANIC = "#E4412AFF";

		public static readonly float AirSupplySlice = 0.125f;

		public Text DepthText;

		public GameObject DepthPanel;

		private EventDispatcher dispatcher;

		private Animator anim;

		private bool isVisibleState;

		private bool isAnimatingVisibility;

		private ParticleSystem spawnedParticles;

		private void Start()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<DivingEvents.ShowHud>(showHudRequested);
			dispatcher.AddListener<DivingEvents.HideHud>(hideHudRequested);
			anim = base.gameObject.GetComponent<Animator>();
		}

		private void Awake()
		{
			hide();
		}

		private void Update()
		{
			if (isAnimatingVisibility)
			{
				return;
			}
			if (isVisibleState)
			{
				if (!DepthPanel.gameObject.activeInHierarchy)
				{
					showHud();
				}
			}
			else if (DepthPanel.gameObject.activeInHierarchy)
			{
				hideHud();
			}
		}

		public void OnDestroy()
		{
			dispatcher.RemoveListener<DivingEvents.DepthUpdated>(onDepthUpdated);
			dispatcher.RemoveListener<DivingEvents.ShowHud>(showHudRequested);
			dispatcher.RemoveListener<DivingEvents.HideHud>(hideHudRequested);
		}

		private bool onDepthUpdated(DivingEvents.DepthUpdated evt)
		{
			DepthText.text = evt.CurrentDepth + "m";
			return false;
		}

		private void show()
		{
			LayoutElement component = base.gameObject.GetComponent<LayoutElement>();
			component.ignoreLayout = false;
			DepthPanel.gameObject.SetActive(true);
			DepthPanel.gameObject.SetActive(true);
			isAnimatingVisibility = false;
		}

		private void hide()
		{
			LayoutElement component = base.gameObject.GetComponent<LayoutElement>();
			component.ignoreLayout = true;
			DepthPanel.gameObject.SetActive(false);
			DepthPanel.gameObject.SetActive(false);
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
			dispatcher.AddListener<DivingEvents.DepthUpdated>(onDepthUpdated);
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
			dispatcher.RemoveListener<DivingEvents.DepthUpdated>(onDepthUpdated);
			anim.SetTrigger("hideUI");
		}
	}
}
