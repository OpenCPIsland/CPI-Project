using DevonLocalization.Core;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace DevonLocalization.TestTools
{
	public class LanguagesPanelBehaviour : MonoBehaviour
	{
		public delegate void OnLanguageChangedDelegate(Language language);

		public GameObject LanguageTogglePrefab;

		public OnLanguageChangedDelegate OnLanguageChanged;

		private void Start()
		{
			foreach (Language value in Enum.GetValues(typeof(Language)))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(LanguageTogglePrefab);
				gameObject.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>(), false);
				gameObject.GetComponent<LanguageToggleBehaviour>().Language = value;
				if (Service.Get<Localizer>().Language == value)
				{
					gameObject.GetComponent<Toggle>().isOn = true;
				}
				gameObject.GetComponent<Toggle>().group = GetComponent<ToggleGroup>();
				LanguageToggleBehaviour component = gameObject.GetComponent<LanguageToggleBehaviour>();
				component.OnToggleClicked = (LanguageToggleBehaviour.OnToggleClickedDelegate)Delegate.Combine(component.OnToggleClicked, new LanguageToggleBehaviour.OnToggleClickedDelegate(OnLangToggleClicked));
				gameObject.GetComponentInChildren<Text>().text = LocalizationLanguage.GetLanguageString(value);
			}
		}

		private void OnLangToggleClicked(Language language)
		{
			if (OnLanguageChanged != null)
			{
				OnLanguageChanged(language);
			}
		}
	}
}
