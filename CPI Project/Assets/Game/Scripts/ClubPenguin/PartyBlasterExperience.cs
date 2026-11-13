using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.Props;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(PropExperience))]
	public class PartyBlasterExperience : MonoBehaviour
	{
		[SerializeField]
		private GameObject EffectRadiusPrefab;

		[SerializeField]
		private float EffectRadiusStartTimeInSecs;

		[SerializeField]
		private GameObject BlastEffectPrefab;

		[SerializeField]
		private float BlastEffectStartTimeInSecs;

		private string partyBlasterId;

		private PropExperience propExperience;

		private List<long> playersToReward;

		private GameObject effectRadiusInstance;

		private GameObject blastEffectInstance;

		private CPDataEntityCollection dataEntityCollection;

		private ServerObjectPositionData serverObjectPositionData;

		private CPMMOItemId cpMMOItemId;

		private DataEntityHandle serverObjectHandle;

		private void Awake()
		{
			playersToReward = new List<long>();
			propExperience = GetComponent<PropExperience>();
			propExperience.PropExperienceStarted += onExperienceStarted;
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
		}

		private void onExperienceStarted(string instanceId, long ownerId, bool isOwnerLocalPlayer, PropDefinition propDef)
		{
			partyBlasterId = instanceId;
			setupNetworkServiceListeners();
			if (BlastEffectPrefab != null)
			{
				CoroutineRunner.Start(playBlastEffect(), this, "playBlastEffect");
			}
			if (EffectRadiusPrefab != null)
			{
				CoroutineRunner.Start(playEffectRadius(), this, "playEffectRadius");
			}
		}

		private void setupNetworkServiceListeners()
		{
			cpMMOItemId = new CPMMOItemId(long.Parse(partyBlasterId), CPMMOItemId.CPMMOItemParent.WORLD);
			if (dataEntityCollection.TryFindEntity<ServerObjectItemData, CPMMOItemId>(cpMMOItemId, out serverObjectHandle) && dataEntityCollection.TryGetComponent(serverObjectHandle, out serverObjectPositionData))
			{
				serverObjectPositionData.PositionChanged += onItemMoved;
				dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.EntityRemovedEvent>(onItemRemoved);
			}
			Service.Get<EventDispatcher>().AddListener<RewardServiceEvents.RewardsEarned>(onRewardsReceived);
		}

		private void onItemMoved(Vector3 obj)
		{
			base.gameObject.transform.position = obj;
		}

		private bool onItemRemoved(DataEntityEvents.EntityRemovedEvent evt)
		{
			if (evt.EntityHandle == serverObjectHandle)
			{
				onPartyBlasterConsumed(cpMMOItemId.Id.ToString(), null);
			}
			return false;
		}

		public void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			if (serverObjectPositionData != null)
			{
				serverObjectPositionData.PositionChanged -= onItemMoved;
			}
			if (!serverObjectHandle.IsNull)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.EntityRemovedEvent>(onItemRemoved);
			}
			Service.Get<EventDispatcher>().RemoveListener<RewardServiceEvents.RewardsEarned>(onRewardsReceived);
			if (effectRadiusInstance != null)
			{
				Object.Destroy(effectRadiusInstance);
			}
			if (blastEffectInstance != null)
			{
				Object.Destroy(blastEffectInstance);
			}
		}

		private bool onRewardsReceived(RewardServiceEvents.RewardsEarned evt)
		{
			if (!string.IsNullOrEmpty(evt.RewardedUsers.sourceId) && evt.RewardedUsers.sourceId == partyBlasterId)
			{
				if (playersToReward == null)
				{
					Log.LogError(this, "Error: received rewards before party blaster experience was initialized");
					return false;
				}
				if (evt.RewardedUsers != null && evt.RewardedUsers.rewards != null && evt.RewardedUsers.rewards.Count > 0)
				{
					foreach (KeyValuePair<long, Reward> reward in evt.RewardedUsers.rewards)
					{
						playersToReward.Add(reward.Key);
					}
					rewardCoinsWithCelebration();
				}
			}
			return false;
		}

		private void onPartyBlasterConsumed(string blasterInstanceId, List<long> playersToReward)
		{
			if (!(partyBlasterId != blasterInstanceId))
			{
				this.playersToReward = playersToReward;
			}
		}

		private void rewardCoinsWithCelebration()
		{
			SceneRefs.CelebrationRunner.PlayCelebrationAnimation(playersToReward);
		}

		private IEnumerator playEffectRadius()
		{
			yield return new WaitForSeconds(EffectRadiusStartTimeInSecs);
			effectRadiusInstance = Object.Instantiate(EffectRadiusPrefab);
			effectRadiusInstance.transform.position = GetComponent<Transform>().position;
		}

		private IEnumerator playBlastEffect()
		{
			yield return new WaitForSeconds(BlastEffectStartTimeInSecs);
			Renderer rend = base.gameObject.GetComponent<Renderer>();
			if (rend != null)
			{
				rend.enabled = false;
			}
			blastEffectInstance = Object.Instantiate(BlastEffectPrefab);
			blastEffectInstance.transform.position = GetComponent<Transform>().position;
		}
	}
}
