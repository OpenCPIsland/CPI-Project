using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Participation;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class CPDataEntityCollectionImpl : DataEntityCollectionDictionaryImpl, CPDataEntityCollection, DataEntityCollection
	{
		private EventDispatcher eventDispatcher;

		private static string[] sceneTransitionStartScopes = new string[1]
		{
			CPDataScopes.Scene.ToString()
		};

		private static string[] questEndingScopes = new string[1]
		{
			CPDataScopes.Quest.ToString()
		};

		private static string[] sessionEndingScopes = new string[2]
		{
			CPDataScopes.Session.ToString(),
			CPDataScopes.Quest.ToString()
		};

		private static string[] zoneEndingScopes = new string[2]
		{
			CPDataScopes.Zone.ToString(),
			CPDataScopes.LocalPlayerZone.ToString()
		};

		public DataEntityHandle LocalPlayerHandle
		{
			get;
			private set;
		}

		public long LocalPlayerSessionId
		{
			get;
			private set;
		}

		public CPDataEntityCollectionImpl()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			eventDispatcher.AddListener<ZoneTransitionEvents.ZoneTransition>(onZoneTransition, Disney.LaunchPadFramework.EventDispatcher.Priority.FIRST);
			eventDispatcher.AddListener<QuestEvents.QuestUpdated>(onQuestEnded);
			eventDispatcher.AddListener<SessionEvents.SessionEndedEvent>(onSessionEnded);
			eventDispatcher.AddListener<SessionEvents.SessionPausedEvent>(onSessionPause);
			eventDispatcher.AddListener<SceneTransitionEvents.TransitionStart>(onSceneTransitionStart);
			base.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<SessionIdData>>(onSessionIdAdded);
			base.EventDispatcher.AddListener<DataEntityEvents.ComponentRemovedEvent>(onComponentRemoved);
			LocalPlayerHandle = DataEntityHandle.NullHandle;
		}

		public void ResetLocalPlayerHandle()
		{
			if (!LocalPlayerHandle.IsNull)
			{
				RemoveAllComponents(LocalPlayerHandle);
			}
			LocalPlayerSessionId = 0L;
			LocalPlayerHandle = AddEntity("LocalPlayer");
			AddComponent<ParticipationData>(LocalPlayerHandle);
			AddComponent<EntityKeepAliveData>(LocalPlayerHandle);
		}

		private bool onSessionIdAdded(DataEntityEvents.ComponentAddedEvent<SessionIdData> evt)
		{
			if (!LocalPlayerHandle.IsNull && evt.Handle == LocalPlayerHandle)
			{
				LocalPlayerSessionId = evt.Component.SessionId;
			}
			return false;
		}

		private bool onComponentRemoved(DataEntityEvents.ComponentRemovedEvent evt)
		{
			if (evt.Component is SessionIdData && !LocalPlayerHandle.IsNull && evt.Handle == LocalPlayerHandle)
			{
				LocalPlayerSessionId = 0L;
			}
			return false;
		}

		public bool IsLocalPlayer(long sessionId)
		{
			return LocalPlayerSessionId == sessionId;
		}

		public bool IsLocalPlayerMember()
		{
			MembershipData component;
			if (!DataEntityHandle.IsNullValue(LocalPlayerHandle) && TryGetComponent(LocalPlayerHandle, out component))
			{
				return component.IsMember;
			}
			return false;
		}

		public IList<Transform> GetRemotePlayers()
		{
			DataEntityHandle[] entitiesByType = GetEntitiesByType<RemotePlayerData>();
			IList<Transform> list = new List<Transform>();
			for (int i = 0; i < entitiesByType.Length; i++)
			{
				GameObjectReferenceData component;
				if (TryGetComponent(entitiesByType[i], out component))
				{
					list.Add(component.GameObject.transform);
				}
			}
			return list;
		}

		public DataEntityHandle[] GetRemotePlayerHandles()
		{
			return GetEntitiesByType<RemotePlayerData>();
		}

		private bool onSceneTransitionStart(SceneTransitionEvents.TransitionStart evt)
		{
			EndScopes(sceneTransitionStartScopes);
			return false;
		}

		private bool onZoneTransition(ZoneTransitionEvents.ZoneTransition evt)
		{
			if (evt.State == ZoneTransitionEvents.ZoneTransition.States.Begin)
			{
				ClearZoneScope();
				PausedStateData component;
				if (evt.FromZone != evt.ToZone && TryGetComponent(LocalPlayerHandle, out component))
				{
					RemoveComponent<PausedStateData>(LocalPlayerHandle);
				}
			}
			return false;
		}

		private bool onQuestEnded(QuestEvents.QuestUpdated evt)
		{
			if (evt.Quest.State != Quest.QuestState.Active)
			{
				EndScopes(questEndingScopes);
			}
			return false;
		}

		private bool onSessionPause(SessionEvents.SessionPausedEvent evt)
		{
			ClearZoneScope();
			return false;
		}

		private bool onSessionEnded(SessionEvents.SessionEndedEvent evt)
		{
			EndScopes(sessionEndingScopes);
			ClearZoneScope();
			ResetLocalPlayerHandle();
			return false;
		}

		public void ClearZoneScope()
		{
			EndScopes(zoneEndingScopes);
		}
	}
}
