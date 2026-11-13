using ClubPenguin.Adventure;
using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Interactables.Domain;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.NPC;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Collectibles
{
	public class Pickupable : Collectible
	{
		public ParticleSystem PickupParticles;

		public Vector3 ParticleRotation = Vector3.zero;

		public bool DestroyPickupObject = true;

		public string SendQuestEvent = "";

		private bool hasBeenPickedUp;

		private ParticleSystem spawnedParticles;

		public override void Awake()
		{
			base.Awake();
		}

		public override void StartCollectible(RespawnResponse respawnResponse)
		{
			isInitialized = true;
			checkIfCollected(respawnResponse);
		}

		private void checkIfCollected(RespawnResponse respawnResponse)
		{
			switch (respawnResponse.State)
			{
			case RespawnState.READY_FOR_PICKUP:
				break;
			case RespawnState.NOT_AVAILABLE:
				pickup(false);
				break;
			case RespawnState.WAITING_TO_RESPAWN:
				pickup(false);
				break;
			}
		}

		private void OnTriggerEnter(Collider trigger)
		{
			if (!hasBeenPickedUp && (string.IsNullOrEmpty(Tag) || trigger.CompareTag(Tag)))
			{
				pickup(true);
			}
		}

		private void OnTriggerStay(Collider trigger)
		{
			OnTriggerEnter(trigger);
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			if (spawnedParticles != null)
			{
				Object.Destroy(spawnedParticles.gameObject);
			}
		}

		private void pickup(bool sendPickupEvent)
		{
			float t = 0f;
			hasBeenPickedUp = true;
			if (OnCollected != null)
			{
				OnCollected(sendPickupEvent);
			}
			Renderer component = GetComponent<Renderer>();
			if (component != null)
			{
				component.enabled = false;
			}
			if (sendPickupEvent)
			{
				Service.Get<INetworkServicesManager>().TaskService.Pickup(base.gameObject.GetPath(), base.gameObject.tag, base.gameObject.transform.position);
				playAudioEvent();
				giveReward();
				t = spawnParticles();
				if (!string.IsNullOrEmpty(SendQuestEvent))
				{
					Service.Get<QuestService>().SendEvent(SendQuestEvent);
				}
			}
			if (DestroyPickupObject)
			{
				Object.Destroy(base.gameObject, t);
			}
			else if (spawnedParticles != null)
			{
				Object.Destroy(spawnedParticles.gameObject, t);
			}
		}

		private void giveReward()
		{
			List<MascotXPRewardDefinition> definitions = RewardDef.GetDefinitions<MascotXPRewardDefinition>();
			if (definitions.Count > 0 && definitions[0].XP > 0)
			{
				Reward reward = RewardDef.ToReward();
				dispatcher.DispatchEvent(new RewardServiceEvents.MyRewardEarned(RewardSource.WORLD_OBJECT, base.gameObject.GetPath(), reward));
			}
			int num = CoinRewardableDefinition.Coins(RewardDef);
			List<CollectibleRewardDefinition> definitions2 = RewardDef.GetDefinitions<CollectibleRewardDefinition>();
			if (num > 0 && definitions2.Count > 0)
			{
				Service.Get<ICPSwrveService>().CoinsGiven(num, "pickupable", definitions2[0].Collectible.CollectibleType);
				Service.Get<CPDataEntityCollection>().GetComponent<CoinsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).AddCoins(num);
				dispatcher.DispatchEvent(new InteractablesEvents.InWorldItemCollected(definitions2[0].Collectible.CollectibleType, num));
			}
		}

		private float spawnParticles()
		{
			float result = 0f;
			if (PickupParticles != null)
			{
				spawnedParticles = Object.Instantiate<ParticleSystem>(rotation: (!(ParticleRotation == Vector3.zero)) ? Quaternion.Euler(ParticleRotation) : base.transform.rotation, original: PickupParticles, position: base.transform.position);
				spawnedParticles.Play();
				result = PickupParticles.main.duration;
			}
			return result;
		}

		public override void RespawnCollectible()
		{
		}
	}
}
