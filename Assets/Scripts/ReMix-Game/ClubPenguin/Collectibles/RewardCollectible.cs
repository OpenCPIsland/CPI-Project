using ClubPenguin.Analytics;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native.iOS;
using Fabric;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Collectibles
{
	[RequireComponent(typeof(Animator))]
	public class RewardCollectible : Collectible
	{
		public GameObject ParticlesAppear;

		public GameObject ParticlesPickup;

		public GameObject ParticlesReward;

		[Tooltip("Time to wait until before spawning particles")]
		public float ParticlesWaitTime = 0f;

		[Tooltip("Offset from object's zero-point")]
		public Vector3 ParticlesOffset = Vector3.zero;

		public Color ParticleSpawnPointColor = Color.red;

		[Tooltip("Drag the interaction object here")]
		public GameObject InteractionObj;

		[Tooltip("Animator bool variable that change object's state")]
		public string NameIsCollected;

		private Renderer[] rendObjects;

		private Animator animController;

		private RewardState internalState = RewardState.COLLECTED_TODAY;

		private int hashParamIsCollected;

		private bool hasValidParameter = false;

		public override void Awake()
		{
			base.Awake();
			if (InteractionObj != null)
			{
				base.InteractionPath = InteractionObj.GetPath();
			}
			else
			{
				Log.LogError(this, string.Format("{0} requires an interation object to be defined on the prefab.", base.Path));
			}
			animController = base.gameObject.GetComponent<Animator>();
			rendObjects = base.gameObject.GetComponentsInChildren<Renderer>();
			if (!string.IsNullOrEmpty(NameIsCollected))
			{
				hashParamIsCollected = Animator.StringToHash(NameIsCollected);
				hasValidParameter = true;
			}
			else
			{
				Log.LogError(this, string.Format("{0} is missing it's collected variable name", base.gameObject.name));
			}
			changeState(RewardState.INVISIBLE);
		}

		public override void StartCollectible(RespawnResponse respawnResponse)
		{
			isInitialized = true;
			checkIfCollected(respawnResponse);
		}

		private void checkIfCollected(RespawnResponse respawnResponse)
		{
			switch (respawnResponse.State)
			{
			case RespawnState.NOT_AVAILABLE:
				setNotAvailable();
				break;
			case RespawnState.READY_FOR_PICKUP:
				setAvailable();
				break;
			case RespawnState.WAITING_TO_RESPAWN:
				registerToRespawn(respawnResponse.Time);
				setNotAvailable();
				break;
			}
		}

		public void OnActionGraphActivation()
		{
			if (internalState == RewardState.READY_TO_ACTIVATE)
			{
				Service.Get<iOSHapticFeedback>().TriggerImpactFeedback(iOSHapticFeedback.ImpactFeedbackStyle.Medium);
				int num = CoinRewardableDefinition.Coins(RewardDef);
				List<CollectibleRewardDefinition> definitions = RewardDef.GetDefinitions<CollectibleRewardDefinition>();
				if (num > 0 && definitions.Count > 0)
				{
					string collectibleType = definitions[0].Collectible.CollectibleType;
					int num2 = num;
					Service.Get<ICPSwrveService>().CoinsGiven(num2, "picked_up", collectibleType);
				}
				setPickedUp();
				sendQuestEvent();
			}
		}

		private void changeState(RewardState newState)
		{
			switch (newState)
			{
			case RewardState.NONE:
				break;
			case RewardState.INVISIBLE:
				setRenders(false);
				break;
			case RewardState.READY_TO_ACTIVATE:
				setRenders(true);
				internalState = newState;
				if (hasValidParameter)
				{
					animController.SetBool(hashParamIsCollected, false);
				}
				if (InteractionObj != null)
				{
					InteractionObj.SetActive(true);
				}
				break;
			case RewardState.COLLECTED_TODAY:
				setRenders(true);
				internalState = newState;
				if (hasValidParameter)
				{
					animController.SetBool(hashParamIsCollected, true);
				}
				if (InteractionObj != null)
				{
					InteractionObj.SetActive(false);
				}
				break;
			}
		}

		private void setPickedUp()
		{
			StartCoroutine(AddParticlesAfterPause(ParticlesWaitTime));
			playSound();
			setNotAvailable();
		}

		private void setNotAvailable()
		{
			changeState(RewardState.COLLECTED_TODAY);
		}

		private void setAvailable()
		{
			if (ParticlesAppear != null)
			{
				Object.Instantiate(ParticlesAppear, base.gameObject.transform.position, Quaternion.identity);
			}
			changeState(RewardState.READY_TO_ACTIVATE);
		}

		private void playSound()
		{
			if (!string.IsNullOrEmpty(AudioEvent))
			{
				EventManager.Instance.PostEvent(AudioEvent, EventAction.PlaySound);
			}
		}

		private IEnumerator AddParticlesAfterPause(float timeWait)
		{
			yield return new WaitForSeconds(timeWait);
			if (ParticlesPickup != null)
			{
				Object.Instantiate(ParticlesPickup, base.gameObject.transform.position + ParticlesOffset, ParticlesPickup.transform.rotation);
			}
			if (ParticlesReward != null)
			{
				Object.Instantiate(ParticlesReward, base.gameObject.transform.position + ParticlesOffset, ParticlesReward.transform.rotation);
			}
		}

		private void setRenders(bool isVisible)
		{
			int num = rendObjects.Length;
			for (int i = 0; i < num; i++)
			{
				rendObjects[i].enabled = isVisible;
			}
		}

		public override void OnDrawGizmosSelected()
		{
			base.OnDrawGizmosSelected();
			Gizmos.color = ParticleSpawnPointColor;
			Vector3 vector = base.gameObject.transform.position + ParticlesOffset;
			Gizmos.DrawSphere(vector, 0.05f);
			Gizmos.DrawLine(vector + new Vector3(-1f, 0f, 0f), vector + new Vector3(1f, 0f, 0f));
			Gizmos.DrawLine(vector + new Vector3(0f, -1f, 0f), vector + new Vector3(0f, 1f, 0f));
			Gizmos.DrawLine(vector + new Vector3(0f, 0f, -1f), vector + new Vector3(0f, 0f, 1f));
		}

		public override void RespawnCollectible()
		{
			setAvailable();
		}
	}
}
