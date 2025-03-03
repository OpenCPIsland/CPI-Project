using ClubPenguin.Core;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Text))]
	public class CurrentServerText : MonoBehaviour
	{
		public void Start()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			PresenceData component = cPDataEntityCollection.GetComponent<PresenceData>(cPDataEntityCollection.LocalPlayerHandle);
			if (component != null)
			{
				GetComponent<Text>().text = string.Format("{0} ", component.World);
			}
		}
	}
}
