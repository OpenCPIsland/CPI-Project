using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using Foundation.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Avatar
{
	public class AvatarViewCombined : AvatarView
	{
		public int MaxAtlasDimension = 512;

		public bool UseGpuSkinning;

		private AvatarService avatarService;

		private AvatarService.Request combineRequest;

		private OutfitService.Request<EquipmentViewDefinition> partsRequest;

		private OutfitService.Request<Texture2D> decalsRequest;

		private readonly List<ViewPart> partViews = new List<ViewPart>();

		private Renderer rend;

		private bool modelDirty;

		private MeshDefinition meshDef;

		protected override void onAwake()
		{
			avatarService = Service.Get<AvatarService>();
			Rig component = GetComponent<Rig>();
			SkinnedMeshDefinition skinnedMeshDefinition = new SkinnedMeshDefinition(UseGpuSkinning);
			skinnedMeshDefinition.RootBoneName = component.RootBone.name;
			skinnedMeshDefinition.BoneNames = new string[component.Bones.Length];
			for (int i = 0; i < component.Bones.Length; i++)
			{
				skinnedMeshDefinition.BoneNames[i] = component.Bones[i].name;
			}
			meshDef = skinnedMeshDefinition;
			rend = meshDef.CreateRenderer(base.gameObject);
			Model.Definition.RenderProperties.Apply(rend);
		}

		public override Bounds GetBounds()
		{
			return (rend != null) ? rend.bounds : default(Bounds);
		}

		public void OnEnable()
		{
			Model.ColorChanged += model_ColorChanged;
			Model.PartChanged += model_PartChanged;
			Model.OutfitSet += model_OutfitSet;
		}

		public void OnDisable()
		{
			Model.ColorChanged -= model_ColorChanged;
			Model.PartChanged -= model_PartChanged;
			Model.OutfitSet -= model_OutfitSet;
		}

		public void LateUpdate()
		{
			if (modelDirty && !base.IsBusy)
			{
				modelDirty = false;
				startWork();
				cleanup();
				partViews.Clear();
				List<KeyValuePair<TypedAssetContentKey<Texture2D>, Action<Texture2D>>> list = new List<KeyValuePair<TypedAssetContentKey<Texture2D>, Action<Texture2D>>>();
				List<KeyValuePair<TypedAssetContentKey<EquipmentViewDefinition>, Action<EquipmentViewDefinition>>> list2 = new List<KeyValuePair<TypedAssetContentKey<EquipmentViewDefinition>, Action<EquipmentViewDefinition>>>();
				foreach (AvatarModel.Part item in Model.IterateParts())
				{
					ViewPart viewPart = new ViewPart();
					if (item.Equipment != null)
					{
						EquipmentModelDefinition.Part eqPart = item.Equipment.Parts[item.Index];
						list2.Add(AvatarView.CreatePartContent(Model, viewPart, item, eqPart));
						if (item.Decals != null)
						{
							AvatarView.CreateDecalContent(viewPart, item, list);
						}
					}
					else
					{
						BodyViewDefinition bodyViewDefinition = Model.Definition.Slots[item.Index].LOD[Model.LodLevel];
						bodyViewDefinition.ApplyToViewPart(viewPart);
					}
					partViews.Add(viewPart);
				}
				partsRequest = outfitService.Load(list2, outfitService.EquipmentPartCache);
				decalsRequest = outfitService.Load(list, outfitService.DecalCache);
			}
		}

		public void Update()
		{
			if (combineRequest == null)
			{
				if (partsRequest != null && partsRequest.Finished && decalsRequest != null && decalsRequest.Finished)
				{
					BodyColorMaterialProperties bodycolor = new BodyColorMaterialProperties(Model.BeakColor, Model.BellyColor, Model.BodyColor);
					combineRequest = avatarService.CombineParts(Model.Definition, bodycolor, partViews, MaxAtlasDimension);
				}
			}
			else if (combineRequest.Finished)
			{
				setupRenderer();
				outfitService.Unload(partsRequest);
				partsRequest = null;
				outfitService.Unload(decalsRequest);
				decalsRequest = null;
				combineRequest = null;
				stopWork();
			}
		}

		private void setupRenderer()
		{
			ComponentExtensions.DestroyIfInstance(rend.sharedMaterial);
			rend.sharedMaterial = meshDef.CreateCombinedMaterial(combineRequest.Atlas);
			meshDef.ApplyMesh(base.gameObject, combineRequest.Mesh);
			rend.enabled = true;
		}

		protected override void cleanup()
		{
			meshDef.CleanUp(base.gameObject);
			ComponentExtensions.DestroyIfInstance(rend.sharedMaterial);
			rend.enabled = false;
			if (partsRequest != null)
			{
				outfitService.Unload(partsRequest);
				partsRequest = null;
			}
			if (decalsRequest != null)
			{
				outfitService.Unload(decalsRequest);
				decalsRequest = null;
			}
			combineRequest = null;
		}

		private void model_ColorChanged()
		{
			modelDirty = true;
		}

		private void model_OutfitSet(IEnumerable<AvatarModel.ApplyResult> results)
		{
			modelDirty = true;
		}

		private void model_PartChanged(int arg1, int arg2, AvatarModel.Part arg3, AvatarModel.Part arg4)
		{
			modelDirty = true;
		}
	}
}
