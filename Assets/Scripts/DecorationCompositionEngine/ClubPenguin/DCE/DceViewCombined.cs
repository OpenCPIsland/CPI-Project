using Disney.Kelowna.Common;
using Foundation.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.DCE
{
	public class DceViewCombined : DceView
	{
		public int MaxAtlasDimension = 512;

		public bool UseGpuSkinning;

		private DceService.Request combineRequest;

		private DceLoadingService.Request<BasePartDefinition> partsRequest;

		private DceLoadingService.Request<Texture2D> decalsRequest;

		private readonly List<ViewPart> partViews = new List<ViewPart>();

		private Renderer rend;

		private bool modelDirty;

		private MeshDefinition meshDef;

		protected override void onAwake()
		{
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
		}

		public override Bounds GetBounds()
		{
			return (rend != null) ? rend.bounds : default(Bounds);
		}

		public void OnEnable()
		{
			Model.PartChanged += model_PartChanged;
			Model.OutfitSet += model_OutfitSet;
		}

		public void OnDisable()
		{
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
				List<KeyValuePair<TypedAssetContentKey<Texture2D>, Action<Texture2D>>> content = new List<KeyValuePair<TypedAssetContentKey<Texture2D>, Action<Texture2D>>>();
				decalsRequest = outfitService.Load(content, outfitService.DecalCache);
			}
		}

		public void Update()
		{
			if (combineRequest == null)
			{
				if (partsRequest == null || !partsRequest.Finished || decalsRequest == null || !decalsRequest.Finished)
				{
				}
			}
			else if (combineRequest.Finished)
			{
				outfitService.Unload(partsRequest);
				partsRequest = null;
				outfitService.Unload(decalsRequest);
				decalsRequest = null;
				setupRenderer();
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
		}

		private void model_ColorChanged()
		{
			modelDirty = true;
		}

		private void model_OutfitSet()
		{
			modelDirty = true;
		}

		private void model_PartChanged(int arg1, int arg2, DceModel.Part arg3, DceModel.Part arg4)
		{
			modelDirty = true;
		}
	}
}
