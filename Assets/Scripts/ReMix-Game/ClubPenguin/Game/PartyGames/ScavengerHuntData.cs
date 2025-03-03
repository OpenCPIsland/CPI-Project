using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class ScavengerHuntData
	{
		private Animator localPlayerAnimator;

		private Animator otherPlayerAnimator;

		public ScavengerHunt.ScavengerHuntRoles LocalPlayerRole
		{
			get;
			set;
		}

		public ScavengerHunt.ScavengerHuntRoles OtherPlayerRole
		{
			get;
			set;
		}

		public int GameSessionId
		{
			get;
			private set;
		}

		public long LocalPlayerSessionId
		{
			get;
			private set;
		}

		public long OtherPlayerSessionId
		{
			get;
			private set;
		}

		public int TotalMarbleCount
		{
			get;
			private set;
		}

		public RewardDefinition WinRewardDefinition
		{
			get;
			private set;
		}

		public RewardDefinition LoseRewardDefinition
		{
			get;
			private set;
		}

		public string LocalPlayerName
		{
			get;
			private set;
		}

		public string OtherPlayerName
		{
			get;
			private set;
		}

		public string RoomName
		{
			get;
			private set;
		}

		public Animator LocalPlayerAnimator
		{
			get
			{
				if (localPlayerAnimator == null)
				{
					GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
					localPlayerAnimator = localPlayerGameObject.GetComponent<Animator>();
				}
				return localPlayerAnimator;
			}
		}

		public Animator OtherPlayerAnimator
		{
			get
			{
				if (otherPlayerAnimator == null)
				{
					CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
					DataEntityHandle handle = cPDataEntityCollection.FindEntity<SessionIdData, long>(OtherPlayerSessionId);
					GameObjectReferenceData component;
					if (cPDataEntityCollection.TryGetComponent(handle, out component))
					{
						otherPlayerAnimator = component.GameObject.GetComponent<Animator>();
					}
				}
				return otherPlayerAnimator;
			}
		}

		public ScavengerHuntData(int sessionId, long localPlayerSessionId, long otherPlayerSessionId, int totalMarbleCount, RewardDefinition winRewardDefinition, RewardDefinition loseRewardDefinition)
		{
			GameSessionId = sessionId;
			LocalPlayerSessionId = localPlayerSessionId;
			OtherPlayerSessionId = otherPlayerSessionId;
			TotalMarbleCount = totalMarbleCount;
			WinRewardDefinition = winRewardDefinition;
			LoseRewardDefinition = loseRewardDefinition;
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle handle = cPDataEntityCollection.FindEntity<SessionIdData, long>(otherPlayerSessionId);
			DataEntityHandle handle2 = cPDataEntityCollection.FindEntity<SessionIdData, long>(localPlayerSessionId);
			DisplayNameData component;
			if (cPDataEntityCollection.TryGetComponent(handle2, out component))
			{
				LocalPlayerName = component.DisplayName;
			}
			DisplayNameData component2;
			if (cPDataEntityCollection.TryGetComponent(handle, out component2))
			{
				OtherPlayerName = component2.DisplayName;
			}
			PresenceData component3;
			if (cPDataEntityCollection.TryGetComponent(handle2, out component3))
			{
				string zoneToken = Service.Get<ZoneTransitionService>().GetZone(component3.Room).ZoneToken;
				RoomName = Service.Get<Localizer>().GetTokenTranslation(zoneToken);
			}
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			localPlayerAnimator = localPlayerGameObject.GetComponent<Animator>();
			GameObjectReferenceData component4;
			if (cPDataEntityCollection.TryGetComponent(handle, out component4))
			{
				otherPlayerAnimator = component4.GameObject.GetComponent<Animator>();
			}
		}

		public override string ToString()
		{
			return string.Format("[ScavengerHuntData: GameSessionId={0}, LocalPlayerSessionId={1}, OtherPlayerSessionId={2}, LocalPlayerName={3}, OtherPlayerName={4}, RoomName={5}, LocalPlayerRole={6}, WinRewardDefinition={7}, LoseRewardDefinition={8}, LocalPlayerAnimator={9}, OtherPlayerAnimator={10}]", GameSessionId, LocalPlayerSessionId, OtherPlayerSessionId, LocalPlayerName, OtherPlayerName, RoomName, LocalPlayerRole, WinRewardDefinition, LoseRewardDefinition, LocalPlayerAnimator, OtherPlayerAnimator);
		}
	}
}
