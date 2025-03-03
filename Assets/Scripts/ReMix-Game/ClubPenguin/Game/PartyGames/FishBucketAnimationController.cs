using ClubPenguin.Core;
using ClubPenguin.Props;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class FishBucketAnimationController
	{
		private const float TIME_DELAY_CANNON_CATCH_IN_SECONDS = 1f;

		private const float TIME_DELAY_CANNON_SQUID_IN_SECONDS = 0.8f;

		private const float TIME_DELAY_SCREENINK_IN_SECONDS = 1f;

		private const float TIME_DELAY_CANNON_SHOOT_IN_SECONDS = 1f;

		public Action<int> ShowTurnOutputCompleteAction;

		public Action CannonRotationCompleteAction;

		private Dictionary<long, Animator> playerIdToAnimator;

		private FishBucketCannon cannon;

		private FishBucketHud hud;

		private readonly int ANIMATOR_HASH_PENGUIN_PROPMODE = Animator.StringToHash("PropMode");

		private readonly int ANIMATOR_HASH_PENGUIN_TORSOACTION1 = Animator.StringToHash("TorsoAction1");

		private readonly int ANIMATOR_HASH_PENGUIN_TORSOACTION2 = Animator.StringToHash("TorsoAction2");

		private readonly int ANIMATOR_HASH_PENGUIN_TORSOACTION3 = Animator.StringToHash("TorsoAction3");

		private readonly int ANIMATOR_HASH_PENGUIN_PROPRETRIEVETYPE = Animator.StringToHash("PropRetrieveType");

		public void Destroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			if (cannon != null)
			{
				FishBucketCannon fishBucketCannon = cannon;
				fishBucketCannon.ShotCompleteAction = (Action<long, GameObject, int>)Delegate.Remove(fishBucketCannon.ShotCompleteAction, new Action<long, GameObject, int>(onCannonShotComplete));
			}
		}

		public void RemovePlayer(long playerId)
		{
			playerIdToAnimator.Remove(playerId);
		}

		public void SetPlayers(List<long> playerIds)
		{
			playerIdToAnimator = new Dictionary<long, Animator>();
			for (int i = 0; i < playerIds.Count; i++)
			{
				Animator animator = FindAnimatorFromPlayerId(playerIds[i]);
				if (!(animator == null))
				{
					playerIdToAnimator.Add(playerIds[i], animator);
					setPropModeOnPropEquip(playerIds[i]);
				}
			}
		}

		public void SetFishBucketHud(FishBucketHud hud)
		{
			this.hud = hud;
		}

		public void SetCannon(GameObject cannon)
		{
			this.cannon = cannon.GetComponentInChildren<FishBucketCannon>();
			FishBucketCannon fishBucketCannon = this.cannon;
			fishBucketCannon.ShotCompleteAction = (Action<long, GameObject, int>)Delegate.Combine(fishBucketCannon.ShotCompleteAction, new Action<long, GameObject, int>(onCannonShotComplete));
			FishBucketCannon fishBucketCannon2 = this.cannon;
			fishBucketCannon2.RotationCompleteAction = (Action)Delegate.Combine(fishBucketCannon2.RotationCompleteAction, new Action(onCannonRotationComplete));
		}

		public void StartPlayerTurn(long playerId)
		{
			cannon.PointCannonAtPlayer(playerId, playerIdToAnimator[playerId].gameObject);
			foreach (KeyValuePair<long, Animator> item in playerIdToAnimator)
			{
				if (item.Key == playerId)
				{
					playerIdToAnimator[item.Key].SetInteger(ANIMATOR_HASH_PENGUIN_PROPMODE, 2);
				}
				else
				{
					playerIdToAnimator[item.Key].SetInteger(ANIMATOR_HASH_PENGUIN_PROPMODE, 3);
				}
			}
		}

		public void ShowTurnOutput(long playerId, int fishDelta)
		{
			CoroutineRunner.Start(showTurnOutput(playerId, fishDelta), this, "");
		}

		private IEnumerator showTurnOutput(long playerId, int fishDelta)
		{
			yield return CoroutineRunner.Start(finishAskForFishAnimation(playerId), this, "");
			if (playerIdToAnimator.ContainsKey(playerId) && playerIdToAnimator[playerId] != null)
			{
				cannon.ShootFish(playerIdToAnimator[playerId].gameObject, fishDelta);
				if (fishDelta >= 0)
				{
					yield return new WaitForSeconds(1f);
					playerIdToAnimator[playerId].SetTrigger(ANIMATOR_HASH_PENGUIN_TORSOACTION2);
				}
				else
				{
					yield return new WaitForSeconds(0.8f);
					playerIdToAnimator[playerId].SetTrigger(ANIMATOR_HASH_PENGUIN_TORSOACTION3);
				}
			}
		}

		private IEnumerator finishAskForFishAnimation(long playerId)
		{
			if (playerIdToAnimator.ContainsKey(playerId) && playerIdToAnimator[playerId] != null)
			{
				playerIdToAnimator[playerId].SetTrigger(ANIMATOR_HASH_PENGUIN_TORSOACTION1);
			}
			yield return new WaitForSeconds(1f);
		}

		private Animator FindAnimatorFromPlayerId(long playerId)
		{
			DataEntityHandle dataEntityHandle = Service.Get<CPDataEntityCollection>().FindEntity<SessionIdData, long>(playerId);
			if (dataEntityHandle == null)
			{
				return null;
			}
			GameObjectReferenceData component = Service.Get<CPDataEntityCollection>().GetComponent<GameObjectReferenceData>(dataEntityHandle);
			if (component == null)
			{
				return null;
			}
			return component.GameObject.GetComponent<Animator>();
		}

		private void setPropModeOnPropEquip(long playerId)
		{
			PropUser component = playerIdToAnimator[playerId].gameObject.GetComponent<PropUser>();
			component.EPropRetrieved += delegate
			{
				playerIdToAnimator[playerId].SetInteger(ANIMATOR_HASH_PENGUIN_PROPMODE, 3);
			};
		}

		private void onCannonRotationComplete()
		{
			if (CannonRotationCompleteAction != null)
			{
				CannonRotationCompleteAction();
			}
		}

		private void onCannonShotComplete(long shotTargetPlayerId, GameObject shotTarget, int fishDelta)
		{
			if (fishDelta < 0)
			{
				CoroutineRunner.Start(showInkScreenWipe(shotTargetPlayerId, fishDelta), this, "");
			}
			if (ShowTurnOutputCompleteAction != null)
			{
				ShowTurnOutputCompleteAction(fishDelta);
			}
		}

		private IEnumerator showInkScreenWipe(long shotTargetPlayerId, int fishDelta)
		{
			if (shotTargetPlayerId == Service.Get<CPDataEntityCollection>().LocalPlayerSessionId)
			{
				hud.ShowInkedOverlay(fishDelta);
			}
			yield return new WaitForSeconds(1f);
			playerIdToAnimator[shotTargetPlayerId].SetFloat(ANIMATOR_HASH_PENGUIN_PROPRETRIEVETYPE, 1f);
			playerIdToAnimator[shotTargetPlayerId].Play("Retrieve");
		}
	}
}
