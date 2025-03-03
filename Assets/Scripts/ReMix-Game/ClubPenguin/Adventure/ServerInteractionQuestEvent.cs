using ClubPenguin.Net.Client.Event;
using Disney.LaunchPadFramework;
using HutongGames.PlayMaker;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest (Server)")]
	[HutongGames.PlayMaker.Tooltip("Have the server monitor and confirm the interaction with an object in the scene that broadcasts the quest event, this has no affect on the client flow")]
	public class ServerInteractionQuestEvent : FsmStateAction, ServerVerifiableAction
	{
		private class ActionedObjectWithPosition
		{
			public ActionedObject gameObject;

			public Vector3 position;
		}

		[RequiredField]
		[HutongGames.PlayMaker.Tooltip("The name of the quest event, likely used as a transition event name in a sub state machine")]
		public string QuestEvent;

		public object GetVerifiableParameters()
		{
			List<ActionedObjectWithPosition> list = new List<ActionedObjectWithPosition>();
			SendQuestEvent[] array = Object.FindObjectsOfType<SendQuestEvent>();
			foreach (SendQuestEvent sendQuestEvent in array)
			{
				if (sendQuestEvent.QuestEvent == QuestEvent)
				{
					ActionedObjectWithPosition actionedObjectWithPosition = new ActionedObjectWithPosition();
					ActionedObject actionedObject = new ActionedObject();
					actionedObject.type = ObjectType.LOCAL;
					actionedObject.id = sendQuestEvent.gameObject.GetPath();
					actionedObject.tag = sendQuestEvent.tag;
					actionedObjectWithPosition.gameObject = actionedObject;
					actionedObjectWithPosition.position = sendQuestEvent.gameObject.transform.position;
					list.Add(actionedObjectWithPosition);
				}
			}
			if (list.Count == 0)
			{
				Disney.LaunchPadFramework.Log.LogError(this, "No objects with the quest event " + QuestEvent + " found in the scene, export will not work");
			}
			return list;
		}

		public string GetVerifiableType()
		{
			return "ActionButtonClicked";
		}

		public override void OnEnter()
		{
			Finish();
		}
	}
}
