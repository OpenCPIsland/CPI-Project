using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(Collider))]
	public class MapTransition : MonoBehaviour
	{
		private const bool USE_CAMERASPACE = true;

		private const string MAP_IDLE_ANIM_STATE_NAME = "MarketplaceOn";

		private static readonly PrefabContentKey prefabContentKey = new PrefabContentKey("Prefabs/WorldMap");

		private static readonly string accessibilityKey = "Accessibility.Popup.Title.Map";

		private OutOfBoundsWarper outOfBoundsWarper;

		private Animator mapAnimator;

		public void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				Service.Get<ActionConfirmationService>().ConfirmAction(typeof(ZoneTransition), null, delegate
				{
					SceneRefs.FullScreenPopupManager.CreatePopup(prefabContentKey, accessibilityKey, true, onPrefabSpawned);
				});
				outOfBoundsWarper = other.GetComponent<OutOfBoundsWarper>();
			}
		}

		private void onPrefabSpawned(PrefabContentKey key, GameObject go)
		{
			mapAnimator = go.GetComponent<Animator>();
			StartCoroutine(WaitForMapAnimationToFinish());
		}

		private IEnumerator WaitForMapAnimationToFinish()
		{
			while (mapAnimator != null && !mapAnimator.GetCurrentAnimatorStateInfo(0).IsName("MarketplaceOn"))
			{
				yield return new WaitForEndOfFrame();
			}
			outOfBoundsWarper.ResetPlayer();
		}
	}
}
