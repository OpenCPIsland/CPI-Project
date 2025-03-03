using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin.SpecialEvents
{
	[Serializable]
	public class ScheduledSwapMaterialData
	{
		public GameObject SwapTarget;

		public MaterialContentKey SwapMaterialKey;

		public bool DestroyTexture = false;
	}
}
