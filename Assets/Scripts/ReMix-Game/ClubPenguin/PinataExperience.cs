using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.Props;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native.iOS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(PropExperience))]
	public class PinataExperience : MonoBehaviour
	{
		private const float PINATA_HIT_REACTION_DELAY_IN_SECS = 0.5f;

		private const float DESTROY_PINATA_DELAY_IN_SECS = 2f;

		[SerializeField]
		private List<GameObject> PinataDamageObjects;

		[SerializeField]
		private GameObject PinataDestroyEffectPrefab;

		[SerializeField]
		private GameObject PinataHitEffectPrefab;

		[SerializeField]
		private GameObject CoinsEffectPrefab;

		[SerializeField]
		private GameObject CoinRewardPrefab;

		private string pinataInstanceId;

		private PropExperience propExperience;

		private CPDataEntityCollection dataEntityCollection;

		private ServerObjectItemData serverObjectItemData;

		private DataEntityHandle serverObjectHandle;

		private int currentDamageIndex;

		private GameObject pinataDestroyEffectInstance;

		private GameObject coinsEffectInstance;

		private GameObject coinRewardInstance;

		private List<long> playersToReward;

		private int numCoinsToReward;

		private int hitsLeft;

		private bool isGameObjectDestroyed = false;

		private void Awake()
		{
			propExperience = GetComponent<PropExperience>();
			propExperience.PropExperienceStarted += onExperienceStarted;
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
		}

		private void onExperienceStarted(string instanceId, long ownerId, bool isOwnerLocalPlayer, PropDefinition propDef)
		{
			pinataInstanceId = instanceId;
			numCoinsToReward = 0;
			currentDamageIndex = 0;
			playersToReward = new List<long>();
			long num = long.Parse(pinataInstanceId);
			setupNetworkServiceListeners(num);
			Service.Get<PropService>().propExperienceDictionary[num] = base.gameObject;
		}

		private void setupNetworkServiceListeners(long id)
		{
			CPMMOItemId cPMMOItemId = new CPMMOItemId(id, CPMMOItemId.CPMMOItemParent.WORLD);
			if (dataEntityCollection.TryFindEntity<ServerObjectItemData, CPMMOItemId>(cPMMOItemId, out serverObjectHandle))
			{
				serverObjectItemData = dataEntityCollection.GetComponent<ServerObjectItemData>(serverObjectHandle);
				GetComponent<NetworkObjectController>().ItemId = cPMMOItemId;
				serverObjectItemData.ItemChanged += onItemChanged;
				dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.EntityRemovedEvent>(onItemRemoved);
				parseCPMMOItem(serverObjectItemData.Item);
			}
			Service.Get<EventDispatcher>().AddListener<RewardServiceEvents.RewardsEarned>(onRewardsReceived);
		}

		public void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			Service.Get<EventDispatcher>().RemoveListener<RewardServiceEvents.RewardsEarned>(onRewardsReceived);
			if (serverObjectItemData != null)
			{
				serverObjectItemData.ItemChanged -= onItemChanged;
			}
			if (!serverObjectHandle.IsNull)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.EntityRemovedEvent>(onItemRemoved);
			}
			ClearDataModelInstance();
		}

		private void ClearDataModelInstance()
		{
			long key = long.Parse(pinataInstanceId);
			if (Service.Get<PropService>().propExperienceDictionary.ContainsKey(key))
			{
				Service.Get<PropService>().propExperienceDictionary.Remove(key);
			}
		}

		private void parseCPMMOItem(CPMMOItem cPMMOItem)
		{
			ActionedItem actionedItem = (ActionedItem)cPMMOItem;
			if (actionedItem.ActionCount > 0)
			{
				onPinataHit(cPMMOItem.Id.Id.ToString(), actionedItem.ActionCount);
			}
		}

		private void onItemChanged(CPMMOItem cPMMOItem)
		{
			parseCPMMOItem(cPMMOItem);
		}

		private bool onItemRemoved(DataEntityEvents.EntityRemovedEvent evt)
		{
			if (evt.EntityHandle == serverObjectHandle)
			{
				onItemRemoved();
			}
			return false;
		}

		private void onItemRemoved()
		{
			pinataDestroyEffectInstance = Object.Instantiate(PinataDestroyEffectPrefab);
			pinataDestroyEffectInstance.transform.position = PinataDamageObjects[currentDamageIndex].transform.position;
			if (!isGameObjectDestroyed)
			{
				Object.Destroy(base.gameObject);
				isGameObjectDestroyed = true;
			}
		}

		private bool onRewardsReceived(RewardServiceEvents.RewardsEarned evt)
		{
			if (!string.IsNullOrEmpty(evt.RewardedUsers.sourceId) && evt.RewardedUsers.sourceId == pinataInstanceId)
			{
				if (evt.RewardedUsers.rewards.Count > 0)
				{
					coinsEffectInstance = Object.Instantiate(CoinsEffectPrefab);
					coinsEffectInstance.transform.position = PinataDamageObjects[currentDamageIndex].transform.position;
					foreach (KeyValuePair<long, Reward> reward in evt.RewardedUsers.rewards)
					{
						long key = reward.Key;
						playersToReward.Add(reward.Key);
						if (Service.Get<CPDataEntityCollection>().IsLocalPlayer(key))
						{
							CoinReward rewardable;
							if (reward.Value.TryGetValue(out rewardable))
							{
								numCoinsToReward = rewardable.Coins;
							}
							else
							{
								numCoinsToReward = 0;
							}
						}
					}
				}
				hidePinata();
			}
			return false;
		}

		private void onPinataHit(string pinataInstanceId, int hitsLeft)
		{
			if (this.pinataInstanceId.Equals(pinataInstanceId) && hitsLeft != this.hitsLeft)
			{
				this.hitsLeft = hitsLeft;
				CoroutineRunner.Start(pinataHitReaction(), this, "pinataHitReaction");
			}
		}

		private IEnumerator pinataHitReaction()
		{
			yield return new WaitForSeconds(0.5f);
			if (!isGameObjectDestroyed)
			{
			}
		}

		public void OnHitFrame(object hitter)
		{
			GameObject gameObject = hitter as GameObject;
			if (!(gameObject != null))
			{
				return;
			}
			Vector3 direction = PinataDamageObjects[currentDamageIndex].transform.position - gameObject.transform.position;
			direction.y = 0f;
			direction.Normalize();
			if (Vector3.Dot(direction, gameObject.transform.forward) > 0f)
			{
				showPinataHitEfect(ref direction);
				StringPhysics componentInParent = PinataDamageObjects[currentDamageIndex].GetComponentInParent<StringPhysics>();
				if (componentInParent != null)
				{
					float twistDir = (Random.value >= 0.5f) ? 1f : (-1f);
					float value = Random.value;
					componentInParent.ApplyImpulse(ref direction, value, twistDir);
				}
			}
		}

		private void showPinataHitEfect(ref Vector3 direction)
		{
			GameObject gameObject = Object.Instantiate(PinataHitEffectPrefab);
			gameObject.transform.position = PinataDamageObjects[currentDamageIndex].transform.GetChild(0).position;
			gameObject.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
			Object.Destroy(gameObject, gameObject.GetComponentInChildren<ParticleSystem>().main.duration);
		}

		private void hidePinata()
		{
			PinataDamageObjects[currentDamageIndex].SetActive(false);
			SceneRefs.CelebrationRunner.PlayCelebrationAnimation(playersToReward);
			if (numCoinsToReward > 0)
			{
				Service.Get<CPDataEntityCollection>().GetComponent<CoinsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).AddCoins(numCoinsToReward);
				GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
				if (localPlayerGameObject != null)
				{
					Service.Get<iOSHapticFeedback>().TriggerImpactFeedback(iOSHapticFeedback.ImpactFeedbackStyle.Medium);
					Vector3 position = localPlayerGameObject.transform.position;
					position.y = localPlayerGameObject.transform.position.y + 1.5f;
					coinRewardInstance = Object.Instantiate(CoinRewardPrefab);
					coinRewardInstance.transform.position = position;
				}
			}
			CoroutineRunner.Start(waitToDestroyPinata(), this, "waitToDestroyPinata");
		}

		private IEnumerator waitToDestroyPinata()
		{
			yield return new WaitForSeconds(2f);
			destroyPinata();
		}

		private void destroyPinata()
		{
			if (pinataDestroyEffectInstance != null)
			{
				Object.Destroy(pinataDestroyEffectInstance);
			}
			if (coinsEffectInstance != null)
			{
				Object.Destroy(coinsEffectInstance);
				coinsEffectInstance = null;
			}
			if (coinRewardInstance != null)
			{
				Object.Destroy(coinRewardInstance);
				coinRewardInstance = null;
			}
			if (!isGameObjectDestroyed)
			{
				Object.Destroy(base.gameObject);
				isGameObjectDestroyed = true;
			}
		}
	}
}
