using ClubPenguin.Analytics;
using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class CustomGraphicsController : MonoBehaviour
	{
		private enum DisplayMode
		{
			Fullscreen,
			Windowed
		}

		[SerializeField]
		private Toggle antiAliasingToggle;

		[SerializeField]
		private Toggle cameraPostToggle;

		[SerializeField]
		private Toggle graphicsQualityToggleHigh;

		[SerializeField]
		private Toggle graphicsQualityToggleMedium;

		[SerializeField]
		private Toggle graphicsQualityToggleLow;

		[SerializeField]
		private Toggle lodPenguinQualityToggleHigh;

		[SerializeField]
		private Toggle lodPenguinQualityToggleMedium;

		[SerializeField]
		private Toggle lodPenguinQualityToggleLow;

		[SerializeField]
		private Dropdown displayModeMenu;

		[SerializeField]
		private Dropdown aspectRatioMenu;

		[SerializeField]
		private Dropdown resolutionsMenu;

		[SerializeField]
		private Text antiAliasOnText;

		[SerializeField]
		private Text cameraPostOnText;

		private CustomGraphicsService customGraphicsService;

		private readonly string windowedToken = "Settings.Graphics.WindowMode.Windowed";

		private readonly string fullscreenToken = "Settings.Graphics.WindowMode.Fullscreen";

		private Localizer localizer;

		private readonly Dictionary<string, Resolution> AvailableResolutions = new Dictionary<string, Resolution>();

		private bool isInitialized;

		private Dictionary<int, Resolution> displayedResolutionsMap;

		private int initialWidth;

		private int initialHeight;

		private void Start()
		{
			SetTogglesInteractable(true, antiAliasingToggle, cameraPostToggle);
			customGraphicsService = Service.Get<CustomGraphicsService>();
			displayedResolutionsMap = new Dictionary<int, Resolution>();
			initialWidth = Screen.width;
			initialHeight = Screen.height;
			QualityLevel qualityLevel = customGraphicsService.LodPenguinQualityLevel;
			QualityLevel qualityLevel2 = customGraphicsService.GraphicsLevel;
			int num = customGraphicsService.AntiAliasLevel;
			bool isOn = customGraphicsService.CameraPostEnabled;
			graphicsQualityToggleHigh.isOn = (qualityLevel2 == QualityLevel.High);
			graphicsQualityToggleMedium.isOn = (qualityLevel2 == QualityLevel.Medium);
			graphicsQualityToggleLow.isOn = (qualityLevel2 == QualityLevel.Low);
			lodPenguinQualityToggleHigh.isOn = (qualityLevel == QualityLevel.High);
			lodPenguinQualityToggleMedium.isOn = (qualityLevel == QualityLevel.Medium);
			lodPenguinQualityToggleLow.isOn = (qualityLevel == QualityLevel.Low);
			antiAliasingToggle.isOn = (num > 0);
			cameraPostToggle.isOn = isOn;
			SetTogglesInteractable(!graphicsQualityToggleLow.isOn, antiAliasingToggle);
			SetTogglesInteractable(graphicsQualityToggleHigh.isOn, cameraPostToggle);
			localizer = Service.Get<Localizer>();
			Localizer obj = localizer;
			obj.TokensUpdated = (Localizer.TokensUpdatedDelegate)Delegate.Combine(obj.TokensUpdated, new Localizer.TokensUpdatedDelegate(LocalizeDisplayModeMenu));
			LocalizeDisplayModeMenu();
			SetDisplayModeValueFromSettings();
			aspectRatioMenu.ClearOptions();
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, float> supportedAspectRatio in customGraphicsService.SupportedAspectRatios)
			{
				if (anyResolutionMatchesAspectRatio(Screen.resolutions, supportedAspectRatio.Value))
				{
					list.Add(supportedAspectRatio.Key);
				}
			}
			if (list.Count <= 0)
			{
				foreach (KeyValuePair<string, float> supportedAspectRatio2 in customGraphicsService.SupportedAspectRatios)
				{
					list.Add(supportedAspectRatio2.Key);
				}
			}
			aspectRatioMenu.AddOptions(list);
			for (int i = 0; i < aspectRatioMenu.options.Count; i++)
			{
				Dropdown.OptionData optionData = aspectRatioMenu.options[i];
				if (optionData.text == CustomGraphicsService.AspectAlias)
				{
					aspectRatioMenu.value = i;
				}
			}
			aspectRatioMenu.onValueChanged.AddListener(onAspectRatioChanged);
			filterResolutionOptions(Screen.fullScreen);
			string b = nearestFormattedResolution(Screen.width, Screen.height);
			for (int i = 0; i < resolutionsMenu.options.Count; i++)
			{
				Dropdown.OptionData optionData = resolutionsMenu.options[i];
				if (optionData.text == b)
				{
					resolutionsMenu.value = i;
				}
			}
			resolutionsMenu.onValueChanged.AddListener(onResolutionChanged);
			isInitialized = true;
		}

		public void SetGraphicsQuality()
		{
			string text = "";
			if (PlatformUtils.GetPlatformType() == PlatformType.Standalone)
			{
				text = "Standalone_";
			}
			else if (PlatformUtils.GetPlatformType() == PlatformType.Mobile)
			{
				text = "Mobile_";
			}
			else
			{
				string name = Enum.GetName(typeof(string), PlatformUtils.GetPlatformType());
				Log.LogErrorFormatted("Quality setting not defined on Platform {0}", name);
			}
			if (graphicsQualityToggleHigh.isOn)
			{
				antiAliasingToggle.isOn = true;
				cameraPostToggle.isOn = true;
				lodPenguinQualityToggleHigh.isOn = true;
				text += "High";
				SetTogglesInteractable(true, antiAliasingToggle, cameraPostToggle);
				SetPenguinTogglesInteractable(true, lodPenguinQualityToggleHigh, lodPenguinQualityToggleMedium);
			}
			else if (graphicsQualityToggleMedium.isOn)
			{
				antiAliasingToggle.isOn = true;
				cameraPostToggle.isOn = false;
				lodPenguinQualityToggleMedium.isOn = true;
				text += "Medium";
				SetTogglesInteractable(true, antiAliasingToggle);
				SetTogglesInteractable(false, cameraPostToggle);
				SetPenguinTogglesInteractable(true, lodPenguinQualityToggleMedium);
				SetPenguinTogglesInteractable(false, lodPenguinQualityToggleHigh);
			}
			else if (graphicsQualityToggleLow.isOn)
			{
				antiAliasingToggle.isOn = false;
				cameraPostToggle.isOn = false;
				lodPenguinQualityToggleLow.isOn = true;
				text += "Low";
				SetTogglesInteractable(false, antiAliasingToggle, cameraPostToggle);
				SetPenguinTogglesInteractable(false, lodPenguinQualityToggleHigh, lodPenguinQualityToggleMedium);
			}
			if (isInitialized)
			{
				customGraphicsService.SetGraphicsLevel(text);
			}
		}

		private void SetTogglesInteractable(bool value, params Toggle[] toggles)
		{
			foreach (Toggle toggle in toggles)
			{
				toggle.GetComponent<Image>().color = (value ? Color.white : Color.gray);
				toggle.enabled = value;
			}
		}

		private void SetPenguinTogglesInteractable(bool value, params Toggle[] penguinToggles)
		{
			foreach (Toggle toggle in penguinToggles)
			{
				toggle.transform.Find("Overlay").gameObject.SetActive(!value);
				toggle.interactable = value;
			}
		}

		public void SetLodPenguinQualityLevel()
		{
			if (isInitialized)
			{
				if (lodPenguinQualityToggleHigh.isOn)
				{
					customGraphicsService.SetLodPenguinQualityLevel(QualityLevel.High);
				}
				else if (lodPenguinQualityToggleMedium.isOn)
				{
					customGraphicsService.SetLodPenguinQualityLevel(QualityLevel.Medium);
				}
				else if (lodPenguinQualityToggleLow.isOn)
				{
					customGraphicsService.SetLodPenguinQualityLevel(QualityLevel.Low);
				}
			}
		}

		public void ToggleAntiAliasing()
		{
			int num = 0;
			if (antiAliasingToggle.isOn)
			{
				num = (((QualityLevel)customGraphicsService.GraphicsLevel == QualityLevel.High) ? customGraphicsService.AntiAliasSamplesHigh : (((QualityLevel)customGraphicsService.GraphicsLevel != QualityLevel.Medium) ? 2 : customGraphicsService.AntiAliasSamplesMedium));
				antiAliasOnText.enabled = true;
			}
			else
			{
				num = 0;
				antiAliasOnText.enabled = false;
			}
			if (isInitialized)
			{
				customGraphicsService.SetAntialiasing(num);
			}
		}

		public void ToggleCameraPostEffects()
		{
			bool flag = false;
			if (cameraPostToggle.isOn)
			{
				cameraPostOnText.enabled = true;
				flag = true;
			}
			else
			{
				flag = false;
				cameraPostOnText.enabled = false;
			}
			if (isInitialized)
			{
				customGraphicsService.SetCameraPostEffects(flag);
			}
		}

		public void SetFullScreen()
		{
			string text = displayModeMenu.captionText.text;
			bool flag = Screen.fullScreen;
			if (text == localizer.GetTokenTranslation(fullscreenToken))
			{
				flag = true;
			}
			else if (text == localizer.GetTokenTranslation(windowedToken))
			{
				flag = false;
			}
			customGraphicsService.SetFullscreen(flag);
			StartCoroutine(setFullScreenNextFrame(flag));
		}

		private IEnumerator setFullScreenNextFrame(bool fullScreen)
		{
			yield return null;
			filterResolutionOptions(fullScreen);
		}

		private void onAspectRatioChanged(int value)
		{
			setAspectRatio();
			filterResolutionOptions(Screen.fullScreen);
			int num = int.MaxValue;
			int value2 = 0;
			foreach (KeyValuePair<int, Resolution> item in displayedResolutionsMap)
			{
				int num2 = Math.Abs(Screen.height - item.Value.height);
				if (num2 < num)
				{
					num = num2;
					value2 = item.Key;
				}
			}
			resolutionsMenu.value = value2;
		}

		private void setAspectRatio()
		{
			string text = aspectRatioMenu.captionText.text;
			float value;
			if (customGraphicsService.SupportedAspectRatios.TryGetValue(text, out value))
			{
				customGraphicsService.SetAspectRatio(value);
			}
		}

		private void onResolutionChanged(int value)
		{
			string text = resolutionsMenu.captionText.text;
			Resolution value2;
			if (AvailableResolutions.TryGetValue(text, out value2) && (Screen.fullScreen || customGraphicsService.TryFitWindowedScreen(value2.width, value2.height)))
			{
				DisplayResolutionManager.SetResolution(value2, Screen.fullScreen);
			}
		}

		private void filterResolutionOptions(bool fullScreen)
		{
			resolutionsMenu.ClearOptions();
			AvailableResolutions.Clear();
			HashSet<Resolution> hashSet = filterResolutions(Screen.resolutions, CustomGraphicsService.AspectRatio);
			List<string> list = new List<string>();
			if (hashSet.Count <= 0)
			{
				Log.LogError(this, "The 'resolutions' list is EMPTY so the 'Custom settings|Resolution' GUI will be EMPTY!");
			}
			displayedResolutionsMap.Clear();
			int num = 0;
			foreach (Resolution item in hashSet)
			{
				string key = formattedResolution(item);
				Resolution value;
				if (AvailableResolutions.TryGetValue(key, out value))
				{
					if (item.refreshRate > value.refreshRate)
					{
						AvailableResolutions[key] = item;
					}
				}
				else
				{
					AvailableResolutions[key] = item;
				}
			}
			foreach (KeyValuePair<string, Resolution> availableResolution in AvailableResolutions)
			{
				if (fullScreen || (availableResolution.Value.height <= Screen.currentResolution.height && availableResolution.Value.width <= Screen.currentResolution.width))
				{
					list.Add(availableResolution.Key);
					displayedResolutionsMap.Add(num, availableResolution.Value);
					num++;
				}
			}
			if (list.Count <= 0)
			{
				Log.LogError(this, "The 'resOptions' list is EMPTY so the 'Custom settings|Resolution' GUI will be EMPTY!");
			}
			resolutionsMenu.AddOptions(list);
			resolutionsMenu.RefreshShownValue();
		}

		private string formattedResolution(Resolution res)
		{
			return res.width + " x " + res.height;
		}

		private string nearestFormattedResolution(int width, int height)
		{
			int num = 1920;
			int num2 = 1080;
			Resolution[] resolutions = Screen.resolutions;
			for (int i = 0; i < resolutions.Length; i++)
			{
				Resolution resolution = resolutions[i];
				if (Mathf.Abs(resolution.width - width) + Mathf.Abs(resolution.height - height) < Mathf.Abs(resolution.width - num) + Mathf.Abs(resolution.height - num2))
				{
					num = resolution.width;
					num2 = resolution.height;
				}
			}
			return num + " x " + num2;
		}

		private HashSet<Resolution> filterResolutions(Resolution[] resolutions, float aspectRatio)
		{
			HashSet<Resolution> hashSet = new HashSet<Resolution>();
			Resolution[] array = resolutions;
			foreach (Resolution resolution in array)
			{
				if (resolutionMatchesAspectRatio(resolution, aspectRatio))
				{
					hashSet.Add(resolution);
				}
			}
			if (hashSet.Count <= 0)
			{
				array = resolutions;
				foreach (Resolution resolution in array)
				{
					hashSet.Add(resolution);
				}
			}
			return hashSet;
		}

		private bool resolutionMatchesAspectRatio(Resolution resolution, float aspectRatio)
		{
			if (resolution.width >= 640 && resolution.height >= 480)
			{
				float num = (float)resolution.width / (float)resolution.height;
				if (Mathf.Abs(num - aspectRatio) < 0.01f)
				{
					return true;
				}
			}
			return false;
		}

		private bool anyResolutionMatchesAspectRatio(Resolution[] resolutions, float aspectRatio)
		{
			foreach (Resolution resolution in resolutions)
			{
				if (resolutionMatchesAspectRatio(resolution, aspectRatio))
				{
					return true;
				}
			}
			return false;
		}

		private void OnDestroy()
		{
			if ((initialWidth != Screen.width) | (initialHeight != Screen.height))
			{
				string tier = initialWidth + "x" + initialHeight;
				string tier2 = Screen.width + "x" + Screen.height;
				Service.Get<ICPSwrveService>().Action("desktop_display_settings", "resolution_set", tier, tier2);
			}
			Localizer obj = localizer;
			obj.TokensUpdated = (Localizer.TokensUpdatedDelegate)Delegate.Remove(obj.TokensUpdated, new Localizer.TokensUpdatedDelegate(LocalizeDisplayModeMenu));
		}

		private void SetDisplayModeValueFromSettings()
		{
			int value;
			string token;
			if (Screen.fullScreen)
			{
				value = 0;
				token = fullscreenToken;
			}
			else
			{
				value = 1;
				token = windowedToken;
			}
			displayModeMenu.value = value;
			displayModeMenu.captionText.text = localizer.GetTokenTranslation(token);
		}

		private void LocalizeDisplayModeMenu()
		{
			displayModeMenu.options[0].text = localizer.GetTokenTranslation(fullscreenToken);
			displayModeMenu.options[1].text = localizer.GetTokenTranslation(windowedToken);
		}
	}
}
