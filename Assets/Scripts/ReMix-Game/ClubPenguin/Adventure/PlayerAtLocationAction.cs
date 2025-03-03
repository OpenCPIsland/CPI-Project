using Disney.LaunchPadFramework;
using HutongGames.PlayMaker;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest (Server)")]
	public class PlayerAtLocationAction : FsmStateAction, ServerVerifiableAction
	{
		[RequiredField]
		public ZoneDefinition WaypointZone;

		[RequiredField]
		public string WaypointName;

		public float ActivationDistance = 1f;

		public string GetVerifiableType()
		{
			return "EnterLocation";
		}

		public object GetVerifiableParameters()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			GameObject gameObject = GameObject.Find(WaypointName);
			if (gameObject == null)
			{
				Disney.LaunchPadFramework.Log.LogError(this, "Unable to find waypoint: " + WaypointName + " in the current scene");
			}
			dictionary.Add("position", gameObject.transform.position);
			dictionary.Add("radius", ActivationDistance);
			dictionary.Add("room", WaypointZone.ZoneName);
			return dictionary;
		}
	}
}
