using ClubPenguin.DecorationInventory;
using System;
using UnityEngine;

namespace ClubPenguin.Core
{
	public class LightingDefinitionOverride : MonoBehaviour
	{
		[Serializable]
		public struct TrilightOverride
		{
			public LightingDefinitionKey LightingDef;

			public LightingController.Trilight Trilight;
		}

		public TrilightOverride[] TrilightOverrides;
	}
}
