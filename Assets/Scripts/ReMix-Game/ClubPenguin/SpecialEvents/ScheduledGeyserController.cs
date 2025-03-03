using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.Manimal.Common.Util;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.SpecialEvents
{
	public class ScheduledGeyserController : ScheduledCore
	{
		private enum AttractPhase
		{
			None,
			Off,
			Low,
			Medium,
			High
		}

		private enum GeyserPhase
		{
			None,
			Attract,
			Boost,
			Waiting
		}

		private const int RANDOM_POOLSIZE_MAX = 1440;

		public ScheduledFog FogController;

		[Header("Geyser Time Settings")]
		public int StartOnMinute = 3;

		public int AttractSeconds = 20;

		public int AttractLowSeconds = 7;

		public int AttractMedSeconds = 7;

		public int AttractHighSeconds = 6;

		public int BoostSeconds = 10;

		[Header("Geyser Settings")]
		public float GeyserDiameter = 2f;

		public float GeyserHeight = 3f;

		public GameObject[] CollectibleObjects;

		public GameObject SparklePrefab;

		public GameObject AttractPrefab;

		public GameObject GeyserPrefab;

		public GameObject SpringPrefab;

		[Header("Audio Settings")]
		public string AudioEventStart;

		public string AudioAttractStart;

		public string AudioGeyserStart;

		public string AudioEventEnd;

		private INetworkServicesManager network;

		private Vector3?[] spawnPoint;

		private int[] sharedRandom;

		private GameObject sparkleObject;

		private ParticleSystem sparklePartSys;

		private GameObject attractObject;

		private Animator attractAnim;

		private GameObject geyserObject;

		private Animator geyserAnim;

		private GameObject springObject;

		private bool isEventFogActive;

		private bool isAdjustmentOverride;

		private GeyserPhase geyserPhase;

		public override void Awake()
		{
			base.Awake();
			network = Service.Get<INetworkServicesManager>();
		}

		public override void DisplayAdjustments()
		{
			initialize();
			calculateEventPhase();
		}

		private void initialize()
		{
			isEventFogActive = false;
			isAdjustmentOverride = false;
			geyserPhase = GeyserPhase.None;
			int num = CollectibleObjects.Length;
			spawnPoint = new Vector3?[num];
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = CollectibleObjects[i];
				spawnPoint[i] = PhysicsUtil.IntersectionPointWithLayer(gameObject, LayerConstants.GetPlayerLayerCollisionMask(), 20f, Vector3.down);
				gameObject.SetActive(false);
			}
			sparkleObject = UnityEngine.Object.Instantiate(SparklePrefab);
			sparklePartSys = sparkleObject.GetComponent<ParticleSystem>();
			sparkleEffectActive(false);
			attractObject = UnityEngine.Object.Instantiate(AttractPrefab);
			attractAnim = attractObject.GetComponent<Animator>();
			setAttractPhase(AttractPhase.Off);
			geyserObject = UnityEngine.Object.Instantiate(GeyserPrefab);
			geyserAnim = geyserObject.GetComponent<Animator>();
			setGeyserPhase(GeyserPhase.Waiting);
			sharedRandom = new int[1440]
			{
				1,
				4,
				21,
				0,
				9,
				10,
				14,
				7,
				20,
				23,
				12,
				18,
				17,
				16,
				8,
				6,
				19,
				15,
				3,
				22,
				11,
				5,
				13,
				2,
				14,
				9,
				22,
				13,
				21,
				4,
				7,
				18,
				12,
				23,
				11,
				15,
				8,
				1,
				10,
				5,
				19,
				0,
				20,
				16,
				3,
				2,
				6,
				17,
				23,
				0,
				22,
				20,
				10,
				14,
				11,
				15,
				9,
				13,
				5,
				1,
				17,
				21,
				3,
				6,
				12,
				18,
				2,
				8,
				4,
				16,
				19,
				7,
				1,
				18,
				22,
				19,
				21,
				8,
				20,
				2,
				17,
				12,
				16,
				13,
				11,
				0,
				10,
				5,
				9,
				4,
				15,
				6,
				7,
				14,
				3,
				23,
				18,
				13,
				14,
				0,
				3,
				10,
				22,
				15,
				1,
				8,
				4,
				11,
				12,
				5,
				7,
				9,
				17,
				20,
				21,
				16,
				2,
				19,
				6,
				23,
				22,
				1,
				12,
				6,
				20,
				5,
				9,
				14,
				19,
				3,
				15,
				13,
				2,
				8,
				0,
				17,
				23,
				10,
				21,
				11,
				18,
				16,
				4,
				7,
				11,
				16,
				15,
				8,
				20,
				19,
				10,
				23,
				9,
				6,
				0,
				14,
				7,
				3,
				21,
				12,
				4,
				17,
				2,
				1,
				5,
				18,
				22,
				13,
				5,
				9,
				4,
				17,
				18,
				3,
				16,
				7,
				1,
				14,
				13,
				0,
				8,
				22,
				20,
				12,
				23,
				11,
				15,
				2,
				6,
				19,
				21,
				10,
				3,
				18,
				7,
				21,
				4,
				11,
				23,
				10,
				22,
				5,
				1,
				0,
				2,
				19,
				17,
				9,
				14,
				6,
				8,
				20,
				16,
				12,
				15,
				13,
				10,
				19,
				4,
				3,
				5,
				15,
				16,
				0,
				9,
				1,
				18,
				14,
				23,
				20,
				17,
				21,
				12,
				2,
				22,
				11,
				13,
				6,
				7,
				8,
				5,
				16,
				11,
				6,
				7,
				4,
				3,
				15,
				2,
				20,
				18,
				1,
				14,
				9,
				19,
				17,
				0,
				8,
				12,
				10,
				21,
				22,
				13,
				23,
				13,
				23,
				20,
				18,
				17,
				22,
				7,
				10,
				12,
				11,
				14,
				15,
				2,
				19,
				9,
				6,
				0,
				21,
				16,
				5,
				1,
				3,
				8,
				4,
				2,
				3,
				6,
				9,
				15,
				5,
				19,
				10,
				4,
				0,
				17,
				18,
				23,
				13,
				11,
				20,
				16,
				14,
				8,
				21,
				12,
				22,
				7,
				1,
				18,
				9,
				6,
				22,
				12,
				3,
				7,
				21,
				11,
				13,
				17,
				1,
				10,
				23,
				20,
				15,
				5,
				8,
				16,
				4,
				14,
				2,
				0,
				19,
				20,
				0,
				12,
				7,
				13,
				9,
				22,
				16,
				17,
				19,
				14,
				15,
				2,
				1,
				3,
				18,
				23,
				21,
				10,
				4,
				5,
				8,
				11,
				6,
				5,
				15,
				1,
				22,
				21,
				0,
				20,
				17,
				14,
				23,
				3,
				4,
				7,
				12,
				19,
				2,
				18,
				11,
				13,
				8,
				16,
				6,
				10,
				9,
				2,
				5,
				7,
				21,
				14,
				18,
				19,
				13,
				1,
				10,
				22,
				3,
				9,
				23,
				12,
				17,
				8,
				0,
				11,
				20,
				15,
				6,
				16,
				4,
				8,
				21,
				1,
				22,
				10,
				7,
				20,
				19,
				0,
				16,
				11,
				5,
				18,
				15,
				13,
				23,
				9,
				14,
				3,
				2,
				17,
				4,
				12,
				6,
				7,
				11,
				3,
				23,
				17,
				19,
				12,
				16,
				0,
				6,
				20,
				9,
				10,
				21,
				13,
				18,
				15,
				2,
				1,
				22,
				14,
				8,
				4,
				5,
				19,
				22,
				8,
				0,
				23,
				1,
				20,
				21,
				3,
				12,
				13,
				15,
				9,
				7,
				10,
				2,
				17,
				11,
				4,
				5,
				6,
				18,
				16,
				14,
				11,
				5,
				15,
				12,
				21,
				17,
				3,
				19,
				0,
				16,
				8,
				2,
				1,
				20,
				9,
				13,
				7,
				6,
				18,
				14,
				22,
				10,
				23,
				4,
				13,
				18,
				16,
				2,
				15,
				8,
				17,
				11,
				14,
				9,
				6,
				3,
				7,
				5,
				23,
				20,
				0,
				22,
				4,
				10,
				21,
				12,
				19,
				1,
				18,
				1,
				9,
				8,
				11,
				5,
				20,
				14,
				7,
				19,
				2,
				17,
				21,
				15,
				12,
				4,
				22,
				0,
				10,
				3,
				16,
				6,
				23,
				13,
				4,
				15,
				13,
				23,
				1,
				11,
				22,
				7,
				12,
				17,
				10,
				18,
				6,
				2,
				20,
				0,
				5,
				19,
				16,
				14,
				3,
				9,
				8,
				21,
				18,
				10,
				3,
				5,
				14,
				6,
				1,
				2,
				15,
				9,
				0,
				11,
				19,
				13,
				23,
				17,
				7,
				4,
				16,
				21,
				22,
				20,
				8,
				12,
				1,
				18,
				0,
				4,
				21,
				11,
				19,
				3,
				8,
				13,
				14,
				9,
				6,
				7,
				20,
				23,
				5,
				17,
				10,
				12,
				15,
				16,
				22,
				2,
				13,
				18,
				10,
				1,
				3,
				11,
				2,
				23,
				17,
				5,
				6,
				7,
				9,
				22,
				12,
				15,
				14,
				21,
				20,
				8,
				19,
				0,
				16,
				4,
				7,
				0,
				6,
				8,
				12,
				14,
				3,
				19,
				20,
				10,
				16,
				13,
				1,
				17,
				2,
				23,
				15,
				21,
				9,
				11,
				18,
				22,
				4,
				5,
				20,
				2,
				9,
				15,
				10,
				12,
				18,
				17,
				8,
				7,
				23,
				1,
				14,
				19,
				6,
				0,
				22,
				21,
				11,
				4,
				5,
				13,
				3,
				16,
				0,
				3,
				6,
				20,
				8,
				2,
				5,
				18,
				19,
				17,
				16,
				10,
				14,
				1,
				15,
				22,
				11,
				7,
				23,
				9,
				12,
				13,
				4,
				21,
				1,
				17,
				0,
				3,
				14,
				12,
				5,
				6,
				15,
				20,
				10,
				22,
				23,
				13,
				9,
				2,
				16,
				19,
				18,
				7,
				8,
				4,
				21,
				11,
				22,
				18,
				23,
				9,
				3,
				0,
				16,
				11,
				15,
				7,
				1,
				17,
				2,
				21,
				12,
				10,
				5,
				4,
				20,
				8,
				19,
				14,
				13,
				6,
				5,
				16,
				18,
				3,
				6,
				10,
				21,
				2,
				14,
				11,
				7,
				20,
				19,
				1,
				12,
				17,
				4,
				8,
				22,
				13,
				9,
				0,
				15,
				23,
				13,
				19,
				15,
				11,
				12,
				0,
				3,
				4,
				2,
				10,
				7,
				23,
				8,
				18,
				5,
				17,
				14,
				1,
				20,
				9,
				6,
				21,
				16,
				22,
				11,
				19,
				6,
				9,
				4,
				13,
				14,
				3,
				5,
				21,
				20,
				1,
				2,
				8,
				17,
				22,
				7,
				10,
				15,
				12,
				16,
				23,
				0,
				18,
				12,
				9,
				0,
				15,
				1,
				21,
				8,
				17,
				7,
				11,
				2,
				23,
				6,
				16,
				22,
				20,
				18,
				13,
				14,
				10,
				4,
				19,
				5,
				3,
				21,
				12,
				6,
				9,
				11,
				17,
				18,
				4,
				13,
				14,
				20,
				3,
				0,
				7,
				16,
				15,
				19,
				8,
				23,
				5,
				2,
				22,
				1,
				10,
				2,
				7,
				10,
				13,
				5,
				23,
				12,
				21,
				6,
				20,
				19,
				18,
				4,
				11,
				14,
				16,
				15,
				3,
				22,
				0,
				9,
				17,
				1,
				8,
				14,
				13,
				9,
				17,
				5,
				10,
				1,
				16,
				2,
				15,
				12,
				7,
				0,
				22,
				21,
				20,
				23,
				19,
				8,
				18,
				11,
				4,
				3,
				6,
				9,
				10,
				4,
				3,
				0,
				1,
				17,
				11,
				16,
				2,
				14,
				23,
				18,
				6,
				13,
				12,
				7,
				22,
				8,
				21,
				20,
				5,
				15,
				19,
				1,
				15,
				5,
				10,
				7,
				9,
				0,
				20,
				21,
				4,
				22,
				8,
				12,
				3,
				6,
				17,
				11,
				14,
				16,
				18,
				19,
				13,
				23,
				2,
				22,
				14,
				2,
				23,
				4,
				20,
				3,
				16,
				13,
				8,
				11,
				15,
				1,
				5,
				18,
				10,
				19,
				9,
				0,
				7,
				17,
				12,
				6,
				21,
				11,
				22,
				3,
				13,
				21,
				10,
				14,
				2,
				8,
				6,
				1,
				16,
				9,
				0,
				15,
				4,
				12,
				7,
				17,
				20,
				23,
				18,
				5,
				19,
				4,
				22,
				23,
				7,
				16,
				18,
				13,
				1,
				9,
				3,
				8,
				20,
				0,
				17,
				5,
				14,
				2,
				19,
				6,
				12,
				21,
				10,
				11,
				15,
				23,
				17,
				11,
				20,
				14,
				3,
				8,
				9,
				6,
				4,
				15,
				2,
				12,
				18,
				16,
				7,
				13,
				19,
				0,
				10,
				5,
				21,
				1,
				22,
				6,
				7,
				14,
				19,
				12,
				15,
				0,
				8,
				3,
				4,
				23,
				10,
				1,
				21,
				18,
				9,
				22,
				5,
				20,
				16,
				13,
				2,
				17,
				11,
				0,
				17,
				13,
				5,
				3,
				1,
				12,
				8,
				16,
				18,
				7,
				15,
				22,
				9,
				21,
				23,
				4,
				10,
				2,
				14,
				20,
				19,
				6,
				11,
				18,
				13,
				6,
				3,
				22,
				15,
				17,
				2,
				11,
				23,
				1,
				5,
				16,
				19,
				14,
				8,
				4,
				7,
				9,
				12,
				21,
				10,
				20,
				0,
				0,
				13,
				10,
				2,
				5,
				23,
				14,
				7,
				21,
				19,
				15,
				6,
				22,
				4,
				11,
				12,
				18,
				3,
				1,
				20,
				9,
				16,
				8,
				17,
				15,
				21,
				22,
				8,
				4,
				18,
				11,
				10,
				3,
				17,
				23,
				7,
				12,
				13,
				19,
				2,
				16,
				6,
				14,
				5,
				20,
				1,
				0,
				9,
				12,
				19,
				15,
				10,
				14,
				20,
				16,
				11,
				8,
				5,
				6,
				21,
				22,
				2,
				3,
				9,
				13,
				7,
				17,
				0,
				4,
				18,
				1,
				23,
				0,
				11,
				20,
				2,
				6,
				7,
				23,
				14,
				8,
				16,
				9,
				4,
				3,
				15,
				22,
				19,
				18,
				13,
				21,
				17,
				10,
				1,
				12,
				5,
				14,
				21,
				13,
				9,
				0,
				20,
				7,
				11,
				15,
				18,
				16,
				4,
				6,
				2,
				17,
				12,
				3,
				19,
				5,
				10,
				1,
				8,
				22,
				23,
				2,
				17,
				6,
				10,
				0,
				12,
				23,
				4,
				7,
				9,
				8,
				13,
				3,
				16,
				15,
				14,
				21,
				11,
				19,
				20,
				22,
				1,
				5,
				18,
				10,
				19,
				5,
				21,
				14,
				23,
				3,
				2,
				7,
				16,
				8,
				20,
				9,
				0,
				11,
				12,
				15,
				4,
				13,
				17,
				6,
				1,
				18,
				22,
				6,
				5,
				10,
				2,
				20,
				23,
				13,
				12,
				14,
				22,
				9,
				3,
				1,
				7,
				16,
				0,
				19,
				17,
				21,
				4,
				18,
				15,
				11,
				8,
				16,
				14,
				21,
				18,
				23,
				6,
				5,
				20,
				17,
				0,
				2,
				13,
				4,
				15,
				9,
				1,
				22,
				7,
				3,
				12,
				11,
				8,
				10,
				19,
				13,
				17,
				6,
				0,
				1,
				19,
				11,
				15,
				20,
				4,
				12,
				8,
				9,
				23,
				7,
				5,
				14,
				3,
				2,
				10,
				22,
				16,
				18,
				21,
				11,
				6,
				21,
				0,
				15,
				17,
				1,
				22,
				4,
				12,
				16,
				13,
				19,
				8,
				5,
				23,
				14,
				20,
				10,
				9,
				3,
				7,
				18,
				2,
				16,
				8,
				14,
				17,
				9,
				10,
				20,
				4,
				7,
				18,
				1,
				5,
				3,
				15,
				23,
				6,
				11,
				19,
				21,
				12,
				2,
				22,
				13,
				0
			};
		}

		private void calculateEventPhase()
		{
			DateTime serverTime = getServerTime();
			if (!(serverTime > DateTime.MinValue))
			{
				return;
			}
			int num = serverTime.Hour * 60;
			int num2 = serverTime.Minute / StartOnMinute;
			int num3 = sharedRandom[(num + num2) % 1440];
			int num4 = StartOnMinute * 60;
			int num5 = secondsUntilStart();
			int num6 = num4 - num5;
			if (num6 == num4)
			{
				num6 = 0;
			}
			if (num6 >= 0 && num6 < AttractSeconds)
			{
				int num7 = AttractSeconds - num6;
				if (geyserPhase != GeyserPhase.Attract)
				{
					geyserPhase = GeyserPhase.Attract;
					FogController.ChangeFog();
					isEventFogActive = true;
					isAdjustmentOverride = false;
					setAllCollectiblesVisible(false);
					int num8 = num3 % spawnPoint.Length;
					CollectibleObjects[num8].SetActive(true);
					sparkleEffectActive(true, CollectibleObjects[num8].gameObject.transform.position);
					attractObject.transform.position = spawnPoint[num8].Value;
					setGeyserPhase(GeyserPhase.Attract);
				}
				if (num7 > AttractMedSeconds + AttractHighSeconds)
				{
					setAttractPhase(AttractPhase.Low);
					num7 -= AttractMedSeconds + AttractHighSeconds;
				}
				else if (num7 > AttractHighSeconds)
				{
					setAttractPhase(AttractPhase.Medium);
					num7 -= AttractHighSeconds;
				}
				else
				{
					setAttractPhase(AttractPhase.High);
				}
				Invoke("calculateEventPhase", num7);
			}
			else if (num6 >= AttractSeconds && num6 < AttractSeconds + BoostSeconds)
			{
				int num7 = AttractSeconds + BoostSeconds - num6;
				if (geyserPhase != GeyserPhase.Boost)
				{
					geyserPhase = GeyserPhase.Boost;
					if (!isEventFogActive || isAdjustmentOverride)
					{
						FogController.ChangeFog();
						isEventFogActive = true;
						isAdjustmentOverride = false;
					}
					setAllCollectiblesVisible(false);
					int num8 = num3 % spawnPoint.Length;
					CollectibleObjects[num8].SetActive(true);
					sparkleEffectActive(false);
					setAttractPhase(AttractPhase.Off);
					geyserObject.transform.position = spawnPoint[num8].Value;
					setGeyserPhase(GeyserPhase.Boost);
					springObject = UnityEngine.Object.Instantiate(SpringPrefab);
					springObject.transform.position = spawnPoint[num8].Value;
				}
				Invoke("calculateEventPhase", num7);
			}
			else
			{
				if (num6 < AttractSeconds + BoostSeconds || num6 >= num4)
				{
					return;
				}
				int num7 = secondsUntilStart();
				if (geyserPhase != GeyserPhase.Waiting)
				{
					geyserPhase = GeyserPhase.Waiting;
					if (isEventFogActive || isAdjustmentOverride)
					{
						FogController.RestoreFog();
						isEventFogActive = false;
						isAdjustmentOverride = false;
					}
					setAllCollectiblesVisible(false);
					sparkleEffectActive(false);
					setAttractPhase(AttractPhase.Off);
					setGeyserPhase(GeyserPhase.Waiting);
					UnityEngine.Object.Destroy(springObject);
				}
				Invoke("calculateEventPhase", num7);
			}
		}

		public override void AdjustSceneForQuest(bool isQuestActive)
		{
			CancelInvoke();
			geyserPhase = GeyserPhase.None;
			isAdjustmentOverride = true;
			isEventFogActive = false;
			setAttractPhase(AttractPhase.Off);
			setGeyserPhase(GeyserPhase.Waiting);
			if (springObject != null)
			{
				UnityEngine.Object.Destroy(springObject);
			}
			if (isQuestActive)
			{
				setAllCollectiblesVisible(false);
				FogController.RestoreFog();
			}
			else
			{
				calculateEventPhase();
			}
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

		private void setAllCollectiblesVisible(bool isActive)
		{
			GameObject[] collectibleObjects = CollectibleObjects;
			foreach (GameObject gameObject in collectibleObjects)
			{
				gameObject.SetActive(isActive);
			}
		}

		private void setAttractPhase(AttractPhase phase)
		{
			if (attractAnim != null)
			{
				attractAnim.ResetTrigger("Off");
				attractAnim.ResetTrigger("Low");
				attractAnim.ResetTrigger("Medium");
				attractAnim.ResetTrigger("High");
				switch (phase)
				{
				case AttractPhase.None:
					break;
				case AttractPhase.Off:
					attractAnim.SetTrigger("Off");
					break;
				case AttractPhase.Low:
					attractAnim.SetTrigger("Low");
					break;
				case AttractPhase.Medium:
					attractAnim.SetTrigger("Medium");
					break;
				case AttractPhase.High:
					attractAnim.SetTrigger("High");
					break;
				}
			}
		}

		private void setGeyserPhase(GeyserPhase phase)
		{
			if (geyserAnim != null)
			{
				geyserAnim.ResetTrigger("On");
				geyserAnim.ResetTrigger("Off");
				switch (phase)
				{
				case GeyserPhase.None:
					break;
				case GeyserPhase.Attract:
				case GeyserPhase.Waiting:
					geyserAnim.SetTrigger("Off");
					break;
				case GeyserPhase.Boost:
					geyserAnim.SetTrigger("On");
					break;
				}
			}
		}

		private void sparkleEffectActive(bool isActive, Vector3? worldPos = null)
		{
			if (sparkleObject != null)
			{
				if (worldPos.HasValue)
				{
					sparkleObject.transform.position = worldPos.Value;
				}
				if (isActive)
				{
					sparklePartSys.Play();
				}
				else
				{
					sparklePartSys.Stop();
				}
			}
		}

		private void OnValidate()
		{
			GeyserDiameter = Mathf.Clamp(GeyserDiameter, 0f, 10f);
			AttractLowSeconds = Mathf.Abs(AttractLowSeconds);
			AttractMedSeconds = Mathf.Abs(AttractMedSeconds);
			AttractHighSeconds = Mathf.Abs(AttractHighSeconds);
			if (AttractLowSeconds + AttractMedSeconds + AttractHighSeconds != AttractSeconds)
			{
				AttractSeconds = AttractLowSeconds + AttractMedSeconds + AttractHighSeconds;
			}
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			CancelInvoke();
		}
	}
}
