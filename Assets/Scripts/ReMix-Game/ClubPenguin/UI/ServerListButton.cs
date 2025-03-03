using ClubPenguin.Core;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class ServerListButton : MonoBehaviour
	{
		private void Start()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			PresenceData component = cPDataEntityCollection.GetComponent<PresenceData>(cPDataEntityCollection.LocalPlayerHandle);
			if (component == null || string.IsNullOrEmpty(component.World))
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
