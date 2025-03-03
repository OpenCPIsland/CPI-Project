using UnityEngine;

namespace ClubPenguin.Interactables.Domain
{
	public static class InteractablesEvents
	{
		public enum Actions
		{
			DoneInteracting = -1,
			Action1,
			Action2,
			Action3
		}

		public struct ActionEvent
		{
			public readonly GameObject InteractableTarget;

			public readonly Actions Action;

			public ActionEvent(GameObject _interactableTarget, Actions _action)
			{
				InteractableTarget = _interactableTarget;
				Action = _action;
			}
		}

		public struct InvitationalItemUsed
		{
			public readonly GameObject Item;

			public readonly int QuantityLeft;

			public InvitationalItemUsed(GameObject _item, int _quantityLeft)
			{
				Item = _item;
				QuantityLeft = _quantityLeft;
			}
		}

		public struct InWorldItemCollected
		{
			public readonly string PickupTag;

			public readonly int CoinCount;

			public InWorldItemCollected(string pickupTag, int coinCount)
			{
				PickupTag = pickupTag;
				CoinCount = coinCount;
			}
		}
	}
}
