using ClubPenguin.Core;
using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class MannequinEffects : MonoBehaviour
	{
		private const float Z_OFFSET_TO_CAMERA = 5f;

		public GameObject ApplyFabricEffect;

		public GameObject ApplyDecalEffect;

		public GameObject CustomizerItemSavingEffect;

		private EventChannel eventChannel;

		private Camera guiCamera;

		private GameObject itemSavingEffectInstance;

		private void Start()
		{
			guiCamera = GameObject.FindWithTag(UIConstants.Tags.GUI_CAMERA).GetComponent<Camera>();
			setupListeners();
		}

		private void setupListeners()
		{
			eventChannel = new EventChannel(CustomizationContext.EventBus);
			eventChannel.AddListener<CustomizerEffectsEvents.FabricPlaced>(onFabricPlacedShowEffect);
			eventChannel.AddListener<CustomizerEffectsEvents.DecalPlaced>(onDecalPlacedShowEffect);
			eventChannel.AddListener<CustomizerEffectsEvents.ItemSaving>(onItemSavingShowEffect);
			eventChannel.AddListener<CustomizerEffectsEvents.ItemSaved>(onItemSavedShowEffect);
			eventChannel.AddListener<CustomizerUIEvents.EndPurchaseMoment>(onEndPurchaseMoment);
		}

		private bool onFabricPlacedShowEffect(CustomizerEffectsEvents.FabricPlaced evt)
		{
			Vector3 position = guiCamera.ScreenToWorldPoint(evt.PlacedPosition);
			position.z = guiCamera.transform.position.z + 5f;
			Object.Instantiate(ApplyFabricEffect, position, ApplyFabricEffect.transform.rotation);
			return false;
		}

		private bool onDecalPlacedShowEffect(CustomizerEffectsEvents.DecalPlaced evt)
		{
			Vector3 position = guiCamera.ScreenToWorldPoint(evt.PlacedPosition);
			position.z = guiCamera.transform.position.z + 5f;
			Object.Instantiate(ApplyDecalEffect, position, ApplyDecalEffect.transform.rotation);
			return false;
		}

		private bool onItemSavingShowEffect(CustomizerEffectsEvents.ItemSaving evt)
		{
			if (CustomizerItemSavingEffect != null)
			{
				Vector3 position = Camera.main.transform.position;
				position.y = base.transform.position.y;
				Quaternion rotation = Quaternion.LookRotation(position - base.transform.position);
				itemSavingEffectInstance = Object.Instantiate(CustomizerItemSavingEffect, base.transform.position, rotation);
			}
			return false;
		}

		private bool onItemSavedShowEffect(CustomizerEffectsEvents.ItemSaved evt)
		{
			if (itemSavingEffectInstance != null)
			{
				ParticleSystem[] componentsInChildren = itemSavingEffectInstance.GetComponentsInChildren<ParticleSystem>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].Stop();
				}
				Invoke("delayedDestroyEffect", 2f);
			}
			return false;
		}

		private void delayedDestroyEffect()
		{
			if (itemSavingEffectInstance != null)
			{
				Object.Destroy(itemSavingEffectInstance);
			}
		}

		private bool onEndPurchaseMoment(CustomizerUIEvents.EndPurchaseMoment evt)
		{
			if (itemSavingEffectInstance != null)
			{
				CancelInvoke("delayedDestroyEffect");
				Object.Destroy(itemSavingEffectInstance);
			}
			return false;
		}

		private void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
			if (itemSavingEffectInstance != null)
			{
				Object.Destroy(itemSavingEffectInstance);
			}
		}
	}
}
