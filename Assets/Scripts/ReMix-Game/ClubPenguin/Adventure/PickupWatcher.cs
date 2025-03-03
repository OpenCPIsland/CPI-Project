using ClubPenguin.Core;
using ClubPenguin.Interactables.Domain;
using System;

namespace ClubPenguin.Adventure
{
	[Serializable]
	public class PickupWatcher : TaskWatcher
	{
		public string PickupTag;

		public override object GetExportParameters()
		{
			return PickupTag;
		}

		public override string GetWatcherType()
		{
			return "pickup";
		}

		public override void OnActivate()
		{
			base.OnActivate();
			base.dispatcher.AddListener<InteractablesEvents.InWorldItemCollected>(onItemPickedUp);
		}

		public override void OnDeactivate()
		{
			base.OnDeactivate();
			base.dispatcher.RemoveListener<InteractablesEvents.InWorldItemCollected>(onItemPickedUp);
		}

		private bool onItemPickedUp(InteractablesEvents.InWorldItemCollected evt)
		{
			bool flag = false;
			if (string.IsNullOrEmpty(PickupTag) || evt.PickupTag == PickupTag)
			{
				flag = true;
			}
			if (flag)
			{
				for (int i = 0; i < evt.CoinCount; i++)
				{
					taskIncrement();
				}
			}
			return false;
		}
	}
}
