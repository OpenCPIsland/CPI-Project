using ClubPenguin.Collectibles;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[HutongGames.PlayMaker.Tooltip("Finds a Collectible with a Matching Name.")]
	[ActionCategory("Collectibles")]
	public class FindCollectible : FsmStateAction
	{
		public enum MatchType
		{
			StartsWith,
			Contains,
			EndsWith
		}

		[HutongGames.PlayMaker.Tooltip("The Collectible name to find.")]
		[RequiredField]
		public FsmString TextToMatch;

		[HutongGames.PlayMaker.Tooltip("How to match the name.")]
		public MatchType HowToMatch = MatchType.StartsWith;

		[UIHint(UIHint.Variable)]
		[HutongGames.PlayMaker.Tooltip("Store the result in a GameObject variable.")]
		[RequiredField]
		public FsmGameObject Store;

		public override void Reset()
		{
			TextToMatch = "";
			HowToMatch = MatchType.StartsWith;
			Store = null;
		}

		public override void OnEnter()
		{
			Find();
			Finish();
		}

		private void Find()
		{
			Collectible[] array = Object.FindObjectsOfType(typeof(Collectible)) as Collectible[];
			Collectible[] array2 = array;
			foreach (Collectible collectible in array2)
			{
				if ((HowToMatch == MatchType.StartsWith && collectible.name.StartsWith(TextToMatch.Value)) || (HowToMatch == MatchType.Contains && collectible.name.Contains(TextToMatch.Value)) || (HowToMatch == MatchType.EndsWith && collectible.name.EndsWith(TextToMatch.Value)))
				{
					Store.Value = collectible.gameObject;
					return;
				}
			}
			Store.Value = null;
		}

		public override string ErrorCheck()
		{
			if (string.IsNullOrEmpty(TextToMatch.Value))
			{
				return "Specify Collectible name to match";
			}
			return null;
		}
	}
}
