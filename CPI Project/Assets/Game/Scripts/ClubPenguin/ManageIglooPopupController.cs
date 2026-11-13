using ClubPenguin.Core;
using ClubPenguin.Igloo;
using ClubPenguin.Igloo.UI;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain.Igloo;
using ClubPenguin.Progression;
using ClubPenguin.Tutorial;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using Tweaker.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	internal class ManageIglooPopupController : MonoBehaviour, IIglooDeleteLayoutErrorHandler, IIglooUpdateDataErrorHandler
	{
		private struct IglooSlotProgressionData
		{
			public readonly bool IsLocked;

			public readonly int Level;

			public IglooSlotProgressionData(bool isLocked, int level)
			{
				IsLocked = isLocked;
				Level = level;
			}
		}

		private const int MAX_SLOTS = 10;

		private static PrefabContentKey ManageIglooPopupKey = new PrefabContentKey("UI/ManageIglooPopup/ManageIglooPopup");

		public PrefabContentKey IglooPropertiesCardPrefab;

		public PrefabContentKey CreateNewIglooPrefab;

		public RectTransform MemberSlotContainer;

		public RectTransform MemberActiveIglooBackground;

		public RectTransform NonMemberSlotContainer;

		public RectTransform NonMemberActiveIglooBackground;

		public GameObject LapsedMembershipNotification;

		[SerializeField]
		private LayoutGroup slotContainerLayoutGroup;

		private LayoutElement slotContainerLayoutElement;

		[SerializeField]
		private GameObject inputDisabler;

		private int igloosCount = 0;

		private int createButtonCount = 0;

		private EventDispatcher eventDispatcher;

		private IIglooService iglooService;

		private CPDataEntityCollection dataEntityCollection;

		private SavedIgloosMetaData savedIgloosMetaData;

		private Dictionary<long, IglooPropertiesCard> iglooPropertiesCards;

		private List<IglooCreateButton> iglooCreateButtonList;

		private long? initialActiveIglooId;

		private IglooVisibility? initialIglooVisibility;

		private DataEventListener profileListener;

		private DataEventListener membershipListener;

		private DataEventListener sceneStateListener;

		private DataEventListener savedIgloosListener;

		private ProfileData profileData;

		private MembershipData membershipData;

		private SceneStateData sceneStateData;

		private PrivacyButtonController privacyButton;

		private SceneStateData.SceneState sceneState;

		private bool hasNonMemberIgloo = false;

		private IglooUIStateController stateController;

		private ProgressionService progressionService;

		private List<IglooSlotProgressionData> progressionData;

		private SavedIglooMetaData iglooToDelete;

		private SceneLayoutData newActiveIglooSceneLayoutData;

		[Invokable("Igloo.ShowIglooPopup")]
		public static void ShowManageIglooPopup()
		{
			Content.LoadAsync(onManageIglooPopupLoaded, ManageIglooPopupKey);
		}

		private static void onManageIglooPopupLoaded(string path, GameObject obj)
		{
			GameObject popup = Object.Instantiate(obj);
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(popup));
		}

		public void OnCloseButton()
		{
			privacyButton.PopupWasClosed = true;
			eventDispatcher.DispatchEvent(new IglooUIEvents.CloseManageIglooPopup(newActiveIglooSceneLayoutData));
		}

		public void MarketplaceScreenIntroComplete()
		{
		}

		private void Awake()
		{
			iglooPropertiesCards = new Dictionary<long, IglooPropertiesCard>();
			progressionData = new List<IglooSlotProgressionData>();
			iglooCreateButtonList = new List<IglooCreateButton>();
			eventDispatcher = Service.Get<EventDispatcher>();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			iglooService = Service.Get<INetworkServicesManager>().IglooService;
			progressionService = Service.Get<ProgressionService>();
			stateController = ClubPenguin.Core.SceneRefs.Get<IglooUIStateController>();
			igloosCount = 10;
			createButtonCount = 10;
		}

		private void Start()
		{
			profileListener = dataEntityCollection.When<ProfileData>(dataEntityCollection.LocalPlayerHandle, onProfileData);
			membershipListener = dataEntityCollection.When<MembershipData>(dataEntityCollection.LocalPlayerHandle, onMembershipData);
			sceneStateListener = dataEntityCollection.When<SceneStateData>(Service.Get<SceneLayoutDataManager>().GetActiveHandle(), onSceneStateData);
			savedIgloosListener = dataEntityCollection.When<SavedIgloosMetaData>(dataEntityCollection.LocalPlayerHandle, onSavedIgloosMetaData);
			Service.Get<MembershipService>().OnPurchaseSuccess += onMembershipPurchased;
			eventDispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("Joystick"));
		}

		private void OnDestroy()
		{
			if (eventDispatcher != null)
			{
				eventDispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("Joystick"));
			}
			if (profileListener != null)
			{
				profileListener.StopListening();
			}
			if (savedIgloosListener != null)
			{
				savedIgloosListener.StopListening();
			}
			if (membershipListener != null)
			{
				membershipListener.StopListening();
			}
			if (sceneStateListener != null)
			{
				sceneStateListener.StopListening();
			}
			if (savedIgloosMetaData != null)
			{
				savedIgloosMetaData.SavedIgloosUpdated -= onSavedIgloosUpdated;
				savedIgloosMetaData.PublishedStatusUpdated -= onPublishStatusUpdated;
				savedIgloosMetaData.ActiveIglooIdUpdated -= onActiveIglooIdUpdated;
			}
			if (iglooCreateButtonList != null && iglooCreateButtonList.Count > 0)
			{
				for (int i = 0; i < iglooCreateButtonList.Count; i++)
				{
					iglooCreateButtonList[i].GetComponent<Button>().onClick.RemoveListener(onCreateNewIglooButtonPressed);
				}
			}
			Service.Get<MembershipService>().OnPurchaseSuccess -= onMembershipPurchased;
		}

		private void onProfileData(ProfileData profileData)
		{
			this.profileData = profileData;
			checkForData();
		}

		private void onMembershipData(MembershipData membershipData)
		{
			this.membershipData = membershipData;
			checkForData();
		}

		private void onSceneStateData(SceneStateData sceneStateData)
		{
			this.sceneStateData = sceneStateData;
			checkForData();
		}

		private void onSavedIgloosMetaData(SavedIgloosMetaData savedIgloosMetaData)
		{
			this.savedIgloosMetaData = savedIgloosMetaData;
			this.savedIgloosMetaData.SavedIgloosUpdated += onSavedIgloosUpdated;
			this.savedIgloosMetaData.PublishedStatusUpdated += onPublishStatusUpdated;
			this.savedIgloosMetaData.ActiveIglooIdUpdated += onActiveIglooIdUpdated;
			initialActiveIglooId = savedIgloosMetaData.ActiveIglooId;
			initialIglooVisibility = savedIgloosMetaData.IglooVisibility;
			if (savedIgloosMetaData.SavedIgloos.Count > 0)
			{
				for (int i = 0; i < savedIgloosMetaData.SavedIgloos.Count; i++)
				{
					if (!savedIgloosMetaData.SavedIgloos[i].MemberOnly)
					{
						hasNonMemberIgloo = true;
						break;
					}
				}
			}
			checkForData();
		}

		private void onMembershipPurchased()
		{
			NonMemberSlotContainer.gameObject.SetActive(false);
			LapsedMembershipNotification.SetActive(false);
			for (int i = 0; i < savedIgloosMetaData.SavedIgloos.Count; i++)
			{
				if (!savedIgloosMetaData.SavedIgloos[i].MemberOnly && iglooPropertiesCards.ContainsKey(savedIgloosMetaData.SavedIgloos[i].LayoutId))
				{
					GameObject gameObject = iglooPropertiesCards[savedIgloosMetaData.SavedIgloos[i].LayoutId].gameObject;
					iglooPropertiesCards.Remove(savedIgloosMetaData.SavedIgloos[i].LayoutId);
					Object.Destroy(gameObject);
				}
			}
			setCreateButtonStates();
		}

		private void checkForData()
		{
			if (profileData != null && membershipData != null && sceneStateData != null && savedIgloosMetaData != null)
			{
				init();
			}
		}

		private void init()
		{
			privacyButton = GetComponentInChildren<PrivacyButtonController>();
			getProgressionLockedIglooSlots();
			setContainers();
			if (savedIgloosMetaData.SavedIgloos.Count == 0)
			{
				privacyButton.Setup(base.transform.parent, savedIgloosMetaData, false);
				createButtonCount = 10;
				Content.LoadAsync(delegate(string path, GameObject asset)
				{
					initializeCreateButtons(asset);
				}, CreateNewIglooPrefab);
			}
			else
			{
				Content.LoadAsync(onIglooPropertiesCardLoaded, IglooPropertiesCardPrefab);
			}
		}

		private void setContainers()
		{
			if (!membershipData.IsMember)
			{
				NonMemberSlotContainer.gameObject.SetActive(true);
				MemberSlotContainer.gameObject.SetActive(true);
				if (membershipData.HasHadMembership)
				{
					LapsedMembershipNotification.SetActive(true);
					igloosCount = 4;
				}
			}
			else
			{
				MemberSlotContainer.gameObject.SetActive(true);
				NonMemberSlotContainer.gameObject.SetActive(false);
			}
		}

		private void resetSlots()
		{
			igloosCount = 10;
			createButtonCount = 10;
			iglooPropertiesCards.Clear();
			iglooCreateButtonList.Clear();
			removeSlotsFromContainer(NonMemberSlotContainer.transform, NonMemberActiveIglooBackground.transform);
			removeSlotsFromContainer(MemberSlotContainer.transform, MemberActiveIglooBackground.transform);
			NonMemberSlotContainer.gameObject.SetActive(false);
			MemberSlotContainer.gameObject.SetActive(false);
			LapsedMembershipNotification.SetActive(false);
		}

		private void removeSlotsFromContainer(Transform container, Transform background)
		{
			foreach (Transform item in container.transform)
			{
				if (item != background)
				{
					Object.Destroy(item.gameObject);
				}
			}
		}

		private void onIglooPropertiesCardLoaded(string path, GameObject obj)
		{
			int num = savedIgloosMetaData.SavedIgloos.Count;
			for (int i = 0; i < savedIgloosMetaData.SavedIgloos.Count; i++)
			{
				if (membershipData.IsMember && !savedIgloosMetaData.SavedIgloos[i].MemberOnly)
				{
					num--;
					continue;
				}
				Transform parent = MemberSlotContainer;
				if (!savedIgloosMetaData.SavedIgloos[i].MemberOnly)
				{
					parent = NonMemberSlotContainer.transform;
				}
				GameObject gameObject = Object.Instantiate(obj, parent, false);
				IglooPropertiesCard component = gameObject.GetComponent<IglooPropertiesCard>();
				iglooPropertiesCards.Add(savedIgloosMetaData.SavedIgloos[i].LayoutId, component);
				LotDefinition lotDefinitionFromZoneName = IglooMediator.GetLotDefinitionFromZoneName(savedIgloosMetaData.SavedIgloos[i].LotZoneName);
				if (lotDefinitionFromZoneName != null)
				{
					setIglooPropertiesCard(lotDefinitionFromZoneName, savedIgloosMetaData.SavedIgloos[i]);
				}
			}
			createButtonCount = igloosCount - num;
			if (createButtonCount > 0)
			{
				Content.LoadAsync(delegate(string createButtonPath, GameObject asset)
				{
					initializeCreateButtons(asset);
				}, CreateNewIglooPrefab);
			}
			else
			{
				privacyButton.Setup(base.transform.parent, savedIgloosMetaData, true);
			}
			StartCoroutine(orderIglooCards(false));
		}

		private void setIglooPropertiesCard(LotDefinition lotDefinition, SavedIglooMetaData iglooMetaData)
		{
			if (membershipData.IsMember && !iglooMetaData.MemberOnly)
			{
				return;
			}
			if (!iglooPropertiesCards.ContainsKey(iglooMetaData.LayoutId))
			{
				Log.LogErrorFormatted(this, "Unable to find igloo layout card with layout id of {0}.", iglooMetaData.LayoutId);
				return;
			}
			IglooPropertiesCard.IglooCardState state = IglooPropertiesCard.IglooCardState.InActive;
			if (iglooMetaData.MemberOnly && !membershipData.IsMember)
			{
				state = IglooPropertiesCard.IglooCardState.MemberLocked;
			}
			else if (savedIgloosMetaData.ActiveIglooId.HasValue && iglooMetaData.LayoutId == savedIgloosMetaData.ActiveIglooId.Value)
			{
				state = IglooPropertiesCard.IglooCardState.Active;
			}
			iglooPropertiesCards[iglooMetaData.LayoutId].Init(this, lotDefinition, iglooMetaData, state);
		}

		private void onSavedIgloosUpdated(List<SavedIglooMetaData> data)
		{
			resetSlots();
			init();
		}

		private void onPublishStatusUpdated(IglooVisibility iglooVisibility)
		{
			savedIgloosMetaData.IsDirty = IsSavedIgloosMetaDataChanged();
		}

		private void onActiveIglooIdUpdated(long activeIglooId)
		{
			savedIgloosMetaData.IsDirty = IsSavedIgloosMetaDataChanged();
		}

		private bool IsSavedIgloosMetaDataChanged()
		{
			if (savedIgloosMetaData != null && (initialActiveIglooId != savedIgloosMetaData.ActiveIglooId || initialIglooVisibility != savedIgloosMetaData.IglooVisibility))
			{
				return true;
			}
			return false;
		}

		private void initializeCreateButtons(GameObject createButtonPrefab)
		{
			if (base.gameObject.IsDestroyed())
			{
				return;
			}
			bool enableButton = true;
			for (int i = 0; i < createButtonCount; i++)
			{
				Transform slotContainer = MemberSlotContainer;
				if (!membershipData.IsMember && !hasNonMemberIgloo && i == 0)
				{
					slotContainer = NonMemberSlotContainer;
					enableButton = false;
				}
				IglooCreateButton item = createNewCreateButton(createButtonPrefab, slotContainer);
				iglooCreateButtonList.Add(item);
			}
			setCreateButtonStates();
			checkTutorial();
			privacyButton.Setup(base.transform.parent, savedIgloosMetaData, enableButton);
			eventDispatcher.DispatchEvent(default(IglooUIEvents.ManageIglooPopupDisplayed));
		}

		private void checkTutorial()
		{
			if (savedIgloosMetaData.IsFirstIglooLoad())
			{
				TutorialManager tutorialManager = Service.Get<TutorialManager>();
				if (!tutorialManager.IsTutorialRunning() && tutorialManager.IsTutorialAvailable(IglooButtonUtils.IGLOO_TUTORIAL_ID) && !tutorialManager.TryStartTutorial(IglooButtonUtils.IGLOO_TUTORIAL_ID, "BaseIgloo"))
				{
					Log.LogErrorFormatted(this, "An error occurred trying to start the igloo tutorial.");
				}
			}
		}

		private IglooCreateButton createNewCreateButton(GameObject prefab, Transform slotContainer)
		{
			GameObject gameObject = Object.Instantiate(prefab, slotContainer, false);
			return gameObject.GetComponent<IglooCreateButton>();
		}

		private void setCreateButtonStates()
		{
			for (int i = 0; i < iglooCreateButtonList.Count; i++)
			{
				IglooCreateButton iglooCreateButton = iglooCreateButtonList[i];
				if (!membershipData.IsMember && iglooCreateButton.GetSlotContainer != NonMemberSlotContainer)
				{
					iglooCreateButton.SetState(IglooPropertiesCard.IglooCardState.MemberLocked);
					continue;
				}
				int num = i + iglooPropertiesCards.Count;
				if (num > progressionData.Count)
				{
					Log.LogErrorFormatted(this, "Caluclated index for create igloo buttons was larger than progression count. Index {0}, Count {1}", num, progressionData.Count);
				}
				else if (num == progressionData.Count)
				{
					setupButtonActiveAndClickable(iglooCreateButton);
				}
				else if (membershipData.IsMember && num >= 0 && isProgressionLocked(num))
				{
					iglooCreateButton.SetState(IglooPropertiesCard.IglooCardState.ProgressionLocked);
					iglooCreateButton.SetLockedLevel(progressionData[num].Level);
				}
				else
				{
					setupButtonActiveAndClickable(iglooCreateButton);
				}
			}
		}

		private void setupButtonActiveAndClickable(IglooCreateButton iglooCreateButton)
		{
			Button component = iglooCreateButton.GetComponent<Button>();
			component.onClick.AddListener(onCreateNewIglooButtonPressed);
			iglooCreateButton.SetState(IglooPropertiesCard.IglooCardState.Active);
		}

		private void replacePropertiesButtonWithCreate(GameObject createButtonPrefab, Transform slotContainer)
		{
			if (!base.gameObject.IsDestroyed())
			{
				IglooCreateButton item = createNewCreateButton(createButtonPrefab, slotContainer);
				iglooCreateButtonList.Add(item);
				setCreateButtonStates();
			}
		}

		private bool isProgressionLocked(int progressionDataIndex)
		{
			if (progressionDataIndex >= progressionData.Count)
			{
				return false;
			}
			return progressionData[progressionDataIndex].IsLocked;
		}

		private void onCreateNewIglooButtonPressed()
		{
			eventDispatcher.DispatchEvent(default(IglooUIEvents.CreateIglooButtonPressed));
		}

		private void getProgressionLockedIglooSlots()
		{
			int level = progressionService.Level;
			for (int i = 0; i <= progressionService.MaxUnlockLevel; i++)
			{
				int unlockedCountsForLevel = progressionService.GetUnlockedCountsForLevel(i, ProgressionUnlockCategory.iglooSlots);
				if (unlockedCountsForLevel > 0)
				{
					bool isLocked = i > level;
					IglooSlotProgressionData item = new IglooSlotProgressionData(isLocked, i);
					progressionData.Add(item);
				}
			}
		}

		public void DeleteIglooLayout(long iglooId)
		{
			foreach (SavedIglooMetaData savedIgloo in savedIgloosMetaData.SavedIgloos)
			{
				if (savedIgloo.LayoutId == iglooId)
				{
					iglooToDelete = savedIgloo;
				}
			}
			if (iglooToDelete != null)
			{
				eventDispatcher.AddListener<IglooServiceEvents.IglooLayoutDeleted>(onIglooLayoutDeleted);
				stateController.ShowLoadingModalPopup();
				if (iglooToDelete.LayoutId == initialActiveIglooId)
				{
					stateController.DataManager.UpdateIglooData(onIglooDataUpdated);
				}
				else
				{
					iglooService.DeleteIglooLayout(iglooId);
				}
			}
			else
			{
				Log.LogErrorFormatted(this, "Tried to delete igloo (ID: {0}), but this igloo was not found in the SavedIgloosMetaData!", iglooId);
				stateController.IglooSaveStatusNotification(IglooUIStateController.IglooSaveStatus.GeneralError, false);
			}
		}

		private void onIglooDataUpdated(bool success, SceneLayoutData sceneLayoutData)
		{
			if (success)
			{
				newActiveIglooSceneLayoutData = sceneLayoutData;
				initialActiveIglooId = sceneLayoutData.LayoutId;
				if (iglooToDelete != null)
				{
					iglooService.DeleteIglooLayout(iglooToDelete.LayoutId);
					return;
				}
				stateController.HideLoadingModalPopup();
				stateController.IglooSaveStatusNotification(IglooUIStateController.IglooSaveStatus.GeneralError, false);
			}
			else
			{
				stateController.HideLoadingModalPopup();
				stateController.IglooSaveStatusNotification(IglooUIStateController.IglooSaveStatus.GeneralError, false);
			}
		}

		private bool onIglooLayoutDeleted(IglooServiceEvents.IglooLayoutDeleted evt)
		{
			eventDispatcher.RemoveListener<IglooServiceEvents.IglooLayoutDeleted>(onIglooLayoutDeleted);
			stateController.HideLoadingModalPopup();
			if (newActiveIglooSceneLayoutData != null && newActiveIglooSceneLayoutData.LotZoneName != iglooToDelete.LotZoneName)
			{
				OnCloseButton();
				return false;
			}
			iglooToDelete = null;
			GameObject gameObject = iglooPropertiesCards[evt.LayoutId].gameObject;
			Transform slotContainer = gameObject.transform.parent;
			iglooPropertiesCards.Remove(evt.LayoutId);
			Object.Destroy(gameObject);
			createButtonCount = 1;
			Content.LoadAsync(delegate(string path, GameObject asset)
			{
				replacePropertiesButtonWithCreate(asset, slotContainer);
			}, CreateNewIglooPrefab);
			savedIgloosMetaData.IsDirty = true;
			return false;
		}

		public void OnUpdateDataError()
		{
			stateController.HideLoadingModalPopup();
			stateController.IglooSaveStatusNotification(IglooUIStateController.IglooSaveStatus.GeneralError, false);
		}

		public void OnDeleteLayoutError()
		{
			stateController.HideLoadingModalPopup();
			stateController.IglooSaveStatusNotification(IglooUIStateController.IglooSaveStatus.GeneralError, false);
		}

		public void SetActiveIgloo(long iglooId)
		{
			long? activeIglooId = savedIgloosMetaData.ActiveIglooId;
			newActiveIglooSceneLayoutData = null;
			savedIgloosMetaData.ActiveIglooId = iglooId;
			iglooPropertiesCards[iglooId].SetCardState(IglooPropertiesCard.IglooCardState.Active);
			if (activeIglooId.HasValue)
			{
				if (activeIglooId.Value == iglooId)
				{
					return;
				}
				iglooPropertiesCards[activeIglooId.Value].SetCardState(IglooPropertiesCard.IglooCardState.InActive);
			}
			else
			{
				Log.LogError(this, "There was no Active Igloo! There should always be an Active Igloo at this point!");
			}
			StartCoroutine(orderIglooCards(true));
		}

		private IEnumerator orderIglooCards(bool animated)
		{
			if (!savedIgloosMetaData.ActiveIglooId.HasValue || !iglooPropertiesCards.ContainsKey(savedIgloosMetaData.ActiveIglooId.Value))
			{
				yield break;
			}
			if (animated)
			{
				Transform activeIglooCardTransform = iglooPropertiesCards[savedIgloosMetaData.ActiveIglooId.Value].transform;
				IglooPropertiesCardAnimator[] animators = new IglooPropertiesCardAnimator[igloosCount];
				for (int i = 0; i < animators.Length; i++)
				{
					RectTransform rectTransform = MemberSlotContainer.GetChild(i + 1) as RectTransform;
					animators[i] = rectTransform.GetComponent<IglooPropertiesCardAnimator>();
				}
				inputDisabler.SetActive(true);
				for (int i = 0; i < animators.Length; i++)
				{
					animators[i].gameObject.SetActive(false);
				}
				if (slotContainerLayoutElement == null)
				{
					slotContainerLayoutElement = slotContainerLayoutGroup.gameObject.GetComponent<LayoutElement>();
				}
				slotContainerLayoutElement.preferredWidth = slotContainerLayoutGroup.preferredWidth;
				slotContainerLayoutElement.preferredHeight = slotContainerLayoutGroup.preferredHeight;
				slotContainerLayoutGroup.enabled = false;
				for (int i = 0; i < animators.Length; i++)
				{
					animators[i].gameObject.SetActive(true);
				}
				float animDuration4 = 0f;
				switch (activeIglooCardTransform.GetSiblingIndex())
				{
				case 2:
					animDuration4 = Mathf.Max(animDuration4, animators[0].PlayAnimation(IglooPropertiesCardAnimator.AnimationType.MoveDownFromSlot1));
					animDuration4 = Mathf.Max(animDuration4, animators[1].PlayAnimation(IglooPropertiesCardAnimator.AnimationType.ActivateFromSlot2));
					break;
				case 3:
					animDuration4 = Mathf.Max(animDuration4, animators[0].PlayAnimation(IglooPropertiesCardAnimator.AnimationType.MoveDownFromSlot1));
					animDuration4 = Mathf.Max(animDuration4, animators[1].PlayAnimation(IglooPropertiesCardAnimator.AnimationType.MoveDownFromSlot2));
					animDuration4 = Mathf.Max(animDuration4, animators[2].PlayAnimation(IglooPropertiesCardAnimator.AnimationType.ActivateFromSlot3));
					break;
				}
				yield return new WaitForSeconds(animDuration4);
				activeIglooCardTransform.SetSiblingIndex(1);
				slotContainerLayoutGroup.enabled = true;
				inputDisabler.SetActive(false);
			}
			else
			{
				iglooPropertiesCards[savedIgloosMetaData.ActiveIglooId.Value].transform.SetSiblingIndex(1);
			}
		}
	}
}
