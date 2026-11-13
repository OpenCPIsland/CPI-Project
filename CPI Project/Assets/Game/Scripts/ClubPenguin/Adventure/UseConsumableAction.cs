using HutongGames.PlayMaker;
using System.Collections.Generic;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest (Server)")]
	[Tooltip("Have the server monitor and confirm the usage of a consumable")]
	public class UseConsumableAction : FsmStateAction, ServerVerifiableAction
	{
		[RequiredField]
		public string Consumable;

		[Tooltip("Enable if the consumable is given as part of the quest and doesn't need to be purchased and managed via the consumable inventory system")]
		public bool Granted;

		public object GetVerifiableParameters()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("consumable", Consumable);
			dictionary.Add("free", Granted);
			return dictionary;
		}

		public string GetVerifiableType()
		{
			return "ConsumableUsed";
		}

		public override void OnEnter()
		{
			Finish();
		}
	}
}
