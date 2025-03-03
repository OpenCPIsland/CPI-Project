using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Disney.Native
{
	public class NativeAccessibility : MonoBehaviour
	{
		private static bool isVoiceOverInEditorOn = false;

		private Transform Holder;

		private GameObject Element;

		private float lastClick = 0f;

		private int lastClickId = 0;

		public event EventHandler<ButtonClickedEventArgs> OnButtonClicked = delegate
		{
		};

		public virtual int GetAccessibilityLevel()
		{
			return isVoiceOverInEditorOn ? 10 : 0;
		}

		public virtual bool IsSwitchControlEnabled()
		{
			return false;
		}

		public virtual bool IsVoiceOverEnabled()
		{
			return isVoiceOverInEditorOn;
		}

		public virtual bool IsDisplayZoomEnabled()
		{
			return false;
		}

		public virtual void RemoveView(int aId)
		{
			if (!(Holder == null))
			{
				Transform transform = Holder.Find("item_" + aId + "_text");
				if (transform == null)
				{
					transform = Holder.Find("item_" + aId + "_button");
				}
				if (transform != null)
				{
					UnityEngine.Object.Destroy(transform.gameObject);
				}
			}
		}

		public virtual void RenderText(int aId, Rect aRect, string aLabel)
		{
			SetupCanvas();
			if (!(Holder.Find("item_" + aId + "_text") != null))
			{
				GameObject newButton = UnityEngine.Object.Instantiate(Element);
				newButton.name = "item_" + aId + "_text";
				newButton.transform.SetParent(Holder, false);
				newButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(aRect.x, 0f - aRect.y);
				newButton.GetComponent<RectTransform>().sizeDelta = new Vector2(aRect.width, aRect.height);
				newButton.GetComponent<Button>().onClick.AddListener(delegate
				{
					OnElementClicked(newButton.GetComponent<Button>(), aLabel, false);
				});
			}
		}

		public virtual void RenderButton(int aId, Rect aRect, string aLabel)
		{
			SetupCanvas();
			if (!(Holder.Find("item_" + aId + "_button") != null))
			{
				GameObject newButton = UnityEngine.Object.Instantiate(Element);
				newButton.name = "item_" + aId + "_button";
				newButton.transform.SetParent(Holder, false);
				newButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(aRect.x, 0f - aRect.y);
				newButton.GetComponent<RectTransform>().sizeDelta = new Vector2(aRect.width, aRect.height);
				newButton.GetComponent<Button>().onClick.AddListener(delegate
				{
					OnElementClicked(newButton.GetComponent<Button>(), aLabel, true);
				});
			}
		}

		public virtual void SelectElement(int aId)
		{
		}

		public virtual void HandleError(int aId)
		{
		}

		public virtual void ClearAllElements()
		{
			if (Holder != null)
			{
				foreach (Transform item in Holder)
				{
					UnityEngine.Object.Destroy(item.gameObject);
				}
			}
		}

		public virtual void UpdateView(int aId, Rect aRect, string aLabel)
		{
			Transform element = Holder.Find("item_" + aId + "_button");
			bool isButton = true;
			if (element == null)
			{
				element = Holder.Find("item_" + aId + "_text");
				isButton = false;
			}
			if (element != null)
			{
				element.GetComponent<RectTransform>().anchoredPosition = new Vector2(aRect.x, 0f - aRect.y);
				element.GetComponent<RectTransform>().sizeDelta = new Vector2(aRect.width, aRect.height);
				element.GetComponent<Button>().onClick.RemoveAllListeners();
				element.GetComponent<Button>().onClick.AddListener(delegate
				{
					OnElementClicked(element.GetComponent<Button>(), aLabel, isButton);
				});
			}
		}

		public virtual void Speak(string aTextToSpeak)
		{
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				try
				{
					Process process = new Process();
					process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
					process.StartInfo.CreateNoWindow = true;
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.FileName = "say";
					process.StartInfo.Arguments = aTextToSpeak;
					process.EnableRaisingEvents = true;
					process.Start();
				}
				catch (Exception)
				{
				}
			}
		}

		private void SetupCanvas()
		{
			if (!(GameObject.Find("AccessibilityHolder") != null))
			{
				GameObject original = Resources.Load<GameObject>("AccessibilityHolder");
				GameObject gameObject = UnityEngine.Object.Instantiate(original);
				gameObject.name = "AccessibilityHolder";
				Holder = gameObject.transform.Find("Canvas");
				Element = Resources.Load<GameObject>("AccessibilityElement");
			}
		}

		private void OnElementClicked(Button element, string label, bool isButton)
		{
			int num = int.Parse(element.name.Replace("item_", "").Replace("_button", "").Replace("_text", ""));
			if (lastClickId == num)
			{
				if (lastClick + 1f > Time.time)
				{
					if (isButton)
					{
						ButtonClicked(string.Concat(num));
					}
					lastClickId = 0;
					lastClick = 0f;
					return;
				}
				lastClick = Time.time;
			}
			else
			{
				lastClickId = num;
				lastClick = Time.time;
			}
			Speak(label);
		}

		public virtual void ButtonClicked(string aId)
		{
			this.OnButtonClicked(this, new ButtonClickedEventArgs(int.Parse(aId)));
		}
	}
}
