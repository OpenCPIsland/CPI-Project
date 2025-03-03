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
				GameObject gameObject = Object.Instantiate(asset);
				gameObject.transform.SetParent(base.transform, false);
				CameraCullingMaskHelper.SetLayerRecursive(gameObject.transform, LayerMask.LayerToName(base.gameObject.layer));
				if (isLocalPlayer)
				{
					StartCoroutine(DelayResetStatus(15f));
				}
			}
		}

		private IEnumerator DelayResetStatus(float waitTime)
		{
			yield return new WaitForSeconds(waitTime);
			Service.Get<INetworkServicesManager>().PlayerStateService.SetTemporaryHeadStatus(0);
		}
	}
}
