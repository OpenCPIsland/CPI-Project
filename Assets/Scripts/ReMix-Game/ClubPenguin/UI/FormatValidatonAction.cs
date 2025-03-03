using Disney.Kelowna.Common;
using System.Collections;
using System.Text.RegularExpressions;

namespace ClubPenguin.UI
{
	public class FormatValidatonAction : InputFieldValidatonAction
	{
		public string CheckRegex = "";

		public bool MustMatch;

		public override IEnumerator Execute(ScriptableActionPlayer player)
		{
			setup(player);
			if (MustMatch)
			{
				HasError = !Regex.IsMatch(inputString, CheckRegex);
			}
			else
			{
				HasError = Regex.IsMatch(inputString, CheckRegex);
			}
			yield break;
		}
	}
}
