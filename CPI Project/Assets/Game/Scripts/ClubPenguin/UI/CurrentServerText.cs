using ClubPenguin.Core;
using Disney.MobileNetwork;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class CurrentServerText : MonoBehaviour
	{
		public void Start()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			PresenceData component = cPDataEntityCollection.GetComponent<PresenceData>(cPDataEntityCollection.LocalPlayerHandle);
			if (component != null)
			{
				GetComponent<TextMeshProUGUI>().text = string.Format("{0} ", component.World);
			}
		}
	}
}
