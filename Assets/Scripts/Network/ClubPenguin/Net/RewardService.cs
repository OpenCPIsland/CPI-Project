using ClubPenguin.Net.Client;
using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;
using System.Collections.Generic;

namespace ClubPenguin.Net
{
	public class RewardService : BaseNetworkService, IRewardService, INetworkService
	{
		private Dictionary<RewardedUserCollection, SignedResponse<RewardedUserCollectionJsonHelper>> delayedRewards;

		public override void Initialize(ClubPenguinClient clubPenguinClient)
		{
			delayedRewards = new Dictionary<RewardedUserCollection, SignedResponse<RewardedUserCollectionJsonHelper>>();
			base.Initialize(clubPenguinClient);
		}

		protected override void setupListeners()
		{
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.RECEIVED_REWARDS, onRewardsReceived);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.RECEIVED_REWARDS_DELAYED, onRewardsReceivedDelayed);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.RECEIVED_ROOOM_REWARDS, onRoomRewardsReceived);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.LEVELUP, onRemoteUserLevelUp);
			Service.Get<EventDispatcher>().AddListener<RewardServiceEvents.ClaimDelayedReward>(onClaimDelayedReward);
		}

		public void ExchangeAllForCoins()
		{
			APICall<ExchangeAllForCoinsOperation> aPICall = clubPenguinClient.RewardApi.ExchangeAllForCoins();
			aPICall.OnResponse += onExchangeAllForCoins;
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		private void onExchangeAllForCoins(ExchangeAllForCoinsOperation operation, HttpResponse arg2)
		{
			onAssetsSet(operation.ResponseBody);
		}

		public void CalculateExchangeAllForCoins(IBaseNetworkErrorHandler errorHandler)
		{
			APICall<CalculateExchangeForCoinsOperation> aPICall = clubPenguinClient.RewardApi.CalculateExchangeAllForCoins();
			aPICall.OnResponse += onCalculateExchangeAllForCoins;
			aPICall.OnError += delegate(CalculateExchangeForCoinsOperation o, HttpResponse response)
			{
				NetworkErrorService.OnError(response, errorHandler);
			};
			aPICall.Execute();
		}

		private void onCalculateExchangeAllForCoins(CalculateExchangeForCoinsOperation operation, HttpResponse arg2)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.MyRewardCalculated(operation.ResponseBody.coins));
		}

		public void ClaimPreregistrationRewards()
		{
			APICall<ClaimPreregistrationRewardOperation> aPICall = clubPenguinClient.RewardApi.ClaimPreregistationRewards();
			aPICall.OnResponse += onClaimPreregistrationRewardsResponse;
			aPICall.OnError += onClaimPreregistrationRewardsError;
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		private void onClaimPreregistrationRewardsError(ClaimPreregistrationRewardOperation operation, HttpResponse httpResponse)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(RewardServiceEvents.ClaimPreregistrationRewardNotFound));
		}

		private void onClaimPreregistrationRewardsResponse(ClaimPreregistrationRewardOperation operation, HttpResponse httpResponse)
		{
			Reward reward = operation.ResponseBody.reward.ToReward();
			if (reward != null && !reward.isEmpty())
			{
				Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.MyRewardEarned(RewardSource.SERVER_OBJECT, "PreRegistrationReward", reward));
				Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.ClaimPreregistrationRewardFound(reward));
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(RewardServiceEvents.ClaimPreregistrationRewardNotFound));
			}
			handleCPResponse(operation.ResponseBody);
		}

		public void ClaimServerAddedRewards()
		{
			APICall<ClaimServerAddedRewardsOperation> aPICall = clubPenguinClient.RewardApi.ClaimServerAddedRewards();
			aPICall.OnResponse += onClaimServerAddedRewardsResponse;
			aPICall.OnError += onClaimServerAddedRewardsError;
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		private void onClaimServerAddedRewardsError(ClaimServerAddedRewardsOperation operation, HttpResponse httpResponse)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(RewardServiceEvents.ClaimServerAddedRewardsNotFound));
		}

		private void onClaimServerAddedRewardsResponse(ClaimServerAddedRewardsOperation operation, HttpResponse httpResponse)
		{
			List<ServerAddedReward> list = new List<ServerAddedReward>();
			foreach (ClaimedServerAddedReward claimedReward in operation.ResponseBody.claimedRewards)
			{
				ServerAddedReward item = default(ServerAddedReward);
				item.definitionId = claimedReward.rewardId.definitionId;
				item.instanceId = claimedReward.rewardId.instanceId;
				item.reward = claimedReward.reward.ToReward();
				Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.MyRewardEarned(RewardSource.SERVER_OBJECT, item.instanceId, item.reward));
				list.Add(item);
			}
			if (list.Count > 0)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.ClaimServerAddedRewardsFound(list));
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(RewardServiceEvents.ClaimServerAddedRewardsNotFound));
			}
			handleCPResponse(operation.ResponseBody);
		}

		public void ClaimReward(int rewardId)
		{
			APICall<ClaimRewardOperation> aPICall = clubPenguinClient.RewardApi.ClaimReward(rewardId);
			aPICall.OnResponse += onClaimRewardResponse;
			aPICall.OnError += onClaimRewardFailed;
			aPICall.Execute();
		}

		private void onClaimRewardFailed(ClaimRewardOperation operation, HttpResponse httpResponse)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.ClaimableRewardFail(operation.RewardId));
			handleCPResponseError(operation, httpResponse);
		}

		private void onClaimRewardResponse(ClaimRewardOperation operation, HttpResponse httpResponse)
		{
			Reward reward = operation.ResponseBody.reward.ToReward();
			if (reward != null && !reward.isEmpty())
			{
				Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.MyRewardEarned(RewardSource.CLAIMABLE_REWARD, "ClaimableReward", reward));
				Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.ClaimedReward(reward));
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.ClaimableRewardFail(operation.RewardId));
			}
			handleCPResponse(operation.ResponseBody);
		}

		public void ClaimQuickNotificationReward()
		{
			APICall<ClaimQuickNotificationRewardOperation> aPICall = clubPenguinClient.RewardApi.ClaimQuickNotificationReward();
			aPICall.OnResponse += onClaimQuickNotificationRewardResponse;
			aPICall.OnError += onClaimQuickNotificationRewardFailed;
			aPICall.Execute();
		}

		private void onClaimQuickNotificationRewardResponse(ClaimQuickNotificationRewardOperation operation, HttpResponse httpResponse)
		{
			Reward reward = operation.ResponseBody.reward.ToReward();
			if (reward != null && !reward.isEmpty())
			{
				Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.ClaimQuickNotificationRewardSuccess(reward));
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(RewardServiceEvents.ClaimQuickNotificationRewardFailed));
			}
		}

		private void onClaimQuickNotificationRewardFailed(ClaimQuickNotificationRewardOperation operation, HttpResponse httpResponse)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(RewardServiceEvents.ClaimQuickNotificationRewardFailed));
			handleCPResponseError(operation, httpResponse);
		}

		public void ClaimDailySpinReward()
		{
			APICall<ClaimDailySpinRewardOperation> aPICall = clubPenguinClient.RewardApi.ClaimDailySpinReward();
			aPICall.OnResponse += onClaimDailySpinRewardResponse;
			aPICall.OnError += onClaimDailySpinRewardFailed;
			aPICall.Execute();
		}

		private void onClaimDailySpinRewardResponse(ClaimDailySpinRewardOperation operation, HttpResponse httpResponse)
		{
			Reward reward = operation.ResponseBody.reward.ToReward();
			Reward chestReward = null;
			if (operation.ResponseBody.chestReward != null)
			{
				chestReward = operation.ResponseBody.chestReward.ToReward();
			}
			if (reward != null && !reward.isEmpty())
			{
				Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.ClaimDailySpinRewardSuccess(reward, chestReward, operation.ResponseBody.spinOutcomeId));
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(RewardServiceEvents.ClaimDailySpinRewardFailed));
			}
		}

		private void onClaimDailySpinRewardFailed(ClaimDailySpinRewardOperation operation, HttpResponse httpResponse)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(RewardServiceEvents.ClaimDailySpinRewardFailed));
			handleCPResponseError(operation, httpResponse);
		}

		public void QA_SetReward(Reward reward)
		{
			APICall<QASetRewardOperation> aPICall = clubPenguinClient.RewardApi.QA_SetReward(reward);
			aPICall.OnResponse += onAssetsSet;
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		private void onRewardsReceived(GameServerEvent gameServerEvent, object data)
		{
			SignedResponse<RewardedUserCollectionJsonHelper> signedResponse = (SignedResponse<RewardedUserCollectionJsonHelper>)data;
			RewardedUserCollection rewardedUserCollection = signedResponse.Data.toRewardedUserCollection();
			if (rewardedUserCollection.rewards.ContainsKey(clubPenguinClient.PlayerSessionId))
			{
				APICall<AddRewardOperation> aPICall = clubPenguinClient.RewardApi.AddReward(signedResponse);
				aPICall.OnResponse += onAssetsSet;
				aPICall.OnError += handleCPResponseError;
				aPICall.Execute();
			}
			Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.RewardsEarned(rewardedUserCollection));
		}

		private void onRewardsReceivedDelayed(GameServerEvent gameServerEvent, object data)
		{
			SignedResponse<RewardedUserCollectionJsonHelper> signedResponse = (SignedResponse<RewardedUserCollectionJsonHelper>)data;
			RewardedUserCollection key = signedResponse.Data.toRewardedUserCollection();
			delayedRewards.Add(key, signedResponse);
		}

		private void onRoomRewardsReceived(GameServerEvent gameServerEvent, object data)
		{
			SignedResponse<InRoomRewards> signedResponse = (SignedResponse<InRoomRewards>)data;
			APICall<AddRoomRewardsOperation> aPICall = clubPenguinClient.RewardApi.AddRoomRewards(signedResponse);
			aPICall.OnResponse += onAssetsSet;
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
			Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.RoomRewardsReceived(signedResponse.Data.room, signedResponse.Data.collected));
		}

		private void onRemoteUserLevelUp(GameServerEvent gameServerEvent, object data)
		{
			UserLevelUpEvent userLevelUpEvent = (UserLevelUpEvent)data;
			Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.LevelUp(userLevelUpEvent.SessionId, userLevelUpEvent.Level));
		}

		private void onAssetsSet(AddRoomRewardsOperation operation, HttpResponse httpResponse)
		{
			onAssetsSet(operation.ResponseBody);
		}

		private void onAssetsSet(AddRewardOperation operation, HttpResponse httpResponse)
		{
			onAssetsSet(operation.ResponseBody);
		}

		private void onAssetsSet(QASetRewardOperation operation, HttpResponse httpResponse)
		{
			onAssetsSet(operation.ResponseBody);
		}

		private void onAssetsSet(RewardGrantedResponse response)
		{
			PlayerAssets assets = response.assets;
			Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.MyAssetsReceived(assets));
			handleCPResponse(response);
		}

		private bool onClaimDelayedReward(RewardServiceEvents.ClaimDelayedReward evt)
		{
			bool flag = false;
			foreach (KeyValuePair<RewardedUserCollection, SignedResponse<RewardedUserCollectionJsonHelper>> delayedReward in delayedRewards)
			{
				if (delayedReward.Key.source == evt.Source && delayedReward.Key.sourceId == evt.SourceId)
				{
					RewardedUserCollection key = delayedReward.Key;
					SignedResponse<RewardedUserCollectionJsonHelper> value = delayedReward.Value;
					if (key.rewards.ContainsKey(clubPenguinClient.PlayerSessionId))
					{
						APICall<AddRewardOperation> aPICall = clubPenguinClient.RewardApi.AddReward(value);
						aPICall.OnResponse += onAssetsSet;
						aPICall.OnError += handleCPResponseError;
						aPICall.Execute();
					}
					Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.RewardsEarned(key));
					delayedRewards.Remove(key);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
			}
			return false;
		}
	}
}
