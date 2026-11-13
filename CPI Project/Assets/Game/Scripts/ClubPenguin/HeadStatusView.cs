using ClubPenguin.Cinematography;
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin
{
	[DisallowMultipleComponent]
	public class HeadStatusView : MonoBehaviour
	{
		private bool isLocalPlayer;

		public static readonly Dictionary<TemporaryHeadStatusType, TemporaryHeadStatusDefinition> HeadStatusToDefinition = new Dictionary<TemporaryHeadStatusType, TemporaryHeadStatusDefinition>();

		public PrefabContentKey MembershipUnlockFXKey = new PrefabContentKey("FX/Character/Prefabs/MembershipUnlockFX");

		public PrefabContentKey MembershipUnlockAnimationKey = new PrefabContentKey("CelebrationAnimations/AllAccessCelebration");

		public PrefabContentKey LevelUpFXKey = new PrefabContentKey("FX/Character/Prefabs/LevelUp");

		private GameObject membershipUnlockAnimationPrefab;

		private GameObject membershipUnlockFXPrefab;

		private GameObject levelUpFXPrefab;

		[Invokable("Avatar.Particles.Trophy Platinum", Description = "Tests out the particles for race finished")]
		[PublicTweak]
		public static void TestParticlesTrophyA()
		{
			GameObject gameObject = GameObject.Find("Penguin");
			if (gameObject != null)
			{
				HeadStatusView component = gameObject.GetComponent<HeadStatusView>();
				component.LoadParticlePrefab(TemporaryHeadStatusType.TrophyA);
				Service.Get<INetworkServicesManager>().PlayerStateService.SetTemporaryHeadStatus(1);
			}
		}

		[PublicTweak]
		[Invokable("Avatar.Particles.Trophy Gold", Description = "Tests out the particles for race finished")]
		public static void TestParticlesTrophyB()
		{
			GameObject gameObject = GameObject.Find("Penguin");
			if (gameObject != null)
			{
				HeadStatusView component = gameObject.GetComponent<HeadStatusView>();
				component.LoadParticlePrefab(TemporaryHeadStatusType.TrophyB);
				Service.Get<INetworkServicesManager>().PlayerStateService.SetTemporaryHeadStatus(2);
			}
		}

		[PublicTweak]
		[Invokable("Avatar.Particles.Trophy Silver", Description = "Tests out the particles for race finished")]
		public static void TestParticlesTrophyC()
		{
			GameObject gameObject = GameObject.Find("Penguin");
			if (gameObject != null)
			{
				HeadStatusView component = gameObject.GetComponent<HeadStatusView>();
				component.LoadParticlePrefab(TemporaryHeadStatusType.TrophyC);
				Service.Get<INetworkServicesManager>().PlayerStateService.SetTemporaryHeadStatus(3);
			}
		}

		[Invokable("Avatar.Particles.Trophy Bronze", Description = "Tests out the particles for race finished")]
		[PublicTweak]
		public static void TestParticlesTrophyD()
		{
			GameObject gameObject = GameObject.Find("Penguin");
			if (gameObject != null)
			{
				HeadStatusView component = gameObject.GetComponent<HeadStatusView>();
				component.LoadParticlePrefab(TemporaryHeadStatusType.TrophyD);
				Service.Get<INetworkServicesManager>().PlayerStateService.SetTemporaryHeadStatus(4);
			}
		}

		[Invokable("Avatar.Particles.CFC Donation", Description = "Tests out the particles for CFC Donation")]
		[PublicTweak]
		public static void TestParticlesCFCDonation()
		{
			GameObject gameObject = GameObject.Find("Penguin");
			if (gameObject != null)
			{
				HeadStatusView component = gameObject.GetComponent<HeadStatusView>();
				component.LoadParticlePrefab(TemporaryHeadStatusType.CFCDonation);
				Service.Get<INetworkServicesManager>().PlayerStateService.SetTemporaryHeadStatus(4);
			}
		}

		[Invokable("Avatar.Particles.Membership Unlock", Description = "Tests out the membership unlock particles")]
		[PublicTweak]
		public static void TestParticlesMembershipUnlock()
		{
			GameObject gameObject = GameObject.Find("Penguin");
			if (gameObject != null)
			{
				HeadStatusView component = gameObject.GetComponent<HeadStatusView>();
				if (component != null)
				{
					component.membershipUnlockAnimationPrefab = null;
					component.membershipUnlockFXPrefab = null;
					if (component.MembershipUnlockAnimationKey != null && !string.IsNullOrEmpty(component.MembershipUnlockAnimationKey.Key))
					{
						Content.LoadAsync(component.onTestMembershipUnlockAnimationLoaded, component.MembershipUnlockAnimationKey);
					}
					if (component.MembershipUnlockFXKey != null && !string.IsNullOrEmpty(component.MembershipUnlockFXKey.Key))
					{
						Content.LoadAsync(component.onTestMembershipUnlockFXLoaded, component.MembershipUnlockFXKey);
					}
				}
			}
		}

		[Invokable("Avatar.Particles.Level Up", Description = "Tests out the level up particles")]
		[PublicTweak]
		public static void TestParticlesLevelUp()
		{
			GameObject gameObject = GameObject.Find("Penguin");
			if (gameObject != null)
			{
				HeadStatusView component = gameObject.GetComponent<HeadStatusView>();
				if (component != null && component.LevelUpFXKey != null && !string.IsNullOrEmpty(component.LevelUpFXKey.Key))
				{
					Content.LoadAsync(component.onTestLevelUpFXLoaded, component.LevelUpFXKey);
				}
			}
		}

		public void Start()
		{
			isLocalPlayer = (base.gameObject == SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject);
			if (isLocalPlayer)
			{
				Service.Get<EventDispatcher>().AddListener<HeadStatusEvents.ShowHeadStatus>(onShowHeadStatus);
			}
		}

		public void OnDestroy()
		{
			if (isLocalPlayer)
			{
				Service.Get<EventDispatcher>().RemoveListener<HeadStatusEvents.ShowHeadStatus>(onShowHeadStatus);
			}
		}

		private bool onShowHeadStatus(HeadStatusEvents.ShowHeadStatus evt)
		{
			showHeadStatus(evt.StatusType);
			return false;
		}

		private void showHeadStatus(TemporaryHeadStatusType type)
		{
			LoadParticlePrefab(type);
			Service.Get<INetworkServicesManager>().PlayerStateService.SetTemporaryHeadStatus((int)type);
		}

		public void LoadParticlePrefab(TemporaryHeadStatusType trophy)
		{
			if (trophy != 0 && HeadStatusToDefinition.ContainsKey(trophy))
			{
				Content.LoadAsync(OnParticlesLoaded, HeadStatusToDefinition[trophy].EffectsContentKey);
			}
		}

		private void OnParticlesLoaded(string key, GameObject asset)
		{
			if (!base.gameObject.IsDestroyed())
			{
				spawnEffect(asset);
				if (isLocalPlayer)
				{
					StartCoroutine(DelayResetStatus(15f));
				}
			}
		}

		private void OnMembershipUnlockFXLoaded(string key, GameObject asset)
		{
			if (!base.gameObject.IsDestroyed())
			{
				spawnEffect(asset);
			}
		}

		private void onTestMembershipUnlockAnimationLoaded(string path, GameObject prefab)
		{
			membershipUnlockAnimationPrefab = prefab;
			if (membershipUnlockFXPrefab != null)
			{
				playTestMembershipUnlock();
			}
		}

		private void onTestMembershipUnlockFXLoaded(string path, GameObject prefab)
		{
			membershipUnlockFXPrefab = prefab;
			if (membershipUnlockAnimationPrefab != null)
			{
				playTestMembershipUnlock();
			}
			else
			{
				spawnEffect(prefab);
			}
		}

		private void playTestMembershipUnlock()
		{
			GameObject gameObject = Object.Instantiate(membershipUnlockAnimationPrefab);
			PenguinCelebrationAnimation component = gameObject.GetComponent<PenguinCelebrationAnimation>();
			component.DelaySeconds = 0f;
			component.FramerOffset = new Vector3(0f, 0.1f, 0f);
			component.OnAnimationStarted += onTestMembershipUnlockAnimationStarted;
			component.OnAnimationEnded += onTestMembershipUnlockAnimationEnded;
		}

		private void onTestMembershipUnlockAnimationStarted()
		{
			if (membershipUnlockFXPrefab != null)
			{
				GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
				if (localPlayerGameObject != null)
				{
					GameObject gameObject = Object.Instantiate(membershipUnlockFXPrefab);
					gameObject.transform.SetParent(localPlayerGameObject.transform, false);
				}
			}
		}

		private void onTestMembershipUnlockAnimationEnded(bool animationComplete)
		{
		}

		private void onTestLevelUpFXLoaded(string path, GameObject prefab)
		{
			levelUpFXPrefab = prefab;
			playTestLevelUp();
		}

		private void playTestLevelUp()
		{
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (localPlayerGameObject != null)
			{
				GameObject gameObject = Object.Instantiate(levelUpFXPrefab);
				gameObject.transform.SetParent(localPlayerGameObject.transform, false);
			}
		}

		private void spawnEffect(GameObject asset)
		{
			GameObject gameObject = Object.Instantiate(asset);
			gameObject.transform.SetParent(base.transform, false);
			CameraCullingMaskHelper.SetLayerRecursive(gameObject.transform, LayerMask.LayerToName(base.gameObject.layer));
		}

		private IEnumerator DelayResetStatus(float waitTime)
		{
			yield return new WaitForSeconds(waitTime);
			Service.Get<INetworkServicesManager>().PlayerStateService.SetTemporaryHeadStatus(0);
		}
	}
}
