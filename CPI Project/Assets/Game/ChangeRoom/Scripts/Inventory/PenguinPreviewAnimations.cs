using ClubPenguin.Avatar;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.Inventory
{
	[RequireComponent(typeof(Animator))]
	public class PenguinPreviewAnimations : MonoBehaviour
	{
		private static int OUTFIT_CREATED_TRIGGER_ID = Animator.StringToHash("OUTFIT");

		private Animator penguinAnimator;

		private AvatarViewDistinct avatarViewDistinct;

		private EventChannel eventChannel;

		private void Awake()
		{
			penguinAnimator = GetComponent<Animator>();
			avatarViewDistinct = GetComponent<AvatarViewDistinct>();
			setupListeners();
		}

		private void Start()
		{
			avatarViewDistinct.Model.OutfitSet += onOutfitSet;
		}

		private void onOutfitSet(IEnumerable<AvatarModel.ApplyResult> results)
		{
			avatarViewDistinct.Model.OutfitSet -= onOutfitSet;
			penguinAnimator.Rebind();
		}

		private void setupListeners()
		{
			eventChannel = new EventChannel(InventoryContext.EventBus);
			eventChannel.AddListener<InventoryPenguinAnimEvents.EquipmentEquipped>(onEquipmentEquipped);
			eventChannel.AddListener<InventoryPenguinAnimEvents.OutfitCreatedAnimation>(onOutfitCreatedAnimation);
		}

		private bool onEquipmentEquipped(InventoryPenguinAnimEvents.EquipmentEquipped evt)
		{
			EquipmentCategoryDefinitionContentKey categoryForEquipment = getCategoryForEquipment(evt.DefinitionId);
			CoroutineRunner.Start(loadCategoryDefinition(categoryForEquipment), this, "LoadCategoryDefinition");
			return false;
		}

		private EquipmentCategoryDefinitionContentKey getCategoryForEquipment(int definitionId)
		{
			EquipmentCategoryDefinitionContentKey result = null;
			Dictionary<int, TemplateDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
			if (dictionary.ContainsKey(definitionId))
			{
				result = dictionary[definitionId].CategoryKey;
			}
			else
			{
				Log.LogErrorFormatted(this, "Unable to find template definition with id {0}. No equipment category definition found.", definitionId);
			}
			return result;
		}

		private IEnumerator loadCategoryDefinition(EquipmentCategoryDefinitionContentKey categoryKey)
		{
			AssetRequest<EquipmentCategoryDefinition> equipmentCategoryDefinitionAssetRequest = null;
			try
			{
				equipmentCategoryDefinitionAssetRequest = Content.LoadAsync(categoryKey);
			}
			catch (Exception)
			{
				Log.LogErrorFormatted(this, "Could not load category definition {0}.", categoryKey);
			}
			if (equipmentCategoryDefinitionAssetRequest != null)
			{
				yield return equipmentCategoryDefinitionAssetRequest;
				string animTrigger = equipmentCategoryDefinitionAssetRequest.Asset.EquipAnimationTrigger;
				penguinAnimator.SetTrigger(animTrigger);
			}
		}

		private bool onOutfitCreatedAnimation(InventoryPenguinAnimEvents.OutfitCreatedAnimation evt)
		{
			penguinAnimator.SetTrigger(OUTFIT_CREATED_TRIGGER_ID);
			return false;
		}

		private void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
			if (avatarViewDistinct != null)
			{
				avatarViewDistinct.Model.OutfitSet -= onOutfitSet;
			}
		}
	}
}
