using ClubPenguin.Analytics;
using ClubPenguin.Mix;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Mix.SDK;
using Disney.MobileNetwork;

namespace ClubPenguin.ContentGates
{
	public class AgeGate : AbstractGateController
	{
		private PrefabContentKey gateContentKey = new PrefabContentKey("ParentGatePrefabs/AgeGatePopupPrefab");

		protected override PrefabContentKey GateContentKey
		{
			get
			{
				return gateContentKey;
			}
		}

		protected override void prepGate()
		{
			Service.Get<ICPSwrveService>().Action("view.age_gate");
		}

		protected override void onValueChanged(string strAnswer)
		{
			if (strAnswer.Length == 2)
			{
				if (strAnswer != "10" && strAnswer != "11" && strAnswer != "12")
				{
					onSubmitClicked();
				}
			}
			else if (strAnswer.Length == 3)
			{
				onSubmitClicked();
			}
		}

		protected override void onSubmitClicked()
		{
			string text = gatePrefabController.AnswerInputField.text;
			int intAnswer;
			int.TryParse(text, out intAnswer);
			Service.Get<MixLoginCreateService>().RegistrationConfig.GetUpdateAgeBand(intAnswer, Service.Get<Localizer>().LanguageString, delegate(IGetAgeBandResult result)
			{
				if (result.Success)
				{
					switch (result.AgeBand.AgeBandType)
					{
					case AgeBandType.Unknown:
					case AgeBandType.Child:
						Service.Get<ICPSwrveService>().AgeGate(false, intAnswer, result.AgeBand.CountryCode);
						handleGateFailure();
						break;
					case AgeBandType.Teen:
					case AgeBandType.Adult:
						Service.Get<ICPSwrveService>().AgeGate(true, intAnswer, result.AgeBand.CountryCode);
						handleGateSuccess();
						break;
					}
				}
				else
				{
					Service.Get<ICPSwrveService>().AgeGate(false, intAnswer, "");
					handleGateFailure();
				}
			});
		}

		protected override string getAnalyticsContext()
		{
			return "age_gate";
		}
	}
}
