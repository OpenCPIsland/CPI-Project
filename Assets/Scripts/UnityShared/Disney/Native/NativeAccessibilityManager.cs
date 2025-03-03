using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Disney.Native
{
	public class NativeAccessibilityManager : MonoSingleton<NativeAccessibilityManager>
	{
		private List<GameObject> PriorityGameObjects = new List<GameObject>();

		public List<GameObject> ScrollContentComponents = new List<GameObject>();

		public List<AccessibilitySettings> HiddenScrollItemIds = new List<AccessibilitySettings>();

		private ParseButton buttonParser;

		private ParseInput inputParser;

		private ParseToggle toggleParser;

		private ParseText textParser;

		private ParseImage imageParser;

		private ParseSlider sliderParser;

		private GameObject servicesGameObject;

		private NativeAccessibility native;

		public bool IsEnabled
		{
			get;
			private set;
		}

		public NativeAccessibilityLevel AccessibilityLevel
		{
			get;
			private set;
		}

		public NativeAccessibility Native
		{
			get
			{
				return native;
			}
			private set
			{
				native = value;
			}
		}

		public event EventHandler<ToggleAccessibilitiesEventArgs> OnToggleAccessibilities = delegate
		{
		};

		public void AddPriorityGameObject(GameObject aGameObject)
		{
			PriorityGameObjects.Remove(aGameObject);
			PriorityGameObjects.Add(aGameObject);
		}

		public void RemovePriorityGameObject(GameObject aGameObject)
		{
			PriorityGameObjects.Remove(aGameObject);
		}

		public void Awake()
		{
			AccessibilityLevel = NativeAccessibilityLevel.NONE;
			native = base.gameObject.AddComponent<NativeIOSAccessibility>();
		}

		private void OnApplicationPause(bool aIsGoingToBackground)
		{
		}

		public void Init(IAccessibilityLocalization aLocalization)
		{
			buttonParser = new ParseButton(aLocalization);
			inputParser = new ParseInput(aLocalization);
			toggleParser = new ParseToggle(aLocalization);
			textParser = new ParseText(aLocalization);
			imageParser = new ParseImage(aLocalization);
			sliderParser = new ParseSlider(aLocalization);
			base.gameObject.name = "AccessibilityManager";
			InvokeRepeating("CheckSwitchControlEnabled", 0.1f, 1f);
		}

		public void CheckSwitchControlEnabled()
		{
			AccessibilityLevel = (NativeAccessibilityLevel)MonoSingleton<NativeAccessibilityManager>.Instance.Native.GetAccessibilityLevel();
			if (AccessibilityLevel != 0)
			{
				MonoSingleton<NativeAccessibilityManager>.Instance.Enable();
			}
			else
			{
				MonoSingleton<NativeAccessibilityManager>.Instance.Disable();
			}
		}

		public void Enable()
		{
			if (!IsEnabled)
			{
				IsEnabled = true;
				Clear();
				Native.OnButtonClicked += HandleOnButtonClicked;
				this.OnToggleAccessibilities(this, new ToggleAccessibilitiesEventArgs(true));
				if (!IsInvoking("ParseScene"))
				{
					InvokeRepeating("ParseScene", 0.1f, 1f);
				}
			}
		}

		public void Disable()
		{
			if (IsEnabled)
			{
				IsEnabled = false;
				Clear();
				Native.OnButtonClicked -= HandleOnButtonClicked;
				this.OnToggleAccessibilities(this, new ToggleAccessibilitiesEventArgs(false));
				if (IsInvoking("ParseScene"))
				{
					CancelInvoke("ParseScene");
				}
			}
		}

		public void Clear()
		{
			buttonParser.Clear();
			inputParser.Clear();
			toggleParser.Clear();
			textParser.Clear();
			imageParser.Clear();
			sliderParser.Clear();
			Native.ClearAllElements();
		}

		public void ParseHiddenScrollItems()
		{
			HiddenScrollItemIds = new List<AccessibilitySettings>();
			foreach (GameObject scrollContentComponent in MonoSingleton<NativeAccessibilityManager>.Instance.ScrollContentComponents)
			{
				if (scrollContentComponent != null)
				{
					ScrollContentAccessibilitySettings component = scrollContentComponent.GetComponent<ScrollContentAccessibilitySettings>();
					if (component != null)
					{
						ScrollRect referenceScrollRect = component.ReferenceScrollRect;
						if (referenceScrollRect != null && referenceScrollRect.viewport != null)
						{
							Rect rectInScreenSpace = Util.GetRectInScreenSpace(referenceScrollRect.viewport);
							foreach (Transform item in component.gameObject.transform)
							{
								RectTransform component2 = item.gameObject.GetComponent<RectTransform>();
								if (component2 != null)
								{
									Rect rectInScreenSpace2 = Util.GetRectInScreenSpace(component2);
									if (!rectInScreenSpace.Contains(new Vector2(rectInScreenSpace2.x + rectInScreenSpace2.width / 2f, rectInScreenSpace2.y + rectInScreenSpace2.height / 2f)))
									{
										HiddenScrollItemIds.Add(item.GetComponent<AccessibilitySettings>());
									}
								}
							}
						}
					}
				}
			}
		}

		private void ParseScene()
		{
			if (this == null || Equals(null) || base.gameObject == null)
			{
				return;
			}
			GameObject[] gameObjects;
			if (PriorityGameObjects.Count > 0)
			{
				gameObjects = new GameObject[1]
				{
					PriorityGameObjects[PriorityGameObjects.Count - 1]
				};
			}
			else
			{
				List<GameObject> list = new List<GameObject>();
				GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
				GameObject[] array = rootGameObjects;
				foreach (GameObject item in array)
				{
					list.Add(item);
				}
				if (servicesGameObject == null)
				{
					servicesGameObject = GameObject.Find("Services");
				}
				list.Add(servicesGameObject);
				gameObjects = list.ToArray();
			}
			ParseHiddenScrollItems();
			if (AccessibilityLevel == NativeAccessibilityLevel.VOICE)
			{
				textParser.Parse(gameObjects);
				imageParser.Parse(gameObjects);
			}
			buttonParser.Parse(gameObjects);
			inputParser.Parse(gameObjects);
			toggleParser.Parse(gameObjects);
			sliderParser.Parse(gameObjects);
		}

		private void HandleOnButtonClicked(object sender, ButtonClickedEventArgs args)
		{
			buttonParser.Click(args.Id);
			inputParser.Click(args.Id);
			toggleParser.Click(args.Id);
			textParser.Click(args.Id);
			imageParser.Click(args.Id);
			sliderParser.Click(args.Id);
		}
	}
}
