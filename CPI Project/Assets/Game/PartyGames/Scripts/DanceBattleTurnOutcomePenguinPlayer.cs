using ClubPenguin.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class DanceBattleTurnOutcomePenguinPlayer
	{
		private DataEntityHandle handle;

		private Queue<int> moveQueue;

		private HeldObjectsData heldObjectData;

		private Animator animator;

		public DanceBattleTurnOutcomePenguinPlayer(DanceBattleTurnOutcomeMoveData.PlayerMoveData moveData)
		{
			moveQueue = new Queue<int>(moveData.DanceMoveIds);
			handle = getDataEntityHandle(moveData.PlayerSessionId);
			heldObjectData = getHeldObjectData(handle);
			animator = getPenguinAnimator(handle);
			if (animator == null || heldObjectData == null || handle.IsNull)
			{
				Destroy();
				return;
			}
			heldObjectData.PlayerHeldObjectChanged += onHeldObjectChanged;
			Service.Get<EventDispatcher>().AddListener<DanceBattleEvents.DanceMoveAnimationComplete>(onDanceMoveAnimationComplete);
			Service.Get<EventDispatcher>().AddListener<NetworkControllerEvents.RemotePlayerLeftRoomEvent>(onRemotePlayerLeftRoom);
			playNextDanceMove();
		}

		public void Destroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<DanceBattleEvents.DanceMoveAnimationComplete>(onDanceMoveAnimationComplete);
			Service.Get<EventDispatcher>().RemoveListener<NetworkControllerEvents.RemotePlayerLeftRoomEvent>(onRemotePlayerLeftRoom);
			handle = null;
			animator = null;
			moveQueue.Clear();
			if (heldObjectData != null)
			{
				heldObjectData.PlayerHeldObjectChanged -= onHeldObjectChanged;
				heldObjectData = null;
			}
		}

		private void playNextDanceMove()
		{
			if (moveQueue.Count > 0)
			{
				playDanceMove(moveQueue.Dequeue());
				return;
			}
			if (handle == Service.Get<CPDataEntityCollection>().LocalPlayerHandle)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(DanceBattleEvents.LocalPlayerDanceSequenceComplete));
			}
			Destroy();
		}

		private void playDanceMove(int danceMoveId)
		{
			if (animator != null)
			{
				animator.SetInteger(AnimationHashes.Params.Emote, danceMoveId);
				animator.SetTrigger(AnimationHashes.Params.PlayEmote);
			}
		}

		private void onHeldObjectChanged(DHeldObject heldObject)
		{
			Destroy();
		}

		private bool onDanceMoveAnimationComplete(DanceBattleEvents.DanceMoveAnimationComplete evt)
		{
			if (evt.Handle == handle)
			{
				playNextDanceMove();
			}
			return false;
		}

		private bool onRemotePlayerLeftRoom(NetworkControllerEvents.RemotePlayerLeftRoomEvent evt)
		{
			if (evt.Handle == handle)
			{
				Destroy();
			}
			return false;
		}

		private DataEntityHandle getDataEntityHandle(long sessionId)
		{
			return Service.Get<CPDataEntityCollection>().FindEntity<SessionIdData, long>(sessionId);
		}

		private HeldObjectsData getHeldObjectData(DataEntityHandle handle)
		{
			return Service.Get<CPDataEntityCollection>().GetComponent<HeldObjectsData>(handle);
		}

		private Animator getPenguinAnimator(DataEntityHandle dataHadle)
		{
			Animator result = null;
			GameObjectReferenceData component = Service.Get<CPDataEntityCollection>().GetComponent<GameObjectReferenceData>(dataHadle);
			if (component != null && component.GameObject != null)
			{
				result = component.GameObject.GetComponent<Animator>();
			}
			return result;
		}
	}
}
