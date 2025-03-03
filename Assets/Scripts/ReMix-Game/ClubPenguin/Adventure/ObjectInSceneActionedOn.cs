using Disney.LaunchPadFramework;
using HutongGames.PlayMaker;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[HutongGames.PlayMaker.Tooltip("Validate that the user actioned at the position of an object in the scene")]
	[ActionCategory("Interactables (Server)")]
	public class ObjectInSceneActionedOn : FsmStateAction, ServerVerifiableAction
	{
		[RequiredField]
		public string InteractObjectName;

		public string GetVerifiableType()
		{
			return "ActionButtonClicked";
		}

		public object GetVerifiableParameters()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			GameObject gameObject = GameObject.Find(InteractObjectName);
			if (gameObject == null)
			{
				Disney.LaunchPadFramework.Log.LogError(this, "Unable to find object: " + InteractObjectName + " in the current scene");
			}
			dictionary.Add("position", gameObject);
			return dictionary;
		}

		public override void OnEnter()
		{
			Finish();
		}
	}
}
