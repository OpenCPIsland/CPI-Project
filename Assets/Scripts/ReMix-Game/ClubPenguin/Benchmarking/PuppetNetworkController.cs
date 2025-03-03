using UnityEngine;

namespace ClubPenguin.Benchmarking
{
	internal class PuppetNetworkController : NetworkController
	{
		public PuppetNetworkController(MonoBehaviour ctx)
			: base(ctx, false)
		{
		}
	}
}
