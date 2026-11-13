using ClubPenguin.BlobShadows;
using ClubPenguin.Configuration;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(BlobShadowRenderer))]
	public class BlobShadowMediator : MonoBehaviour
	{
		public ConditionalDefinition_Bool DisableBlobShadows;

		private void Awake()
		{
			if (DisableBlobShadows != null)
			{
				ConditionalProperty<bool> property = Service.Get<ConditionalConfiguration>().GetProperty<bool>(DisableBlobShadows.name);
				if (property != null && property.Value)
				{
					GetComponent<BlobShadowRenderer>().BlobShadowsSupported = false;
				}
			}
		}
	}
}
