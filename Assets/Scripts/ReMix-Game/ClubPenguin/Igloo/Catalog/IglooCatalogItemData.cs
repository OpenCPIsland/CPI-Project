using ClubPenguin.DecorationInventory;
using ClubPenguin.Net.Domain.Decoration;
using ClubPenguin.Progression;
using UnityEngine;

namespace ClubPenguin.Igloo.Catalog
{
	public class IglooCatalogItemData
	{
		public DecorationType ItemType
		{
			get;
			private set;
		}

		public int ID
		{
			get;
			private set;
		}

		public string TitleToken
		{
			get;
			private set;
		}

		public string DescriptionToken
		{
			get;
			private set;
		}

		public int Cost
		{
			get;
			private set;
		}

		public bool IsMemberOnly
		{
			get;
			private set;
		}

		public int Level
		{
			get;
			private set;
		}

		public bool LevelLocked
		{
			get;
			private set;
		}

		public string MascotName
		{
			get;
			private set;
		}

		public bool ProgressionLocked
		{
			get;
			private set;
		}

		public Sprite IconSprite
		{
			get;
			private set;
		}

		public bool IconSpriteLoaded
		{
			get;
			private set;
		}

		public int StructureSize
		{
			get;
			private set;
		}

		public IglooCatalogItemData(DecorationDefinition definition, ProgressionUtils.ParsedProgression<DecorationDefinition> progressData)
		{
			ItemType = DecorationType.Decoration;
			ID = definition.Id;
			TitleToken = definition.Name;
			DescriptionToken = definition.Description;
			Cost = definition.Cost;
			IsMemberOnly = progressData.MemberLocked;
			Level = progressData.Level;
			ProgressionLocked = progressData.ProgressionLocked;
			MascotName = progressData.MascotName;
			LevelLocked = progressData.LevelLocked;
			StructureSize = -1;
		}

		public IglooCatalogItemData(StructureDefinition definition, ProgressionUtils.ParsedProgression<StructureDefinition> progressData)
		{
			ItemType = DecorationType.Structure;
			ID = definition.Id;
			TitleToken = definition.Name;
			DescriptionToken = definition.Description;
			Cost = definition.Cost;
			IsMemberOnly = progressData.MemberLocked;
			Level = progressData.Level;
			ProgressionLocked = progressData.ProgressionLocked;
			MascotName = progressData.MascotName;
			LevelLocked = progressData.LevelLocked;
			StructureSize = definition.SizeUnits;
		}

		public void SetImageFromTexture2D(string path, Texture2D icon)
		{
			createIconSprite(icon);
		}

		private void createIconSprite(Texture2D texture)
		{
			IconSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), default(Vector2));
			IconSpriteLoaded = true;
		}

		public bool IsDecoration()
		{
			return ItemType == DecorationType.Decoration;
		}
	}
}
