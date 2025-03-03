using ClubPenguin.Actions;
using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Locomotion;
using ClubPenguin.Props;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class RemotePenguinInteraction : AbstractPenguinInteraction
	{
		public override void Awake()
		{
			base.Awake();
			GetComponent<LocomotionReceiver>().OnTriggerActionEvent += OnPositionTimelineTriggerActionEvent;
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			GetComponent<LocomotionReceiver>().OnTriggerActionEvent -= OnPositionTimelineTriggerActionEvent;
		}

		public void OnPositionTimelineTriggerActionEvent(LocomotionActionEvent evt)
		{
			if (evt.Type != LocomotionAction.Interact || evt.Object == null || interactRequest.Active)
			{
				return;
			}
			switch (evt.Object.type)
			{
			case ObjectType.PLAYER:
				break;
			case ObjectType.LOCAL:
			{
				GameObject gameObject = GameObject.Find(evt.Object.id);
				if (gameObject != null)
				{
					RunActionGraph(gameObject);
				}
				break;
			}
			case ObjectType.SERVER:
			{
				long key = long.Parse(evt.Object.id);
				if (Service.Get<PropService>().propExperienceDictionary.ContainsKey(key))
				{
					currentActionGraphGameObject = ActionSequencer.FindActionGraphObject(Service.Get<PropService>().propExperienceDictionary[key]);
					interactRequest.Set();
				}
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private void RunActionGraph(GameObject targetGameObject)
		{
			GameObject gameObject = ActionSequencer.FindActionGraphObject(targetGameObject);
			if (gameObject != null && canInteractWithActionGraph(gameObject))
			{
				currentActionGraphGameObject = gameObject;
				interactRequest.Set();
			}
		}

		public void OnTriggerEnter(Collider collider)
		{
			if (interactRequest.Active)
			{
				return;
			}
			GameObject gameObject = ActionSequencer.FindActionGraphObject(collider.gameObject);
			DataEntityHandle handle;
			if (gameObject != null && AvatarDataHandle.TryGetPlayerHandle(base.gameObject, out handle) && canInteractWithActionGraph(gameObject))
			{
				ForceInteractionAction component = gameObject.GetComponent<ForceInteractionAction>();
				if (component != null)
				{
					currentActionGraphGameObject = gameObject;
					interactRequest.Set();
				}
			}
		}

		private void Update()
		{
			if (interactRequest.Active && currentActionGraphGameObject != null && currentActionGraphGameObject.activeInHierarchy)
			{
				CoroutineRunner.Start(preStartInteraction(currentActionGraphGameObject), this, "preStartInteraction");
			}
			interactRequest.Update();
		}
	}
}
