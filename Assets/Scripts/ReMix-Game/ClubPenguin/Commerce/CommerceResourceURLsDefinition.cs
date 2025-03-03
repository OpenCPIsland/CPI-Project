using ClubPenguin.Core.StaticGameData;
using System;
using UnityEngine;

namespace ClubPenguin.Commerce
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/CommerceResourceURLs")]
	public class CommerceResourceURLsDefinition : StaticGameDataDefinition
	{
		[StaticGameDataDefinitionId]
		public string Id;

		public string BaseURL;

		public string[] JavascriptURLs;

		public string[] CSSURLs;
	}
}
