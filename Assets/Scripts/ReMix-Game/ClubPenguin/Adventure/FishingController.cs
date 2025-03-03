using ClubPenguin.Analytics;
using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.MiniGames;
using ClubPenguin.MiniGames.Fishing;
using ClubPenguin.Net;
using ClubPenguin.Props;
using ClubPenguin.Tutorial;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native.iOS;
using Fabric;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	public class FishingController : MonoBehaviour, ICastFishingRodErrorHandler, IBaseNetworkErrorHandler
	{
		private enum FishingAnimationStates
		{
			Cast,
			ReelFinish,
			CelebrateCatch,
			FailedLook,
			LookAtFish,
			Reel
		}

		public enum FishingGameplayStates
		{
			WaitingForServer,
			Catch,
			Reel,
			Done,
			Paused
		}

		public const int MAX_BAIT_COUNT = 10;

		private const int TORSO_ANIMATION_LAYER = 1;

		private const string MINIGAME_PROGRESS_KEY = "fishing";

		private const float CAST_TIMEOUT = 5f;

		private FishingGameplayStates _gameplayState = FishingGameplayStates.WaitingForServer;

		private static InputButtonContentKey fishingRodContentKey = new InputButtonContentKey("Definitions/ControlsScreen/FishingRod/FishingRod1ButtonDefinition");

		private static InputButtonContentKey bobberContentKey = new InputButtonContentKey("Definitions/ControlsScreen/FishingRod/FishingBobberButtonDefinition");

		[SerializeField]
		private FishingGameConfig config;

		[SerializeField]
		private Transform prizeDropContainer;

		private MinigameService minigameService;

		private EventDispatcher eventDispatcher;

		private QuestService questService;

		private PenguinUserControl userControl;

		private CameraController cinematicCameraFishingController;

		private CameraController cinematicCameraFishingControllerZoom;

		private RailDolly _cameraDolly = null;

		private RailDolly _cameraDollyFocus = null;

		private Vector3 originalCameraPosition;

		public TutorialDefinitionKey TutorialDefinition;

		private bool _isPrizeLoaded = false;

		private bool _isCast = false;

		private bool _isMissDelay = false;

		private bool isQuest;

		private string[] prizeNames = null;

		private PrefabContentKey[] prizeAssetPathKeys = null;

		public Transform rootTransform = null;

		private float _circleT = 0f;

		private Transform _rodPropLineEndTransform = null;

		private Transform _rodPropLineParent = null;

		private readonly int ANIM_DROP = Animator.StringToHash("BobberDrop");

		public float dropSplashFactor = 0.5f;

		private readonly int ANIM_MISS = Animator.StringToHash("BobberMiss");

		public float missSplashFactor = 0.5f;

		private readonly int ANIM_REEL_IN = Animator.StringToHash("BobberReelIn");

		public Transform bobberRootTransform = null;

		private Animator _bobberAnimator = null;

		private readonly int PARAM_IS_ATTACKING = Animator.StringToHash("isAttacking");

		private RaycastHit[] _hits = new RaycastHit[5];

		public FishingFish fishPrefab = null;

		private List<FishingFish> _fishes = new List<FishingFish>();

		private int _reelFishIndex = -1;

		private float _reelValue = 0f;

		private float _innerReelExtreme = 1f;

		public SimpleSpringInterper reelSpring = null;

		public ParticleSystem fxSmallSplash = null;

		public ParticleSystem fxIdle = null;

		public ParticleSystem fxUnderBobber = null;

		public ParticleSystem fxHooked = null;

		public ParticleSystem fxReeling = null;

		public ParticleSystem fxReveal = null;

		private GameObject player;

		private GameObject fishingRod;

		private Animator playerAnimator;

		private Animator fishingRodAnimator;

		private Animator prizeDropContainerAnimator;

		private LocomotionEventBroadcaster locomotionBroadcaster;

		private ClickListener clickListener;

		private ShowFishingRewardPopup rewardPopup;

		private GameObject prizeGameObject;

		private int patternIndex = -1;

		private List<int> sessionIndexList = new List<int>();

		private FishingGamePatternConfig currentPatternConfig
		{
			get
			{
				return config.patterns[patternIndex];
			}
		}

		private void Awake()
		{
			hideGameAssets();
		}

		private void Start()
		{
			init();
			Service.Get<ICPSwrveService>().Action("game.minigame", "fishing");
			Service.Get<ICPSwrveService>().StartTimer("fishing_time", "minigame.fishing");
		}

		private void hideGameAssets()
		{
			fishPrefab.SetPosition(Vector3.down * 100f);
		}

        private void init()
        {
            // Ensure all references are fetched correctly
            getReferences();

            // Check if cinematicCameraFishingController is null
            if (cinematicCameraFishingController == null)
            {
                Debug.LogError("CinematicCameraFishingController is not set.");
                return; // Return early if the camera controller is missing
            }
            originalCameraPosition = cinematicCameraFishingController.transform.position;

            // Check if _rodPropLineEndTransform is null
            if (_rodPropLineEndTransform == null)
            {
                Debug.LogError("RodPropLineEndTransform is not set.");
                return; // Return early if the rod transform is missing
            }
            _rodPropLineParent = _rodPropLineEndTransform.parent;

            // Check if reelSpring is null before using it
            if (reelSpring == null)
            {
                Debug.LogError("ReelSpring is not set.");
                return;
            }
            SimpleSpringInterper simpleSpringInterper = reelSpring;
            simpleSpringInterper.OnSpringValueChanged = (SimpleSpringInterper.SpringValueChangedHandler)Delegate.Combine(simpleSpringInterper.OnSpringValueChanged, new SimpleSpringInterper.SpringValueChangedHandler(OnReelSpringUpdated));

            // Check if locomotionBroadcaster is null
            if (locomotionBroadcaster == null)
            {
                Debug.LogError("LocomotionEventBroadcaster is missing on the player.");
                return;
            }
            locomotionBroadcaster.BroadcastOnControlsLocked();

            // Check if eventDispatcher is null
            if (eventDispatcher == null)
            {
                Debug.LogError("EventDispatcher is not set.");
                return;
            }
            eventDispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("Joystick", true));

            // Check if userControl is null
            if (userControl == null)
            {
                Debug.LogError("PenguinUserControl is missing on the player.");
                return;
            }
            userControl.enabled = true;

            // Continue with other events
            eventDispatcher.DispatchEvent(new ControlsScreenEvents.SetButton(bobberContentKey, 0));

            // Check if player is null before accessing its components
            if (player == null)
            {
                Debug.LogError("Player GameObject is not set.");
                return;
            }

            Quaternion rotation = Quaternion.Euler(config.PlayerRotationTowardsWater);
            player.GetComponent<RunController>().SnapToFacing(rotation * Vector3.forward);

            // Check if prizeDropContainer is null
            if (prizeDropContainer == null)
            {
                Debug.LogError("PrizeDropContainer is not set.");
                return;
            }
            prizeDropContainer.transform.localPosition = config.PrizeDropOffset;

            patternIndex = getRandomPatternIndex();
            setGameplayState(FishingGameplayStates.WaitingForServer, true);

            // Start Coroutine for ExecuteCast()
            CoroutineRunner.Start(ExecuteCast(), this, "ExecuteCast");

            // Fetch fishing game prize
            getFishingGamePrize();

            // Check if clickListener is null
            clickListener = GetComponent<ClickListener>();
            if (clickListener == null)
            {
                Debug.LogError("ClickListener component is missing.");
                return;
            }

            eventDispatcher.DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(false));

            // Platform-specific checks
            if (PlatformUtils.GetPlatformType() != PlatformType.Standalone)
            {
                eventDispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElementGroup("MainNavButtons"));
                eventDispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElement("ChatButtons"));
            }
        }


        private void getReferences()
        {
            // Service references
            minigameService = Service.Get<MinigameService>();
            if (minigameService == null)
            {
                Debug.LogError("MinigameService is not initialized.");
            }

            eventDispatcher = Service.Get<EventDispatcher>();
            if (eventDispatcher == null)
            {
                Debug.LogError("EventDispatcher is not initialized.");
            }

            questService = Service.Get<QuestService>();
            if (questService == null)
            {
                Debug.LogError("QuestService is not initialized.");
            }

            // Player reference
            player = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
            if (player == null)
            {
                Debug.LogError("Player GameObject is not set.");
                return; // If player is null, we can't continue, so return early
            }

            // LocomotionEventBroadcaster reference
            locomotionBroadcaster = player.GetComponent<LocomotionEventBroadcaster>();
            if (locomotionBroadcaster == null)
            {
                Debug.LogError("LocomotionEventBroadcaster is missing on the player.");
            }

            // FishingRod reference
            fishingRod = getChildByNameRecursive(player, "FishingRodProp(Clone)");
            if (fishingRod == null)
            {
                Debug.LogError("FishingRodProp(Clone) child is missing from the player.");
                return; // Return early as we can't proceed without the fishing rod
            }

            // _rodPropLineEndTransform reference (Recursively searching for "string_end_jnt")
            GameObject rodObject = getChildByNameRecursive(fishingRod, "string_end_jnt");
            if (rodObject == null)
            {
                Debug.LogError("string_end_jnt child is missing from the fishing rod.");
                return; // Return early as we can't proceed without the rod's transform
            }
            _rodPropLineEndTransform = rodObject.transform; // Now that we have a valid GameObject, access its Transform

            // PlayerAnimator reference
            playerAnimator = player.GetComponent<Animator>();
            if (playerAnimator == null)
            {
                Debug.LogError("Animator component is missing on the player.");
            }

            // FishingRodAnimator reference
            fishingRodAnimator = fishingRod.GetComponent<Animator>();
            if (fishingRodAnimator == null)
            {
                Debug.LogError("Animator component is missing on the fishing rod.");
            }

            // PrizeDropContainerAnimator reference
            if (prizeDropContainer == null)
            {
                Debug.LogError("PrizeDropContainer is not set.");
                return;
            }
            prizeDropContainerAnimator = prizeDropContainer.GetComponentInChildren<Animator>();
            if (prizeDropContainerAnimator == null)
            {
                Debug.LogError("Animator component is missing in the PrizeDropContainer.");
            }

            // BobberAnimator reference
            if (bobberRootTransform == null)
            {
                Debug.LogError("BobberRootTransform is not set.");
                return;
            }
            _bobberAnimator = bobberRootTransform.GetComponentInChildren<Animator>();
            if (_bobberAnimator == null)
            {
                Debug.LogError("Animator component is missing in the bobber root.");
            }

            // CinematicFishing and CinematicFishingZoom GameObject references
            GameObject cinematicFishingObject = GameObject.Find("CinematicFishing");
            if (cinematicFishingObject == null)
            {
                Debug.LogError("CinematicFishing GameObject is missing from the scene.");
                return;
            }
            cinematicCameraFishingController = cinematicFishingObject.GetComponent<CameraController>();
            if (cinematicCameraFishingController == null)
            {
                Debug.LogError("CameraController component is missing on CinematicFishing.");
                return;
            }

            GameObject cinematicFishingZoomObject = GameObject.Find("CinematicFishingZoom");
            if (cinematicFishingZoomObject == null)
            {
                Debug.LogError("CinematicFishingZoom GameObject is missing from the scene.");
                return;
            }
            cinematicCameraFishingControllerZoom = cinematicFishingZoomObject.GetComponent<CameraController>();
            if (cinematicCameraFishingControllerZoom == null)
            {
                Debug.LogError("CameraController component is missing on CinematicFishingZoom.");
                return;
            }

            // RailDollyGoalPlanner setup
            for (int i = 0; i < cinematicCameraFishingController.GoalPlanners.Length; i++)
            {
                RailDollyGoalPlanner railDollyGoalPlanner = cinematicCameraFishingController.GoalPlanners[i] as RailDollyGoalPlanner;
                if (railDollyGoalPlanner != null)
                {
                    _cameraDolly = railDollyGoalPlanner.Dolly;
                    break;
                }
            }

            // RailDollyFramer setup
            RailDollyFramer railDollyFramer = cinematicCameraFishingController.Framer as RailDollyFramer;
            if (railDollyFramer != null)
            {
                _cameraDollyFocus = railDollyFramer.Dolly;
            }

            // UserControl reference
            userControl = player.GetComponent<PenguinUserControl>();
            if (userControl == null)
            {
                Debug.LogError("PenguinUserControl component is missing on the player.");
            }
        }

        private GameObject getChildByNameRecursive(GameObject parent, string name)
        {
            // Print the parent's name to check the context
            Debug.Log($"Searching for '{name}' in parent: {parent.name}");

            Transform[] children = parent.GetComponentsInChildren<Transform>(true); // Get all child transforms (true to include inactive ones)

            foreach (Transform child in children)
            {
                Debug.Log($"Checking child: {child.name}"); // Log each child's name

                if (child.name == name)
                {
                    Debug.Log($"Found '{name}' under parent '{parent.name}'");
                    return child.gameObject; // Return the GameObject when a match is found
                }
            }

            Debug.Log($"'{name}' not found under parent '{parent.name}'");
            return null; // Return null if no match is found
        }


        private int getRandomPatternIndex()
		{
			int num = 0;
			if (sessionIndexList.Count <= 0)
			{
				for (int i = 0; i < config.patterns.Length - 1; i++)
				{
					sessionIndexList.Add(i);
				}
			}
			num = sessionIndexList[UnityEngine.Random.Range(0, sessionIndexList.Count - 1)];
			sessionIndexList.RemoveAt(num);
			return num;
		}

		private void getFishingGamePrize()
		{
			prizeNames = new string[Enum.GetNames(typeof(FishingFish.Rarities)).Length];
			prizeAssetPathKeys = new PrefabContentKey[prizeNames.Length];
			string currentFishingPrize = questService.CurrentFishingPrize;
			isQuest = !string.IsNullOrEmpty(currentFishingPrize);
			if (isQuest)
			{
				for (int i = 0; i < prizeNames.Length; i++)
				{
					prizeNames[i] = currentFishingPrize;
				}
				setupFishing();
			}
			else
			{
				eventDispatcher.AddListener<MinigameServiceEvents.FishingResultRecieved>(onFishingResultReceived);
				CoroutineRunner.Start(WaitForGamePropBeforeCasting(), this, "WaitForGamePropBeforeCasting");
			}
		}

		private IEnumerator WaitForGamePropBeforeCasting()
		{
			yield return new WaitForSeconds(1f);
			CPDataEntityCollection dataEntityCollection = Service.Get<CPDataEntityCollection>();
			bool isReadyToCast = false;
			if (!dataEntityCollection.LocalPlayerHandle.IsNull)
			{
				DHeldObject heldObject = dataEntityCollection.GetComponent<HeldObjectsData>(dataEntityCollection.LocalPlayerHandle).HeldObject;
				isReadyToCast = (heldObject != null);
			}
			if (isReadyToCast)
			{
				IncrementPlayCount();
				minigameService.CastFishingRod(this);
				yield return new WaitForSeconds(5f);
				if (_gameplayState == FishingGameplayStates.WaitingForServer)
				{
					finishMinigame();
				}
			}
			else
			{
				finishMinigame();
			}
		}

		private void IncrementPlayCount()
		{
			if (isQuest)
			{
				return;
			}
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			MiniGamePlayCountData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				if (!component.MinigamePlayCounts.ContainsKey("fishing"))
				{
					component.SetMinigamePlayCount("fishing", 0);
					return;
				}
				int value = component.MinigamePlayCounts["fishing"] + 1;
				component.MinigamePlayCounts["fishing"] = value;
			}
		}

		private bool onFishingResultReceived(MinigameServiceEvents.FishingResultRecieved evt)
		{
			eventDispatcher.RemoveListener<MinigameServiceEvents.FishingResultRecieved>(onFishingResultReceived);
			if (evt.Result.Data != null && evt.Result.Data.availablePrizeMap != null && evt.Result.Data.availablePrizeMap.Count >= 3)
			{
				prizeNames[0] = evt.Result.Data.availablePrizeMap[FishingFish.Rarities.Common.ToString()];
				prizeNames[1] = evt.Result.Data.availablePrizeMap[FishingFish.Rarities.Rare.ToString()];
				prizeNames[2] = evt.Result.Data.availablePrizeMap[FishingFish.Rarities.Legendary.ToString()];
				setupFishing();
			}
			else
			{
				Log.LogError(this, "Received an empty string after requesting prize name.");
				finishMinigame();
			}
			return false;
		}

		public void onBadFishingState()
		{
			onFishingResultError();
		}

		public void onRequestTimeOut()
		{
			onFishingResultError();
		}

		public void onGeneralNetworkError()
		{
			onFishingResultError();
		}

		private bool onFishingResultError()
		{
			eventDispatcher.RemoveListener<MinigameServiceEvents.FishingResultRecieved>(onFishingResultReceived);
			finishMinigame();
			return false;
		}

		private void setupFishing()
		{
			for (int i = 0; i < prizeNames.Length; i++)
			{
				LootTableRewardDefinition lootRewardDefinition = minigameService.GetLootRewardDefinition(prizeNames[i]);
				if (lootRewardDefinition == null)
				{
					throw new NullReferenceException("Unable to retrieve LootTableRewardDefinitions from the minigames minigame Service.");
				}
				prizeAssetPathKeys[i] = lootRewardDefinition.ModelAsset;
				if (string.IsNullOrEmpty(prizeAssetPathKeys[i].Key))
				{
					Log.LogError(this, "Prize path was empty, exiting out of fishing game.");
					finishMinigame();
					return;
				}
			}
			_isPrizeLoaded = true;
			StartGameplay();
		}

		private void setGameplayState(FishingGameplayStates newState, bool forceChange = false)
		{
			if (_gameplayState != newState || forceChange)
			{
				switch (newState)
				{
				case FishingGameplayStates.Catch:
					fishingRod.GetComponent<PropCancel>().enabled = false;
					break;
				case FishingGameplayStates.Done:
					fishingRod.GetComponent<PropCancel>().enabled = true;
					break;
				}
				_gameplayState = newState;
				eventDispatcher.DispatchEvent(new FishingEvents.FishingGameplayStateChanged(newState));
			}
		}

		private IEnumerator ExecuteCast()
		{
			RemotePlayerVisibilityState.HideRemotePlayers();
			sendCameraEvent(cinematicCameraFishingController);
			cinematicCameraFishingController.transform.position = player.transform.position;
			playerAnimator.SetInteger("PropMode", 3);
			fishingRodAnimator.SetInteger("PropMode", 3);
			yield return new WaitForEndOfFrame();
			float castingAnimationTime = playerAnimator.GetCurrentAnimatorClipInfo(1)[0].clip.length;
			yield return new WaitForSeconds(castingAnimationTime);
			setupStartingAnimatorValues();
			SetBobberHierarchy(true);
			rootTransform.localPosition = config.BobberLocationInWater;
			Vector3 hookPosition = GetFishPosition(currentPatternConfig.hookInterp);
			SetBobberPosition(hookPosition);
			_bobberAnimator.Play(ANIM_DROP);
			yield return new WaitForEndOfFrame();
			float bobberDropAnimationLength = _bobberAnimator.GetCurrentAnimatorStateInfo(0).length;
			yield return new WaitForSeconds(bobberDropAnimationLength * dropSplashFactor);
			fxSmallSplash.Play();
			yield return new WaitForSeconds(bobberDropAnimationLength * (1f - dropSplashFactor));
			_isCast = true;
			StartGameplay();
		}

		private void setupStartingAnimatorValues()
		{
			playerAnimator.SetBool("UseTorsoNull", true);
			playerAnimator.SetBool("ExitTorsoNull", false);
			playerAnimator.SetBool("GameSubStates", true);
			fishingRodAnimator.SetBool("GameSubStates", true);
			setGameModeInAnimators(FishingAnimationStates.Cast);
		}

		private void setGameModeInAnimators(FishingAnimationStates gameModeValue)
		{
			playerAnimator.SetInteger("GameMode", (int)gameModeValue);
			fishingRodAnimator.SetInteger("GameMode", (int)gameModeValue);
		}

		private void SetBobberHierarchy(bool doSet)
		{
			fishingRodAnimator.enabled = !doSet;
			Transform parent = _rodPropLineParent;
			if (doSet)
			{
				parent = _bobberAnimator.transform;
			}
			_rodPropLineEndTransform.SetParent(parent, false);
			if (doSet)
			{
				_rodPropLineEndTransform.localPosition = Vector3.zero;
			}
		}

		private void StartGameplay()
		{
			if (_isCast && _isPrizeLoaded)
			{
				LayoutFish();
				Service.Get<TutorialManager>().TryStartTutorial(TutorialDefinition.Id);
				setGameplayState(FishingGameplayStates.Catch);
				fxIdle.Play();
				eventDispatcher.AddListener<InputEvents.ActionEvent>(onActionEvent);
				eventDispatcher.DispatchEvent(default(FishingEvents.ActivateBobberButton));
				clickListener.OnClicked += onScreenClicked;
			}
			eventDispatcher.DispatchEvent(default(HudEvents.SuppressCoinDisplay));
		}

		private void Update()
		{
			if (_gameplayState == FishingGameplayStates.Catch)
			{
				_circleT += Time.deltaTime;
				for (int i = 0; i < _fishes.Count; i++)
				{
					FishingFish fishingFish = _fishes[i];
					float t = _circleT * fishingFish.speed * config.baseFishSpeed * currentPatternConfig.baseSpeed + fishingFish.offsetT;
					Vector3 fishPosition = GetFishPosition(t);
					fishingFish.SetPosition(fishPosition);
				}
				bool flag = GetFish() != null;
				_bobberAnimator.SetBool(PARAM_IS_ATTACKING, flag);
				if (flag && !fxUnderBobber.isPlaying)
				{
					fxUnderBobber.Play();
					eventDispatcher.DispatchEvent(default(FishingEvents.PulseBobberButton));
					playSoundEffect("SFX/Player/FishingRod/NibbleBubbles");
				}
				else if (!flag && fxUnderBobber.isPlaying)
				{
					fxUnderBobber.Stop();
					eventDispatcher.DispatchEvent(default(FishingEvents.StopBobberButtonPulse));
				}
			}
			else if (_gameplayState == FishingGameplayStates.Reel)
			{
				float num = config.baseFishReelStrength * _fishes[_reelFishIndex].reelStrength;
				_reelValue += Time.deltaTime * num;
				if (_reelValue >= config.reelExtremes.y)
				{
					reelIn(null);
				}
				reelSpring.SetSpringGoal(_reelValue);
			}
		}

		private void LayoutFish()
		{
			int num = currentPatternConfig.fishPatternDatas.Length;
			for (int i = 0; i < num; i++)
			{
				if (!currentPatternConfig.fishPatternDatas[i].isEmpty)
				{
					FishingFish fishingFish = fishPrefab;
					if (_fishes.Count > 0)
					{
						fishingFish = UnityEngine.Object.Instantiate(fishPrefab);
						fishingFish.transform.SetParent(fishPrefab.transform.parent, false);
					}
					float offsetT = (float)i / ((float)num * 1f);
					fishingFish.Init(config, currentPatternConfig.fishPatternDatas[i], offsetT);
					fishingFish.SetActive(true);
					_fishes.Add(fishingFish);
				}
			}
			for (int i = 0; i < _fishes.Count; i++)
			{
				_fishes[i].SetVisible(false, true);
				_fishes[i].SetVisible(true);
			}
		}

		private void SetBobberPosition(Vector3 position)
		{
			bobberRootTransform.position = position;
			if (_reelFishIndex != -1)
			{
				_fishes[_reelFishIndex].SetPosition(position);
			}
		}

		private Vector3 GetFishPosition(float t)
		{
			Vector3 zero = Vector3.zero;
			switch (currentPatternConfig.shape)
			{
			case FishingGamePatternConfig.Shapes.Circle:
			{
				Vector2 circlePosition = FishingMath.GetCirclePosition(t, config.patternRadius);
				zero.x = circlePosition.x;
				zero.z = circlePosition.y;
				break;
			}
			case FishingGamePatternConfig.Shapes.Rose:
			{
				Vector2 rosePosition = FishingMath.GetRosePosition(t, config.patternRadius, currentPatternConfig);
				zero.x = rosePosition.x;
				zero.z = rosePosition.y;
				break;
			}
			}
			return rootTransform.TransformPoint(zero);
		}

		private FishingFish GetFish()
		{
			FishingFish result = null;
			Ray ray = new Ray(bobberRootTransform.position, Vector3.down);
			int num = Physics.SphereCastNonAlloc(ray, config.spherecastRadius, _hits, config.spherecastDepth);
			float num2 = -1f;
			for (int i = 0; i < num; i++)
			{
				FishingFish componentInParent = _hits[i].transform.GetComponentInParent<FishingFish>();
				if (componentInParent != null)
				{
					float sqrMagnitude = (componentInParent.cachedTransform.position - ray.origin).sqrMagnitude;
					if (num2 < 0f || sqrMagnitude < num2)
					{
						num2 = sqrMagnitude;
						result = componentInParent;
					}
				}
			}
			return result;
		}

		private void ScareFish()
		{
			Ray ray = new Ray(bobberRootTransform.position, Vector3.down);
			int num = Physics.SphereCastNonAlloc(ray, config.spherecastRadiusScare, _hits, config.spherecastDepth);
			for (int i = 0; i < num; i++)
			{
				FishingFish componentInParent = _hits[i].transform.GetComponentInParent<FishingFish>();
				if (componentInParent != null)
				{
					componentInParent.Scare();
				}
			}
		}

		private bool onActionEvent(InputEvents.ActionEvent evt)
		{
			switch (evt.Action)
			{
			case InputEvents.Actions.Action1:
				hookAndReel();
				break;
			case InputEvents.Actions.Cancel:
				finishMinigame();
				questService.SendEvent("FishingGameFailed");
				break;
			}
			return false;
		}

		private void onScreenClicked()
		{
			hookAndReel();
		}

		private void hookAndReel()
		{
			if (_gameplayState == FishingGameplayStates.Catch)
			{
				if (!_isMissDelay)
				{
					FishingFish fish = GetFish();
					if (fish != null)
					{
						HookFish(fish);
					}
					else
					{
						CoroutineRunner.Start(ExecuteMiss(), this, "ExecuteMiss");
					}
				}
			}
			else if (_gameplayState == FishingGameplayStates.Reel)
			{
				playSoundEffect("SFX/Player/FishingRod/PullIn");
				_reelValue -= config.perReelStrength;
				if (_reelValue <= _innerReelExtreme)
				{
					reelIn(_fishes[_reelFishIndex]);
				}
				reelSpring.SetSpringGoal(_reelValue);
			}
		}

		private void HookFish(FishingFish fish)
		{
			Service.Get<iOSHapticFeedback>().TriggerImpactFeedback(iOSHapticFeedback.ImpactFeedbackStyle.Medium);
			setGameplayState(FishingGameplayStates.Reel);
			_reelValue = 0f;
			reelSpring.SetSpringGoal(_reelValue, true);
			for (int i = 0; i < _fishes.Count; i++)
			{
				if (fish == _fishes[i])
				{
					_reelFishIndex = i;
					continue;
				}
				_fishes[i].SetVisible(false);
				_fishes[i].SetActive(false);
			}
			Vector3 b = rootTransform.TransformPoint(Vector3.forward * config.reelExtremes.x);
			Vector3 fishPosition = GetFishPosition(currentPatternConfig.hookInterp);
			float magnitude = (fishPosition - b).magnitude;
			_innerReelExtreme = 0f - magnitude;
			CoroutineRunner.Start(loadPrizeAsset(fish.rarity), this, "loadPrizeAsset");
			fxIdle.Stop();
			fxUnderBobber.Stop();
			fxHooked.Play();
			fxReeling.Play();
			playSoundEffect("SFX/Player/FishingRod/PullStartLoop");
			if (_cameraDolly != null && _cameraDollyFocus != null)
			{
				_cameraDolly.Active = false;
				_cameraDollyFocus.Active = false;
			}
		}

		private IEnumerator loadPrizeAsset(FishingFish.Rarities rarity)
		{
			PrefabContentKey prizeAssetPathKey = prizeAssetPathKeys[(int)rarity];
			AssetRequest<GameObject> assetRequestPrize = Content.LoadAsync(prizeAssetPathKey);
			yield return assetRequestPrize;
			if (assetRequestPrize == null)
			{
				Log.LogErrorFormatted(this, "Asset {0} was null, exiting Fishing Game.", prizeAssetPathKey);
				finishMinigame();
			}
			else
			{
				prizeGameObject = UnityEngine.Object.Instantiate(assetRequestPrize.Asset);
				prizeGameObject.SetActive(false);
			}
		}

		private IEnumerator ExecuteMiss()
		{
			ScareFish();
			_isMissDelay = true;
			eventDispatcher.DispatchEvent(default(FishingEvents.DeactivateBobberButton));
			fxSmallSplash.Play();
			_bobberAnimator.Play(ANIM_MISS);
			float animDuration = _bobberAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
			yield return new WaitForSeconds(animDuration * missSplashFactor);
			fxSmallSplash.Play();
			yield return new WaitForSeconds(animDuration * (1f - missSplashFactor));
			eventDispatcher.DispatchEvent(default(FishingEvents.ActivateBobberButton));
			_isMissDelay = false;
			ScareFish();
		}

		private void OnReelSpringUpdated(float interp)
		{
			if (_gameplayState != FishingGameplayStates.Reel)
			{
				return;
			}
			Vector3 b = rootTransform.TransformPoint(Vector3.forward * config.reelExtremes.x);
			Vector3 fishPosition = GetFishPosition(currentPatternConfig.hookInterp);
			Vector3 normalized = (fishPosition - b).normalized;
			Vector3 b2 = normalized * interp;
			Vector3 bobberPosition = fishPosition + b2;
			SetBobberPosition(bobberPosition);
			if (_cameraDolly != null && _cameraDollyFocus != null)
			{
				float num = Mathf.Abs(_innerReelExtreme);
				float t = 1f;
				if (interp < 0f)
				{
					t = Mathf.Clamp01((num - Mathf.Abs(interp)) / num);
				}
				_cameraDolly.Timer = Mathf.Lerp(0f, _cameraDolly.Duration, t);
				_cameraDollyFocus.Timer = Mathf.Lerp(0f, _cameraDollyFocus.Duration, t);
			}
		}

		private void reelIn(FishingFish fish)
		{
			eventDispatcher.RemoveListener<InputEvents.ActionEvent>(onActionEvent);
			eventDispatcher.DispatchEvent(default(FishingEvents.DeactivateBobberButton));
			clickListener.OnClicked -= onScreenClicked;
			userControl.enabled = false;
			setGameplayState(FishingGameplayStates.Done);
			_fishes[_reelFishIndex].SetVisible(false);
			if (fish != null && !isQuest)
			{
				minigameService.CatchFish(prizeNames[(int)fish.rarity]);
				Service.Get<ICPSwrveService>().Action("game.minigame.fishing", "success");
				Service.Get<ICPSwrveService>().Action("game.fishing.reward", prizeNames[(int)fish.rarity]);
			}
			setGameModeInAnimators(FishingAnimationStates.Reel);
			if (fish != null)
			{
				Invoke("pullLineIn", config.ReelingAnimationTime);
				return;
			}
			Invoke("playFailedLookAnimation", 1f);
			EventManager.Instance.PostEvent("SFX/Player/FishingRod/PullStartLoop", EventAction.StopSound, base.gameObject);
			ShowFishingTryAgainPopup showFishingTryAgainPopup = new ShowFishingTryAgainPopup(config.TryAgainPopupShowTime, false);
			showFishingTryAgainPopup.PopupDismissed += onTryAgainPopupDismissed;
			showFishingTryAgainPopup.Init();
			playSoundEffect("SFX/UI/Fishing/GotAwayOut");
			Service.Get<ICPSwrveService>().Action("game.minigame.fishing", "failed");
		}

		private void sendCameraEvent(CameraController cameraController)
		{
			CinematographyEvents.CameraLogicChangeEvent evt = default(CinematographyEvents.CameraLogicChangeEvent);
			evt.Controller = cameraController;
			eventDispatcher.DispatchEvent(evt);
		}

		private void resetCameraEvent()
		{
			CinematographyEvents.CameraLogicResetEvent evt = default(CinematographyEvents.CameraLogicResetEvent);
			evt.Controller = cinematicCameraFishingController;
			eventDispatcher.DispatchEvent(evt);
			CinematographyEvents.CameraLogicResetEvent evt2 = default(CinematographyEvents.CameraLogicResetEvent);
			evt2.Controller = cinematicCameraFishingControllerZoom;
			eventDispatcher.DispatchEvent(evt2);
		}

		private void playFailedLookAnimation()
		{
			SetBobberHierarchy(false);
			fxReeling.Stop();
			setGameModeInAnimators(FishingAnimationStates.FailedLook);
			playSoundEffect("SFX/Player/FishingRod/PullOutFail");
		}

		private void onTryAgainPopupDismissed()
		{
			removeLoadedAssets();
			resetCameraToPenguin();
			if (isQuest)
			{
				questService.SendEvent("FishingGameFailed");
			}
			finishMinigame();
		}

		private void pullLineIn()
		{
			_bobberAnimator.Play(ANIM_REEL_IN);
			fxReeling.Stop();
			fxReveal.Play();
			EventManager.Instance.PostEvent("SFX/Player/FishingRod/PullStartLoop", EventAction.StopSound, base.gameObject);
			setGameModeInAnimators(FishingAnimationStates.ReelFinish);
			SetBobberHierarchy(false);
			Invoke("fishingRodReelingCameraZoom", config.CameraZoomDelay);
		}

		private void fishingRodReelingCameraZoom()
		{
			sendCameraEvent(cinematicCameraFishingControllerZoom);
			eventDispatcher.DispatchEvent(new CinematographyEvents.ChangeCameraTarget(player.transform));
			CinematographyEvents.CameraLogicResetEvent evt = default(CinematographyEvents.CameraLogicResetEvent);
			evt.Controller = cinematicCameraFishingController;
			eventDispatcher.DispatchEvent(evt);
			Invoke("parentPrizeDropToAnimator", config.PrizeDropDelay);
		}

		private void parentPrizeDropToAnimator()
		{
			prizeGameObject.SetActive(true);
			prizeGameObject.transform.SetParent(prizeDropContainerAnimator.transform, false);
			prizeGameObject.transform.localPosition = Vector3.zero;
			prizeGameObject.transform.localEulerAngles = Vector3.zero;
			prizeDropContainerAnimator.SetTrigger("FishingRewardDrop");
			fishingRodAnimator.enabled = true;
			Invoke("celebratePrizeCaughtAnimation", config.CelebrateDelay);
		}

		private void celebratePrizeCaughtAnimation()
		{
			setGameModeInAnimators(FishingAnimationStates.CelebrateCatch);
			Invoke("playHoldAnimation", config.HoldPrizeCaughtDelay);
		}

		private void playHoldAnimation()
		{
			setGameModeInAnimators(FishingAnimationStates.LookAtFish);
			showRewardUI();
		}

		private void showRewardUI()
		{
			eventDispatcher.DispatchEvent(default(HudEvents.UnsuppressCoinDisplay));
			string prizeName = prizeNames[(int)_fishes[_reelFishIndex].rarity];
			rewardPopup = new ShowFishingRewardPopup(prizeName, minigameService, isQuest);
			rewardPopup.PopupDismissed += onPopupClosed;
			rewardPopup.Init();
		}

		private void onPopupClosed()
		{
			rewardPopup.PopupDismissed -= onPopupClosed;
			finishMinigame();
			if (isQuest)
			{
				questService.CurrentFishingPrize = "";
				questService.SendEvent("FishingGameSuccess");
			}
		}

		private void finishMinigame()
		{
			if (fishingRod != null)
			{
				SetBobberHierarchy(false);
				fishingRodAnimator.enabled = true;
			}
			RemotePlayerVisibilityState.ShowRemotePlayers();
			resetCameraToPenguin();
			resetAnimators();
			if (fishingRod != null)
			{
				PropCancel component = fishingRod.GetComponent<PropCancel>();
				if (component != null)
				{
					component.enabled = true;
				}
			}
			eventDispatcher.DispatchEvent(new ControlsScreenEvents.SetButton(fishingRodContentKey, 0));
			eventDispatcher.DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(true));
			eventDispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElementGroup("MainNavButtons"));
			eventDispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("ChatButtons"));
			userControl.enabled = true;
			eventDispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElement("Joystick"));
			locomotionBroadcaster.BroadcastOnControlsUnLocked();
			eventDispatcher.DispatchEvent(default(FishingEvents.FishingGameComplete));
			eventDispatcher.DispatchEvent(default(HudEvents.UnsuppressCoinDisplay));
			CoroutineRunner.StopAllForOwner(this);
			removeLoadedAssets();
			Service.Get<ICPSwrveService>().EndTimer("fishing_time");
			UnityEngine.Object.Destroy(base.gameObject);
			if (fishingRod != null)
			{
				Service.Get<PropService>().LocalPlayerRetrieveProp("FishingRod");
			}
		}

		private void resetAnimators()
		{
			playerAnimator.SetBool("UseTorsoNull", false);
			playerAnimator.SetBool("GameSubStates", false);
			playerAnimator.SetInteger("PropMode", 2);
			if (fishingRod != null)
			{
				fishingRodAnimator.SetInteger("PropMode", 2);
				fishingRodAnimator.SetBool("GameSubStates", false);
			}
		}

		private void resetCameraToPenguin()
		{
			sendCameraEvent(cinematicCameraFishingController);
			cinematicCameraFishingController.transform.position = originalCameraPosition;
			resetCameraEvent();
			eventDispatcher.DispatchEvent(new CinematographyEvents.ChangeCameraTarget(player.transform));
		}

		private void playSoundEffect(string soundEffectEvent)
		{
			EventManager.Instance.PostEvent(soundEffectEvent, EventAction.PlaySound, base.gameObject);
		}

		private void removeLoadedAssets()
		{
			if (prizeGameObject != null)
			{
				UnityEngine.Object.Destroy(prizeGameObject);
			}
		}

		private GameObject getChildByName(GameObject root, string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			foreach (Transform item in root.transform)
			{
				if (item.name.Equals(name))
				{
					return item.gameObject;
				}
				GameObject childByName = getChildByName(item.gameObject, name);
				if (childByName != null)
				{
					return childByName;
				}
			}
			return null;
		}
	}
}
