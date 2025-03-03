using ClubPenguin.Mix;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.Tests;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Tests
{
	public class LoadRegionalPrefabTest : BaseLoginCreateIntegrationTest
	{
		[Header("Legal Panels")]
		public GameObject DefaultRegion;

		public GameObject AURegion;

		public GameObject SpainRegion;

		public GameObject EMEARegion;

		public GameObject SEARegion;

		public GameObject GermanRegion;

		private MixLoginCreateService loginService;

		private bool isConfigComplete = false;

		private bool isTestStepDone = true;

		private KeyValuePair<string, string> current;

		private Dictionary<string, string> regionMap = new Dictionary<string, string>
		{
			{
				"AE",
				"EMEA Bloc"
			},
			{
				"AM",
				"EMEA Bloc"
			},
			{
				"AR",
				"US Bloc"
			},
			{
				"AT",
				"EMEA Bloc"
			},
			{
				"AU",
				"Australia (AU Bloc)"
			},
			{
				"AZ",
				"US Bloc"
			},
			{
				"BE",
				"EMEA Bloc"
			},
			{
				"BG",
				"EMEA Bloc"
			},
			{
				"BR",
				"US Bloc"
			},
			{
				"BY",
				"US Bloc"
			},
			{
				"CA",
				"Canada (CA)"
			},
			{
				"CH",
				"EMEA Bloc"
			},
			{
				"CL",
				"US Bloc"
			},
			{
				"CO",
				"US Bloc"
			},
			{
				"CR",
				"US Bloc"
			},
			{
				"CZ",
				"EMEA Bloc"
			},
			{
				"DE",
				"EMEA Bloc"
			},
			{
				"DK",
				"EMEA Bloc"
			},
			{
				"DO",
				"US Bloc"
			},
			{
				"EC",
				"US Bloc"
			},
			{
				"ES",
				"Spain (ES)"
			},
			{
				"FI",
				"EMEA Bloc"
			},
			{
				"FR",
				"EMEA Bloc"
			},
			{
				"GR",
				"EMEA Bloc"
			},
			{
				"GT",
				"US Bloc"
			},
			{
				"HN",
				"US Bloc"
			},
			{
				"HU",
				"EMEA Bloc"
			},
			{
				"ID",
				"SEA Bloc"
			},
			{
				"IE",
				"EMEA Bloc"
			},
			{
				"IL",
				"EMEA Bloc"
			},
			{
				"IN",
				"US Bloc"
			},
			{
				"IT",
				"EMEA Bloc"
			},
			{
				"KG",
				"US Bloc"
			},
			{
				"KW",
				"EMEA Bloc"
			},
			{
				"KZ",
				"US Bloc"
			},
			{
				"LU",
				"EMEA Bloc"
			},
			{
				"MN",
				"US Bloc"
			},
			{
				"MX",
				"US Bloc"
			},
			{
				"MY",
				"SEA Bloc"
			},
			{
				"NI",
				"US Bloc"
			},
			{
				"NL",
				"EMEA Bloc"
			},
			{
				"NO",
				"EMEA Bloc"
			},
			{
				"NZ",
				"Australia (AU Bloc)"
			},
			{
				"PE",
				"US Bloc"
			},
			{
				"PH",
				"SEA Bloc"
			},
			{
				"PL",
				"EMEA Bloc"
			},
			{
				"PT",
				"EMEA Bloc"
			},
			{
				"PY",
				"US Bloc"
			},
			{
				"QA",
				"EMEA Bloc"
			},
			{
				"RO",
				"EMEA Bloc"
			},
			{
				"RU",
				"US Bloc"
			},
			{
				"SA",
				"EMEA Bloc"
			},
			{
				"SE",
				"EMEA Bloc"
			},
			{
				"SG",
				"SEA Bloc"
			},
			{
				"TH",
				"SEA Bloc"
			},
			{
				"TJ",
				"US Bloc"
			},
			{
				"TM",
				"US Bloc"
			},
			{
				"TR",
				"EMEA Bloc"
			},
			{
				"UK",
				"EMEA Bloc"
			},
			{
				"US",
				"US Bloc"
			},
			{
				"UY",
				"US Bloc"
			},
			{
				"UZ",
				"US Bloc"
			},
			{
				"VN",
				"SEA Bloc"
			},
			{
				"ZA",
				"EMEA Bloc"
			}
		};

		protected override IEnumerator runTest()
		{
			yield return StartCoroutine(base.runTest());
			loginService = Service.Get<MixLoginCreateService>();
			foreach (KeyValuePair<string, string> entry in regionMap)
			{
				while (!isTestStepDone)
				{
					yield return null;
				}
				isTestStepDone = false;
				current = entry;
				KeyValuePair<string, string> keyValuePair = entry;
				string key = keyValuePair.Key;
				keyValuePair = entry;
				yield return StartCoroutine(testStep(key, keyValuePair.Value));
			}
		}

		protected IEnumerator testStep(string countryCode, string region)
		{
			isConfigComplete = false;
			loginService.OnRegistrationConfigUpdated += onConfigUpdated;
			MixLoginCreateService.OverrideCountryCode = countryCode;
			loginService.GetRegistrationConfig();
			while (!isConfigComplete)
			{
				yield return null;
			}
			bool pass;
			switch (region)
			{
			case "EMEA Bloc":
				pass = (!DefaultRegion.activeInHierarchy && !AURegion.activeInHierarchy && !SpainRegion.activeInHierarchy && EMEARegion.activeInHierarchy && !SEARegion.activeInHierarchy && !GermanRegion.activeInHierarchy);
				break;
			case "Germany (DE)":
				pass = (!DefaultRegion.activeInHierarchy && !AURegion.activeInHierarchy && !SpainRegion.activeInHierarchy && !EMEARegion.activeInHierarchy && !SEARegion.activeInHierarchy && GermanRegion.activeInHierarchy);
				break;
			case "SEA Bloc":
				pass = (!DefaultRegion.activeInHierarchy && !AURegion.activeInHierarchy && !SpainRegion.activeInHierarchy && !EMEARegion.activeInHierarchy && SEARegion.activeInHierarchy && !GermanRegion.activeInHierarchy);
				break;
			case "Spain (ES)":
				pass = (!DefaultRegion.activeInHierarchy && !AURegion.activeInHierarchy && SpainRegion.activeInHierarchy && !EMEARegion.activeInHierarchy && !SEARegion.activeInHierarchy && !GermanRegion.activeInHierarchy);
				break;
			case "Australia (AU Bloc)":
				pass = (!DefaultRegion.activeInHierarchy && AURegion.activeInHierarchy && !SpainRegion.activeInHierarchy && !EMEARegion.activeInHierarchy && !SEARegion.activeInHierarchy && !GermanRegion.activeInHierarchy);
				break;
			default:
				pass = (DefaultRegion.activeInHierarchy && !AURegion.activeInHierarchy && !SpainRegion.activeInHierarchy && !EMEARegion.activeInHierarchy && !SEARegion.activeInHierarchy && !GermanRegion.activeInHierarchy);
				break;
			}
			IntegrationTestEx.FailIf(!pass, "(" + countryCode + ", " + region + ") Failed ");
			isTestStepDone = true;
		}

		private void onConfigUpdated(IRegistrationConfiguration RegistrationConfig)
		{
			if (loginService.RegistrationAgeBand.CountryCode == current.Key)
			{
				loginService.OnRegistrationConfigUpdated -= onConfigUpdated;
				isConfigComplete = true;
			}
		}

		protected override void tearDown()
		{
		}
	}
}
