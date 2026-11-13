using ClubPenguin.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.TutorialUI
{
	internal class TutorialTooltip : MonoBehaviour
	{
		public GameObject Bubble;

		public GameObject Pointer;

		public Text HeaderText;

		public Text SubHeaderText;

		public Text BodyText;

		public bool AutoDestroy = true;

		public float ScreenPadding = 50f;

		private Color textColor;

		private List<Text> textElements;

		private GameObject defaultTextPrefab;

		private bool hasOpened = false;

		public Color TextColor
		{
			get
			{
				return TextColor;
			}
		}

		private void Awake()
		{
			textElements = new List<Text>();
			if (HeaderText != null)
			{
				textElements.Add(HeaderText);
			}
			if (SubHeaderText != null)
			{
				textElements.Add(SubHeaderText);
			}
			if (BodyText != null)
			{
				textElements.Add(BodyText);
			}
		}

		public void Show()
		{
			GetComponent<Animator>().SetBool("IsOpen", true);
		}

		public void Hide()
		{
			GetComponent<Animator>().SetBool("IsOpen", false);
		}

		public void OnTooltipOpenAnimationComplete()
		{
			hasOpened = true;
		}

		public void OnTooltipCloseAnimationComplete()
		{
			if (AutoDestroy && hasOpened)
			{
				Object.Destroy(base.gameObject);
			}
		}

		public void SetTextColor(Color color)
		{
			textColor = color;
			for (int i = 0; i < textElements.Count; i++)
			{
				textElements[i].color = textColor;
			}
		}

		public void SetDefaultTextPrefab(GameObject prefab)
		{
			defaultTextPrefab = prefab;
		}

		public void ClearAllText()
		{
			for (int i = 0; i < textElements.Count; i++)
			{
				Object.Destroy(textElements[i].gameObject);
			}
		}

		public GameObject AddText(string contents, Color color, int fontSize)
		{
			if (defaultTextPrefab == null)
			{
				return null;
			}
			GameObject gameObject = Object.Instantiate(defaultTextPrefab);
			gameObject.transform.SetParent(Bubble.transform, false);
			Text component = gameObject.GetComponent<Text>();
			component.text = contents;
			component.color = color;
			component.fontSize = fontSize;
			return gameObject;
		}

		public Text GetTextAt(int index)
		{
			if (index < textElements.Count)
			{
				return textElements[index];
			}
			return null;
		}

		public void ReplaceTextAt(int index, Text text)
		{
			if (index < textElements.Count)
			{
				textElements[index] = text;
			}
		}

		public void OnTooltipButtonPressed()
		{
			Hide();
		}

		public void SetPosition(Vector2 position)
		{
			CanvasScalerExt component = GetComponentInParent<Canvas>().GetComponent<CanvasScalerExt>();
			Vector2 vector = new Vector2(component.ReferenceResolutionY / (float)Screen.height, component.ReferenceResolutionY / (float)Screen.height);
			vector *= 1f / component.ScaleModifier;
			GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Clamp(position.x, ScreenPadding, (float)Screen.width - ScreenPadding), position.y);
			float num = Bubble.GetComponent<RectTransform>().sizeDelta.x * 0.5f;
			float x = GetComponent<RectTransform>().anchoredPosition.x;
			float num2 = (float)Screen.width * vector.x - GetComponent<RectTransform>().anchoredPosition.x;
			if (x < num)
			{
				Bubble.GetComponent<RectTransform>().anchoredPosition = new Vector2(num - x, Bubble.GetComponent<RectTransform>().anchoredPosition.y);
			}
			else if (num2 < num)
			{
				Bubble.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f - (num - num2), Bubble.GetComponent<RectTransform>().anchoredPosition.y);
			}
		}
	}
}
