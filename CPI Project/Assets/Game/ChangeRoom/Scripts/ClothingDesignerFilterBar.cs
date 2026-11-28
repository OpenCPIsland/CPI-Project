using DevonLocalization.Core;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ClubPenguin.ClothingDesigner
{
	public class ClothingDesignerFilterBar : MonoBehaviour
	{
		[SerializeField]
		private TMP_InputField searchInput;

		[SerializeField]
		private Button searchClickSound;

		[SerializeField]
		private Button tagButton;

		[SerializeField]
		private TMP_Text tagButtonLabel;

		[SerializeField]
		private GameObject tagList;

		[SerializeField]
		private Animator tagListAnimator;

		[SerializeField]
		private string tagListOpenParameter = "IsOpen";

		[SerializeField]
		private string tagListClosedState = "ToolTipOff";

		[SerializeField]
		private RectTransform tagListContent;

		[SerializeField]
		private Button tagItemTemplate;

		[SerializeField]
		private float tagItemHeight = 80f;

		[SerializeField]
		private float tagItemSpacing = 20f;

		[SerializeField]
		private string searchPlaceholder = "Search";

		[SerializeField]
		private string allTagsLabel = "All Tags";

		[SerializeField]
		private int characterLimit = 40;

		public event System.Action FilterChanged;

		private readonly List<string> tagOptions = new List<string>();

		private readonly List<Button> tagItems = new List<Button>();

		private bool searchWasFocused;

		private bool tagListOpen;

		private readonly Dictionary<TemplateDefinition, string> nameCache = new Dictionary<TemplateDefinition, string>();

		public string SearchText
		{
			get;
			private set;
		}

		public string SelectedTag
		{
			get;
			private set;
		}

		public bool IsActive
		{
			get
			{
				return !string.IsNullOrEmpty(SearchText) || !string.IsNullOrEmpty(SelectedTag);
			}
		}

		private void Awake()
		{
			SearchText = string.Empty;
			SelectedTag = null;
			if (searchInput != null)
			{
				searchInput.characterLimit = characterLimit;
				TMP_Text placeholder = searchInput.placeholder as TMP_Text;
				if (placeholder != null)
				{
					placeholder.text = searchPlaceholder;
				}
				searchInput.onValueChanged.AddListener(onSearchTextChanged);
			}
			if (tagItemTemplate != null)
			{
				tagItemTemplate.gameObject.SetActive(false);
			}
			if (tagListAnimator != null)
			{
				tagListAnimator.fireEvents = false;
			}
			hideTagListImmediate();
			if (tagButton != null)
			{
				tagButton.onClick.AddListener(toggleTagList);
			}
			updateTagButtonLabel();
		}

		private void Update()
		{
			closeTagListOnOutsidePress();
			deactivateClosedTagList();
			if (searchInput == null)
			{
				return;
			}
			bool focused = searchInput.isFocused;
			if (focused && !searchWasFocused && searchClickSound != null)
			{
				searchClickSound.onClick.Invoke();
			}
			searchWasFocused = focused;
		}

		private void closeTagListOnOutsidePress()
		{
			if (tagList == null || !tagListOpen)
			{
				return;
			}
			Vector2 position;
			if (!tryGetPressPosition(out position))
			{
				return;
			}
			if (isPointerOver(tagList.transform as RectTransform, position) || (tagButton != null && isPointerOver(tagButton.transform as RectTransform, position)))
			{
				return;
			}
			setTagListOpen(false);
		}

		private void setTagListOpen(bool open)
		{
			if (tagList == null)
			{
				return;
			}
			tagListOpen = open;
			if (tagListAnimator == null)
			{
				tagList.SetActive(open);
				return;
			}
			if (open && !tagList.activeSelf)
			{
				tagList.SetActive(true);
			}
			if (tagList.activeInHierarchy)
			{
				tagListAnimator.SetBool(tagListOpenParameter, open);
			}
			else if (!open)
			{
				tagList.SetActive(false);
			}
		}

		private void hideTagListImmediate()
		{
			tagListOpen = false;
			if (tagList != null)
			{
				tagList.SetActive(false);
			}
		}

		private void deactivateClosedTagList()
		{
			if (tagListOpen || tagList == null || tagListAnimator == null || !tagList.activeSelf || tagListAnimator.IsInTransition(0))
			{
				return;
			}
			if (tagListAnimator.GetCurrentAnimatorStateInfo(0).IsName(tagListClosedState))
			{
				tagList.SetActive(false);
			}
		}

		private static bool tryGetPressPosition(out Vector2 position)
		{
			position = Vector2.zero;
			if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
			{
				position = Mouse.current.position.ReadValue();
				return true;
			}
			if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
			{
				position = Touchscreen.current.primaryTouch.position.ReadValue();
				return true;
			}
			return false;
		}

		private static bool isPointerOver(RectTransform rect, Vector2 position)
		{
			if (rect == null)
			{
				return false;
			}
			Canvas canvas = rect.GetComponentInParent<Canvas>();
			Camera eventCamera = null;
			if (canvas != null && canvas.rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
			{
				eventCamera = canvas.rootCanvas.worldCamera;
			}
			return RectTransformUtility.RectangleContainsScreenPoint(rect, position, eventCamera);
		}

		private void OnDisable()
		{
			searchWasFocused = false;
			hideTagListImmediate();
		}

		private void toggleTagList()
		{
			if (tagList == null)
			{
				return;
			}
			bool open = !tagListOpen;
			if (open)
			{
				rebuildTagItems();
			}
			setTagListOpen(open);
		}

		private void rebuildTagItems()
		{
			if (tagItemTemplate == null || tagListContent == null)
			{
				return;
			}
			int count = tagOptions.Count + 1;
			while (tagItems.Count < count)
			{
				Button created = UnityEngine.Object.Instantiate(tagItemTemplate, tagListContent, false);
				created.gameObject.SetActive(true);
				int itemIndex = tagItems.Count;
				created.onClick.AddListener(delegate
				{
					onTagItemClicked(itemIndex);
				});
				tagItems.Add(created);
			}
			for (int i = 0; i < tagItems.Count; i++)
			{
				Button item = tagItems[i];
				bool used = i < count;
				item.gameObject.SetActive(used);
				if (!used)
				{
					continue;
				}
				TMP_Text label = item.GetComponentInChildren<TMP_Text>(true);
				if (label != null)
				{
					label.text = (i == 0) ? allTagsLabel : tagOptions[i - 1];
				}
			}
			tagListContent.sizeDelta = new Vector2(tagListContent.sizeDelta.x, count * tagItemHeight + Mathf.Max(0, count - 1) * tagItemSpacing);
			tagListContent.anchoredPosition = Vector2.zero;
		}

		private void onTagItemClicked(int index)
		{
			StartCoroutine(selectTagNextFrame(index));
		}

		private IEnumerator selectTagNextFrame(int index)
		{
			yield return null;
			onTagSelected(index);
		}

		private void updateTagButtonLabel()
		{
			if (tagButtonLabel != null)
			{
				tagButtonLabel.text = string.IsNullOrEmpty(SelectedTag) ? allTagsLabel : SelectedTag;
			}
		}

		public void SetAvailableTags(IEnumerable<string> tags)
		{
			SortedSet<string> sorted = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
			if (tags != null)
			{
				foreach (string tag in tags)
				{
					if (!string.IsNullOrEmpty(tag))
					{
						sorted.Add(tag);
					}
				}
			}
			tagOptions.Clear();
			tagOptions.AddRange(sorted);
			string previous = SelectedTag;
			int index = string.IsNullOrEmpty(previous) ? -1 : tagOptions.IndexOf(previous);
			string current = (index >= 0) ? previous : null;
			if (current != SelectedTag)
			{
				SelectedTag = current;
				notifyChanged();
			}
			updateTagButtonLabel();
			if (tagListOpen)
			{
				rebuildTagItems();
			}
		}

		public static IEnumerable<string> CollectTags(IEnumerable<TemplateDefinition> definitions)
		{
			HashSet<string> tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (TemplateDefinition definition in definitions)
			{
				if (definition == null || definition.Tags == null)
				{
					continue;
				}
				for (int i = 0; i < definition.Tags.Length; i++)
				{
					if (definition.Tags[i] != null && !string.IsNullOrEmpty(definition.Tags[i].Tag))
					{
						tags.Add(definition.Tags[i].Tag);
					}
				}
			}
			return tags;
		}

		public bool Matches(TemplateDefinition definition)
		{
			if (!IsActive)
			{
				return true;
			}
			if (definition == null)
			{
				return false;
			}
			if (!string.IsNullOrEmpty(SelectedTag) && !hasTag(definition, SelectedTag))
			{
				return false;
			}
			if (string.IsNullOrEmpty(SearchText))
			{
				return true;
			}
			if (getDisplayName(definition).IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) > -1)
			{
				return true;
			}
			if (!string.IsNullOrEmpty(definition.AssetName) && definition.AssetName.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) > -1)
			{
				return true;
			}
			if (definition.Tags != null)
			{
				for (int i = 0; i < definition.Tags.Length; i++)
				{
					if (definition.Tags[i] != null && !string.IsNullOrEmpty(definition.Tags[i].Tag) && definition.Tags[i].Tag.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) > -1)
					{
						return true;
					}
				}
			}
			return false;
		}

		private static bool hasTag(TemplateDefinition definition, string tag)
		{
			if (definition.Tags == null)
			{
				return false;
			}
			for (int i = 0; i < definition.Tags.Length; i++)
			{
				if (definition.Tags[i] != null && string.Equals(definition.Tags[i].Tag, tag, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private string getDisplayName(TemplateDefinition definition)
		{
			string value;
			if (nameCache.TryGetValue(definition, out value))
			{
				return value;
			}
			value = string.Empty;
			if (!string.IsNullOrEmpty(definition.Name))
			{
				value = Service.Get<Localizer>().GetTokenTranslation(definition.Name);
			}
			if (string.IsNullOrEmpty(value))
			{
				value = definition.AssetName ?? string.Empty;
			}
			nameCache[definition] = value;
			return value;
		}

		private void onSearchTextChanged(string value)
		{
			string trimmed = (value ?? string.Empty).Trim();
			if (trimmed != SearchText)
			{
				SearchText = trimmed;
				notifyChanged();
			}
		}

		private void onTagSelected(int index)
		{
			string tag = (index > 0 && index - 1 < tagOptions.Count) ? tagOptions[index - 1] : null;
			setTagListOpen(false);
			if (tag != SelectedTag)
			{
				SelectedTag = tag;
				updateTagButtonLabel();
				notifyChanged();
			}
		}

		private void notifyChanged()
		{
			if (FilterChanged != null)
			{
				FilterChanged();
			}
		}

		private void OnDestroy()
		{
			if (searchInput != null)
			{
				searchInput.onValueChanged.RemoveListener(onSearchTextChanged);
			}
			if (tagButton != null)
			{
				tagButton.onClick.RemoveListener(toggleTagList);
			}
			FilterChanged = null;
		}
	}
}
