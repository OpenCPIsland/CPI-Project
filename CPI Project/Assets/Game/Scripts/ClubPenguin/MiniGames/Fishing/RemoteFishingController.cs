using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.MiniGames.Fishing
{
	public class RemoteFishingController : MonoBehaviour
	{
		private const int TORSO_ANIMATION_LAYER = 1;

		[SerializeField]
		private FishingGameConfig config;

		private GameObject player;

		private Animator playerAnimator;

		private Animator fishingRodAnimator;

		private void Start()
		{
			init();
		}

		private void init()
		{
			getReferences();
			base.transform.SetParent(player.transform, false);
			base.transform.localPosition = Vector3.zero;
			player.transform.eulerAngles = config.PlayerRotationTowardsWater;
			if (playerAnimator != null && fishingRodAnimator != null)
			{
				CoroutineRunner.Start(castingAnimation(), this, "castingAnimation");
			}
		}

		private void getReferences()
		{
			player = base.gameObject.GetComponentInParent<AvatarDataHandle>().gameObject;
			playerAnimator = player.GetComponent<Animator>();
			GameObject childByName = getChildByName(player, "FishingRodProp(Clone)");
			fishingRodAnimator = childByName.GetComponent<Animator>();
		}

		private IEnumerator castingAnimation()
		{
			if (!base.gameObject.IsDestroyed())
			{
				playerAnimator.SetInteger("PropMode", 3);
				fishingRodAnimator.SetInteger("PropMode", 3);
			}
			yield return new WaitForEndOfFrame();
			if (!base.gameObject.IsDestroyed())
			{
				if (playerAnimator.GetCurrentAnimatorClipInfo(1).Length > 0)
				{
					float waitTime = playerAnimator.GetCurrentAnimatorClipInfo(1)[0].clip.length;
					yield return new WaitForSeconds(waitTime + 1f);
				}
				playerAnimator.SetInteger("PropMode", 2);
				fishingRodAnimator.SetInteger("PropMode", 2);
			}
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}

		private GameObject getChildByName(GameObject root, string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			foreach (Transform item in root.transform)
			{
				if (item.name.Equals(name))
				{
					return item.gameObject;
				}
				GameObject childByName = getChildByName(item.gameObject, name);
				if (childByName != null)
				{
					return childByName;
				}
			}
			return null;
		}
	}
}
