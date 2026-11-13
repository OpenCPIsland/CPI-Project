using ClubPenguin.Mix;
using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class LegalCheckboxesValidator : MonoBehaviour, IFormValidationItem
	{
		public GameObject LegalPanel;

		public Transform LegalTextParent;

		public GameObject LegalSpinner;

		public GameObject EMEALegalTextExtra;

		[HideInInspector]
		public bool IsValidationComplete = false;

		[HideInInspector]
		public bool IsValidationInProgress = false;

		[HideInInspector]
		public bool HasError = false;

		public LegalTextController LegalTextPrefab;

		public ValidationEvent OnValidationError;

		public UnityEvent OnValidationSuccess;

		private MixLoginCreateService mixService;

		private List<Toggle> checkBoxes;

		private List<string> emeaCountries = new List<string>
		{
			"TR",
			"SA",
			"AE",
			"AF",
			"AX",
			"AL",
			"DZ",
			"AD",
			"AO",
			"AM",
			"AT",
			"AZ",
			"BH",
			"BY",
			"BE",
			"BJ",
			"BA",
			"BW",
			"BV",
			"BG",
			"BF",
			"BI",
			"CM",
			"CV",
			"CF",
			"TD",
			"KM",
			"CD",
			"CG",
			"CI",
			"HR",
			"CY",
			"CZ",
			"DK",
			"DJ",
			"EG",
			"GQ",
			"ER",
			"EE",
			"ET",
			"FK",
			"FO",
			"FI",
			"FR",
			"GA",
			"GM",
			"GE",
			"DE",
			"GH",
			"GI",
			"GR",
			"GL",
			"GG",
			"GN",
			"GW",
			"VA",
			"HU",
			"IS",
			"IR",
			"IQ",
			"IE",
			"IM",
			"IL",
			"IT",
			"JE",
			"JO",
			"KZ",
			"KE",
			"KV",
			"KR",
			"KW",
			"LV",
			"LB",
			"LS",
			"LR",
			"LY",
			"LI",
			"LT",
			"LU",
			"MK",
			"MG",
			"MW",
			"ML",
			"MT",
			"MR",
			"MU",
			"YT",
			"MD",
			"MC",
			"ME",
			"MA",
			"MZ",
			"NA",
			"NL",
			"NE",
			"NG",
			"NO",
			"OM",
			"PS",
			"PL",
			"PT",
			"QA",
			"RE",
			"RO",
			"RU",
			"RW",
			"SH",
			"SM",
			"ST",
			"SN",
			"RS",
			"SC",
			"SL",
			"SK",
			"SI",
			"SO",
			"ZA",
			"GS",
			"ES",
			"SD",
			"SJ",
			"SZ",
			"SE",
			"CH",
			"SY",
			"TZ",
			"TG",
			"TN",
			"UG",
			"UA",
			"GB",
			"EH",
			"YE",
			"YM",
			"ZW"
		};

		public bool AllBoxesAreChecked
		{
			get
			{
				bool result = false;
				if (checkBoxes != null)
				{
					int num = 0;
					foreach (Toggle checkBox in checkBoxes)
					{
						if (checkBox.isOn)
						{
							num++;
						}
					}
					result = (num == checkBoxes.Count);
				}
				return result;
			}
		}

		public event System.Action OnLegalDocumentsShown;

		protected void onAwake()
		{
		}

		private void OnEnable()
		{
			mixService = Service.Get<MixLoginCreateService>();
			mixService.OnRegistrationConfigUpdated += onRegistrationConfigUpdated;
			LegalSpinner.SetActive(false);
			if (mixService.RegistrationAgeBand == null)
			{
				LegalSpinner.SetActive(true);
			}
			showLegalDocuments();
		}

		private void OnDisable()
		{
			mixService.OnRegistrationConfigUpdated -= onRegistrationConfigUpdated;
		}

		public void ValidateLegalCheckBoxes()
		{
			IsValidationInProgress = true;
			HasError = false;
			string text = "";
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("Account.Create.LegalDoc.errorString");
			if (checkBoxes != null)
			{
				foreach (Toggle checkBox in checkBoxes)
				{
					HasError = (HasError || !checkBox.isOn);
					if (!checkBox.isOn)
					{
						string tokenTranslation2 = Service.Get<Localizer>().GetTokenTranslation("Account.Create.LegalDoc." + checkBox.name);
						text = text + string.Format(tokenTranslation, tokenTranslation2) + "\n";
					}
				}
			}
			else
			{
				Log.LogError(this, "Error while validating checkboxes, could be that there are no checkboxes to validate on the form.");
			}
			if (HasError)
			{
				ShowError(text);
			}
			else
			{
				OnValidationSuccess.Invoke();
			}
			IsValidationComplete = true;
			IsValidationInProgress = false;
		}

		public void ShowError(string errorMessage)
		{
			OnValidationError.Invoke(errorMessage);
		}

		public void ResetValidation()
		{
			if (IsValidationComplete)
			{
				IsValidationComplete = false;
				ValidateLegalCheckBoxes();
			}
		}

		private void onRegistrationConfigUpdated(IRegistrationConfiguration registrationConfig)
		{
			showLegalDocuments();
			LegalSpinner.SetActive(false);
		}

		private void showLegalDocuments()
		{
			if (mixService.RegistrationAgeBand != null)
			{
				checkBoxes = new List<Toggle>();
				foreach (Transform item in LegalTextParent.transform)
				{
					UnityEngine.Object.Destroy(item.gameObject);
				}
				IEnumerable<ILegalDocument> legalDocuments = mixService.RegistrationAgeBand.LegalDocuments;
				if (mixService.UpdateAgeBand != null)
				{
					legalDocuments = mixService.UpdateAgeBand.LegalDocuments;
				}
				foreach (ILegalDocument item2 in legalDocuments)
				{
					ShowLegalText(item2.Text, item2.DisplayCheckbox, item2.Id);
				}
				EMEALegalTextExtra.SetActive(emeaCountries.Contains(mixService.RegistrationAgeBand.CountryCode));
				if (this.OnLegalDocumentsShown != null)
				{
					this.OnLegalDocumentsShown();
				}
			}
		}

		public void ShowLegalText(string legalText, bool needsCheckBox, string legalId)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(LegalTextPrefab.gameObject);
			gameObject.transform.SetParent(LegalTextParent, false);
			LegalTextController component = gameObject.GetComponent<LegalTextController>();
			if (needsCheckBox)
			{
				checkBoxes.Add(component.SetLegalTextWithCheckBox(legalText, legalId, this));
			}
			else
			{
				component.SetLegalText(legalText, this);
			}
		}
	}
}
