using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class InputFieldComparisonValidationAction : InputFieldValidatonAction
	{
		public string[] StringsToCompare;

		[Tooltip("Do an exact match comparison of a string")]
		public bool ExactMatch = true;

		[Tooltip("Reversed logic: true = found string triggers error")]
		public bool ReverseCompare = false;

		public override IEnumerator Execute(ScriptableActionPlayer player)
		{
			setup(player);
			string[] stringsToCompare = StringsToCompare;
			foreach (string value in stringsToCompare)
			{
				if (ExactMatch)
				{
					HasError = (ReverseCompare ? inputString.Equals(value) : (!inputString.Equals(value)));
				}
				else
				{
					HasError = (ReverseCompare ? inputString.Contains(value) : (!inputString.Contains(value)));
				}
				if (HasError)
				{
					break;
				}
			}
			yield break;
		}
	}
}
