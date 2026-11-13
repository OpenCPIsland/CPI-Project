using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;

namespace ClubPenguin.UI
{
	public class TextLengthValidatonAction : InputFieldValidatonAction
	{
		public int MinLength = 5;

		public int MaxLength = 15;

		public override IEnumerator Execute(ScriptableActionPlayer player)
		{
			setup(player);
			HasError = (inputString.Length < MinLength || inputString.Length > MaxLength);
			yield break;
		}

		public override string GetErrorMessage()
		{
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(i18nErrorMessage);
			return string.Format(tokenTranslation, MinLength, MaxLength);
		}
	}
}
