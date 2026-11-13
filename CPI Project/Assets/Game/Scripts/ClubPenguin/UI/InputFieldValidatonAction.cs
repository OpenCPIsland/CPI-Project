using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public abstract class InputFieldValidatonAction : ScriptableAction
	{
		[HideInInspector]
		public bool HasError = false;

		public string i18nErrorMessage;

		public bool isServerSide = false;

		protected InputFieldValidator validator;

		protected string inputString;

		protected void setup(ScriptableActionPlayer player)
		{
			validator = (player as InputFieldValidator);
			inputString = validator.TextInput.text;
		}

		public virtual string GetErrorMessage()
		{
			return Service.Get<Localizer>().GetTokenTranslation(i18nErrorMessage);
		}
	}
}
