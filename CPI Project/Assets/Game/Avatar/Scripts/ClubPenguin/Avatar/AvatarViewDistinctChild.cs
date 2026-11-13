using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Avatar
{
	[DisallowMultipleComponent]
	public class AvatarViewDistinctChild : AvatarBaseAsync
	{
		private Renderer rend;

		private OutfitService outfitService;

		private readonly ViewPart partView = new ViewPart();

		private OutfitService.Request<EquipmentViewDefinition> currentEquipmentRequest;

		private OutfitService.Request<Texture2D> currentDecalRequest;

		private OutfitService.Request<EquipmentViewDefinition> loadingEquipmentRequest;

		private OutfitService.Request<Texture2D> loadingDecalRequest;

		private BodyColorMaterialProperties bodyMatProps;

		public int SlotIndex
		{
			get;
			internal set;
		}

		public int PartIndex
		{
			get;
			internal set;
		}

		public AvatarModel Model
		{
			get;
			internal set;
		}

		public Rig Rig
		{
			get;
			internal set;
		}

		public Renderer Renderer
		{
			get
			{
				return rend;
			}
		}

		public ViewPart ViewPart
		{
			get
			{
				return partView;
			}
		}

		public void Awake()
		{
			outfitService = Service.Get<OutfitService>();
		}

		public void OnEnable()
		{
			Model.ColorChanged += ApplyColor;
		}

		public void OnDisable()
		{
			Model.ColorChanged -= ApplyColor;
		}

		public void OnDestroy()
		{
			cleanupLoading();
			cleanupCurrent();
		}

		public void Apply(AvatarModel.Part newPart)
		{
			startWork();
			if (newPart != null)
			{
				if (newPart.Equipment != null)
				{
					cleanupLoading();
					EquipmentModelDefinition.Part eqPart = newPart.Equipment.Parts[newPart.Index];
					KeyValuePair<TypedAssetContentKey<EquipmentViewDefinition>, Action<EquipmentViewDefinition>>[] content = new KeyValuePair<TypedAssetContentKey<EquipmentViewDefinition>, Action<EquipmentViewDefinition>>[1]
					{
						AvatarView.CreatePartContent(Model, partView, newPart, eqPart)
					};
					List<KeyValuePair<TypedAssetContentKey<Texture2D>, Action<Texture2D>>> list = new List<KeyValuePair<TypedAssetContentKey<Texture2D>, Action<Texture2D>>>();
					if (newPart.Decals != null)
					{
						AvatarView.CreateDecalContent(partView, newPart, list);
					}
					loadingEquipmentRequest = outfitService.Load(content, outfitService.EquipmentPartCache);
					loadingDecalRequest = outfitService.Load(list, outfitService.DecalCache);
				}
				else
				{
					cleanupCurrent();
					BodyViewDefinition bodyViewDefinition = Model.Definition.Slots[newPart.Index].LOD[Model.LodLevel];
					bodyViewDefinition.ApplyToViewPart(partView);
					setupRenderer();
					stopWork();
				}
				base.gameObject.SetActive(true);
			}
			else
			{
				base.gameObject.SetActive(false);
				cleanupCurrent();
				stopWork();
			}
		}

		public void ApplyColor()
		{
			bodyMatProps = new BodyColorMaterialProperties(Model.BeakColor, Model.BellyColor, Model.BodyColor);
		}

		public void Update()
		{
			if (base.IsBusy && loadingEquipmentRequest.Finished && loadingDecalRequest.Finished)
			{
				cleanupCurrent();
				switchLoadingRequestsToCurrent();
				setupRenderer();
				stopWork();
			}
			if (bodyMatProps != null && rend != null)
			{
				Material sharedMaterial = rend.sharedMaterial;
				if (sharedMaterial != null)
				{
					bodyMatProps.Apply(sharedMaterial);
					bodyMatProps = null;
				}
			}
		}

		private void setupRenderer()
		{
			partView.SetupRenderer(base.gameObject, Model, ref rend);
		}

		private void cleanupCurrent()
		{
			partView.CleanUp(base.gameObject);
			if (currentEquipmentRequest != null)
			{
				outfitService.Unload(currentEquipmentRequest);
				currentEquipmentRequest = null;
			}
			if (currentDecalRequest != null)
			{
				outfitService.Unload(currentDecalRequest);
				currentDecalRequest = null;
			}
		}

		private void cleanupLoading()
		{
			if (loadingEquipmentRequest != null)
			{
				outfitService.Unload(loadingEquipmentRequest);
				loadingEquipmentRequest = null;
			}
			if (loadingDecalRequest != null)
			{
				outfitService.Unload(loadingDecalRequest);
				loadingDecalRequest = null;
			}
		}

		private void switchLoadingRequestsToCurrent()
		{
			currentEquipmentRequest = loadingEquipmentRequest;
			currentDecalRequest = loadingDecalRequest;
			loadingEquipmentRequest = null;
			loadingDecalRequest = null;
		}
	}
}
