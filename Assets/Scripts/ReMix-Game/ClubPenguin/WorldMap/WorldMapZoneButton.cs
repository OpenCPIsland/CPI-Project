using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.WorldMap
{
	[RequireComponent(typeof(Button))]
	public class WorldMapZoneButton : MonoBehaviour
	{
		public string ZoneName;

		public RectTransform PopupPoint;

		private Button button;

		private WorldMapController mapController;

		private Animator animator;

		public ZoneDefinition ZoneDefinition
		{
			get;
			private set;
		}

		private void Awake()
		{
			ZoneDefinition = Service.Get<ZoneTransitionService>().GetZone(ZoneName);
		}

		private void Start()
		{
			animator = GetComponent<Animator>();
			button = GetComponent<Button>();
			mapController = GetComponentInParent<WorldMapController>();
			if (!Service.Get<ZoneTransitionService>().IsInIgloo && Service.Get<ZoneTransitionService>().CurrentZone == ZoneDefinition)
			{
				button.interactable = false;
			}
		}

		public void OnIntroAnimationComplete()
		{
			if (!button.interactable)
			{
				animator.StopPlayback();
			}
		}

		public void OnButtonPressed()
		{
			mapController.OnZoneButtonPressed(this);
		}
	}
}
