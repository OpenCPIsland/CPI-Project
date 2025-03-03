using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using DevonLocalization.Core;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Catalog/Theme")]
	public class CatalogThemeDefinition : StaticGameDataDefinition
	{
		[StaticGameDataDefinitionId]
		public int Id;

		[LocalizationToken]
		public string Title;

		[LocalizationToken]
		public string Description;

		[LocalizationToken]
		public string CompleteMessage;

		public TagDefinition[] TemplateTags;

		public TagDefinition[] FabricTags;

		public TagDefinition[] DecalTags;

		public override string ToString()
		{
			return string.Format("[CatalogThemeDefinition] Id: {0}, Title: {1}, Description: {2}", Id, Title, Description);
		}
	}
}
