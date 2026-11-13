using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public class LegalMarketingErrorsBuilder : ILegalMarketingErrorsBuilder
	{
		private readonly IRegistrationConfigurationGetter registrationConfigurationGetter;

		private readonly string languagePreference;

		private readonly IEpochTime epochTime;

		public LegalMarketingErrorsBuilder(IRegistrationConfigurationGetter registrationConfigurationGetter, string languagePreference, IEpochTime epochTime)
		{
			this.registrationConfigurationGetter = registrationConfigurationGetter;
			this.languagePreference = languagePreference;
			this.epochTime = epochTime;
		}

		public void BuildErrors(ISession session, GuestApiErrorCollection errors, Action<BuildLegalMarketingErrorsResult> successCallback, Action failureCallback)
		{
			if (session.LocalUser.RegistrationProfile.CountryCode != null)
			{
				registrationConfigurationGetter.Get(session.LocalUser.RegistrationProfile.CountryCode, delegate(IInternalGetRegistrationConfigurationResult r)
				{
					HandleGetRegistrationConfiguration(r, session, errors, successCallback, failureCallback);
				});
			}
			else
			{
				registrationConfigurationGetter.Get(delegate(IInternalGetRegistrationConfigurationResult r)
				{
					HandleGetRegistrationConfiguration(r, session, errors, successCallback, failureCallback);
				});
			}
		}

		private void HandleGetRegistrationConfiguration(IInternalGetRegistrationConfigurationResult result, ISession session, GuestApiErrorCollection errors, Action<BuildLegalMarketingErrorsResult> successCallback, Action failureCallback)
		{
			IInternalRegistrationConfiguration internalConfiguration = result.InternalConfiguration;
			if (!result.Success || internalConfiguration == null)
			{
				session.Dispose();
				failureCallback();
			}
			else
			{
				int age = CalculateAge(session.LocalUser.RegistrationProfile.DateOfBirth);
				internalConfiguration.GetUpdateAgeBand(session.LocalUser.RegistrationProfile.AgeBandKey, age, languagePreference, delegate(IGetAgeBandResult bandResult)
				{
					if (!bandResult.Success)
					{
						session.Dispose();
						failureCallback();
					}
					else
					{
						FindMatches(bandResult.AgeBand, errors, successCallback, failureCallback);
					}
				});
			}
		}

		private int CalculateAge(DateTime? dateOfBirth)
		{
			if (!dateOfBirth.HasValue)
			{
				return -1;
			}
			int num = epochTime.UtcNow.Year - dateOfBirth.Value.Year;
			if (num < 0)
			{
				num = 0;
			}
			else if (epochTime.UtcNow.Date < dateOfBirth.Value.AddYears(num))
			{
				num--;
			}
			return num;
		}

		private void FindMatches(IAgeBand ageBand, GuestApiErrorCollection errors, Action<BuildLegalMarketingErrorsResult> successCallback, Action failureCallback)
		{
			IEnumerable<GuestApiError> source = errors.errors.Where((GuestApiError error) => error.code == "PPU_MARKETING");
			IEnumerable<GuestApiError> source2 = errors.errors.Where((GuestApiError error) => error.code == "PPU_LEGAL");
			IMarketingItem[] array = source.Select((GuestApiError error) => ageBand.Marketing.FirstOrDefault((IMarketingItem item) => item.Id == error.inputName)).ToArray();
			ILegalDocument[] array2 = source2.Select((GuestApiError error) => ageBand.LegalDocuments.FirstOrDefault((ILegalDocument item) => item.Id == error.inputName)).ToArray();
			if (array.Any((IMarketingItem item) => item == null) || array2.Any((ILegalDocument item) => item == null))
			{
				failureCallback();
			}
			else
			{
				successCallback(new BuildLegalMarketingErrorsResult(array2, array));
			}
		}
	}
}
