using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using ClubPenguin.Props;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ToyBoxHud : MonoBehaviour
	{
		private const string ANIMATOR_SHOW = "Show";

		private const string ANIMATOR_HIDE = "Hide";

		public Image ItemImage;

		public Text ItemCountText;

		private Animator animator;

		private CPDataEntityCollection dataEntityCollection;

		public event System.Action HudOpened;

		public event System.Action HudClosed;

		public void Start()
		{
			animator = GetComponent<Animator>();
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (localPlayerGameObject != null)
			{
				CoroutineRunner.Start(setUpPropEvents(localPlayerGameObject), this, "setUpPropEvents");
			}
			else
			{
				Service.Get<EventDispatcher>().AddListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onLocalPlayerAdded);
			}
			base.gameObject.SetActive(false);
		}

		private bool onLocalPlayerAdded(PlayerSpawnedEvents.LocalPlayerSpawned evt)
		{
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			CoroutineRunner.Start(setUpPropEvents(localPlayerGameObject), this, "setUpPropEvents");
			return false;
		}

		private IEnumerator setUpPropEvents(GameObject player)
		{
			GameObject localPlayer = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			PropUser propUser = localPlayer.GetComponent<PropUser>();
			while (propUser == null)
			{
				propUser = localPlayer.GetComponent<PropUser>();
				yield return null;
			}
			propUser.EPropRetrieved += onPropRetrieved;
			propUser.EPropStored += onPropUsed;
			propUser.EPropUseCompleted += onPropUsed;
			propUser.EPropRemoved += onPropUsed;
		}

		private void onPropRetrieved(Prop prop)
		{
			if (!prop.PropDef.ServerAddedItem && prop.PropDef.GetIconContentKey() != null && !string.IsNullOrEmpty(prop.PropDef.GetIconContentKey().Key))
			{
				LoadIcon(prop.PropDef.GetIconContentKey());
				ConsumableInventory consumableInventory = Service.Get<CPDataEntityCollection>().GetComponent<ConsumableInventoryData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).ConsumableInventory;
				if (consumableInventory.inventoryMap.ContainsKey(prop.PropDef.GetNameOnServer()))
				{
					ItemCountText.text = consumableInventory.inventoryMap[prop.PropDef.GetNameOnServer()].GetItemCount().ToString();
				}
				else
				{
					ItemCountText.text = "0";
				}
			}
		}

		private void onPropUsed(Prop prop)
		{
			animator.SetTrigger("Hide");
			if (this.HudClosed != null)
			{
				this.HudClosed();
			}
		}

		private void LoadIcon(SpriteContentKey contentKey)
		{
			Content.LoadAsync(onSpriteLoaded, contentKey);
		}

		private void onSpriteLoaded(string path, Sprite sprite)
		{
			base.gameObject.SetActive(true);
			ItemImage.sprite = sprite;
			if (this.HudOpened != null)
			{
				this.HudOpened();
			}
			animator.SetTrigger("Show");
		}

		public void OnToyBoxHUDCloseAnimComplete()
		{
			base.gameObject.SetActive(false);
		}
	}
}
