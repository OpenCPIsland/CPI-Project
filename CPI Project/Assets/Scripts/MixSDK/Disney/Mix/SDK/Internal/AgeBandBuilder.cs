using Disney.Mix.SDK.Internal.GuestControllerDomain;
using Disney.Mix.SDK.Internal.MixDomain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public class AgeBandBuilder : IAgeBandBuilder
	{
		private const string WriteOncePermissionValue = "WRITE_ONCE";

		private const string UsernamePermissionKey = "username";

		private const string PasswordPermissionKey = "password";

		private const string DateOfBirthPermissionKey = "dateOfBirth";

		private const string FirstNamePermissionKey = "firstName";

		private const string LastNamePermissionKey = "lastName";

		private const string EmailPermissionKey = "email";

		private const string ParentEmailPermissionKey = "parentEmail";

		private const string FamilyOfBusinessKey = "FOB";

		private const string LineOfBusinessKey = "LOB";

		private const string EmailNewsletterKey = "EMAIL_NEWSLETTER";

		private const string ThirdPartyKey = "THIRD_PARTY";

		private readonly AbstractLogger logger;

		private readonly IMixWebCallFactory webCallFactory;

		public AgeBandBuilder(AbstractLogger logger, IMixWebCallFactory webCallFactory)
		{
			this.logger = logger;
			this.webCallFactory = webCallFactory;
		}

		public void Build(SiteConfigurationData siteConfig, int age, string languageCode, bool registration, Action<IGetAgeBandResult> callback)
		{
			ConfigurationAgeBand configurationAgeBand;
			string configurationAgeBandKey;
			GetConfigurationAgeBand(siteConfig, null, age, out configurationAgeBand, out configurationAgeBandKey);
			GetRegistrationText(siteConfig, configurationAgeBand, configurationAgeBandKey, languageCode, registration, callback);
		}

		public void Build(SiteConfigurationData siteConfig, string ageBandKey, int age, string languageCode, bool registration, Action<IGetAgeBandResult> callback)
		{
			ConfigurationAgeBand configurationAgeBand;
			string configurationAgeBandKey;
			GetConfigurationAgeBand(siteConfig, ageBandKey, age, out configurationAgeBand, out configurationAgeBandKey);
			GetRegistrationText(siteConfig, configurationAgeBand, configurationAgeBandKey, languageCode, registration, callback);
		}

		private void GetRegistrationText(SiteConfigurationData siteConfig, ConfigurationAgeBand configurationAgeBand, string configurationAgeBandKey, string languageCode, bool registration, Action<IGetAgeBandResult> callback)
		{
			List<string> documentIds = GetAllTextIds(siteConfig, configurationAgeBandKey, registration);
			if (documentIds.Count == 0)
			{
				HandleRequestSuccess(siteConfig, configurationAgeBand, configurationAgeBandKey, registration, new List<RegistrationText>(), callback);
			}
			else
			{
				try
				{
					GetRegistrationTextRequest getRegistrationTextRequest = new GetRegistrationTextRequest();
					getRegistrationTextRequest.LanguageCode = languageCode;
					getRegistrationTextRequest.TextCodes = documentIds;
					getRegistrationTextRequest.CountryCode = configurationAgeBand.country;
					getRegistrationTextRequest.AgeBand = configurationAgeBandKey;
					GetRegistrationTextRequest request = getRegistrationTextRequest;
					IWebCall<GetRegistrationTextRequest, GetRegistrationTextResponse> webCall = webCallFactory.RegistrationTextPost(request);
					webCall.OnResponse += delegate(object sender, WebCallEventArgs<GetRegistrationTextResponse> e)
					{
						GetRegistrationTextResponse response = e.Response;
						if (ValidateResponse(response, documentIds))
						{
							HandleRequestSuccess(siteConfig, configurationAgeBand, configurationAgeBandKey, registration, response.RegistrationText, callback);
						}
						else
						{
							callback(new GetAgeBandResult(false, null));
						}
					};
					webCall.OnError += delegate
					{
						callback(new GetAgeBandResult(false, null));
					};
					webCall.Execute();
				}
				catch (Exception arg)
				{
					logger.Critical("Unhandled exception: " + arg);
					callback(new GetAgeBandResult(false, null));
				}
			}
		}

		private static void GetConfigurationAgeBand(SiteConfigurationData siteConfig, string ageBandKey, int age, out ConfigurationAgeBand configurationAgeBand, out string configurationAgeBandKey)
		{
			if (ageBandKey != null)
			{
				GetAgeBandByKey(siteConfig, ageBandKey, out configurationAgeBand, out configurationAgeBandKey);
				if (configurationAgeBand != null)
				{
					return;
				}
			}
			GetAgeBandByAge(siteConfig, age, out configurationAgeBand, out configurationAgeBandKey);
			if (configurationAgeBand == null)
			{
				GetDefaultAgeBand(siteConfig, out configurationAgeBand, out configurationAgeBandKey);
			}
		}

		private static void GetAgeBandByKey(SiteConfigurationData siteConfig, string ageBandKey, out ConfigurationAgeBand configurationAgeBand, out string configurationAgeBandKey)
		{
			configurationAgeBand = null;
			configurationAgeBandKey = null;
			if (!ValidateAgeBandKey(ageBandKey))
			{
				return;
			}
			ConfigurationAgeBand configurationAgeBand2 = DictionaryUtils.TryGetValue(siteConfig.compliance.ageBands, ageBandKey);
			if (configurationAgeBand2 != null)
			{
				LegalGroup legalGroup = DictionaryUtils.TryGetValue(siteConfig.legal, ageBandKey);
				if (legalGroup != null)
				{
					configurationAgeBand = configurationAgeBand2;
					configurationAgeBandKey = ageBandKey;
				}
			}
		}

		private static void GetAgeBandByAge(SiteConfigurationData siteConfig, int age, out ConfigurationAgeBand configurationAgeBand, out string configurationAgeBandKey)
		{
			configurationAgeBand = null;
			configurationAgeBandKey = null;
			foreach (KeyValuePair<string, ConfigurationAgeBand> ageBand in siteConfig.compliance.ageBands)
			{
				string key = ageBand.Key;
				ConfigurationAgeBand value = ageBand.Value;
				if (value != null && age >= value.minAge && age <= value.maxAge && ValidateAgeBandKey(key))
				{
					LegalGroup legalGroup = DictionaryUtils.TryGetValue(siteConfig.legal, key);
					if (legalGroup != null)
					{
						configurationAgeBand = value;
						configurationAgeBandKey = key;
					}
				}
			}
		}

		private static void GetDefaultAgeBand(SiteConfigurationData siteConfig, out ConfigurationAgeBand configurationAgeBand, out string configurationAgeBandKey)
		{
			Compliance compliance = siteConfig.compliance;
			configurationAgeBand = DictionaryUtils.TryGetValue(compliance.ageBands, compliance.defaultAgeBand);
			configurationAgeBandKey = compliance.defaultAgeBand;
		}

		private static List<string> GetAllTextIds(SiteConfigurationData siteConfig, string ageBandKey, bool registration)
		{
			return GetLegalTextIds(siteConfig, ageBandKey, registration).Concat(GetMarketingTextIds(siteConfig, ageBandKey, registration)).ToList();
		}

		private static bool ValidateAgeBandKey(string key)
		{
			return AgeBandTypeConverter.Convert(key) != AgeBandType.Unknown;
		}

		private static bool ValidateResponse(GetRegistrationTextResponse response, IEnumerable<string> ids)
		{
			if (response == null || response.RegistrationText == null)
			{
				return false;
			}
			List<RegistrationText> legalTexts = response.RegistrationText;
			return !ids.Any(delegate(string id)
			{
				RegistrationText registrationText = legalTexts.FirstOrDefault((RegistrationText legalText) => legalText.TextCode == id);
				return registrationText == null || registrationText.Text == null;
			});
		}

		private static void HandleRequestSuccess(SiteConfigurationData siteConfig, ConfigurationAgeBand configurationAgeBand, string configurationAgeBandKey, bool registration, IList<RegistrationText> legalTexts, Action<IGetAgeBandResult> callback)
		{
			IAgeBand ageBand = CreateAgeBand(siteConfig, configurationAgeBand, configurationAgeBandKey, registration, legalTexts);
			callback(new GetAgeBandResult(ageBand != null, ageBand));
		}

		private static IAgeBand CreateAgeBand(SiteConfigurationData siteConfig, ConfigurationAgeBand configurationAgeBand, string configurationAgeBandKey, bool registration, IList<RegistrationText> legalTexts)
		{
			IRegistrationPermissions permissions = CreateRegistrationPermissions(registration ? configurationAgeBand.CREATE : configurationAgeBand.UPDATE);
			IEnumerable<ILegalDocument> legalDocuments = BuildLegalDocs(siteConfig.legal, configurationAgeBandKey, registration, legalTexts);
			IEnumerable<string> legalDocumentsTypeOrder = GetLegalDocumentsTypeOrder(siteConfig.legal, configurationAgeBandKey);
			IEnumerable<IMarketingItem> marketing = BuildMarketingItems(siteConfig, configurationAgeBandKey, registration, legalTexts);
			AgeBandType type = AgeBandTypeConverter.Convert(configurationAgeBandKey);
			string country = configurationAgeBand.country;
			return new AgeBand(permissions, legalDocuments, legalDocumentsTypeOrder, marketing, type, country);
		}

		private static IRegistrationPermissions CreateRegistrationPermissions(IDictionary<string, FieldRequirements> requirements)
		{
			return new RegistrationPermissions(CreateRegistrationPermission(requirements, "username"), CreateRegistrationPermission(requirements, "password"), CreateRegistrationPermission(requirements, "dateOfBirth"), CreateRegistrationPermission(requirements, "firstName"), CreateRegistrationPermission(requirements, "lastName"), CreateRegistrationPermission(requirements, "email"), CreateRegistrationPermission(requirements, "parentEmail"));
		}

		private static IRegistrationPermission CreateRegistrationPermission(IDictionary<string, FieldRequirements> requirements, string key)
		{
			FieldRequirements value;
			if (requirements.TryGetValue(key, out value) && value.required)
			{
				if (value.editable == "WRITE_ONCE")
				{
					return new RegistrationPermissionWriteOnce();
				}
				return new RegistrationPermissionRequired();
			}
			return new RegistrationPermissionNotAllowed();
		}

		private static IEnumerable<string> GetLegalDocumentsTypeOrder(IDictionary<string, LegalGroup> groups, string ageBandKey)
		{
			LegalGroup legalGroup = DictionaryUtils.TryGetValue(groups, ageBandKey);
			return (legalGroup != null) ? legalGroup.documentTypeOrder : Enumerable.Empty<string>();
		}

		private static IEnumerable<ILegalDocument> BuildLegalDocs(IDictionary<string, LegalGroup> groups, string ageBandKey, bool registration, IList<RegistrationText> legalTexts)
		{
			LegalGroup legalGroup = DictionaryUtils.TryGetValue(groups, ageBandKey);
			IEnumerable<ILegalDocument> source = UseCreate(registration, legalGroup) ? CreateDocuments(legalGroup.CREATE, legalTexts) : CreateDocuments(legalGroup.documents, legalTexts);
			List<ILegalDocument> list = new List<ILegalDocument>(source.Count());
			foreach (string type in legalGroup.documentTypeOrder)
			{
				Func<ILegalDocument, bool> predicate = (ILegalDocument doc) => doc.Type == type;
				list.AddRange(source.Where(predicate));
			}
			return list;
		}

		private static IEnumerable<ILegalDocument> CreateDocuments(IDictionary<string, DocumentType> documents, IList<RegistrationText> legalTexts)
		{
			return ((IEnumerable<KeyValuePair<string, DocumentType>>)documents).Select((Func<KeyValuePair<string, DocumentType>, ILegalDocument>)delegate(KeyValuePair<string, DocumentType> pair)
			{
				string id = pair.Key;
				RegistrationText registrationText = legalTexts.First((RegistrationText legalText) => legalText.TextCode == id);
				DocumentType value = pair.Value;
				string type = value.type;
				bool displayCheckbox = value.displayCheckbox;
				return new LegalDocument(id, registrationText.Text, type, displayCheckbox);
			}).ToArray();
		}

		private static IEnumerable<ILegalDocument> CreateDocuments(IEnumerable<LegalProxy> documents, IList<RegistrationText> legalTexts)
		{
			return documents.Select((Func<LegalProxy, ILegalDocument>)delegate(LegalProxy proxy)
			{
				string key = proxy.key;
				RegistrationText registrationText = legalTexts.First((RegistrationText legalText) => legalText.TextCode == CreateTextId(proxy));
				string type = proxy.sortingTag ?? proxy.type;
				bool displayCheckbox = proxy.displayCheckbox;
				return new LegalDocument(key, registrationText.Text, type, displayCheckbox);
			}).ToArray();
		}

		private static bool UseCreate(bool registration, LegalGroup group)
		{
			return registration && group.CREATE != null;
		}

		private static string CreateTextId(LegalProxy proxy)
		{
			return proxy.key + "_create";
		}

		private static IEnumerable<string> GetLegalTextIds(SiteConfigurationData siteConfig, string ageBandKey, bool registration)
		{
			LegalGroup legalGroup = DictionaryUtils.TryGetValue(siteConfig.legal, ageBandKey);
			return UseCreate(registration, legalGroup) ? legalGroup.CREATE.Select((LegalProxy proxy) => CreateTextId(proxy)) : legalGroup.documents.Keys.Distinct();
		}

		private static IEnumerable<IMarketingItem> BuildMarketingItems(SiteConfigurationData siteConfig, string ageBandKey, bool registration, IEnumerable<RegistrationText> legalTexts)
		{
			ConfigurationMarketingAgeBand ageBand = DictionaryUtils.TryGetValue(siteConfig.marketing, ageBandKey);
			return GetMarketingItems(GetConfiguration(ageBand, registration), legalTexts);
		}

		private static Dictionary<string, ConfigurationMarketingItem> GetConfiguration(ConfigurationMarketingAgeBand ageBand, bool registration)
		{
			if (ageBand == null)
			{
				return null;
			}
			return registration ? ageBand.CREATE : ageBand.PARTIAL;
		}

		private static IEnumerable<IMarketingItem> GetMarketingItems(Dictionary<string, ConfigurationMarketingItem> context, IEnumerable<RegistrationText> legalTexts)
		{
			if (context != null)
			{
				return context.Select(delegate(KeyValuePair<string, ConfigurationMarketingItem> pair)
				{
					string id = pair.Key;
					RegistrationText registrationText = legalTexts.First((RegistrationText legalText) => legalText.TextCode == id);
					ConfigurationMarketingItem value = pair.Value;
					MarketingType itemType = GetItemType(value.type);
					bool @checked = value.@checked;
					return new MarketingItem(id, registrationText.Text, itemType, @checked);
				}).ToArray();
			}
			return Enumerable.Empty<IMarketingItem>();
		}

		private static MarketingType GetItemType(string type)
		{
			switch (type)
			{
			case "FOB":
				return MarketingType.FamilyOfBusiness;
			case "LOB":
				return MarketingType.LineOfBusiness;
			case "EMAIL_NEWSLETTER":
				return MarketingType.EmailNewsletter;
			case "THIRD_PARTY":
				return MarketingType.ThirdParty;
			default:
				return MarketingType.Unknown;
			}
		}

		private static IEnumerable<string> GetMarketingTextIds(SiteConfigurationData siteConfig, string ageBandKey, bool registration)
		{
			ConfigurationMarketingAgeBand ageBand = DictionaryUtils.TryGetValue(siteConfig.marketing, ageBandKey);
			Dictionary<string, ConfigurationMarketingItem> configuration = GetConfiguration(ageBand, registration);
			return GetKeysOrEmpty(configuration).Distinct();
		}

		private static IEnumerable<string> GetKeysOrEmpty(Dictionary<string, ConfigurationMarketingItem> items)
		{
			return (items == null) ? Enumerable.Empty<string>() : items.Keys;
		}
	}
}
