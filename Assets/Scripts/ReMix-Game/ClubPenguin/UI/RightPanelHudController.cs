using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class RightPanelHudController : MonoBehaviour
	{
		public GameObject MapHudPrefab;

		public GameObject XPHudPrefab;

		public GameObject CoinsHudPrefab;

		public GameObject CollectiblesHudPrefab;

		private List<GameObject> openedHuds;

		private XPHud xpHud;

		private CoinsHud coinsHud;

		private CollectiblesHud collectiblesHud;

		public void Start()
		{
			openedHuds = new List<GameObject>();
			xpHud = Object.Instantiate(XPHudPrefab).GetComponent<XPHud>();
			xpHud.transform.SetParent(base.transform, false);
			xpHud.GetComponent<XPHud>().HudOpened += onHudOpened;
			xpHud.GetComponent<XPHud>().HudClosed += onHudClosed;
			coinsHud = Object.Instantiate(CoinsHudPrefab).GetComponent<CoinsHud>();
			coinsHud.transform.SetParent(base.transform, false);
			coinsHud.GetComponent<CoinsHud>().HudOpened += onHudOpened;
			coinsHud.GetComponent<CoinsHud>().HudClosed += onHudClosed;
			collectiblesHud = Object.Instantiate(CollectiblesHudPrefab).GetComponent<CollectiblesHud>();
			collectiblesHud.transform.SetParent(base.transform, false);
			collectiblesHud.GetComponent<CollectiblesHud>().HudOpened += onHudOpened;
			collectiblesHud.GetComponent<CollectiblesHud>().HudClosed += onHudClosed;
		}

		private void onHudOpened(GameObject hud)
		{
			if (!openedHuds.Contains(hud))
			{
				openedHuds.Add(hud);
			}
		}

		private void onHudClosed(GameObject hud)
		{
			openedHuds.Remove(hud);
		}
	}
}
