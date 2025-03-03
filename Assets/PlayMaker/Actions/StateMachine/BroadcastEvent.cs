// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

using System;
using UnityEngine;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[Obsolete("This action is obsolete; use Send Event with Event Target instead.")]
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends an Event to all FSMs in the scene or to all FSMs on a Game Object. NOTE: This action won't work on the very first frame of the game...")]
	public class BroadcastEvent : FsmStateAction
	{
		[RequiredField]
        [Tooltip("The event to broadcast.")]
		public FsmString broadcastEvent;

		[Tooltip("By default, the event is broadcast to all FSMs in the scene. " +
                 "Optionally you can specify a game object to target. The event will then be broadcast to all FSMs on that game object.")]
		public FsmGameObject gameObject;

        [Tooltip("Broadcast the event to all the Game Object's children too.")]
		public FsmBool sendToChildren;

        [Tooltip("Don't send the event to self.")]
        public FsmBool excludeSelf;

		public override void Reset()
		{
			broadcastEvent = null;
			gameObject = null;
			sendToChildren = false;
			excludeSelf = false;
		}

		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(broadcastEvent.Value))
			{
				if (gameObject.Value != null)
				{
					Fsm.BroadcastEventToGameObject(gameObject.Value, broadcastEvent.Value, sendToChildren.Value, excludeSelf.Value);
					//BroadcastToGameObject(gameObject.Value);
				}
				else
				{
					Fsm.BroadcastEvent(broadcastEvent.Value, excludeSelf.Value);
					//BroadcastToAll();
				}
			}
			
			Finish();
		}
/*		
		void BroadcastToAll()
		{
			// copy the list in case broadcast event changes Fsm.FsmList
			
			var fsmList = new List<Fsm>(Fsm.FsmList);
			
			//Debug.Log("BroadcastToAll");
			foreach (var fsm in fsmList)
			{
				if (excludeSelf.Value && fsm == Fsm)
				{
					continue;
				}
				
				//Debug.Log("to: " + fsm.Name);
				fsm.Event(broadcastEvent.Value);
			}
		}
		
		void BroadcastToGameObject(GameObject go)
		{
			if (go == null) return;

			Fsm.BroadcastEventToGameObject(go, broadcastEvent.Value, sendToChildren.Value, excludeSelf.Value);
			
			if (go == null) return;
			
			var fsmComponents = go.GetComponents<PlayMakerFSM>();
			
			foreach (var fsmComponent in fsmComponents)
			{
				fsmComponent.Fsm.Event(broadcastEvent.Value);
			}
			
			if (sendToChildren.Value)
			{
				for (int i = 0; i < go.transform.childCount; i++) 
				{
					BroadcastToGameObject(go.transform.GetChild(i).gameObject);	
				}
			}
		}*/
	}
}