using ClubPenguin.Analytics;
using ClubPenguin.Avatar;
using ClubPenguin.Breadcrumbs;
using ClubPenguin.ClothingDesigner.Inventory;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.Progression;
using CpRemixShaders;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.Manimal.Common.Util;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	[RequireComponent(typeof(AvatarViewDistinct))]
	public class CustomizerAvatarController : AvatarController
	{
		public PenguinRotationAnimate PenguinRotater;

		public PersistentBreadcrumbTypeDefinitionKey BreadcrumbType;

		private AvatarViewDistinct avatarViewDistinct;

		private List<CustomizerChild> customizerChildren;

		private List<Renderer> currentEquipmentRenderers;

		private int totalRenders;

		private float itemZoomOffset;

		private EventChannel customizerEventChannel;

		private EventChannel serviceEventChannel;

		private DItemCustomization itemModel;

		private CustomizerModel customizerModel;

		private TemplateDefinition currentTemplateDefition;

		private DCustomEquipment currentEquipment;

		private Dictionary<string, int> decalDefinitionIds;

		public override void Awake()
		{
			base.Awake();
			customizerChildren = new List<CustomizerChild>();
			currentEquipmentRenderers = new List<Renderer>();
			avatarViewDistinct = GetComponent<AvatarViewDistinct>();
			avatarViewDistinct.OnChildAdded += onChildAdded;
			setupListeners();
			decalDefinitionIds = new Dictionary<string, int>();
			Dictionary<int, FabricDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, FabricDefinition>>();
			foreach (KeyValuePair<int, FabricDefinition> item in dictionary)
			{
				decalDefinitionIds[item.Value.AssetName] = item.Key;
			}
			Dictionary<int, DecalDefinition> dictionary2 = Service.Get<GameData>().Get<Dictionary<int, DecalDefinition>>();
			foreach (KeyValuePair<int, DecalDefinition> item2 in dictionary2)
			{
				decalDefinitionIds[item2.Value.AssetName] = item2.Key;
			}
		}

		private void Start()
		{
			Model.ClearAllEquipment();
		}

		public void SetModel(CustomizerModel customizerModel)
		{
			this.customizerModel = customizerModel;
			itemModel = customizerModel.ItemCustomization;
		}

		private void OnEnable()
		{
			PenguinRotater.ResetRotation();
		}

		private void OnDisable()
		{
			reset();
		}

		private void onChildAdded(AvatarViewDistinctChild viewDistinctChild)
		{
			CustomizerChild customizerChild = viewDistinctChild.gameObject.AddComponent<CustomizerChild>();
			customizerChildren.Add(customizerChild);
			customizerChild.EquipmentSet += onCustomizerChildEquipmentSet;
		}

		private void onCustomizerChildEquipmentSet(Renderer rend, DecalMaterialProperties[] defaultDecalMaterials)
		{
			currentEquipmentRenderers.Add(rend);
			if (currentEquipmentRenderers.Count >= totalRenders)
			{
				setDefaultEquipmentValues(defaultDecalMaterials);
			}
			CoroutineRunner.Start(waitForPenguinAtRest(), this, "waitForPenguinAtRest");
		}

		private IEnumerator waitForPenguinAtRest()
		{
			while (!PenguinRotater.isAtRest)
			{
				yield return null;
			}
			cameraZoomInOnGameObject();
		}

		private void cameraZoomInOnGameObject()
		{
			List<MeshCollider> list = new List<MeshCollider>();
			for (int i = 0; i < currentEquipmentRenderers.Count; i++)
			{
				MeshCollider component = currentEquipmentRenderers[i].GetComponent<MeshCollider>();
				if (component != null)
				{
					list.Add(component);
				}
			}
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.CameraZoomInOnGameObject(base.transform, list, itemZoomOffset));
		}

		private void setDefaultEquipmentValues(DecalMaterialProperties[] defaultDecalMaterials)
		{
			itemModel.SetChannelFabric(itemModel.RedChannel, null, Vector2.zero);
			itemModel.SetChannelFabric(itemModel.GreenChannel, null, Vector2.zero);
			itemModel.SetChannelFabric(itemModel.BlueChannel, null, Vector2.zero);
			if (defaultDecalMaterials != null)
			{
				for (int i = 0; i < defaultDecalMaterials.Length; i++)
				{
					DecalMaterialProperties decalMaterialProperties = defaultDecalMaterials[i];
					DecalColorChannel decalColorChannel = (DecalColorChannel)i;
					EquipmentShaderUtils.ApplyDecalScale(decalColorChannel, decalMaterialProperties.Scale, currentEquipmentRenderers);
					EquipmentShaderUtils.ApplyDecalRotation(decalColorChannel, decalMaterialProperties.RotationRads, currentEquipmentRenderers);
				}
			}
			else
			{
				Log.LogErrorFormatted(this, "Default Decal Materals were null and not applied.");
			}
		}

		private void setupListeners()
		{
			customizerEventChannel = new EventChannel(CustomizationContext.EventBus);
			customizerEventChannel.AddListener<CustomizerModelEvents.DecalChangedEvent>(onDecalChanged);
			customizerEventChannel.AddListener<CustomizerModelEvents.DecalTintChangedEvent>(onDecalColorChanged);
			customizerEventChannel.AddListener<CustomizerModelEvents.DecalScaledEvent>(onDecalScaled);
			customizerEventChannel.AddListener<CustomizerModelEvents.DecalRotatedEvent>(onDecalRotated);
			customizerEventChannel.AddListener<CustomizerModelEvents.DecalMovedEvent>(onDecalMoved);
			customizerEventChannel.AddListener<CustomizerModelEvents.DecalTilingChangedEvent>(onDecalTilingChanged);
			customizerEventChannel.AddListener<CustomizerModelEvents.TemplateChangedEvent>(onTemplateChanged);
			customizerEventChannel.AddListener<CustomizerModelEvents.ResetItemModelEvent>(onItemModelReset);
			customizerEventChannel.AddListener<CustomizerUIEvents.ConfirmSaveClickedEvent>(onSaveClothingClicked);
			customizerEventChannel.AddListener<CustomizerUIEvents.ConfirmSubmitClickedEvent>(onSubmitCatalogChallengeItemClicked);
			customizerEventChannel.AddListener<CustomizerUIEvents.ItemZoomOffsetUpdated>(onItemZoomOffsetUpdated);
			customizerEventChannel.AddListener<CustomizerUIEvents.ItemRotationYOffsetUpdated>(onItemRotationYOffsetUpdated);
			customizerEventChannel.AddListener<CustomizerUIEvents.SaveItem>(onItemSaved);
			customizerEventChannel.AddListener<CustomizerUIEvents.ResetRotation>(onItemResetRotation);
			serviceEventChannel = new EventChannel(Service.Get<EventDispatcher>());
		}

		private bool onDecalChanged(CustomizerModelEvents.DecalChangedEvent evt)
		{
			if (evt.DecalLayer.Type == DecalType.DECAL && evt.ChosenRenderer != null && !evt.DecalLayer.IsTiled)
			{
				EquipmentShaderUtils.ApplyDecalTexture(evt.DecalLayer.ShaderChannel, null, currentEquipmentRenderers);
				EquipmentShaderUtils.ApplyDecalTexture(evt.DecalLayer.ShaderChannel, evt.NewDecal, evt.ChosenRenderer);
			}
			else
			{
				EquipmentShaderUtils.ApplyDecalTexture(evt.DecalLayer.ShaderChannel, evt.NewDecal, currentEquipmentRenderers);
			}
			if (evt.DecalLayer.Type == DecalType.FABRIC)
			{
				EquipmentShaderUtils.ApplyDecalRepeat(evt.DecalLayer.ShaderChannel, true, currentEquipmentRenderers);
			}
			return false;
		}

		private bool onDecalColorChanged(CustomizerModelEvents.DecalTintChangedEvent evt)
		{
			EquipmentShaderUtils.ApplyDecalColor(evt.DecalLayer.ShaderChannel, evt.NewColor, currentEquipmentRenderers);
			return false;
		}

		private bool onDecalScaled(CustomizerModelEvents.DecalScaledEvent evt)
		{
			EquipmentShaderUtils.ApplyDecalScale(evt.DecalLayer.ShaderChannel, evt.NewScale, currentEquipmentRenderers);
			return false;
		}

		private bool onDecalRotated(CustomizerModelEvents.DecalRotatedEvent evt)
		{
			EquipmentShaderUtils.ApplyDecalRotation(evt.DecalLayer.ShaderChannel, evt.TotalRotation, currentEquipmentRenderers);
			return false;
		}

		private bool onDecalTilingChanged(CustomizerModelEvents.DecalTilingChangedEvent evt)
		{
			if (evt.DecalLayer.Decal != null && evt.DecalLayer.OriginalRenderer != null)
			{
				if (evt.IsTiled)
				{
					EquipmentShaderUtils.ApplyDecalTexture(evt.DecalLayer.ShaderChannel, evt.DecalLayer.Decal, currentEquipmentRenderers);
				}
				else
				{
					EquipmentShaderUtils.ApplyDecalTexture(evt.DecalLayer.ShaderChannel, null, currentEquipmentRenderers);
					EquipmentShaderUtils.ApplyDecalTexture(evt.DecalLayer.ShaderChannel, evt.DecalLayer.Decal, evt.DecalLayer.OriginalRenderer);
				}
			}
			EquipmentShaderUtils.ApplyDecalRepeat(evt.DecalLayer.ShaderChannel, evt.IsTiled, currentEquipmentRenderers);
			return false;
		}

		private bool onDecalMoved(CustomizerModelEvents.DecalMovedEvent evt)
		{
			EquipmentShaderUtils.ApplyDecalOffset(evt.DecalLayer.ShaderChannel, evt.UVOffset, currentEquipmentRenderers);
			return false;
		}

		private bool onTemplateChanged(CustomizerModelEvents.TemplateChangedEvent evt)
		{
			PenguinRotater.ResetRotationWithOffset(evt.Definition.RotationYOffset);
			reset();
			applyEquipmentToMannequin(evt.Definition);
			return false;
		}

		private void applyEquipmentToMannequin(TemplateDefinition templateDefinition)
		{
			EquipmentModelDefinition equipmentDefinition = Model.Definition.GetEquipmentDefinition(templateDefinition.AssetName);
			currentTemplateDefition = templateDefinition;
			currentEquipment = default(DCustomEquipment);
			currentEquipment.DefinitionId = templateDefinition.Id;
			currentEquipment.Name = templateDefinition.AssetName;
			totalRenders = equipmentDefinition.Parts.Length;
			itemZoomOffset = templateDefinition.ZoomOffset;
			Model.ApplyEquipment(currentEquipment);
		}

		private bool onItemModelReset(CustomizerModelEvents.ResetItemModelEvent evt)
		{
			reset();
			itemModel = customizerModel.ItemCustomization;
			return false;
		}

		private void reset()
		{
			for (int i = 0; i < customizerChildren.Count; i++)
			{
				customizerChildren[i].Reset();
			}
			Model.ClearAllEquipment();
			currentEquipmentRenderers.Clear();
		}

		private bool onSaveClothingClicked(CustomizerUIEvents.ConfirmSaveClickedEvent evt)
		{
			bool flag = false;
			try
			{
				itemModel.CustomEquipmentModel = EquipmentNetworkUtils.GetModelFromCustomizerChildList(customizerChildren, currentTemplateDefition, decalDefinitionIds);
				flag = true;
			}
			catch (Exception ex)
			{
				Log.LogException(this, ex);
			}
			if (flag)
			{
				serviceEventChannel.AddListener<InventoryServiceEvents.EquipmentCreated>(onEquipmentCreated);
				serviceEventChannel.AddListener<InventoryServiceErrors.EquipmentCreationError>(onEquipmentCreationError);
				CustomEquipment equipmentRequest = CustomEquipmentResponseAdaptor.ConvertCustomEquipmentToRequest(itemModel.CustomEquipmentModel);
				Service.Get<INetworkServicesManager>().InventoryService.CreateCustomEquipment(equipmentRequest);
				logItemCreatedAnalytics();
			}
			else
			{
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.SaveClothingItemError));
			}
			return false;
		}

		private bool onEquipmentCreationError(InventoryServiceErrors.EquipmentCreationError evt)
		{
			serviceEventChannel.RemoveListener<InventoryServiceEvents.EquipmentCreated>(onEquipmentCreated);
			serviceEventChannel.RemoveListener<InventoryServiceErrors.EquipmentCreationError>(onEquipmentCreationError);
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.SaveClothingItemError));
			return false;
		}

		private bool onEquipmentCreated(InventoryServiceEvents.EquipmentCreated evt)
		{
			serviceEventChannel.RemoveListener<InventoryServiceEvents.EquipmentCreated>(onEquipmentCreated);
			serviceEventChannel.RemoveListener<InventoryServiceErrors.EquipmentCreationError>(onEquipmentCreationError);
			long equipmentId = evt.EquipmentId;
			DCustomEquipment customEquipmentModel = itemModel.CustomEquipmentModel;
			customEquipmentModel.Id = equipmentId;
			customEquipmentModel.DateTimeCreated = DateTime.UtcNow.GetTimeInMilliseconds();
			InventoryData component = Service.Get<CPDataEntityCollection>().GetComponent<InventoryData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle);
			if (component != null)
			{
				InventoryIconModel<DCustomEquipment> value = new InventoryIconModel<DCustomEquipment>(equipmentId, customEquipmentModel, false, true);
				component.Inventory.Add(equipmentId, value);
			}
			else
			{
				Log.LogError(this, "Unable to locate InventoryData object.");
			}
			Service.Get<NotificationBreadcrumbController>().AddPersistentBreadcrumb(BreadcrumbType, equipmentId.ToString());
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.SaveClothingItemSuccess));
			return false;
		}

		private bool onSubmitCatalogChallengeItemClicked(CustomizerUIEvents.ConfirmSubmitClickedEvent evt)
		{
			bool flag = false;
			try
			{
				itemModel.CustomEquipmentModel = EquipmentNetworkUtils.GetModelFromCustomizerChildList(customizerChildren, currentTemplateDefition, decalDefinitionIds);
				flag = true;
			}
			catch (Exception ex)
			{
				Log.LogException(this, ex);
			}
			if (flag)
			{
				CustomEquipment equipment = CustomEquipmentResponseAdaptor.ConvertCustomEquipmentToRequest(itemModel.CustomEquipmentModel);
				long activeThemeScheduleId = Service.Get<CatalogServiceProxy>().GetActiveThemeScheduleId();
				Service.Get<INetworkServicesManager>().CatalogService.SubmitCatalogThemeItem(activeThemeScheduleId, equipment);
				CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.SubmitClothingItemStart(itemModel.CustomEquipmentModel));
			}
			else
			{
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.SubmitClothingItemError));
			}
			return false;
		}

		private bool onItemZoomOffsetUpdated(CustomizerUIEvents.ItemZoomOffsetUpdated evt)
		{
			if (PenguinRotater.isAtRest && customizerChildren.Count > 0)
			{
				cameraZoomInOnGameObject();
			}
			return false;
		}

		private bool onItemRotationYOffsetUpdated(CustomizerUIEvents.ItemRotationYOffsetUpdated evt)
		{
			PenguinRotater.ResetRotationWithOffset(evt.RotationYOffset);
			return false;
		}

		private bool onItemSaved(CustomizerUIEvents.SaveItem evt)
		{
			PenguinRotater.ResetRotation();
			return false;
		}

		private bool onItemResetRotation(CustomizerUIEvents.ResetRotation evt)
		{
			PenguinRotater.ResetRotation();
			return false;
		}

		private void logItemCreatedAnalytics()
		{
			StringBuilder stringBuilder = new StringBuilder("decals=");
			StringBuilder stringBuilder2 = new StringBuilder("fabrics=");
			for (int i = 0; i < itemModel.CustomEquipmentModel.Parts.Length; i++)
			{
				DCustomEquipmentPart dCustomEquipmentPart = itemModel.CustomEquipmentModel.Parts[i];
				for (int j = 0; j < dCustomEquipmentPart.Decals.Length; j++)
				{
					DCustomEquipmentDecal dCustomEquipmentDecal = dCustomEquipmentPart.Decals[j];
					if (dCustomEquipmentDecal.Type == EquipmentDecalType.DECAL)
					{
						stringBuilder.Append(string.Format("ch{0}:{1},", j, dCustomEquipmentDecal.TextureName));
					}
					else if (dCustomEquipmentDecal.Type == EquipmentDecalType.FABRIC)
					{
						stringBuilder2.Append(string.Format("ch{0}:{1},", j, dCustomEquipmentDecal.TextureName));
					}
				}
			}
			Service.Get<ICPSwrveService>().Action("game.item_purchase", itemModel.TemplateDefinition.name, string.Format("{0}|{1}", stringBuilder, stringBuilder2));
			Service.Get<ICPSwrveService>().PurchaseClothing(itemModel.TemplateDefinition.name, itemModel.TemplateDefinition.Cost, 1, Service.Get<ProgressionService>().Level);
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			for (int i = 0; i < customizerChildren.Count; i++)
			{
				customizerChildren[i].EquipmentSet -= onCustomizerChildEquipmentSet;
			}
			if (currentEquipmentRenderers != null)
			{
				currentEquipmentRenderers.Clear();
			}
			if (customizerEventChannel != null)
			{
				customizerEventChannel.RemoveAllListeners();
			}
			if (serviceEventChannel != null)
			{
				serviceEventChannel.RemoveAllListeners();
			}
		}
	}
}
