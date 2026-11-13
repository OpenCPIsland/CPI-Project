using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.DecorationInventory
{
	public class DecorationInventoryLoader : MonoBehaviour
	{
		private CPDataEntityCollection dataEntityCollection;

		private void Awake()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			if (!dataEntityCollection.HasComponent<DecorationInventoryData>(dataEntityCollection.LocalPlayerHandle))
			{
				Service.Get<INetworkServicesManager>().IglooService.GetDecorations();
			}
			Object.Destroy(this);
		}

		private void OnDestroy()
		{
		}
	}
}
