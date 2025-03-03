using ClubPenguin.Locomotion;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Interactables
{
	public class InvitationalWorldItemExperience : MonoBehaviour
	{
		[SerializeField]
		private bool AdditionalItemTakingCoolDown = true;

		[SerializeField]
		private float AdditionalItemTakingCoolDownTimeInSecs = 15f;

		private HashSet<long> itemRecipientsByPlayerIds;

		private void Start()
		{
			Service.Get<EventDispatcher>().AddListener<PenguinInteraction.InteractionStartedEvent>(onInteractionStarted);
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<PenguinInteraction.InteractionStartedEvent>(onInteractionStarted);
		}

		public bool CanInteract(long interactingPlayerId)
		{
			if (AdditionalItemTakingCoolDown)
			{
				return !itemRecipientsByPlayerIds.Contains(interactingPlayerId);
			}
			return !AdditionalItemTakingCoolDown;
		}

		private bool onInteractionStarted(PenguinInteraction.InteractionStartedEvent evt)
		{
			if (!base.gameObject.IsDestroyed() && evt.ObjectInteractedWith.Equals(base.gameObject) && AdditionalItemTakingCoolDown)
			{
				itemRecipientsByPlayerIds.Add(evt.InteractingPlayerId);
				base.gameObject.SetActive(false);
				CoroutineRunner.Start(removePlayerFromRecipientsList(evt.InteractingPlayerId), this, "InvitationalItemExperience.removePlayerFromRecipientsList");
			}
			return false;
		}

		private IEnumerator removePlayerFromRecipientsList(long playerId)
		{
			yield return new WaitForSeconds(AdditionalItemTakingCoolDownTimeInSecs);
			if (!base.gameObject.IsDestroyed())
			{
				base.gameObject.SetActive(true);
				if (itemRecipientsByPlayerIds != null && itemRecipientsByPlayerIds.Count > 0)
				{
					itemRecipientsByPlayerIds.Remove(playerId);
				}
			}
		}
	}
}
