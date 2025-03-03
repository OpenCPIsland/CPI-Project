using Disney.Kelowna.Common;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Error
{
	[RequireComponent(typeof(Button))]
	public class ErrorPopup : MonoBehaviour
	{
		public static PrefabContentKey DefaultContentKey = new PrefabContentKey("Prefabs/Popups/ErrorPopup");

		private Button button;

		private RectTransform rectTransform;

		private Text errorText;

		private void Awake()
		{
			button = GetComponent<Button>();
			rectTransform = GetComponent<RectTransform>();
			errorText = GetComponentInChildren<Text>();
		}

		private void OnEnable()
		{
			button.onClick.AddListener(buttonPressed);
		}

		private void OnDisable()
		{
			button.onClick.RemoveListener(buttonPressed);
		}

		public void ShowErrorMessage(string message, Vector2 position)
		{
			setErrorText(message);
			rectTransform.position = position;
		}

		public void ShowErrorMessage(string message, ErrorDirection errorDirection, RectTransform targetRT, float centerOffset = 100f)
		{
			setErrorText(message);
			setPositionByErrorPositionEnum(errorDirection, targetRT, centerOffset);
			if (errorDirection != 0)
			{
				setErrorArrow(errorDirection);
			}
		}

		private void setErrorText(string message)
		{
			errorText.text = message;
		}

		private void setPositionByErrorPositionEnum(ErrorDirection errorDirection, RectTransform targetRT, float centerOffset)
		{
			Vector2 v = targetRT.position;
			float num = centerOffset / 100f;
			switch (errorDirection)
			{
			case ErrorDirection.DOWN:
				v.y += (0f - targetRT.rect.height) * num / 2f - rectTransform.rect.height / 2f;
				break;
			case ErrorDirection.RIGHT:
				v.x += targetRT.rect.width * num / 2f + rectTransform.rect.width / 2f;
				break;
			case ErrorDirection.LEFT:
				v.x += (0f - targetRT.rect.width) * num / 2f - rectTransform.rect.width / 2f;
				break;
			case ErrorDirection.UP:
				v.y += targetRT.rect.height * num / 2f + rectTransform.rect.height / 2f;
				break;
			}
			rectTransform.position = v;
		}

		private void setErrorArrow(ErrorDirection errorDirection)
		{
			SetErrorArrow component = GetComponent<SetErrorArrow>();
			if (component != null)
			{
				component.SetArrowByDirection(errorDirection);
				return;
			}
			throw new Exception("The prefab is missing the SetErrorArrow component.");
		}

		private void buttonPressed()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
