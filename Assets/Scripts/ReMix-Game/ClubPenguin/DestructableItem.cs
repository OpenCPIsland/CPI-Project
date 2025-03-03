using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.Props;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class DestructableItem : ProximityBroadcaster
	{
		private const string SWORD_TAG = "SwordCollider";

		private const string SWORD_NAME = "Prop_Sword";

		private const string SNOWBALL_TAG = "Snowball";

		[Header("Properties")]
		public MeshRenderer MeshRenderer;

		public Animator Animator;

		public int HitPoints = 1;

		[Header("Destruction Methods")]
		public bool LocalPlayerCollision = true;

		public bool RemotePlayerCollision = false;

		public bool AnySwordCollision = false;

		public PropDefinition[] PropCollision;

		public float PropTriggerDistance = 1f;

		[Range(0f, 1f)]
		public float PropEndPercentage = 0.95f;

		public iTween.EaseType DamageEaseType = iTween.EaseType.easeInCubic;

		public float DamageTweenTime = 0.5f;

		public Vector3 DamageDimensions = new Vector3(0.1f, 0.1f, 0.1f);

		[Header("Restore Method")]
		public bool RestoreOnTriggerExit = true;

		public float RestoreAfterSeconds = 0f;

		[Header("Animation Settings")]
		public string AnimAnticipationStart;

		public string AnimAnticipationStop;

		public bool UseAnimTriggers = true;

		public bool UseMeshFrames = false;

		public string MeshSearchRoot;

		[Header("Thresholds - must start at 0")]
		public DestructableData[] ThresholdData;

		[Header("Particle Effects")]
		public ParticleSystem AppearParticles;

		public ParticleSystem DamagedParticles;

		public ParticleSystem DestroyedParticles;

		public Vector3 ParticleSpawnOffset = Vector3.zero;

		[Header("Audio Effects")]
		public string AudioReappear;

		public string AudioDamage;

		public string AudioDestroyed;

		private int currentHitPoints;

		private bool prevRestoreOnTriggerExit;

		private float prevRestoreAfterSeconds;

		private float savedRestoreAfterSeconds;

		private bool prevUseAnimTriggers;

		private bool prevUseMeshFrames;

		private List<GameObject> meshFrames = new List<GameObject>();

		private CPDataEntityCollection dataEntityCollection;

		private EventChannel eventChannel;

		public override void Start()
		{
			base.Start();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<InputEvents.ActionEvent>(onActionEvent);
			eventChannel.AddListener<PlayerActionServiceEvents.LocomotionActionReceived>(onLocomotionAction);
			initialize();
		}

		private void initialize()
		{
			currentHitPoints = HitPoints;
			prevRestoreOnTriggerExit = RestoreOnTriggerExit;
			prevRestoreAfterSeconds = RestoreAfterSeconds;
			savedRestoreAfterSeconds = RestoreAfterSeconds;
			prevUseAnimTriggers = UseAnimTriggers;
			prevUseMeshFrames = UseMeshFrames;
			if (UseAnimTriggers)
			{
				setAnimTrigger(ThresholdData[ThresholdData.Length - 1].TriggerName);
				return;
			}
			if (meshFrames.Count == 0)
			{
				int num = ThresholdData.Length;
				if (!string.IsNullOrEmpty(MeshSearchRoot) && !MeshSearchRoot.EndsWith("/"))
				{
					MeshSearchRoot += "/";
				}
				for (int i = 0; i < num; i++)
				{
					bool flag = false;
					Transform transform = base.transform.Find(MeshSearchRoot + ThresholdData[i].TriggerName);
					if (transform != null)
					{
						meshFrames.Add(transform.gameObject);
						flag = true;
					}
					if (!flag)
					{
						Log.LogError(this, string.Format("O_o\t Can't find mesh frame named '{0}' on gameobject {1}", ThresholdData[i].TriggerName, base.gameObject.GetPath()));
					}
				}
				if (meshFrames.Count != ThresholdData.Length)
				{
					throw new Exception("Can't continue, missing mesh frames on gameobject: " + base.gameObject.GetPath());
				}
			}
			setMeshFrame(currentHitPoints);
		}

		private void OnTriggerEnter(Collider coll)
		{
			if ((LocalPlayerCollision && coll.CompareTag("Player")) || (RemotePlayerCollision && coll.CompareTag("RemotePlayer")) || (AnySwordCollision && coll.CompareTag("SwordCollider")))
			{
				damageItem();
			}
		}

		public override void OnProximityExit(ProximityListener other)
		{
			if (RestoreOnTriggerExit)
			{
				resetItem();
			}
		}

		private bool onLocomotionAction(PlayerActionServiceEvents.LocomotionActionReceived evt)
		{
			InputEvents.Actions action;
			switch (evt.Action.Type)
			{
			case LocomotionAction.Action1:
				action = InputEvents.Actions.Action1;
				break;
			case LocomotionAction.Action2:
				action = InputEvents.Actions.Action2;
				break;
			case LocomotionAction.Action3:
				action = InputEvents.Actions.Action3;
				break;
			case LocomotionAction.Interact:
				action = InputEvents.Actions.Interact;
				break;
			case LocomotionAction.Jump:
				action = InputEvents.Actions.Jump;
				break;
			default:
				action = InputEvents.Actions.None;
				break;
			}
			processEvent(action, evt.SessionId);
			return false;
		}

		private bool onActionEvent(InputEvents.ActionEvent evt)
		{
			processEvent(evt.Action, 0L);
			return false;
		}

		private void processEvent(InputEvents.Actions action, long sessionId = 0L)
		{
			if (base.gameObject.IsDestroyed())
			{
				return;
			}
			bool flag = isMatchingProp(0L);
			GameObject playerGameObj = getPlayerGameObj(sessionId);
			float num = float.MaxValue;
			if (playerGameObj != null)
			{
				num = Vector2.Distance(new Vector2(playerGameObj.transform.position.x, playerGameObj.transform.position.z), new Vector2(base.transform.position.x, base.transform.position.z));
			}
			bool flag2 = num <= PropTriggerDistance;
			int num2;
			switch (action)
			{
			case InputEvents.Actions.Jump:
				resetAnimTrigger(AnimAnticipationStart);
				resetAnimTrigger(AnimAnticipationStop);
				setAnimTrigger(AnimAnticipationStop);
				return;
			default:
				num2 = ((action != InputEvents.Actions.Action3) ? 1 : 0);
				break;
			case InputEvents.Actions.Action1:
			case InputEvents.Actions.Action2:
				num2 = 0;
				break;
			}
			if (num2 == 0 && currentHitPoints > 0)
			{
				CoroutineRunner.StopAllForOwner(this);
				CancelInvoke();
				if (flag && flag2)
				{
					CoroutineRunner.Start(delayedDamageItemStart(0.2f, playerGameObj), this, "delayedDamageItemStart");
					resetAnimTrigger(AnimAnticipationStart);
					setAnimTrigger(AnimAnticipationStart);
				}
			}
		}

		private IEnumerator delayedDamageItemStart(float waitTime, GameObject playerObject)
		{
			yield return new WaitForSeconds(waitTime);
			if (playerObject != null)
			{
				Animator component = playerObject.GetComponent<Animator>();
				float num = component.GetCurrentAnimatorStateInfo(0).length - component.GetCurrentAnimatorStateInfo(0).length * component.GetCurrentAnimatorStateInfo(0).normalizedTime;
				Invoke("delayedDamageItemStop", num * PropEndPercentage);
			}
		}

		private void delayedDamageItemStop()
		{
			resetAnimTrigger(AnimAnticipationStop);
			setAnimTrigger(AnimAnticipationStop);
			damageItem();
		}

		private void damageItem()
		{
			if (currentHitPoints <= 0)
			{
				return;
			}
			currentHitPoints--;
			int num = ThresholdData.Length;
			for (int i = 0; i < num; i++)
			{
				if (currentHitPoints == ThresholdData[i].Threshold)
				{
					if (i == 0 && DestroyedParticles != null)
					{
						createParticles(DestroyedParticles);
					}
					else
					{
						createParticles(DamagedParticles);
					}
					if (UseAnimTriggers)
					{
						setAnimTrigger(ThresholdData[i].TriggerName);
					}
					else
					{
						setMeshFrame(currentHitPoints);
					}
					if (currentHitPoints > 0)
					{
						iTween.PunchPosition(base.gameObject, iTween.Hash("position", base.gameObject.transform.position, "amount", DamageDimensions, "easeType", DamageEaseType, "time", DamageTweenTime));
						SoundUtils.PlayAudioEvent(AudioDamage, base.gameObject);
					}
					else
					{
						SoundUtils.PlayAudioEvent(AudioDestroyed, base.gameObject);
					}
				}
			}
			if (currentHitPoints <= 0)
			{
				currentHitPoints = 0;
				if (RestoreAfterSeconds > 0f)
				{
					Invoke("resetItem", RestoreAfterSeconds);
				}
			}
		}

		private void resetItem()
		{
			SoundUtils.PlayAudioEvent(AudioReappear, base.gameObject);
			createParticles(AppearParticles);
			initialize();
		}

		private bool isMatchingProp(long sessionId = 0L)
		{
			bool result = false;
			int num = PropCollision.Length;
			for (int i = 0; i < num; i++)
			{
				string currentPropName = getCurrentPropName(sessionId);
				if (currentPropName != "Prop_Sword" && currentPropName == PropCollision[i].NameOnServer)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private string getCurrentPropName(long sessionId = 0L)
		{
			GameObject gameObject = (sessionId == 0) ? SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject : getPlayerGameObj(sessionId);
			if (gameObject != null)
			{
				PropUser component = gameObject.GetComponent<PropUser>();
				if (component != null && component.Prop != null)
				{
					return component.Prop.PropId;
				}
			}
			return "";
		}

		private void setAnimTrigger(string triggerName)
		{
			if (Animator != null && !string.IsNullOrEmpty(triggerName))
			{
				Animator.SetTrigger(triggerName);
			}
		}

		private void resetAnimTrigger(string triggerName)
		{
			if (Animator != null && !string.IsNullOrEmpty(triggerName))
			{
				Animator.ResetTrigger(triggerName);
			}
		}

		private void setMeshFrame(int hitPoints)
		{
			int num = ThresholdData.Length;
			for (int i = 0; i < num; i++)
			{
				bool active = false;
				if (ThresholdData[i].Threshold == hitPoints)
				{
					active = true;
				}
				meshFrames[i].SetActive(active);
			}
		}

		private GameObject getPlayerGameObj(long sessionId = 0L)
		{
			GameObject result = null;
			if (sessionId != 0)
			{
				DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntity<SessionIdData, long>(sessionId);
				if (dataEntityHandle != null)
				{
					GameObjectReferenceData component;
					dataEntityCollection.TryGetComponent(dataEntityHandle, out component);
					if (component != null)
					{
						result = component.GameObject;
					}
				}
			}
			else
			{
				result = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			}
			return result;
		}

		private void createParticles(ParticleSystem effectParticles)
		{
			if (effectParticles != null)
			{
				ParticleSystem particleSystem = null;
				particleSystem = UnityEngine.Object.Instantiate(effectParticles);
				if (particleSystem != null)
				{
					particleSystem.transform.position = base.transform.position + ParticleSpawnOffset;
					particleSystem.Play();
				}
			}
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			CoroutineRunner.StopAllForOwner(this);
			CancelInvoke();
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
		}

		public void OnValidate()
		{
			HitPoints = Mathf.Clamp(HitPoints, 1, int.MaxValue);
			currentHitPoints = HitPoints;
			if (RestoreOnTriggerExit != prevRestoreOnTriggerExit)
			{
				if (RestoreOnTriggerExit)
				{
					savedRestoreAfterSeconds = RestoreAfterSeconds;
					RestoreAfterSeconds = 0f;
				}
				else
				{
					RestoreAfterSeconds = savedRestoreAfterSeconds;
				}
			}
			else if (!prevRestoreAfterSeconds.Equals(RestoreAfterSeconds))
			{
				RestoreOnTriggerExit = false;
				savedRestoreAfterSeconds = RestoreAfterSeconds;
			}
			prevRestoreOnTriggerExit = RestoreOnTriggerExit;
			prevRestoreAfterSeconds = RestoreAfterSeconds;
			int num = ThresholdData.Length;
			for (int i = 0; i < num; i++)
			{
				ThresholdData[i].Threshold = Mathf.Clamp(ThresholdData[i].Threshold, 0, HitPoints);
			}
			if (num > 1)
			{
				ThresholdData[0].Threshold = 0;
				ThresholdData[num - 1].Threshold = HitPoints;
			}
			if (UseAnimTriggers != prevUseAnimTriggers)
			{
				UseMeshFrames = !UseAnimTriggers;
			}
			else if (UseMeshFrames != prevUseMeshFrames)
			{
				UseAnimTriggers = !UseMeshFrames;
			}
			prevUseAnimTriggers = UseAnimTriggers;
			prevUseMeshFrames = UseMeshFrames;
		}

		public override void OnDrawGizmosSelected()
		{
			base.OnDrawGizmosSelected();
			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere(base.transform.position + ParticleSpawnOffset, 0.05f);
			float num = 0.5f;
			Gizmos.DrawLine(base.transform.position + ParticleSpawnOffset + new Vector3(0f - num, 0f, 0f), base.transform.position + ParticleSpawnOffset + new Vector3(num, 0f, 0f));
			Gizmos.DrawLine(base.transform.position + ParticleSpawnOffset + new Vector3(0f, 0f, 0f - num), base.transform.position + ParticleSpawnOffset + new Vector3(0f, 0f, num));
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(base.transform.position, PropTriggerDistance);
		}
	}
}
