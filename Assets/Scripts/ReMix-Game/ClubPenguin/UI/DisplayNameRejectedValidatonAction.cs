using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System.Collections;

namespace ClubPenguin.UI
{
	public class DisplayNameRejectedValidatonAction : InputFieldValidatonAction
	{
		public override IEnumerator Execute(ScriptableActionPlayer player)
		{
			HasError = (Service.Get<SessionManager>().LocalUser.RegistrationProfile.DisplayNameProposedStatus == DisplayNameProposedStatus.Rejected && Service.Get<SessionManager>().LocalUser.DisplayName.Text == inputString);
			yield break;
		}

		public new string GetErrorMessage()
		{
			return Service.Get<Localizer>().GetTokenTranslation(i18nErrorMessage);
		}
	}
}
