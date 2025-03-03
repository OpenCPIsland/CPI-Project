using ClubPenguin.Analytics;
using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.Diving;
using ClubPenguin.Locomotion;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.Tutorial;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using System;
using System.Collections;
using System.Collections.Generic;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin.World.Activities.Diving
{
	public class DivingGameController : MonoBehaviour
	{
		public enum DivingState
		{
			Invalid,
			HoldingBreath,
			PlentifulAir
		}

		public enum BubbleState
		{
			None,
			Good,
			TransitionToGood,
			Warning,
			TransitionToWarning,
			Danger,
			TransitionToDanger,
			Hidden,
			TransitionToHidden,
			TransitionToPlentifulAir
		}

		public const float MAX_AIR_SUPPLY = 10f;

		public const float MIN_AIR_SUPPLY = 0f;

		public const float UPDATE_RATE = 0.01f;

		public const float LOWAIR_PULSE_RATE = 0.5f;

		private const float DIVING_TUTORIAL_BOOST_DEPTH = 15f;

		private static Color PLENTIFUL_AIR_BUBBLE_COLOR = new Color32(8, 196, 77, byte.MaxValue);

		public float AirSupply = 10f;

		public float DegradeRate = 0.04f;

		public float AirThreshold = 5f;

		public float Depth;

		public DivingState State;

		[Tweakable("Avatar.DivingUnlimitedAir", Description = "This turns on unlimited air for BOTH local and remote players on your screen.")]
		[PublicTweak]
		public static bool DivingUnlimitedAir = false;

		private SwimControllerData mutableData;

		private Material bubbleMaterial;

		private float warningThreshold;

		private float dangerThreshold;

		private Color targetColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		private SwimController swimController;

		private GameObject bubble;

		private Vector3 translate;

		private float surface;

		private EventDispatcher dispatcher;

		private CPDataEntityCollection dataEntityCollection;

		private AirBubbleData localPlayerAirBubbleData;

		private static readonly PrefabContentKey resurfacePrefabKey = new PrefabContentKey("FX/UI/Prefabs/DivingOutOfAirTransition");

		private static readonly PrefabContentKey shareBubbleContentKey = new PrefabContentKey("Diving/ShareBubble");

		private bool isLocalPlayer;

		private bool hasCheckedTutorial = false;

		private HashSet<DivingGameController> overlappingBubbles = new HashSet<DivingGameController>();

		public BubbleState bubbleState = BubbleState.None;

		private static TutorialDefinitionKey FindingAirTutorialDefinition = new TutorialDefinitionKey(4);

		private static TutorialDefinitionKey BoostingTutorialDefinition = new TutorialDefinitionKey(7);

		private bool hasLoggedNoAirBI = false;

		private void Start()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			determineIfLocalPlayer();
			if (isLocalPlayer)
			{
				DataEntityHandle localPlayerHandle = dataEntityCollection.LocalPlayerHandle;
				if (dataEntityCollection.TryGetComponent(localPlayerHandle, out localPlayerAirBubbleData))
				{
					AirSupply = localPlayerAirBubbleData.AirBubble.value;
				}
				else
				{
					localPlayerAirBubbleData = dataEntityCollection.AddComponent<AirBubbleData>(localPlayerHandle);
					AirBubble airBubble = new AirBubble();
					airBubble.value = 10f;
					localPlayerAirBubbleData.AirBubble = airBubble;
				}
				if (isLocalPenguinSpawned())
				{
					initializeDiving();
				}
				else
				{
					dispatcher.AddListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onLocalPlayerSpawned);
				}
			}
			else
			{
				initializeDiving();
			}
		}

		public void SetData(SwimControllerData data)
		{
			if ((bool)data && !data.IsDestroyed)
			{
				mutableData = UnityEngine.Object.Instantiate(data);
			}
		}

		private bool isLocalPenguinSpawned()
		{
			DataEntityHandle localPlayerHandle = dataEntityCollection.LocalPlayerHandle;
			bool result = false;
			GameObjectReferenceData component;
			if (!localPlayerHandle.IsNull && dataEntityCollection.TryGetComponent(localPlayerHandle, out component) && component.GameObject != null)
			{
				AvatarView component2 = component.GameObject.GetComponent<AvatarView>();
				if (component2 != null && component2.IsReady)
				{
					result = true;
				}
			}
			return result;
		}

		private bool onLocalPlayerSpawned(PlayerSpawnedEvents.LocalPlayerSpawned evt)
		{
			dispatcher.RemoveListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onLocalPlayerSpawned);
			initializeDiving();
			return false;
		}

		private void initializeDiving()
		{
			surface = mutableData.ResurfaceTransform.position.y;
			State = DivingState.HoldingBreath;
			Content.LoadAsync(onBubbleloaded, shareBubbleContentKey);
			if (isLocalPlayer)
			{
				dispatcher.DispatchEvent(default(DivingEvents.ShowHud));
				dispatcher.AddListener<InputEvents.ActionEvent>(OnAction);
				dispatcher.AddListener<DivingEvents.EnableLocalInfiniteAir>(onEnableLocalInfiniteAir);
				dispatcher.AddListener<DivingEvents.DisableLocalInfiniteAir>(onDisableLocalInfiniteAir);
			}
		}

		private void determineIfLocalPlayer()
		{
			AvatarDataHandle component = GetComponent<AvatarDataHandle>();
			SessionIdData component2;
			if (component != null && !component.Handle.IsNull && dataEntityCollection.TryGetComponent(component.Handle, out component2))
			{
				isLocalPlayer = dataEntityCollection.IsLocalPlayer(component2.SessionId);
			}
		}

		public void RemoveLocalPlayerAirBubbleData()
		{
			if (isLocalPlayer)
			{
				DataEntityHandle localPlayerHandle = dataEntityCollection.LocalPlayerHandle;
				dataEntityCollection.RemoveComponent<AirBubbleData>(localPlayerHandle);
			}
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			UnityEngine.Object.Destroy(bubble);
			UnityEngine.Object.Destroy(bubbleMaterial);
			bubble = null;
			dispatcher.RemoveListener<InputEvents.ActionEvent>(OnAction);
			dispatcher.RemoveListener<DivingEvents.EnableLocalInfiniteAir>(onEnableLocalInfiniteAir);
			dispatcher.RemoveListener<DivingEvents.DisableLocalInfiniteAir>(onDisableLocalInfiniteAir);
			if (isLocalPlayer)
			{
				Service.Get<INetworkServicesManager>().PlayerStateService.RemoveAirBubble();
			}
			else
			{
				AvatarDataHandle component = GetComponent<AvatarDataHandle>();
				if (component != null && !component.Handle.IsNull)
				{
					AirBubbleData component2 = dataEntityCollection.GetComponent<AirBubbleData>(component.Handle);
					if (component2 != null)
					{
						component2.AirBubbleChanged -= OnRemotePlayerAirBubbleChanged;
					}
				}
			}
			foreach (DivingGameController overlappingBubble in overlappingBubbles)
			{
				if (overlappingBubble != null)
				{
					overlappingBubble.RemotePlayerRemoved(this);
				}
			}
		}

		private void onBubbleloaded(string path, GameObject prefab)
		{
			if (this != null && !base.gameObject.IsDestroyed())
			{
				CoroutineRunner.Start(InitializeBubbleCoroutine(path, prefab), this, "InitializeBubbleCoroutine");
			}
		}

		private IEnumerator InitializeBubbleCoroutine(string path, GameObject prefab)
		{
			bubble = UnityEngine.Object.Instantiate(prefab);
			bubble.SetActive(false);
			Transform jnt = null;
			while (jnt == null)
			{
				jnt = base.transform.Find("hips_jnt/backbone_jnt/chest_jnt/neck_jnt");
				yield return new WaitForEndOfFrame();
			}
			CameraFacingController cameraFacingController = bubble.GetComponent<CameraFacingController>();
			if (cameraFacingController != null)
			{
				cameraFacingController.AttachPoint = jnt;
				bubble.transform.SetParent(base.gameObject.transform, false);
			}
			else
			{
				bubble.transform.SetParent(jnt.transform, false);
				bubble.transform.position = jnt.transform.position;
			}
			bubble.SetActive(true);
			swimController = GetComponent<SwimController>();
			bubbleMaterial = bubble.GetComponentInChildren<MeshRenderer>().material;
			float totalAirQuantity = 10f - AirThreshold;
			warningThreshold = AirThreshold + totalAirQuantity * 0.4f;
			dangerThreshold = AirThreshold + totalAirQuantity * 0.2f;
			bubbleState = getStartingBubbleState(AirSupply);
			updateBubbleState();
			if (isLocalPlayer)
			{
				CoroutineRunner.Start(UpdateLocalPlayerBubbleCoRoutine(), this, "UpdateLocalPlayerBubbleCoRoutine");
				CoroutineRunner.Start(SyncAirSupplyNetwork(), this, "SyncAirSupplyNetwork");
				yield break;
			}
			DataEntityHandle handle = GetComponent<AvatarDataHandle>().Handle;
			if (!handle.IsNull)
			{
				AirBubbleData component = dataEntityCollection.GetComponent<AirBubbleData>(handle);
				if (component != null)
				{
					dataEntityCollection.GetComponent<AirBubbleData>(handle).AirBubbleChanged += OnRemotePlayerAirBubbleChanged;
				}
				else
				{
					Log.LogError(this, "Failed to get the air bubble data for the remote player");
				}
			}
		}

		private BubbleState getStartingBubbleState(float airSupply)
		{
			if (airSupply < dangerThreshold)
			{
				return BubbleState.TransitionToDanger;
			}
			if (airSupply < warningThreshold)
			{
				return BubbleState.TransitionToWarning;
			}
			return BubbleState.TransitionToGood;
		}

		private IEnumerator UpdateLocalPlayerBubbleCoRoutine()
		{
			while (bubble != null && !base.gameObject.IsDestroyed() && base.isActiveAndEnabled)
			{
				updateAirSupplyLocalPlayer();
				updateDepth();
				yield return new WaitForSeconds(0.01f);
			}
		}

		private IEnumerator SyncAirSupplyNetwork()
		{
			while (bubble != null && !base.gameObject.IsDestroyed() && base.isActiveAndEnabled)
			{
				Service.Get<INetworkServicesManager>().PlayerStateService.SetAirBubble(AirSupply, (int)State);
				yield return new WaitForSeconds(1f);
			}
		}

		private void pulseBubble(float delay)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("amount", new Vector3(0.15f, 0.15f, 0.15f));
			hashtable.Add("delay", delay);
			hashtable.Add("time", 0.5f);
			iTween.PunchScale(bubble.gameObject, hashtable);
		}

		private void turnPlentifulAreaTriggerLightsOn(FreeAirZone freeAirZone, bool isOn)
		{
			if (freeAirZone.LightsOn != null && freeAirZone.LightsOff != null)
			{
				for (int i = 0; i < freeAirZone.LightsOn.Count; i++)
				{
					freeAirZone.LightsOn[i].SetActive(isOn);
					freeAirZone.LightsOff[i].SetActive(!isOn);
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (isLocalPlayer)
			{
				if (other.gameObject != base.gameObject && !other.transform.IsChildOf(base.transform) && (bool)other.gameObject.GetComponent<FreeAirZone>())
				{
					handleEnteringFreeAirZone(other);
				}
				else if (other.gameObject.CompareTag("TutorialTrigger") && Service.Get<TutorialManager>().TryStartTutorial(FindingAirTutorialDefinition.Id))
				{
					State = DivingState.PlentifulAir;
				}
			}
			ShareBubble component = other.gameObject.GetComponent<ShareBubble>();
			if (component != null && !other.transform.IsChildOf(base.transform))
			{
				DivingGameController componentInParent = other.gameObject.GetComponentInParent<DivingGameController>();
				if (componentInParent != null)
				{
					overlappingBubbles.Add(componentInParent);
				}
			}
		}

		private void OnTriggerStay(Collider other)
		{
			if (isLocalPlayer && other.gameObject != base.gameObject && !other.transform.IsChildOf(base.transform) && (bool)other.gameObject.GetComponent<FreeAirZone>())
			{
				handleEnteringFreeAirZone(other);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (isLocalPlayer && other.gameObject != base.gameObject && !other.transform.IsChildOf(base.transform) && (bool)other.gameObject.GetComponent<FreeAirZone>())
			{
				turnPlentifulAreaTriggerLightsOn(other.gameObject.GetComponent<FreeAirZone>(), false);
				setBubbleColor(new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue));
				State = DivingState.HoldingBreath;
				logEnterNoAirBI();
			}
			ShareBubble component = other.gameObject.GetComponent<ShareBubble>();
			if (component != null && !other.transform.IsChildOf(base.transform))
			{
				DivingGameController componentInParent = other.gameObject.GetComponentInParent<DivingGameController>();
				if (componentInParent != null)
				{
					overlappingBubbles.Remove(componentInParent);
				}
			}
		}

		private void logEnterNoAirBI()
		{
			if (!hasLoggedNoAirBI)
			{
				Service.Get<ICPSwrveService>().Action("game.room_milestone", "dive_cave_no_air_zone");
				hasLoggedNoAirBI = true;
			}
		}

		private void handleEnteringFreeAirZone(Collider zoneCollider)
		{
			if (!targetColor.Equals(PLENTIFUL_AIR_BUBBLE_COLOR))
			{
				turnPlentifulAreaTriggerLightsOn(zoneCollider.gameObject.GetComponent<FreeAirZone>(), true);
				bubbleState = BubbleState.TransitionToPlentifulAir;
				updateBubbleState();
				State = DivingState.PlentifulAir;
			}
		}

		private void onTutorialComplete(TutorialDefinition definition)
		{
			TutorialManager tutorialManager = Service.Get<TutorialManager>();
			tutorialManager.TutorialCompleteAction = (Action<TutorialDefinition>)Delegate.Remove(tutorialManager.TutorialCompleteAction, new Action<TutorialDefinition>(onTutorialComplete));
			State = DivingState.HoldingBreath;
		}

		private void updateBubbleState()
		{
			switch (bubbleState)
			{
			case BubbleState.None:
				break;
			case BubbleState.Hidden:
				break;
			case BubbleState.Good:
				if (AirSupply < warningThreshold)
				{
					bubbleState = BubbleState.TransitionToWarning;
				}
				break;
			case BubbleState.TransitionToGood:
				setBubbleColor(new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue));
				bubbleState = BubbleState.Good;
				playAudioEvent(mutableData.AirCriticalAudioEvent, EventAction.StopSound);
				break;
			case BubbleState.TransitionToPlentifulAir:
				setBubbleColor(PLENTIFUL_AIR_BUBBLE_COLOR);
				bubbleState = BubbleState.Good;
				playAudioEvent(mutableData.AirCriticalAudioEvent, EventAction.StopSound);
				break;
			case BubbleState.Warning:
				if (AirSupply < dangerThreshold)
				{
					bubbleState = BubbleState.TransitionToDanger;
				}
				else if (AirSupply > warningThreshold)
				{
					bubbleState = BubbleState.TransitionToGood;
				}
				break;
			case BubbleState.TransitionToWarning:
				setBubbleColor(new Color32(byte.MaxValue, 162, 0, byte.MaxValue));
				pulseBubble(0f);
				bubbleState = BubbleState.Warning;
				playAudioEvent(mutableData.AirWarningAudioEvent);
				break;
			case BubbleState.Danger:
				if (AirSupply > dangerThreshold)
				{
					bubbleState = BubbleState.TransitionToWarning;
				}
				break;
			case BubbleState.TransitionToDanger:
				setBubbleColor(new Color32(byte.MaxValue, 0, 0, byte.MaxValue));
				pulseBubble(0f);
				bubbleState = BubbleState.Danger;
				playAudioEvent(mutableData.AirCriticalAudioEvent);
				break;
			case BubbleState.TransitionToHidden:
				dispatcher.DispatchEvent(new DivingEvents.AirBubbleBurstEffects(base.gameObject.tag));
				setBubbleColor(new Color32(0, 0, 0, 0));
				bubbleState = BubbleState.Hidden;
				playAudioEvent(mutableData.AirCriticalAudioEvent, EventAction.StopSound);
				break;
			}
		}

		private void updateAirSupplyLocalPlayer()
		{
			lerpAirSupply(0.01f);
			if (AirSupply < AirThreshold && !swimController.TriggerResurface && bubbleState != BubbleState.Hidden)
			{
				swimController.TriggerResurface = true;
				bubbleState = BubbleState.TransitionToHidden;
				if (swimController.CurState != SwimController.State.Resurfacing)
				{
					playAudioEvent(mutableData.AirCriticalAudioEvent, EventAction.StopSound);
					Invoke("startBubbleScreenWipe", 2f);
				}
			}
			dispatcher.DispatchEvent(new DivingEvents.AirSupplyUpdated(AirSupply));
		}

		private void setBubbleColor(Color col)
		{
			targetColor = col;
		}

		private void updateColor()
		{
			if (bubbleMaterial != null)
			{
				Color color = bubbleMaterial.color;
				color = Color.Lerp(color, targetColor, Time.deltaTime * 50f);
				bubbleMaterial.color = color;
			}
		}

		private void lerpAirSupply(float theDeltaTime)
		{
			if (State == DivingState.HoldingBreath)
			{
				if (!DivingUnlimitedAir)
				{
					AirSupply = Mathf.Lerp(AirSupply, 0f, theDeltaTime * DegradeRate);
				}
			}
			else if (State == DivingState.PlentifulAir)
			{
				AirSupply = Mathf.Lerp(AirSupply, 10f, theDeltaTime);
			}
			localPlayerAirBubbleData.AirBubble.value = AirSupply;
			float num = AirSupply / 10f;
			if (bubble != null)
			{
				updateBubbleState();
				updateColor();
				bubble.transform.localScale = new Vector3(num, num, num);
			}
		}

		private void OnRemotePlayerAirBubbleChanged(AirBubble airBubble)
		{
			if (airBubble.value > 10f)
			{
				airBubble.value = 10f;
			}
			if (airBubble.value < 0f)
			{
				airBubble.value = 0f;
			}
			AirSupply = airBubble.value;
			DivingState diveState = (DivingState)airBubble.diveState;
			if (diveState != 0)
			{
				State = diveState;
			}
			float num = AirSupply / 10f;
			if (bubble != null)
			{
				bubble.transform.localScale = new Vector3(num, num, num);
			}
		}

		private void updateDepth()
		{
			Depth = surface - base.transform.position.y;
			if (!hasCheckedTutorial && Depth > 15f)
			{
				Service.Get<TutorialManager>().TryStartTutorial(BoostingTutorialDefinition.Id);
				hasCheckedTutorial = true;
			}
		}

		private bool OnAction(InputEvents.ActionEvent evt)
		{
			InputEvents.Actions action = evt.Action;
			if (action == InputEvents.Actions.Action1 && !swimController.TriggerQuickResurface)
			{
				swimController.TriggerQuickResurface = true;
				showResurfacePrompt();
			}
			return false;
		}

		private void showResurfacePrompt()
		{
			Service.Get<PromptManager>().ShowPrompt("ResurfacePrompt", onPromptButtonPressed);
		}

		private void onPromptButtonPressed(DPrompt.ButtonFlags pressed)
		{
			if (pressed != DPrompt.ButtonFlags.YES)
			{
				return;
			}
			LocomotionController currentController = LocomotionHelper.GetCurrentController(base.gameObject);
			if (currentController is SitController)
			{
				LocomotionHelper.SetCurrentController<SwimController>(base.gameObject);
			}
			bubbleState = BubbleState.TransitionToHidden;
			playAudioEvent(mutableData.AirCriticalAudioEvent, EventAction.StopSound);
			GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root);
			if (gameObject != null)
			{
				StateMachineContext component = gameObject.GetComponent<StateMachineContext>();
				if (component != null)
				{
					component.SendEvent(new ExternalEvent("Root", "noUI"));
				}
			}
			Invoke("startBubbleScreenWipe", 0.7f);
			swimController.ResurfaceAccepted();
			dispatcher.DispatchEvent(default(DivingEvents.PlayerResurfaced));
		}

		private void startBubbleScreenWipe()
		{
			AirSupply = 10f;
			State = DivingState.PlentifulAir;
			Content.LoadAsync(onResurfacePopupLoaded, resurfacePrefabKey);
		}

		private void onResurfacePopupLoaded(string path, GameObject prefab)
		{
			GameObject popup = UnityEngine.Object.Instantiate(prefab);
			dispatcher.DispatchEvent(new PopupEvents.ShowCameraSpacePopup(popup, false, true, "Accessibility.Popup.Title.DivingResurface"));
		}

		private void playAudioEvent(string audioEvent, EventAction fabricEvent = EventAction.PlaySound)
		{
			if (!string.IsNullOrEmpty(audioEvent))
			{
				EventManager.Instance.PostEvent(audioEvent, fabricEvent, base.gameObject);
			}
		}

		public void RemotePlayerRemoved(DivingGameController controller)
		{
			if (!isLocalPlayer)
			{
				return;
			}
			if (controller != null)
			{
				overlappingBubbles.Remove(controller);
			}
			if (overlappingBubbles.Count == 0)
			{
				State = DivingState.HoldingBreath;
				Service.Get<INetworkServicesManager>().PlayerStateService.SetAirBubble(AirSupply, (int)State);
				dispatcher.DispatchEvent(new DivingEvents.FreeAirEffects(false, base.gameObject.tag));
				FreeAirZone componentInChildren = base.gameObject.GetComponentInChildren<FreeAirZone>();
				if (componentInChildren != null)
				{
					UnityEngine.Object.Destroy(componentInChildren);
				}
			}
		}

		private bool onEnableLocalInfiniteAir(DivingEvents.EnableLocalInfiniteAir evt)
		{
			DivingUnlimitedAir = true;
			if (bubbleState == BubbleState.Danger)
			{
				playAudioEvent(mutableData.AirCriticalAudioEvent, EventAction.StopSound);
			}
			return false;
		}

		private bool onDisableLocalInfiniteAir(DivingEvents.DisableLocalInfiniteAir evt)
		{
			DivingUnlimitedAir = false;
			if (bubbleState == BubbleState.Danger)
			{
				playAudioEvent(mutableData.AirCriticalAudioEvent);
			}
			return false;
		}
	}
}
