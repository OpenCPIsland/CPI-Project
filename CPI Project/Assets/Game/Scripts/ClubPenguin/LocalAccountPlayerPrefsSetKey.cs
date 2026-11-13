using ClubPenguin.Core;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin
{
	[ActionCategory("PlayerPrefs")]
	public class LocalAccountPlayerPrefsSetKey : FsmStateAction
	{
		[RequiredField]
		public FsmString key;

		public override void Reset()
		{
			key = "";
		}

		public override void OnEnter()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			string displayName = cPDataEntityCollection.GetComponent<DisplayNameData>(cPDataEntityCollection.LocalPlayerHandle).DisplayName;
			string text = displayName + key.Value;
			PlayerPrefs.SetInt(text, 1);
			Finish();
		}
	}
}
