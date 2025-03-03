using ClubPenguin.Adventure;
using ClubPenguin.Breadcrumbs;
using ClubPenguin.ClothingDesigner.Inventory;
using ClubPenguin.Core;
using ClubPenguin.Progression;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class ClothingTemplateSelectionController : TemplateSelectionController
	{
		[SerializeField]
		private PooledCellsScrollRect pooledScrollRect;

		[SerializeField]
		private TemplateIcon templateIconPrefab;

		[SerializeField]
		private CategoryManager categoryManager;

		[SerializeField]
		private GameObject loadingOverlay;

		public PersistentBreadcrumbTypeDefinitionKey BreadcrumbType;

		private List<DisplayedTemplate> rewardTemplates;

		private List<DisplayedTemplate> displayedTemplates;

		private EventChannel eventChannel;

		private ScrollRect scrollRectReference;

		private bool scrollRectDefaultHorizontal;

		private bool scrollRectDefaultVertical;

		private InventoryData inventoryData;

		private int userLevel;

		private string categoryToSelect;

		private ProgressionService progressionService;

		private HashSet<TemplateDefinition> progressionUnlockedRewards;

		private int lastUnlockedIndex = -1;

		private void Awake()
		{
			scrollRectReference = pooledScrollRect.GetComponent<ScrollRect>();
			scrollRectDefaultHorizontal = scrollRectReference.horizontal;
			scrollRectDefaultVertical = scrollRectReference.vertical;
		}

		public void Init(string categoryToSelect)
		{
			this.categoryToSelect = categoryToSelect;
			progressionService = Service.Get<ProgressionService>();
			displayedTemplates = new List<DisplayedTemplate>();
			rewardTemplates = new List<DisplayedTemplate>();
			if (inventoryData == null)
			{
				DataEntityHandle localPlayerHandle = Service.Get<CPDataEntityCollection>().LocalPlayerHandle;
				if (!localPlayerHandle.IsNull)
				{
					inventoryData = Service.Get<CPDataEntityCollection>().GetComponent<InventoryData>(localPlayerHandle);
					if (inventoryData != null)
					{
						parseItemGroups();
						setCategories(inventoryData.CategoryKeys, categoryToSelect);
						setupListeners();
						setupPooledScrollRect();
					}
					else
					{
						Log.LogError(this, "Unable to locate InventoryData object.");
					}
				}
			}
			else
			{
				parseItemGroups();
				setupListeners();
				setupPooledScrollRect();
			}
		}

		private void OnEnable()
		{
			loadingOverlay.SetActive(false);
		}

		private void setCategories(List<EquipmentCategoryDefinitionContentKey> categoryKeys, string categoryToSelect)
		{
			categoryManager.InitButtons(categoryKeys, new ClothingTemplateCategoryManagerEventProxy(), categoryToSelect);
		}

		private void setupListeners()
		{
			eventChannel = new EventChannel(CustomizationContext.EventBus);
			eventChannel.AddListener<ClothingDesignerUIEvents.ShowAllTemplates>(onShowAllTemplates);
			eventChannel.AddListener<ClothingDesignerUIEvents.CategoryChange>(onCategoryChange);
			eventChannel.AddListener<CustomizerUIEvents.SelectTemplate>(onTemplateSelected);
			eventChannel.AddListener<CustomizerUIEvents.EnableScrollRect>(onEnableScrollRect);
			eventChannel.AddListener<CustomizerUIEvents.DisableScrollRect>(onDisableScrollRect);
		}

		private void setupPooledScrollRect()
		{
			if (!string.IsNullOrEmpty(categoryToSelect))
			{
				updateDisplayedTemplatesToCategory(categoryToSelect);
			}
			else
			{
				updateDisplayedTemplatesToAll();
			}
			pooledScrollRect.gameObject.SetActive(true);
			pooledScrollRect.ObjectAdded += onObjectAdded;
			pooledScrollRect.Init(displayedTemplates.Count, templateIconPrefab.gameObject);
		}

		private bool onShowAllTemplates(ClothingDesignerUIEvents.ShowAllTemplates evt)
		{
			updateDisplayedTemplatesToAll();
			refreshList();
			return false;
		}

		private bool onCategoryChange(ClothingDesignerUIEvents.CategoryChange evt)
		{
			string category = evt.Category;
			updateDisplayedTemplatesToCategory(category);
			refreshList();
			return false;
		}

		private void updateDisplayedTemplatesToAll()
		{
			filterDisplayedTemplatesByTags(rewardTemplates);
		}

		private void updateDisplayedTemplatesToCategory(string categoryKey)
		{
			List<DisplayedTemplate> list = new List<DisplayedTemplate>();
			for (int i = 0; i < rewardTemplates.Count; i++)
			{
				if (rewardTemplates[i].Definition.CategoryKey.Key == categoryKey)
				{
					list.Add(rewardTemplates[i]);
				}
			}
			filterDisplayedTemplatesByTags(list);
		}

		private void filterDisplayedTemplatesByTags(List<DisplayedTemplate> filterTemplates)
		{
			displayedTemplates.Clear();
			List<TagDefinition> filterTags = null;
			if (Service.Get<CatalogServiceProxy>().IsCatalogThemeActive())
			{
				CatalogThemeDefinition catalogTheme = Service.Get<CatalogServiceProxy>().GetCatalogTheme();
				if (catalogTheme != null)
				{
					filterTags = catalogTheme.TemplateTags.ToList();
				}
			}
			if (filterTags != null && filterTags.Count > 0)
			{
				for (int i = 0; i < filterTemplates.Count; i++)
				{
					List<TagDefinition> list = filterTemplates[i].Definition.Tags.ToList();
					int tagIndex;
					for (tagIndex = 0; tagIndex < filterTags.Count; tagIndex++)
					{
						if (list.Exists((TagDefinition x) => x.Tag == filterTags[tagIndex].Tag) && !displayedTemplates.Contains(filterTemplates[i]))
						{
							displayedTemplates.Add(filterTemplates[i]);
							break;
						}
					}
				}
			}
			else
			{
				displayedTemplates.AddRange(filterTemplates);
			}
		}

		private void refreshList()
		{
			pooledScrollRect.RefreshList(displayedTemplates.Count);
			pooledScrollRect.ResetContent();
		}

		private void onObjectAdded(RectTransform item, int index)
		{
			TemplateIcon component = item.GetComponent<TemplateIcon>();
			DisplayedTemplate displayedTemplate = displayedTemplates[index];
			component.gameObject.name = displayedTemplate.Definition.AssetName + "_button";
			RectTransform rectTransform = component.transform as RectTransform;
			rectTransform.anchoredPosition = Vector2.zero;
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.sizeDelta = Vector2.zero;
			rectTransform.localScale = Vector3.one;
			bool flag = Service.Get<CPDataEntityCollection>().IsLocalPlayerMember();
			bool canDrag = flag;
			if (!flag)
			{
				canDrag = !displayedTemplate.Definition.IsMemberOnlyCreatable;
			}
			string assetName = displayedTemplate.Definition.AssetName;
			Texture2DContentKey equipmentIconPath = EquipmentPathUtil.GetEquipmentIconPath(assetName);
			component.Init(equipmentIconPath, BreadcrumbType, displayedTemplate.Definition.Id, canDrag);
			if (!flag && displayedTemplate.Definition.IsMemberOnlyCreatable)
			{
				component.SetTemplateMemberLocked(displayedTemplate.Definition);
			}
			else if (userLevel < displayedTemplate.Level)
			{
				component.SetTemplateToLevelLocked(displayedTemplate.Definition, displayedTemplate.Level);
			}
			else if (!string.IsNullOrEmpty(displayedTemplate.MascotName))
			{
				component.SetTemplateToProgressionLocked(displayedTemplate.Definition, displayedTemplate.MascotName);
			}
			else
			{
				component.SetTemplateToUnlocked(displayedTemplate.Definition);
			}
			AccessibilitySettings component2 = component.GetComponent<AccessibilitySettings>();
			if (component2 != null)
			{
				component2.CustomToken = displayedTemplate.Definition.Name;
			}
		}

		private bool onTemplateSelected(CustomizerUIEvents.SelectTemplate evt)
		{
			if (!loadingOverlay.activeSelf)
			{
				loadingOverlay.SetActive(true);
				itemModel.SetTemplate(evt.TemplateData, evt.TemplateSprite);
			}
			return false;
		}

		private bool onEnableScrollRect(CustomizerUIEvents.EnableScrollRect evt)
		{
			scrollRectReference.horizontal = scrollRectDefaultHorizontal;
			scrollRectReference.vertical = scrollRectDefaultVertical;
			return false;
		}

		private bool onDisableScrollRect(CustomizerUIEvents.DisableScrollRect evt)
		{
			scrollRectReference.horizontal = false;
			scrollRectReference.vertical = false;
			return false;
		}

		private void parseItemGroups()
		{
			progressionService = Service.Get<ProgressionService>();
			TemplateDefinition[] unlockedDefinitionsForCategory = progressionService.GetUnlockedDefinitionsForCategory<TemplateDefinition>(ProgressionUnlockCategory.equipmentTemplates);
			progressionUnlockedRewards = new HashSet<TemplateDefinition>(unlockedDefinitionsForCategory);
			userLevel = progressionService.Level;
			for (int i = 0; i <= progressionService.MaxUnlockLevel; i++)
			{
				TemplateDefinition[] array = progressionService.GetUnlockedDefinitionsForLevel(i, ProgressionUnlockCategory.equipmentTemplates).Definitions as TemplateDefinition[];
				if (array == null || array.Length <= 0)
				{
					continue;
				}
				bool isProgressionLocked = i > userLevel;
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j] != null && array[j].IsEditable)
					{
						DisplayedTemplate newDisplayTemplate = new DisplayedTemplate(array[j], i, null);
						addRewardTemplate(newDisplayTemplate, isProgressionLocked);
						if (progressionUnlockedRewards.Contains(array[j]))
						{
							progressionUnlockedRewards.Remove(array[j]);
						}
					}
				}
			}
			QuestService questService = Service.Get<QuestService>();
			Dictionary<string, Mascot> questToMascotMap = questService.QuestToMascotMap;
			foreach (QuestDefinition knownQuest in questService.KnownQuests)
			{
				Mascot value;
				questToMascotMap.TryGetValue(knownQuest.name, out value);
				if (value != null)
				{
					string mascotName = value.Name;
					Quest quest = questService.GetQuest(knownQuest);
					if (knownQuest.StartReward != null)
					{
						if (quest.State == Quest.QuestState.Active || quest.State == Quest.QuestState.Completed || quest.TimesCompleted > 0)
						{
							mascotName = "";
						}
						parseRewardDefinition(AbstractStaticGameDataRewardDefinition<TemplateDefinition>.ToDefinitionArray(knownQuest.StartReward.GetDefinitions<EquipmentTemplateRewardDefinition>()), mascotName);
					}
					if (knownQuest.CompleteReward != null)
					{
						if (quest.State == Quest.QuestState.Completed || quest.TimesCompleted > 0)
						{
							mascotName = "";
						}
						parseRewardDefinition(AbstractStaticGameDataRewardDefinition<TemplateDefinition>.ToDefinitionArray(knownQuest.CompleteReward.GetDefinitions<EquipmentTemplateRewardDefinition>()), mascotName);
					}
					if (knownQuest.ObjectiveRewards != null)
					{
						if (quest.State == Quest.QuestState.Completed || quest.TimesCompleted > 0)
						{
							mascotName = "";
						}
						for (int j = 0; j < knownQuest.ObjectiveRewards.Length; j++)
						{
							parseRewardDefinition(AbstractStaticGameDataRewardDefinition<TemplateDefinition>.ToDefinitionArray(knownQuest.ObjectiveRewards[j].GetDefinitions<EquipmentTemplateRewardDefinition>()), mascotName);
						}
					}
				}
			}
			if (progressionUnlockedRewards.Count <= 0)
			{
				return;
			}
			TemplateDefinition[] array2 = new TemplateDefinition[progressionUnlockedRewards.Count];
			progressionUnlockedRewards.CopyTo(array2);
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j].IsEditable)
				{
					DisplayedTemplate newDisplayTemplate = new DisplayedTemplate(array2[j], -1, null);
					lastUnlockedIndex++;
					rewardTemplates.Insert(lastUnlockedIndex, newDisplayTemplate);
				}
			}
		}

		private void parseRewardDefinition(TemplateDefinition[] unlocks, string mascotName)
		{
			if (unlocks == null || unlocks.Length <= 0)
			{
				return;
			}
			bool isProgressionLocked = false;
			for (int i = 0; i < unlocks.Length; i++)
			{
				if (unlocks[i] != null && unlocks[i].IsEditable)
				{
					if (!progressionService.IsUnlocked(unlocks[i], ProgressionUnlockCategory.equipmentTemplates))
					{
						isProgressionLocked = true;
					}
					DisplayedTemplate newDisplayTemplate = new DisplayedTemplate(unlocks[i], -1, mascotName);
					addRewardTemplate(newDisplayTemplate, isProgressionLocked);
					if (progressionUnlockedRewards.Contains(unlocks[i]))
					{
						progressionUnlockedRewards.Remove(unlocks[i]);
					}
				}
			}
		}

		private void addRewardTemplate(DisplayedTemplate newDisplayTemplate, bool isProgressionLocked)
		{
			if (rewardTemplates.Exists((DisplayedTemplate x) => x.Definition.Id == newDisplayTemplate.Definition.Id))
			{
			}
			if (Service.Get<CPDataEntityCollection>().IsLocalPlayerMember())
			{
				if (isProgressionLocked)
				{
					rewardTemplates.Add(newDisplayTemplate);
					return;
				}
				lastUnlockedIndex++;
				rewardTemplates.Insert(lastUnlockedIndex, newDisplayTemplate);
			}
			else if (newDisplayTemplate.Definition.IsMemberOnlyCreatable)
			{
				rewardTemplates.Add(newDisplayTemplate);
			}
			else
			{
				lastUnlockedIndex++;
				rewardTemplates.Insert(lastUnlockedIndex, newDisplayTemplate);
			}
		}

		private void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
			pooledScrollRect.ObjectAdded -= onObjectAdded;
		}
	}
}
