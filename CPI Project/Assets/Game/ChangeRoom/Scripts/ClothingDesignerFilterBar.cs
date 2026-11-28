using ClubPenguin.Core;
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
		private Button searchButton;

		[SerializeField]
		private GameObject searchBox;

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
		private string tagListOpenState = "ToolTipOn";

		[SerializeField]
		private int tagListSortingOffset = 100;

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

		[Header("Mobile")]
		[SerializeField]
		[Tooltip("On mobile the bar collapses into a single search button that expands it on tap.")]
		private bool collapseOnMobile = true;

		[SerializeField]
		[Tooltip("Button shown instead of the bar on mobile. Kept inactive in the prefab; only turned on for mobile.")]
		private Button searchToggle;

		[SerializeField]
		[Tooltip("Objects hidden while the bar is expanded on mobile (e.g. the tray title).")]
		private GameObject[] hideWhileExpanded;

		[SerializeField]
		[Tooltip("Space kept free on the right of the expanded bar on mobile (for the edit button).")]
		private float expandedRightInset = 100f;

		public event System.Action FilterChanged;

		private bool compactMode;

		private bool compactExpanded;

		private bool collapsePending;

		private Vector2 collapsedAnchorMin;

		private Vector2 collapsedAnchorMax;

		private Vector2 collapsedAnchoredPosition;

		private Vector2 collapsedSizeDelta;

		private float collapsedToggleWidth;

		private readonly List<GameObject> hiddenWhileExpanded = new List<GameObject>();

		private readonly List<string> tagOptions = new List<string>();

		private readonly List<Button> tagItems = new List<Button>();

		private bool searchWasFocused;

		private bool suppressSearchFocusSound;

		private static int openTagListCount;

		private static int lastTagListCloseFrame = -10;

		private bool tagListOpen;

		private Canvas tagListCanvas;

		private ScrollRect tagListScrollRect;

		private bool tagListSettling;

		private bool mouseScrollBlocked;

		private readonly Dictionary<string, Color> tagColors = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase);

		private Color tagItemDefaultColor = Color.white;

		private bool tagItemDefaultColorRead;

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

		public static bool IsAnyTagListOpenOrJustClosed
		{
			get
			{
				return openTagListCount > 0 || Time.frameCount - lastTagListCloseFrame <= 1;
			}
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
			if (tagList != null)
			{
				tagListCanvas = tagList.GetComponent<Canvas>();
				tagListScrollRect = tagList.GetComponentInChildren<ScrollRect>(true);
			}
			hideTagListImmediate();
			bool mobile = collapseOnMobile && PlatformUtils.GetPlatformType() == PlatformType.Mobile;
			if (searchToggle == null)
			{
				Transform toggleTransform = transform.Find("SearchToggle");
				if (toggleTransform != null)
				{
					searchToggle = toggleTransform.GetComponent<Button>();
				}
			}
			if (searchToggle != null)
			{
				searchToggle.gameObject.SetActive(mobile);
			}
			if (mobile && searchToggle != null)
			{
				setupCompactMode();
			}
			else if (searchButton != null && searchBox != null)
			{
				searchBox.SetActive(false);
				searchButton.onClick.AddListener(toggleSearch);
			}
			if (tagButton != null)
			{
				tagButton.onClick.AddListener(toggleTagList);
			}
			updateTagButtonLabel();
		}

		private void setupCompactMode()
		{
			RectTransform rect = transform as RectTransform;
			if (rect == null || searchInput == null || searchToggle == null)
			{
				return;
			}
			compactMode = true;
			if (searchButton != null)
			{
				searchButton.gameObject.SetActive(false);
			}
			collapsedAnchorMin = rect.anchorMin;
			collapsedAnchorMax = rect.anchorMax;
			collapsedAnchoredPosition = rect.anchoredPosition;
			collapsedSizeDelta = rect.sizeDelta;
			float toggleWidth = LayoutUtility.GetPreferredWidth((RectTransform)searchToggle.transform);
			collapsedToggleWidth = (toggleWidth > 0f) ? toggleWidth : collapsedSizeDelta.y;
			searchToggle.onClick.AddListener(toggleCompactExpanded);
			setCompactExpanded(false);
		}

		private void toggleCompactExpanded()
		{
			bool expand = !compactExpanded;
			setCompactExpanded(expand);
			if (expand && searchInput != null)
			{
				suppressSearchFocusSound = true;
				searchInput.Select();
				searchInput.ActivateInputField();
			}
		}

		private void setCompactExpanded(bool expanded)
		{
			if (!compactMode)
			{
				return;
			}
			compactExpanded = expanded;
			if (!expanded)
			{
				hideTagListImmediate();
				if (searchInput != null)
				{
					searchInput.DeactivateInputField();
				}
			}
			GameObject box = searchBox;
			if (box == null && searchInput != null)
			{
				box = (searchInput.transform.parent != transform) ? searchInput.transform.parent.gameObject : searchInput.gameObject;
			}
			if (box != null)
			{
				box.SetActive(expanded);
			}
			if (tagButton != null)
			{
				tagButton.gameObject.SetActive(expanded);
			}
			RectTransform rect = (RectTransform)transform;
			if (expanded)
			{
				rect.anchorMin = new Vector2(0f, collapsedAnchorMin.y);
				rect.anchorMax = new Vector2(1f, collapsedAnchorMax.y);
				rect.offsetMin = new Vector2(collapsedAnchoredPosition.x, rect.offsetMin.y);
				rect.offsetMax = new Vector2(0f - expandedRightInset, rect.offsetMax.y);
			}
			else
			{
				rect.anchorMin = collapsedAnchorMin;
				rect.anchorMax = collapsedAnchorMax;
				rect.anchoredPosition = collapsedAnchoredPosition;
				rect.sizeDelta = new Vector2(collapsedToggleWidth, collapsedSizeDelta.y);
			}
			setHiddenObjects(expanded);
			LayoutRebuilder.MarkLayoutForRebuild(rect);
		}

		private void setHiddenObjects(bool hide)
		{
			if (hide)
			{
				if (hideWhileExpanded == null)
				{
					return;
				}
				for (int i = 0; i < hideWhileExpanded.Length; i++)
				{
					GameObject target = hideWhileExpanded[i];
					if (target != null && target.activeSelf)
					{
						target.SetActive(false);
						hiddenWhileExpanded.Add(target);
					}
				}
				return;
			}
			for (int i = 0; i < hiddenWhileExpanded.Count; i++)
			{
				if (hiddenWhileExpanded[i] != null)
				{
					hiddenWhileExpanded[i].SetActive(true);
				}
			}
			hiddenWhileExpanded.Clear();
		}

		private void collapseOnOutsidePress()
		{
			if (!compactExpanded || IsActive || tagListOpen)
			{
				return;
			}
			Vector2 position;
			if (!tryGetPressPosition(out position))
			{
				return;
			}
			if (isPointerOver(transform as RectTransform, position))
			{
				return;
			}
			setCompactExpanded(false);
		}

		private void Update()
		{
			if (collapsePending)
			{
				collapsePending = false;
				setCompactExpanded(false);
			}
			collapseOnOutsidePress();
			closeTagListOnOutsidePress();
			deactivateClosedTagList();
			settleTagListScroll();
			if (searchInput == null)
			{
				return;
			}
			bool focused = searchInput.isFocused;
			if (focused && !searchWasFocused)
			{
				if (!suppressSearchFocusSound && searchClickSound != null)
				{
					searchClickSound.onClick.Invoke();
				}
				suppressSearchFocusSound = false;
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
			setMouseScrollBlocked(open);
			if (!open && tagListCanvas != null)
			{
				tagListCanvas.overrideSorting = false;
				tagListCanvas.enabled = false;
			}
			if (!open && tagListScrollRect != null && tagListAnimator != null)
			{
				tagListScrollRect.StopMovement();
				tagListScrollRect.enabled = false;
			}
			if (tagListAnimator == null)
			{
				tagList.SetActive(open);
				applyTagListSorting();
				return;
			}
			if (open && !tagList.activeSelf)
			{
				tagList.SetActive(true);
			}
			applyTagListSorting();
			if (tagList.activeInHierarchy)
			{
				tagListAnimator.SetBool(tagListOpenParameter, open);
			}
			else if (!open)
			{
				tagList.SetActive(false);
			}
		}

		private void scrollTagListToTop(bool updateScrollRect)
		{
			if (tagListContent != null)
			{
				tagListContent.anchoredPosition = Vector2.zero;
			}
			if (updateScrollRect && tagListScrollRect != null)
			{
				tagListScrollRect.StopMovement();
				tagListScrollRect.verticalNormalizedPosition = 1f;
			}
		}

		private void settleTagListScroll()
		{
			if (!tagListSettling)
			{
				return;
			}
			if (!tagListOpen)
			{
				tagListSettling = false;
				if (tagListScrollRect != null)
				{
					tagListScrollRect.enabled = true;
				}
				return;
			}
			bool settled = tagListAnimator == null || (!tagListAnimator.IsInTransition(0) && tagListAnimator.GetCurrentAnimatorStateInfo(0).IsName(tagListOpenState));
			if (tagListScrollRect != null)
			{
				tagListScrollRect.enabled = settled;
			}
			scrollTagListToTop(settled);
			if (settled)
			{
				tagListSettling = false;
			}
		}

		private void applyTagListSorting()
		{
			if (!tagListOpen || tagListCanvas == null || !tagList.activeInHierarchy || tagList.transform.parent == null)
			{
				return;
			}
			Canvas parentCanvas = tagList.transform.parent.GetComponentInParent<Canvas>();
			if (parentCanvas == null)
			{
				return;
			}
			Canvas rootCanvas = parentCanvas.rootCanvas;
			tagListCanvas.enabled = true;
			tagListCanvas.overrideSorting = true;
			tagListCanvas.sortingLayerID = rootCanvas.sortingLayerID;
			tagListCanvas.sortingOrder = rootCanvas.sortingOrder + tagListSortingOffset;
		}

		private void setMouseScrollBlocked(bool blocked)
		{
			if (blocked == mouseScrollBlocked)
			{
				return;
			}
			mouseScrollBlocked = blocked;
			PinchZoomInput.MouseScrollBlockers = Mathf.Max(0, PinchZoomInput.MouseScrollBlockers + (blocked ? 1 : -1));
			openTagListCount = Mathf.Max(0, openTagListCount + (blocked ? 1 : -1));
			if (!blocked)
			{
				lastTagListCloseFrame = Time.frameCount;
			}
		}

		private void toggleSearch()
		{
			if (searchBox == null)
			{
				return;
			}
			bool open = !searchBox.activeSelf;
			searchBox.SetActive(open);
			if (searchInput == null)
			{
				return;
			}
			if (open)
			{
				suppressSearchFocusSound = true;
				searchInput.Select();
				searchInput.ActivateInputField();
			}
			else
			{
				searchInput.text = string.Empty;
			}
		}

		private void hideTagListImmediate()
		{
			tagListOpen = false;
			setMouseScrollBlocked(false);
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
			if (compactExpanded)
			{
				collapsePending = true;
			}
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
			if (open)
			{
				tagListSettling = true;
				settleTagListScroll();
			}
		}

		private void rebuildTagItems()
		{
			if (tagItemTemplate == null || tagListContent == null)
			{
				return;
			}
			if (!tagItemDefaultColorRead && tagItemTemplate.targetGraphic != null)
			{
				tagItemDefaultColor = tagItemTemplate.targetGraphic.color;
				tagItemDefaultColorRead = true;
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
				if (item.targetGraphic != null)
				{
					Color color;
					item.targetGraphic.color = (i > 0 && tagColors.TryGetValue(tagOptions[i - 1], out color)) ? color : tagItemDefaultColor;
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

		public void SetTagColors(IDictionary<string, Color> colors)
		{
			tagColors.Clear();
			if (colors != null)
			{
				foreach (KeyValuePair<string, Color> pair in colors)
				{
					if (!string.IsNullOrEmpty(pair.Key))
					{
						tagColors[pair.Key] = pair.Value;
					}
				}
			}
			if (tagListOpen)
			{
				rebuildTagItems();
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
			setMouseScrollBlocked(false);
			if (searchInput != null)
			{
				searchInput.onValueChanged.RemoveListener(onSearchTextChanged);
			}
			if (tagButton != null)
			{
				tagButton.onClick.RemoveListener(toggleTagList);
			}
			if (searchButton != null)
			{
				searchButton.onClick.RemoveListener(toggleSearch);
			}
			if (searchToggle != null)
			{
				searchToggle.onClick.RemoveListener(toggleCompactExpanded);
			}
			FilterChanged = null;
		}
	}
}
