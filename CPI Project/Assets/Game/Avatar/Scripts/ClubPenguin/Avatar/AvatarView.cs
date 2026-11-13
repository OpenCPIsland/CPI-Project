using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Avatar
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Animator))]
	public abstract class AvatarView : AvatarBaseAsync
	{
		public static readonly AssetContentKey DECAL_KEYPATTERN = new AssetContentKey("decals/*");

		public AvatarModel Model;

		protected OutfitService outfitService;

		public abstract Bounds GetBounds();

		public void Awake()
		{
			outfitService = Service.Get<OutfitService>();
			if (Model == null)
			{
				Model = GetComponent<AvatarModel>();
			}
			Animator component = GetComponent<Animator>();
			component.avatar = Model.Definition.UnityAvatar;
			onAwake();
		}

		public void OnDestroy()
		{
			cleanup();
		}

		public static KeyValuePair<TypedAssetContentKey<EquipmentViewDefinition>, Action<EquipmentViewDefinition>> CreatePartContent(AvatarModel model, ViewPart partView, AvatarModel.Part modelPart, EquipmentModelDefinition.Part eqPart)
		{
			TypedAssetContentKey<EquipmentViewDefinition> key = model.Definition.CreatePartKey(modelPart.Equipment, eqPart, model.LodLevel);
			return new KeyValuePair<TypedAssetContentKey<EquipmentViewDefinition>, Action<EquipmentViewDefinition>>(key, delegate(EquipmentViewDefinition eq)
			{
				onPartDefinitionLoaded(partView, eq);
			});
		}

		public static TypedAssetContentKey<Texture2D> CreateDecalKey(DCustomEquipmentDecal decal)
		{
			return new TypedAssetContentKey<Texture2D>(DECAL_KEYPATTERN, decal.TextureName);
		}

		public static void CreateDecalContent(ViewPart partView, AvatarModel.Part modelPart, List<KeyValuePair<TypedAssetContentKey<Texture2D>, Action<Texture2D>>> decalContent)
		{
			partView.InitDecalProps(modelPart.Decals.Length);
			for (int i = 0; i < modelPart.Decals.Length; i++)
			{
				DCustomEquipmentDecal decal = modelPart.Decals[i];
				DecalMaterialProperties decalMatProp = new DecalMaterialProperties(decal.Index);
				decalMatProp.Import(decal);
				partView.SetDecalProps(i, decalMatProp);
				TypedAssetContentKey<Texture2D> key = CreateDecalKey(decal);
				decalContent.Add(new KeyValuePair<TypedAssetContentKey<Texture2D>, Action<Texture2D>>(key, delegate(Texture2D decalTex)
				{
					decalMatProp.Texture = decalTex;
				}));
			}
		}

		private static void onPartDefinitionLoaded(ViewPart partView, EquipmentViewDefinition eq)
		{
			if (eq != null)
			{
				eq.ApplyToViewPart(partView);
			}
		}

		protected abstract void onAwake();

		protected abstract void cleanup();
	}
}
