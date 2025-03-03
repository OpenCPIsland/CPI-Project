using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.Game.MiniGames;
using ClubPenguin.Locomotion;
using ClubPenguin.Net;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.Manimal.Common.Util;
using Disney.MobileNetwork;
using Fabric;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.MiniGames.TiltATube
{
	public class TiltATubeController : MonoBehaviour
	{
		private const int GAME_LENGTH_SECONDS = 130;

		private const string GAME_NAME = "tiltotubes";

		private const string FTUE_QUEST_NAME = "AAC001Q001LeakyShip";

		[Header("Arena Time Settings")]
		public int StartOnMinute = 2;

		public float SecondsForSorting = 0f;

		public float SecondsRoundLength = 90f;

		public int NumberOfShrinks = 90;

		[Header("Arena Environment Settings")]
		public float MaxDiameter = 1f;

		public float MinDiameter = 0.4f;

		public float MaxTiltAngle = 15f;

		public float SecondsTiltCycle = 20f;

		public float RotationSpeed = 20f;

		public string TextBuoyDefault = ":::";

		[Header("Arena Components")]
		public GameObject BuoyObject;

		public GameObject BuoyAntenna;

		public GameObject BuoyBulbOn;

		public GameObject BuoyBulbOff;

		public GameObject PlatformObject;

		public GameObject PlatformTube;

		public GameObject PlatformRotation;

		public GameObject PlatformShrink;

		public GameObject PlatformTilt;

		public GameObject PlatformCollider;

		public GameObject PlatformSign;

		public GameObject PlatformOneWayIn;

		public GameObject PlatformOneWayOut;

		public GameObject PlatformPistonSpring;

		public Vector3 PistonSpringOffset = Vector3.zero;

		public GameObject LaunchpadObject;

		public Vector3 LaunchpadOffset = Vector3.zero;

		public GameObject SignGameOver;

		public PlayersInVolume SyncPlayerVolume;

		public string PlatformVolumeId;

		public GameObject HelpSign;

		[Header("Arena Effects")]
		public GameObject ParticlesGameOver;

		public Vector3 ParticlesGameOverOffset = Vector3.zero;

		[Header("Arena Audio - Effects")]
		public GameObject AudioPrefab;

		public GameObject AudioAnchor;

		public string BuoyAppear;

		public string BuoyIdle;

		public string BuoyAntennaAppear;

		public string BuoyAntennaBulbOn;

		public string BuoyAntennaBulbOff;

		public string BuoyDisappear;

		public string PlatformDescends;

		public string PlatformTouchdown;

		public string LaunchpadAppears;

		public string LaunchpadActivated;

		public string CountdownWarning;

		public string CountdownSignDisappear;

		public string LaunchpadDisappear;

		public string GamePowerUp;

		public string GameStart;

		public string GamePlaying;

		public string PlayerFalls;

		public string GameOver;

		public string GameOverPiston;

		public string GameOverCelebration;

		public string PlatformClearArea;

		public string PlatformAscendStartup;

		public string PlatformAscend;

		[Header("Arena Audio - Music")]
		public GameObject MusicMixTrigger;

		public string MusicEventName;

		public string ParamWarmup;

		public string ParamGamePlay;

		public string ParamWin;

		public string ParamLose;

		private float diameter;

		private float stepTime;

		private float stepSize;

		private float countDown;

		private float countDownMax;

		private Vector3 platformFinalPosition;

		private Vector3 platformTubeBattlePosition;

		private Vector3 platformTubeSafetyPosition;

		private Vector3 platformOriginalAngle;

		private Vector3 platformOriginalRotation;

		private float tiltAngle;

		private float tileChange;

		private RotateTransform rotateScript;

		private TextMesh textPlatform;

		private TextMesh textBuoy;

		private TextMesh textGameOver;

		private bool isSetupCorrect = true;

		private bool isAntennaVisible = false;

		private bool isGameActive = false;

		private bool isBuoyIdle = false;

		private Collider platformWaterTrigger;

		private Animator animAntenna;

		private Animator animBuoyAndPlatform;

		private Animator animLaunchpad;

		private int hashAntennaUp;

		private int hashAntennaDown;

		private int hashBuoyAppear;

		private int hashBuoyDisappear;

		private int hashPlatformAppear;

		private int hashPlatformGameEndPiston;

		private int hashPlatformDisappear;

		private int hashLaunchpadAppear;

		private int hashLaunchpadDisppear;

		private GameObject launchpadObj;

		private GameObject pistonObj;

		private GameObject audioPrefab;

		private INetworkServicesManager network;

		private DataEntityCollection dataEntityCollection;

		private HashSet<string> activePlayers;

		private List<string> finishedPlayers;

		private int participatingPlayers;

		private string localPlayerName = "";

		private int syncAdjustment;

		private int timeBetweenRounds;

		private int timeBeforeGameStart;

		private Renderer rendBulbOn;

		private Renderer rendBulbOff;

		private EventChannel eventChannel;

		private bool didLocalPlayerParticipate;

		private void Awake()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			if (eventChannel != null)
			{
				eventChannel.AddListener<TiltATubesEvents.AddPlayer>(onAddPlayer);
				eventChannel.AddListener<TiltATubesEvents.RemovePlayer>(onRemovePlayer);
				eventChannel.AddListener<TiltATubesEvents.DisconnectPlayer>(onDisconnectPlayer);
			}
			else
			{
				Log.LogError(null, string.Format("Event dispatcher not set, this mini game will not work properly."));
				isSetupCorrect = false;
			}
			activePlayers = new HashSet<string>();
			finishedPlayers = new List<string>();
			localPlayerName = getLocalPlayerName();
			animBuoyAndPlatform = base.gameObject.GetComponent<Animator>();
			if (animBuoyAndPlatform == null)
			{
				Log.LogError(null, string.Format("Animator component not found on controller object, this mini game will not work properly."));
				isSetupCorrect = false;
			}
			else
			{
				hashBuoyAppear = Animator.StringToHash("BuoyAppear");
				hashBuoyDisappear = Animator.StringToHash("BuoyDisappear");
				hashPlatformAppear = Animator.StringToHash("PlatformAppear");
				hashPlatformGameEndPiston = Animator.StringToHash("PlatformGameEndPiston");
				hashPlatformDisappear = Animator.StringToHash("PlatformDisappear");
			}
			if (BuoyObject == null)
			{
				Log.LogError(null, string.Format("Buoy object not specified, this mini game will not work properly."));
				isSetupCorrect = false;
			}
			else
			{
				textBuoy = BuoyObject.GetComponentInChildren<TextMesh>();
				if (textBuoy == null)
				{
					Log.LogError(null, string.Format("Text mesh component not found on buoy, this mini game will not work properly."));
					isSetupCorrect = false;
				}
				else
				{
					animAntenna = BuoyObject.GetComponent<Animator>();
					if (animAntenna == null)
					{
						Log.LogError(null, string.Format("Animator component not found on buoy, this mini game will not work properly."));
						isSetupCorrect = false;
					}
					else
					{
						hashAntennaUp = Animator.StringToHash("AntennaUp");
						hashAntennaDown = Animator.StringToHash("AntennaDown");
					}
				}
			}
			if (string.IsNullOrEmpty(PlatformVolumeId))
			{
				Log.LogError(null, string.Format("PlatformId not specified, this mini game will not work properly."));
				isSetupCorrect = false;
			}
			if (PlatformObject == null)
			{
				Log.LogError(null, string.Format("Platform object not specified, this mini game will not work properly."));
				isSetupCorrect = false;
			}
			else
			{
				platformFinalPosition = PlatformObject.transform.localPosition;
				platformOriginalAngle = PlatformObject.transform.eulerAngles;
				platformWaterTrigger = PlatformObject.GetComponent<Collider>();
				if (platformWaterTrigger == null)
				{
					Log.LogError(null, string.Format("Platform water triger component not found, this mini game will not work properly."));
					isSetupCorrect = false;
				}
			}
			if (PlatformSign == null)
			{
				Log.LogError(null, string.Format("Platform text object not specified, this mini game will not work properly."));
				isSetupCorrect = false;
			}
			else
			{
				textPlatform = PlatformSign.GetComponentInChildren<TextMesh>();
				if (textPlatform == null)
				{
					Log.LogError(null, string.Format("Platform text component not found, this mini game will not work properly."));
					isSetupCorrect = false;
				}
			}
			if (PlatformRotation == null)
			{
				Log.LogError(null, string.Format("Platform rotation object not specified, this mini game will not work properly."));
				isSetupCorrect = false;
			}
			else
			{
				rotateScript = PlatformRotation.GetComponentInChildren<RotateTransform>();
				if (rotateScript == null)
				{
					Log.LogError(null, string.Format("RotatateTransform component not found, this mini game will not work properly."));
					isSetupCorrect = false;
				}
				else
				{
					platformOriginalRotation = PlatformRotation.transform.eulerAngles;
					rotateScript.isActive = true;
				}
			}
			if (PlatformCollider == null)
			{
				Log.LogError(null, string.Format("Platform collider object not specified, this mini game will not work properly."));
				isSetupCorrect = false;
			}
			if (PlatformShrink == null)
			{
				Log.LogError(null, string.Format("Platform shrink object not specified, this mini game will not work properly."));
				isSetupCorrect = false;
			}
			if (PlatformPistonSpring == null)
			{
				Log.LogError(null, string.Format("Platform piston spring object not specified, this mini game will not work properly."));
				isSetupCorrect = false;
			}
			if (PlatformTilt == null)
			{
				Log.LogError(null, string.Format("Platform tilt object not specified, this mini game will not work properly."));
				isSetupCorrect = false;
			}
			if (PlatformTube == null)
			{
				Log.LogError(null, string.Format("Platform tube object not specified, this mini game will not work properly."));
				isSetupCorrect = false;
			}
			else
			{
				platformTubeBattlePosition = PlatformTube.transform.localPosition;
				Vector3 vector = platformTubeBattlePosition;
				vector.y = 0.6f;
				platformTubeSafetyPosition = vector;
			}
			if (PlatformOneWayIn == null)
			{
				Log.LogError(null, string.Format("Platform one-way-in object not specified, this mini game will not work properly."));
				isSetupCorrect = false;
			}
			if (PlatformOneWayOut == null)
			{
				Log.LogError(null, string.Format("Platform one-way-out object not specified, this mini game will not work properly."));
				isSetupCorrect = false;
			}
			if (SyncPlayerVolume == null)
			{
				Log.LogError(null, string.Format("Sync player volume object not specified, this mini game will not work properly."));
				isSetupCorrect = false;
			}
			if (LaunchpadObject == null)
			{
				Log.LogError(null, string.Format("Launchpad prefab not specified, this mini game will not work properly."));
				isSetupCorrect = false;
			}
			else
			{
				hashLaunchpadAppear = Animator.StringToHash("LaunchpadAppear");
				hashLaunchpadDisppear = Animator.StringToHash("LaunchpadDisappear");
			}
			PlatformObject.SetActive(true);
			BuoyObject.SetActive(true);
			if (BuoyBulbOn == null)
			{
				Log.LogError(null, string.Format("Bulb on prefab not specified, this mini game will not work properly."));
				isSetupCorrect = false;
			}
			else
			{
				BuoyBulbOn.SetActive(true);
				rendBulbOn = BuoyBulbOn.GetComponent<Renderer>();
				if (rendBulbOn == null)
				{
					Log.LogError(null, string.Format("Bulb on renderer not found, this mini game will not work properly."));
					isSetupCorrect = false;
				}
			}
			if (BuoyBulbOff == null)
			{
				Log.LogError(null, string.Format("Bulb off prefab not specified, this mini game will not work properly."));
				isSetupCorrect = false;
			}
			else
			{
				BuoyBulbOff.SetActive(true);
				rendBulbOff = BuoyBulbOff.GetComponent<Renderer>();
				if (rendBulbOff == null)
				{
					Log.LogError(null, string.Format("Bulb off renderer not found, this mini game will not work properly."));
					isSetupCorrect = false;
				}
			}
			if (ParticlesGameOver == null)
			{
				Log.LogError(null, string.Format("GameOver particle object not specified, this mini game will not work properly."));
				isSetupCorrect = false;
			}
			else
			{
				ParticlesGameOver.SetActive(false);
			}
			if (SignGameOver == null)
			{
				Log.LogError(null, string.Format("GameOver sign object not specified, this mini game will not work properly."));
				isSetupCorrect = false;
			}
			else
			{
				textGameOver = SignGameOver.GetComponentInChildren<TextMesh>();
				if (textGameOver == null)
				{
					Log.LogError(null, string.Format("GameOver sign text component not found, this mini game will not work properly."));
					isSetupCorrect = false;
				}
				else
				{
					textGameOver.text = "";
				}
				SignGameOver.SetActive(false);
			}
			if (AudioPrefab == null)
			{
				Log.LogError(null, string.Format("Audio prefab object not specified, this mini game will not work properly."));
				isSetupCorrect = false;
			}
			else
			{
				audioPrefab = UnityEngine.Object.Instantiate(AudioPrefab, base.transform);
			}
			if (AudioAnchor == null)
			{
				Log.LogError(null, string.Format("Audio anchor prefab object not specified, this mini game will not work properly."));
				isSetupCorrect = false;
			}
			if (HelpSign == null)
			{
				Log.LogError(null, string.Format("HelpSign object not specified, this mini game will not work properly."));
				isSetupCorrect = false;
			}
		}

		private void Start()
		{
			if (isSetupCorrect)
			{
				network = Service.Get<INetworkServicesManager>();
				if (Service.Get<GameStateController>().IsFTUEComplete)
				{
					syncGameState();
					return;
				}
				PlatformObject.SetActive(false);
				BuoyObject.SetActive(false);
				HelpSign.SetActive(false);
				eventChannel.AddListener<QuestEvents.QuestUpdated>(onQuestUpdated);
			}
		}

		private void OnDestroy()
		{
			CancelInvoke();
			CoroutineRunner.StopAllForOwner(this);
			UnityEngine.Object.Destroy(audioPrefab);
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
		}

		private bool onQuestUpdated(QuestEvents.QuestUpdated evt)
		{
			if (evt.Quest.Id == "AAC001Q001LeakyShip" && evt.Quest.State == Quest.QuestState.Completed)
			{
				eventChannel.RemoveListener<QuestEvents.QuestUpdated>(onQuestUpdated);
				PlatformObject.SetActive(true);
				BuoyObject.SetActive(true);
				HelpSign.SetActive(true);
				syncGameState();
			}
			return false;
		}

		private void syncGameState()
		{
			SyncData syncData = calculateGameSyncData();
			if (string.IsNullOrEmpty(syncData.methodName))
			{
				Log.LogError(this, string.Format("O_o\t Can't sync game state, this mini game will not work properly"));
			}
			else
			{
				initGame(syncData);
			}
		}

		private void initGame(SyncData syncData)
		{
			CancelInvoke();
			activePlayers.Clear();
			finishedPlayers.Clear();
			isGameActive = false;
			isAntennaVisible = false;
			setBulbState(false);
			ParticlesGameOver.SetActive(false);
			PlatformOneWayIn.SetActive(false);
			PlatformOneWayOut.SetActive(false);
			setRotationSpeed(0f, 0f, 0f);
			stepSize = Mathf.Abs(MaxDiameter / (float)NumberOfShrinks) * -1f;
			stepTime = SecondsRoundLength / (float)NumberOfShrinks;
			diameter = MaxDiameter;
			Vector3 localPosition = platformFinalPosition + new Vector3(0f, 10f, 0f);
			PlatformObject.transform.localPosition = localPosition;
			setPlatformSize(diameter);
			setPlatformTilt(platformOriginalAngle);
			setPlatformRotation(platformOriginalRotation);
			PlatformTube.transform.localPosition = platformTubeSafetyPosition;
			tiltAngle = 0f;
			tileChange = MaxTiltAngle * 2f / SecondsTiltCycle;
			PlatformSign.SetActive(true);
			textPlatform.text = TextBuoyDefault;
			if (launchpadObj != null)
			{
				UnityEngine.Object.Destroy(launchpadObj);
			}
			animLaunchpad = null;
			participatingPlayers = 0;
			didLocalPlayerParticipate = false;
			timeBetweenRounds = StartOnMinute * 60;
			timeBeforeGameStart = timeBetweenRounds - 130;
			if (string.IsNullOrEmpty(syncData.methodName))
			{
				syncData.methodName = "initAttractMode";
				syncData.timeAdjust = 0;
			}
			syncAdjustment = syncData.timeAdjust;
			Invoke(syncData.methodName, 0f);
		}

		private void initAttractMode()
		{
			CancelInvoke();
			countDownMax = secondsUntilStart(syncAdjustment);
			countDown = countDownMax;
			PlatformCollider.SetActive(true);
			animBuoyAndPlatform.SetTrigger(hashBuoyAppear);
			playAudioEvent(BuoyAppear, AudioAnchor);
			float time = 0f;
			InvokeRepeating("displayBuoyCoundown", time, 1f);
			isBuoyIdle = false;
		}

		private void displayBuoyCoundown()
		{
			countDown = secondsUntilStart(syncAdjustment);
			textBuoy.text = secondsToTime(countDown);
			if (!isBuoyIdle)
			{
				playAudioEvent(BuoyIdle, BuoyObject);
				isBuoyIdle = false;
			}
			if (syncAdjustment > 0)
			{
				syncAdjustment--;
			}
			else
			{
				syncAdjustment = 0;
			}
			if (countDown > 40f)
			{
				if (countDown <= 50f)
				{
					if (!isAntennaVisible)
					{
						isAntennaVisible = true;
						animAntenna.SetTrigger(hashAntennaUp);
						playAudioEvent(BuoyAntennaAppear, BuoyObject);
					}
					else if ((int)countDown % 2 == 0)
					{
						setBulbState(false);
						playAudioEvent(BuoyAntennaBulbOff, BuoyObject);
					}
					else
					{
						setBulbState(true);
						playAudioEvent(BuoyAntennaBulbOn, BuoyObject);
					}
				}
			}
			else
			{
				CancelInvoke("displayBuoyCoundown");
				if (isBuoyIdle)
				{
					stopAudioEvent(BuoyIdle, BuoyObject);
					isBuoyIdle = false;
				}
				finishBuoyCoundown();
			}
		}

		private void finishBuoyCoundown()
		{
			setBulbState(false);
			animAntenna.SetTrigger(hashAntennaDown);
			animBuoyAndPlatform.SetTrigger(hashBuoyDisappear);
			playAudioEvent(BuoyDisappear, BuoyObject);
			float time = 0f;
			Invoke("startPlatformDescent", time);
		}

		private void startPlatformDescent()
		{
			animBuoyAndPlatform.SetTrigger(hashPlatformAppear);
			playAudioEvent(PlatformDescends, AudioAnchor);
			float time = 4f;
			Invoke("finishPlatformDescent", time);
		}

		private void finishPlatformDescent()
		{
			PlatformOneWayIn.SetActive(true);
			PlatformOneWayOut.SetActive(false);
			float time = 0f;
			Invoke("startLaunchPadAppear", time);
		}

		private void startLaunchPadAppear()
		{
			launchpadObj = UnityEngine.Object.Instantiate(LaunchpadObject, base.transform);
			launchpadObj.transform.localPosition = LaunchpadOffset;
			playAudioEvent(LaunchpadAppears, AudioAnchor);
			MusicMixTrigger.SetActive(true);
			audioEvent(MusicEventName, EventAction.PlaySound, null, AudioAnchor);
			audioEvent(MusicEventName, EventAction.SetSwitch, ParamWarmup, AudioAnchor);
			animLaunchpad = launchpadObj.GetComponent<Animator>();
			if (animLaunchpad != null)
			{
				animLaunchpad.SetTrigger(hashLaunchpadAppear);
			}
			float time = 2f;
			Invoke("finishLaunchPadAppear", time);
		}

		private void finishLaunchPadAppear()
		{
			countDown = secondsUntilStart(syncAdjustment);
			textPlatform.text = secondsToTime(countDown);
			float time = 0f;
			InvokeRepeating("displayPlatformCountdown", time, 1f);
		}

		private void displayPlatformCountdown()
		{
			countDown = secondsUntilStart(syncAdjustment);
			textPlatform.text = secondsToTime(Mathf.Clamp(countDown, 0f, countDownMax));
			if (countDown <= 3f)
			{
				playAudioEvent(CountdownWarning, AudioAnchor);
			}
			if (countDown <= 0f)
			{
				CancelInvoke("displayPlatformCountdown");
				startLaunchPadDisappear();
			}
		}

		private void startLaunchPadDisappear()
		{
			playAudioEvent(LaunchpadDisappear, AudioAnchor);
			if (animLaunchpad != null)
			{
				animLaunchpad.SetTrigger(hashLaunchpadDisppear);
			}
			if (!isPlatformEmpty())
			{
				playAudioEvent(GamePowerUp, AudioAnchor);
				audioSetSwitchEvent(MusicEventName, ParamGamePlay, AudioAnchor);
			}
			float time = 3f;
			Invoke("finishLaunchPadDisappear", time);
		}

		private void finishLaunchPadDisappear()
		{
			UnityEngine.Object.Destroy(launchpadObj);
			if (!isPlatformEmpty())
			{
				playAudioEvent(GameStart, AudioAnchor);
			}
			float time = 0.5f;
			Invoke("startGameWarmup", time);
		}

		private void startGameWarmup()
		{
			if (isPlatformEmpty())
			{
				audioSetSwitchEvent(MusicEventName, ParamLose, AudioAnchor);
				endGame();
				return;
			}
			PlatformSign.SetActive(false);
			playAudioEvent(CountdownSignDisappear, AudioAnchor);
			PlatformOneWayIn.SetActive(false);
			PlatformOneWayOut.SetActive(true);
			PlatformTube.transform.localPosition = platformTubeBattlePosition;
			setRotationSpeed(0f, RotationSpeed, 0f);
			participatingPlayers = playersOnPlatform();
			float time = 0f;
			Invoke("startGame", time);
		}

		private void startGame()
		{
			playAudioEvent(GamePlaying, AudioAnchor);
			InvokeRepeating("changePlatformAngle", 0f, 0.5f);
			InvokeRepeating("changePlatformSize", 0f, stepTime);
			InvokeRepeating("checkGameOver", 0f, 1f);
			isGameActive = true;
		}

		private void checkGameOver()
		{
			if (isGameActive && (isPlatformEmpty() || (participatingPlayers > 1 && playersOnPlatform() == 1)))
			{
				endGame();
			}
		}

		private void changePlatformAngle()
		{
			tiltAngle += tileChange;
			if (tiltAngle <= 0f - MaxTiltAngle)
			{
				tiltAngle = 0f - MaxTiltAngle;
				tileChange = Mathf.Abs(tileChange);
			}
			else if (tiltAngle >= MaxTiltAngle)
			{
				tiltAngle = MaxTiltAngle;
				tileChange = Mathf.Abs(tileChange) * -1f;
			}
			setPlatformTilt(tiltAngle);
		}

		private void changePlatformSize()
		{
			diameter += stepSize;
			if (diameter < MinDiameter)
			{
				endGame();
			}
			else if (diameter > MaxDiameter)
			{
				diameter = MaxDiameter;
				stepSize = Mathf.Abs(stepSize) * -1f;
			}
			setPlatformSize(diameter);
		}

		private void setPlatformTilt(float newAngle)
		{
			Vector3 eulerAngles = PlatformTilt.transform.eulerAngles;
			eulerAngles.z = newAngle;
			PlatformTilt.transform.eulerAngles = eulerAngles;
		}

		private void setPlatformTilt(Vector3 newRotation)
		{
			PlatformTilt.transform.eulerAngles = newRotation;
		}

		private void setPlatformSize(float newDiameter)
		{
			Vector3 localScale = PlatformShrink.transform.localScale;
			localScale.x = newDiameter;
			localScale.z = newDiameter;
			PlatformShrink.transform.localScale = localScale;
			PlatformOneWayOut.transform.localScale = localScale;
		}

		private void setPlatformRotation(Vector3 newRotation)
		{
			PlatformRotation.transform.eulerAngles = newRotation;
		}

		private void endGame()
		{
			isGameActive = false;
			if (didLocalPlayerParticipate)
			{
				setUIVisibility(true);
			}
			CancelInvoke("changePlatformAngle");
			CancelInvoke("changePlatformSize");
			CancelInvoke("checkGameOver");
			setPlatformTilt(0f);
			setRotationSpeed(0f, 0f, 0f);
			if (!isPlatformEmpty())
			{
				ParticlesGameOver.SetActive(true);
				playAudioEvent(GameOverCelebration, AudioAnchor);
			}
			float time = 1f;
			Invoke("endGameDoPiston", time);
		}

		private void endGameDoPiston()
		{
			displayLocalTrophyRanking();
			float time;
			if (!isPlatformEmpty())
			{
				pistonObj = UnityEngine.Object.Instantiate(PlatformPistonSpring, PlatformShrink.transform);
				pistonObj.transform.localPosition = PistonSpringOffset;
				Vector3 localScale = PlatformShrink.transform.localScale;
				localScale.y = localScale.x;
				pistonObj.transform.localScale = localScale;
				animBuoyAndPlatform.SetTrigger(hashPlatformGameEndPiston);
				playAudioEvent(GameOverPiston, AudioAnchor);
				PlatformCollider.SetActive(false);
				time = 4f;
			}
			else
			{
				pistonObj = null;
				time = 2f;
			}
			Invoke("endGamePlatformStartDisappear", time);
		}

		private void endGamePlatformStartDisappear()
		{
			PlatformCollider.SetActive(false);
			if (pistonObj != null)
			{
				UnityEngine.Object.Destroy(pistonObj);
			}
			animBuoyAndPlatform.SetTrigger(hashPlatformDisappear);
			playAudioEvent(PlatformAscend, AudioAnchor);
			float time = 2f;
			Invoke("endGamePlatformEndDisappear", time);
		}

		private void endGamePlatformEndDisappear()
		{
			textPlatform.text = TextBuoyDefault;
			textBuoy.text = TextBuoyDefault;
			MusicMixTrigger.SetActive(false);
			initGame(new SyncData("initAttractMode"));
		}

		private SyncData calculateGameSyncData()
		{
			SyncData syncData = new SyncData();
			int num = secondsUntilStart();
			if (num >= 131)
			{
				syncData.methodName = "initAttractMode";
				syncData.timeAdjust = 0;
			}
			else if (num < 130 && num >= 127)
			{
				animBuoyAndPlatform.SetTrigger("BuoyIdle");
				syncData.methodName = "finishBuoyCoundown";
				syncData.timeAdjust = -130;
			}
			else if (num < 127 && num >= 125)
			{
				animBuoyAndPlatform.SetTrigger("PlatformGameInProgress");
				syncData.methodName = "startLaunchPadAppear";
				syncData.timeAdjust = -130;
			}
			else if (num < 125 && num >= 100)
			{
				animBuoyAndPlatform.SetTrigger("PlatformGameInProgress");
				syncData.methodName = "startLaunchPadAppear";
				syncData.timeAdjust = -130;
			}
			else if (num < 100 && num >= 97)
			{
				if (SyncPlayerVolume.PlayerCount() == 0)
				{
					syncData.methodName = "initAttractMode";
					syncData.timeAdjust = timeBeforeGameStart;
				}
				else
				{
					animBuoyAndPlatform.SetTrigger("PlatformGameInProgress");
					PlatformSign.SetActive(false);
					syncData.methodName = "startGameWarmup";
					syncData.timeAdjust = -130;
				}
			}
			else if (num < 97 && num >= 7)
			{
				if (SyncPlayerVolume.PlayerCount() == 0)
				{
					syncData.methodName = "initAttractMode";
					syncData.timeAdjust = timeBeforeGameStart;
				}
				else
				{
					animBuoyAndPlatform.SetTrigger("PlatformGameInProgress");
					PlatformSign.SetActive(false);
					syncData.methodName = "startGameWarmup";
					syncData.timeAdjust = -130;
				}
			}
			else if (num < 7 && num >= 2)
			{
				if (SyncPlayerVolume.PlayerCount() == 0)
				{
					syncData.methodName = "initAttractMode";
					syncData.timeAdjust = timeBeforeGameStart;
				}
				else
				{
					animBuoyAndPlatform.SetTrigger("PlatformGameInProgress");
					PlatformSign.SetActive(false);
					setPlatformSize(MinDiameter);
					syncData.methodName = "endGame";
					syncData.timeAdjust = -130;
				}
			}
			else if (num < 2 && num >= 0)
			{
				if (SyncPlayerVolume.PlayerCount() == 0)
				{
					syncData.methodName = "initAttractMode";
					syncData.timeAdjust = timeBeforeGameStart;
				}
				else
				{
					animBuoyAndPlatform.SetTrigger("PlatformGameInProgress");
					PlatformSign.SetActive(false);
					setPlatformSize(MinDiameter);
					syncData.methodName = "endGamePlatformStartDisappear";
					syncData.timeAdjust = -130;
				}
			}
			return syncData;
		}

		private void setBulbState(bool isOn)
		{
			rendBulbOn.enabled = isOn;
			rendBulbOff.enabled = !isOn;
		}

		private DateTime getServerTime()
		{
			if (network != null)
			{
				long gameTimeMilliseconds = network.GameTimeMilliseconds;
				if (gameTimeMilliseconds > 0)
				{
					return gameTimeMilliseconds.MsToDateTime();
				}
			}
			return DateTime.MinValue;
		}

		private int secondsUntilStart(int secondsAdjustment = 0)
		{
			DateTime serverTime = getServerTime();
			int num = StartOnMinute - serverTime.Minute % StartOnMinute;
			int num2 = 60 - serverTime.Second;
			if (num == StartOnMinute && serverTime.Second <= 1)
			{
				return 0;
			}
			num--;
			return num * 60 + num2 + secondsAdjustment;
		}

		private string secondsToTime(float seconds)
		{
			int num = (int)seconds / 60;
			int num2 = (int)seconds % 60;
			return string.Format("{0}:{1:00}", num, num2);
		}

		private void setRotationSpeed(float xSpeed, float ySpeed, float zSpeed)
		{
			if (rotateScript != null)
			{
				rotateScript.XAxisRotationSpeed = xSpeed;
				rotateScript.YAxisRotationSpeed = ySpeed;
				rotateScript.ZAxisRotationSpeed = zSpeed;
			}
		}

		private bool onAddPlayer(TiltATubesEvents.AddPlayer evt)
		{
			if (PlatformVolumeId == evt.VolumeId)
			{
				GameObject playerGameObj = evt.PlayerGameObj;
				string playerName = getPlayerName(playerGameObj);
				if (!isGameActive)
				{
					if (activePlayers.Add(playerName) && playerName == localPlayerName && isPlayerActive(localPlayerName))
					{
						setUIVisibility(false);
						didLocalPlayerParticipate = true;
						closeTray();
						CoroutineRunner.Start(setSlideLocomotion(1.5f, playerGameObj), this, "forceIntoTube");
						MiniGameUtils.StartBiTimer("tiltotubes");
						MiniGameUtils.LogGameStartBi("tiltotubes");
					}
					if (finishedPlayers.Contains(playerName))
					{
						finishedPlayers.Remove(playerName);
					}
				}
			}
			return false;
		}

		private bool onRemovePlayer(TiltATubesEvents.RemovePlayer evt)
		{
			if (PlatformVolumeId == evt.VolumeId)
			{
				GameObject playerGameObj = evt.PlayerGameObj;
				string playerName = getPlayerName(playerGameObj);
				if (activePlayers.Contains(playerName))
				{
					activePlayers.Remove(playerName);
					if (!finishedPlayers.Contains(playerName))
					{
						finishedPlayers.Add(playerName);
					}
					if (playerName == localPlayerName)
					{
						setUIVisibility(true);
					}
					if (isGameActive)
					{
						playAudioEvent(PlayerFalls, AudioAnchor);
					}
					MiniGameUtils.StopBiTimer("tiltotubes");
				}
			}
			return false;
		}

		private bool onDisconnectPlayer(TiltATubesEvents.DisconnectPlayer evt)
		{
			if (PlatformVolumeId == evt.VolumeId)
			{
				string playerName = evt.PlayerName;
				if (activePlayers.Contains(playerName))
				{
					activePlayers.Remove(playerName);
				}
				if (finishedPlayers.Contains(playerName))
				{
					finishedPlayers.Add(playerName);
				}
			}
			return false;
		}

		private IEnumerator setSlideLocomotion(float waitTime, GameObject penguinObj)
		{
			yield return new WaitForSeconds(waitTime);
			if (!LocomotionHelper.IsCurrentControllerOfType<SlideController>(penguinObj))
			{
				if (!LocomotionHelper.SetCurrentController<SlideController>(penguinObj))
				{
					Log.LogErrorFormatted(this, "Failed to set the SlideController on {0}", penguinObj.GetPath());
				}
				CoroutineRunner.Start(toggleWaterTrigger(2f), this, "toggleWaterCollider");
			}
		}

		private bool isTubing(GameObject playerObj)
		{
			if (LocomotionHelper.IsCurrentControllerOfType<SlideController>(playerObj))
			{
				return true;
			}
			return false;
		}

		private IEnumerator toggleWaterTrigger(float waitTime)
		{
			platformWaterTrigger.enabled = false;
			yield return new WaitForSeconds(waitTime);
			platformWaterTrigger.enabled = true;
		}

		private string calculateRankings()
		{
			string text = "";
			int num = 1;
			foreach (string activePlayer in activePlayers)
			{
				text += string.Format("{0}) {1}\n", num, activePlayer);
			}
			if (activePlayers.Count > 0)
			{
				num = 2;
			}
			int count = finishedPlayers.Count;
			for (int num2 = count; num2 > 0; num2--)
			{
				text += string.Format("{0}) {1}\n", num, finishedPlayers[num2 - 1]);
				num++;
				if (num == 3)
				{
					text += "\n";
				}
			}
			return text;
		}

		private void displayLocalTrophyRanking()
		{
			if (activePlayers.Count == 0)
			{
				if (finishedPlayers.Count > 0)
				{
					audioSetSwitchEvent(MusicEventName, ParamLose, AudioAnchor);
				}
			}
			else
			{
				audioSetSwitchEvent(MusicEventName, ParamWin, AudioAnchor);
			}
			if (activePlayers.Contains(localPlayerName))
			{
				if (activePlayers.Count > 1)
				{
					Service.Get<EventDispatcher>().DispatchEvent(new HeadStatusEvents.ShowHeadStatus(TemporaryHeadStatusType.TrophyA));
				}
				else
				{
					Service.Get<EventDispatcher>().DispatchEvent(new HeadStatusEvents.ShowHeadStatus(TemporaryHeadStatusType.TrophyB));
				}
			}
			else
			{
				if (!finishedPlayers.Contains(localPlayerName))
				{
					return;
				}
				if (participatingPlayers == 1)
				{
					Service.Get<EventDispatcher>().DispatchEvent(new HeadStatusEvents.ShowHeadStatus(TemporaryHeadStatusType.TrophyD));
					return;
				}
				int count = finishedPlayers.Count;
				if (finishedPlayers[count - 1] == localPlayerName)
				{
					Service.Get<EventDispatcher>().DispatchEvent(new HeadStatusEvents.ShowHeadStatus(TemporaryHeadStatusType.TrophyC));
				}
				if (count > 1 && finishedPlayers[count - 2] == localPlayerName)
				{
					Service.Get<EventDispatcher>().DispatchEvent(new HeadStatusEvents.ShowHeadStatus(TemporaryHeadStatusType.TrophyD));
				}
			}
		}

		private string getPlayerName(GameObject playerObj)
		{
			DataEntityHandle handle;
			DisplayNameData component;
			if (AvatarDataHandle.TryGetPlayerHandle(playerObj, out handle) && dataEntityCollection.TryGetComponent(handle, out component))
			{
				return component.DisplayName;
			}
			return "Error: name not found";
		}

		private string getLocalPlayerName()
		{
			DataEntityHandle localPlayerHandle = Service.Get<CPDataEntityCollection>().LocalPlayerHandle;
			DisplayNameData component;
			if (dataEntityCollection.TryGetComponent(localPlayerHandle, out component))
			{
				return component.DisplayName;
			}
			return "Error: name not found";
		}

		private void setUIVisibility(bool isVisible)
		{
			if (isVisible)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("ActionButton"));
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("ControlsButton1"));
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("ControlsButton2"));
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("CellphoneButton"));
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElementGroup("MainNavButtons"));
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("ActionButton"));
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("ControlsButton1"));
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("ControlsButton2"));
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("CellphoneButton"));
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElementGroup("MainNavButtons"));
			}
		}

		private void closeTray()
		{
			StateMachineContext component = SceneRefs.UiTrayRoot.GetComponent<StateMachineContext>();
			component.SendEvent(new ExternalEvent("Root", "mainnav_locomotion"));
		}

		private bool isPlayerActive(string name)
		{
			return activePlayers.Contains(name);
		}

		private bool isPlatformEmpty()
		{
			return activePlayers.Count == 0;
		}

		private int playersOnPlatform()
		{
			return activePlayers.Count;
		}

		private void debugDisplayPlayers()
		{
			int num = 0;
			string str = "Game Data\n+--------+\nActive:\n";
			int count = activePlayers.Count;
			foreach (string activePlayer in activePlayers)
			{
				str += string.Format("  {0:00}) {1}\n", num++, activePlayer);
			}
			str += "\n+--------+\nFinish positions:\n";
			count = finishedPlayers.Count;
			for (int i = 0; i < count; i++)
			{
				str += string.Format("  {0:00}) {1}\n", i + 1, finishedPlayers[count - 1 - i]);
			}
			str += "\n+--------+";
		}

		private void playAudioEvent(string audioEventName, GameObject anchorObj = null)
		{
			if (!string.IsNullOrEmpty(audioEventName))
			{
				if (anchorObj != null)
				{
					EventManager.Instance.PostEvent(audioEventName, EventAction.PlaySound, anchorObj);
				}
				else
				{
					EventManager.Instance.PostEvent(audioEventName, EventAction.PlaySound);
				}
			}
		}

		private void stopAudioEvent(string audioEventName, GameObject anchorObj = null)
		{
			if (!string.IsNullOrEmpty(audioEventName))
			{
				if (anchorObj != null)
				{
					EventManager.Instance.PostEvent(audioEventName, EventAction.StopSound, anchorObj);
				}
				else
				{
					EventManager.Instance.PostEvent(audioEventName, EventAction.StopSound);
				}
			}
		}

		private void audioSetSwitchEvent(string eventName, string childComponentName, GameObject go = null)
		{
			audioEvent(eventName, EventAction.SetSwitch, childComponentName, go);
		}

		private void audioEvent(string eventName, EventAction fabricEvent, string childComponentName, GameObject go = null)
		{
			if (!string.IsNullOrEmpty(eventName))
			{
				if (go == null)
				{
					EventManager.Instance.PostEvent(eventName, fabricEvent, childComponentName);
				}
				else
				{
					EventManager.Instance.PostEvent(eventName, fabricEvent, childComponentName, go);
				}
			}
		}
	}
}
