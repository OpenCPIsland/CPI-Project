using ClubPenguin.UI;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ClubPenguin.TutorialUI
{
    internal class TutorialTooltipTMP : MonoBehaviour
    {
        public GameObject Bubble;

        public GameObject Pointer;

        public TMP_Text HeaderText;

        public TMP_Text SubHeaderText;

        public TMP_Text BodyText;

        public bool AutoDestroy = true;

        public float ScreenPadding = 50f;

        private Color textColor;

        private List<TMP_Text> textElements;

        private GameObject defaultTextPrefab;

        private bool hasOpened = false;

        public Color TextColor
        {
            get
            {
                return textColor;
            }
        }

        private void Awake()
        {
            textElements = new List<TMP_Text>();

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
                Object.Destroy(gameObject);
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

            textElements.Clear();
        }

        public GameObject AddText(string contents, Color color, float fontSize)
        {
            if (defaultTextPrefab == null)
            {
                return null;
            }

            GameObject gameObject = Object.Instantiate(defaultTextPrefab);
            gameObject.transform.SetParent(Bubble.transform, false);

            TMP_Text component = gameObject.GetComponent<TMP_Text>();
            component.text = contents;
            component.color = color;
            component.fontSize = fontSize;

            textElements.Add(component);

            return gameObject;
        }

        public TMP_Text GetTextAt(int index)
        {
            if (index < textElements.Count)
            {
                return textElements[index];
            }

            return null;
        }

        public void ReplaceTextAt(int index, TMP_Text text)
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

            Vector2 vector = new Vector2(
                component.ReferenceResolutionY / (float)Screen.height,
                component.ReferenceResolutionY / (float)Screen.height);

            vector *= 1f / component.ScaleModifier;

            GetComponent<RectTransform>().anchoredPosition = new Vector2(
                Mathf.Clamp(position.x, ScreenPadding, Screen.width - ScreenPadding),
                position.y);

            float bubbleHalfWidth = Bubble.GetComponent<RectTransform>().sizeDelta.x * 0.5f;
            float leftSpace = GetComponent<RectTransform>().anchoredPosition.x;
            float rightSpace = Screen.width * vector.x - GetComponent<RectTransform>().anchoredPosition.x;

            if (leftSpace < bubbleHalfWidth)
            {
                Bubble.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    bubbleHalfWidth - leftSpace,
                    Bubble.GetComponent<RectTransform>().anchoredPosition.y);
            }
            else if (rightSpace < bubbleHalfWidth)
            {
                Bubble.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    -(bubbleHalfWidth - rightSpace),
                    Bubble.GetComponent<RectTransform>().anchoredPosition.y);
            }
        }
    }
}