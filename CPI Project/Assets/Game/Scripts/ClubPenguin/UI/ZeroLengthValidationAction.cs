using Disney.Kelowna.Common;
using System.Collections;

namespace ClubPenguin.UI
{
	public class ZeroLengthValidationAction : InputFieldValidatonAction
	{
		public override IEnumerator Execute(ScriptableActionPlayer player)
		{
			setup(player);
			HasError = string.IsNullOrEmpty(inputString);
			yield break;
		}
	}
}
