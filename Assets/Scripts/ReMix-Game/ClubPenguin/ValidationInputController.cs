using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Native;
using Fabric;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(InputFieldValidator))]
	public class ValidationInputController : MonoBehaviour
	{
		[Header("Input Style")]
		public Sprite ErrorInput;

		public Sprite ValidInput;

		[Tooltip("Default input only required if calling Reset")]
		public Sprite DefaultInput;

		public bool ShowValidationStyles = true;

		[Header("Sound Effect")]
		public string ErrorInputSfx;

		public string ValidInputSfx;

		[Header("Error/Description Boxes")]
		public GameObject ErrorBox;

		public TextWithEvents errorText;

		public bool hasDescription;

		public GameObject DescriptionBox;

		private InputFieldValidator validator;

		private Image backGroundImage;

		private bool updateLinks;

		private PrefabContentKey suggestionButtonKey = new PrefabContentKey("AccountSystemPrefabs/SuggestionButtonPrefab");

		private static Regex regex = new Regex("<a href=#suggestedName>(.*?)</a>", RegexOptions.Singleline);

		private void Awake()
		{
			validator = GetComponent<InputFieldValidator>();
			if (validator == null)
			{
				throw new Exception("ValidationInputController requires a Validator (InputFieldValidator) script on the same game object");
			}
			backGroundImage = validator.TextInput.GetComponent<Image>();
		}

		private void Update()
		{
			if (validator.TextInput.isFocused)
			{
				if (hasDescription && !DescriptionBox.activeInHierarchy)
				{
					DescriptionBox.SetActive(true);
				}
				if ((MonoSingleton<NativeAccessibilityManager>.Instance.Native.GetAccessibilityLevel() <= 0 || !validator.HasError) && ErrorBox.activeInHierarchy)
				{
					ErrorBox.SetActive(false);
				}
			}
			else if (hasDescription && DescriptionBox.activeInHierarchy)
			{
				DescriptionBox.SetActive(false);
			}
			if (updateLinks && errorText.linksDefined)
			{
				initLinks();
			}
		}

		public void SetError(string errorMessage)
		{
			if (!string.IsNullOrEmpty(ErrorInputSfx))
			{
				EventManager.Instance.PostEvent(ErrorInputSfx, EventAction.PlaySound);
			}
			if (ShowValidationStyles)
			{
				backGroundImage.sprite = ErrorInput;
			}
			ErrorBox.SetActive(true);
			errorMessage = prepareSuggestionButtons(errorMessage);
			errorText.text = errorMessage;
			updateLinks = true;
			MonoSingleton<NativeAccessibilityManager>.Instance.Native.HandleError(errorText.GetInstanceID());
		}

		public void SetValid()
		{
			if (!string.IsNullOrEmpty(ValidInputSfx))
			{
				EventManager.Instance.PostEvent(ValidInputSfx, EventAction.PlaySound);
			}
			if (ShowValidationStyles)
			{
				backGroundImage.sprite = ValidInput;
			}
			errorText.text = "";
			ErrorBox.SetActive(false);
		}

		private string prepareSuggestionButtons(string errorMessage)
		{
			foreach (Transform item in ErrorBox.transform)
			{
				if (item.gameObject.GetComponentInChildren<Button>(true) != null)
				{
					UnityEngine.Object.Destroy(item.gameObject);
				}
			}
			errorMessage = errorMessage.Replace("\"", null);
			errorMessage = errorMessage.Replace("target=_blank ", null);
			Match match = regex.Match(errorMessage);
			while (match.Success)
			{
				string value = match.Groups[1].Value;
				errorMessage = errorMessage.Replace("\n\t<a href=#suggestedName>" + value + "</a>", "");
				initializeSuggestionButton(value);
				match = match.NextMatch();
			}
			return errorMessage;
		}

		public void initializeSuggestionButton(string suggestedName)
		{
			CoroutineRunner.Start(loadButtonFromPrefab(suggestedName), this, "loadButtonFromPrefab");
		}

		private IEnumerator loadButtonFromPrefab(string suggestedName)
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(suggestionButtonKey);
			yield return assetRequest;
			GameObject suggestionGameObject = UnityEngine.Object.Instantiate(assetRequest.Asset);
			Text suggestionButtonText = suggestionGameObject.GetComponentInChildren<Text>();
			suggestionButtonText.text = suggestedName;
			Button suggestionButton = suggestionGameObject.GetComponentInChildren<Button>();
			suggestionButton.onClick.AddListener(delegate
			{
				onSuggestionClicked(suggestedName);
			});
			suggestionGameObject.transform.SetParent(ErrorBox.transform, false);
			yield return null;
		}

		private void initLinks()
		{
			if (errorText.links.Count > 0)
			{
				errorText.AddButtons();
				foreach (string key in errorText.links.Keys)
				{
					int num = 1;
					Transform transform;
					while ((transform = errorText.transform.Find(key + num)) != null)
					{
						string[] linkInfo = errorText.links[key];
						Button component = transform.GetComponent<Button>();
						component.onClick.AddListener(delegate
						{
							onLinkClicked(linkInfo);
						});
						num++;
					}
				}
			}
			updateLinks = false;
		}

		private void onLinkClicked(string[] linkInfo)
		{
			if (linkInfo[0] == "#suggestedName")
			{
				onSuggestionClicked(linkInfo[1]);
			}
		}

		private void onSuggestionClicked(string suggestedName)
		{
			validator.TextInput.text = suggestedName;
			validator.HasError = false;
			validator.IsValidationComplete = true;
			validator.OnValidationSuccess.Invoke();
		}

		public void Reset()
		{
			backGroundImage.sprite = DefaultInput;
			ErrorBox.SetActive(false);
			DescriptionBox.SetActive(false);
		}
	}
}
