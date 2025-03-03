using ClubPenguin.Avatar;
using ClubPenguin.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(Canvas))]
	public abstract class AvatarPositionTranslator : MonoBehaviour
	{
		private Camera gameCamera;

		private Canvas canvas;

		private Dictionary<long, Transform> sessionIdToAvatarTransforms;

		protected Transform localPlayer;

		protected CPDataEntityCollection dataEntityCollection;

		private SessionIdData sessionIdData;

		protected long localSessionId
		{
			get
			{
				if (sessionIdData != null)
				{
					return sessionIdData.SessionId;
				}
				if (dataEntityCollection != null && !dataEntityCollection.LocalPlayerHandle.IsNull && dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out sessionIdData))
				{
					return sessionIdData.SessionId;
				}
				return -1L;
			}
		}

		private void Awake()
		{
			canvas = GetComponent<Canvas>();
			sessionIdToAvatarTransforms = new Dictionary<long, Transform>();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			awakeInit();
		}

		private void Start()
		{
			gameCamera = Camera.main;
			startInit();
		}

		protected virtual void awakeInit()
		{
		}

		protected virtual void startInit()
		{
		}

		protected Vector3 getScreenPoint(Vector3 worldPosition)
		{
			return gameCamera.WorldToScreenPoint(worldPosition) * (1f / canvas.scaleFactor);
		}

		protected bool isWithinViewport(Vector3 worldPosition)
		{
			Vector3 viewportPoint = gameCamera.WorldToViewportPoint(worldPosition);
			return isPointInViewport(viewportPoint);
		}

		protected Transform getAvatar(long sessionId)
		{
			Transform transform = null;
			if (isLocalPlayer(sessionId))
			{
				if (localPlayer != null)
				{
					return localPlayer;
				}
				transform = getTransform(sessionId);
				if (isAvatarReady(transform))
				{
					localPlayer = transform;
					return transform;
				}
			}
			else
			{
				if (sessionIdToAvatarTransforms.ContainsKey(sessionId))
				{
					if (sessionIdToAvatarTransforms[sessionId] != null)
					{
						return sessionIdToAvatarTransforms[sessionId];
					}
					sessionIdToAvatarTransforms.Remove(sessionId);
				}
				transform = getTransform(sessionId);
				if (isAvatarReady(transform))
				{
					sessionIdToAvatarTransforms.Add(sessionId, transform);
					return transform;
				}
			}
			return null;
		}

		protected bool isLocalPlayer(long sessionId)
		{
			return sessionId == localSessionId;
		}

		protected bool isWithinRange(Transform remotePlayer, float maximumRange)
		{
			if (localPlayer == null || remotePlayer == null)
			{
				return false;
			}
			float num = Math.Abs(Vector3.Distance(remotePlayer.position, localPlayer.position));
			return num <= maximumRange;
		}

		protected Transform getTransform(long sessionId)
		{
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntity<SessionIdData, long>(sessionId);
			if (!dataEntityHandle.IsNull)
			{
				GameObjectReferenceData component = dataEntityCollection.GetComponent<GameObjectReferenceData>(dataEntityHandle);
				if (component != null && component.GameObject != null)
				{
					return component.GameObject.transform;
				}
			}
			return null;
		}

		private bool isAvatarReady(Transform avatar)
		{
			return avatar != null && avatar.GetComponent<AvatarView>().IsReady;
		}

		private bool isPointInViewport(Vector3 viewportPoint)
		{
			if (viewportPoint.x < 0f || viewportPoint.x > 1f || viewportPoint.y < 0f || viewportPoint.y > 1f || viewportPoint.z < 0f)
			{
				return false;
			}
			return true;
		}
	}
}
