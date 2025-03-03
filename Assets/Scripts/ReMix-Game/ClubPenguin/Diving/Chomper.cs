using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.UI;
using ClubPenguin.World.Activities.Diving;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Diving
{
	[RequireComponent(typeof(Animator))]
	public class Chomper : ProximityBroadcaster
	{
		private const float LUNGE_DISTANCE = 2f;

		private static int HASH_PARAM_WARN_DISTANCE = Animator.StringToHash("WarningDistance");

		private static int HASH_PARAM_ANIM_STATE = Animator.StringToHash("AnimationState");

		public float SpitStrength = 1f;

		private Animator anim;

		private GameObject penguinObj;

		private Collider savedCollider;

		private bool isPenguinGrabbed = false;

		private ChomperState chomperState = ChomperState.IDLE;

		private ChomperAnimation oldChomperAnimation = ChomperAnimation.NONE;

		private PenguinUserControl userControl;

		private SwimController swimControl;

		private SkinnedMeshRenderer[] skinnedMeshRenderers;

		private MeshRenderer[] meshRenderers;

		private Vector3 newDirection = Vector3.zero;

		private float rotateSpeed = 0f;

		private float idleTimeout = 0f;

		private float idleMinTime = 5f;

		private float idleMaxTime = 15f;

		private bool playerResurfaced = false;

		private EventDispatcher dispatcher;

		public bool IsPenguinGrabbed
		{
			get
			{
				return isPenguinGrabbed;
			}
			set
			{
				isPenguinGrabbed = value;
			}
		}

		public override void Awake()
		{
			base.Awake();
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<DivingEvents.PlayerResurfaced>(onPlayerResurfaced);
			anim = GetComponent<Animator>();
			chomperState = ChomperState.IDLE;
			setAnimation(ChomperAnimation.ANIM_IDLE_DEFAULT);
			savedCollider = null;
			isPenguinGrabbed = false;
		}

		public override void Start()
		{
			base.Start();
			penguinObj = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			dispatcher.RemoveListener<DivingEvents.PlayerResurfaced>(onPlayerResurfaced);
		}

		private void FixedUpdate()
		{
			if (chomperState != ChomperState.IDLE)
			{
				return;
			}
			if (Time.time >= idleTimeout)
			{
				idleTimeout = Time.time + Random.Range(idleMinTime, idleMaxTime);
				newDirection = new Vector3(0f, Random.Range(0f, 90f), 0f);
				if ((double)Random.value > 0.5)
				{
					newDirection.y += 270f;
				}
				rotateSpeed = Random.Range(30f, 60f);
			}
			float y = Mathf.MoveTowardsAngle(base.gameObject.transform.eulerAngles.y, newDirection.y, Time.deltaTime * rotateSpeed);
			base.gameObject.transform.eulerAngles = new Vector3(0f, y, 0f);
		}

		public override void OnProximityEnter(ProximityListener other)
		{
			if (other.gameObject.CompareTag("Player"))
			{
				chomperState = ChomperState.HUNGRY;
				setAnimation(ChomperAnimation.ANIM_IDLE_HUNGRY);
				savedCollider = penguinObj.GetComponent<Collider>();
				isPenguinGrabbed = false;
			}
		}

		public override void OnProximityStay(ProximityListener other)
		{
			if (!other.gameObject.CompareTag("Player"))
			{
				return;
			}
			float num = calcDistance();
			float value = num;
			value = 3f - Mathf.Clamp(value, 2.5f, 3f);
			value = 1f - value * 2f;
			anim.SetFloat(HASH_PARAM_WARN_DISTANCE, value);
			switch (chomperState)
			{
			case ChomperState.HUNGRY:
				if (num <= 2f)
				{
					chomperState = ChomperState.LUNGE;
					break;
				}
				setAnimation(ChomperAnimation.ANIM_IDLE_HUNGRY);
				base.transform.LookAt(LookAtPlayer(chomperState));
				break;
			case ChomperState.LUNGE:
				setAnimation(ChomperAnimation.ANIM_LUNGE);
				base.transform.LookAt(LookAtPlayer(chomperState));
				if (IsPenguinGrabbed)
				{
					chomperState = ChomperState.LUNGE_SUCCESS;
					dispatcher.DispatchEvent(new DivingEvents.CollisionEffects(penguinObj.tag));
				}
				break;
			case ChomperState.LUNGE_SUCCESS:
				enablePenguinSwallow(false);
				setAnimation(ChomperAnimation.ANIM_SUCCESS);
				chomperState = ChomperState.LUNGE_SUCCESS_SPIT_WAIT;
				break;
			case ChomperState.LUNGE_SUCCESS_SPIT_COMPLETE:
				anim.SetFloat(HASH_PARAM_WARN_DISTANCE, 1f);
				enablePenguinSwallow(true);
				if (savedCollider != null)
				{
					Vector3 vector = (penguinObj.transform.position - base.transform.position).normalized * SpitStrength;
					Rigidbody attachedRigidbody = savedCollider.attachedRigidbody;
					if (attachedRigidbody != null)
					{
						attachedRigidbody.AddForce(vector, ForceMode.Impulse);
					}
					else
					{
						LocomotionController currentController = LocomotionHelper.GetCurrentController(savedCollider.gameObject);
						if (currentController != null)
						{
							currentController.SetForce(vector, base.gameObject);
						}
					}
				}
				isPenguinGrabbed = false;
				chomperState = ChomperState.LUNGE_SUCCESS_ANIMATION_WAIT;
				break;
			case ChomperState.LUNGE_SUCCESS_ANIMATION_COMPLETE:
				StartCoroutine(PauseChomper(3f));
				chomperState = ChomperState.REVIVE_PAUSE_WAIT;
				break;
			case ChomperState.LUNGE_MISS:
				setAnimation(ChomperAnimation.ANIM_MISS);
				break;
			case ChomperState.LUNGE_MISS_ANIMATION_COMPLETE:
				StartCoroutine(PauseChomper(1f));
				chomperState = ChomperState.REVIVE_PAUSE_WAIT;
				break;
			default:
				Log.LogError(this, string.Format("O_o\t ChomperController.OnTriggerStay: ERROR: illegal internalState {0}, defaulting to hungry", (int)chomperState));
				chomperState = ChomperState.HUNGRY;
				setAnimation(ChomperAnimation.ANIM_IDLE_HUNGRY);
				break;
			case ChomperState.LUNGE_SUCCESS_SPIT_WAIT:
			case ChomperState.LUNGE_SUCCESS_ANIMATION_WAIT:
			case ChomperState.LUNGE_MISS_ANIMATION_WAIT:
			case ChomperState.REVIVE_PAUSE_WAIT:
				break;
			}
			if (chomperState == ChomperState.LUNGE_SUCCESS_ANIMATION_WAIT)
			{
				return;
			}
			Vector3 eulerAngles = base.gameObject.transform.eulerAngles;
			if (chomperState == ChomperState.LUNGE)
			{
				eulerAngles.x = (eulerAngles.x + 90f) % 360f;
				if (eulerAngles.x > 90f && eulerAngles.x <= 180f)
				{
					eulerAngles.x = 90f;
				}
				else if (eulerAngles.x > 180f && eulerAngles.x < 270f)
				{
					eulerAngles.x = 270f;
				}
			}
			else
			{
				eulerAngles.x = 0f;
			}
			if (eulerAngles.y > 90f && eulerAngles.y <= 180f)
			{
				eulerAngles.y = 90f;
			}
			else if (eulerAngles.y > 180f && eulerAngles.y < 270f)
			{
				eulerAngles.y = 270f;
			}
			base.gameObject.transform.eulerAngles = eulerAngles;
		}

		public override void OnProximityExit(ProximityListener other)
		{
			if (other.gameObject.CompareTag("Player"))
			{
				anim.SetFloat(HASH_PARAM_WARN_DISTANCE, 1f);
				savedCollider = null;
				chomperState = ChomperState.IDLE;
				base.transform.LookAt(LookAtPlayer(chomperState));
				setAnimation(ChomperAnimation.ANIM_IDLE_DEFAULT);
			}
		}

		private float calcDistance()
		{
			return Vector2.Distance(base.gameObject.transform.position, penguinObj.transform.position);
		}

		private Vector3 LookAtPlayer(ChomperState state)
		{
			return (state == ChomperState.LUNGE) ? new Vector3(penguinObj.transform.position.x, penguinObj.transform.position.y, base.transform.position.z) : new Vector3(penguinObj.transform.position.x, base.transform.position.y, penguinObj.transform.position.z);
		}

		private void setAnimation(ChomperAnimation newAnim)
		{
			if (oldChomperAnimation != newAnim)
			{
				oldChomperAnimation = newAnim;
				anim.SetInteger(HASH_PARAM_ANIM_STATE, (int)newAnim);
			}
		}

		public void OnLungeComplete()
		{
			if (chomperState == ChomperState.LUNGE)
			{
				if (IsPenguinGrabbed)
				{
					chomperState = ChomperState.LUNGE_SUCCESS;
				}
				else
				{
					chomperState = ChomperState.LUNGE_MISS;
				}
			}
		}

		public void OnSpitStart()
		{
			if (!playerResurfaced)
			{
				chomperState = ChomperState.LUNGE_SUCCESS_SPIT_COMPLETE;
			}
			playerResurfaced = false;
		}

		public void OnSuccessMissAnimationComplete(string result)
		{
			if (result.Equals("Success"))
			{
				chomperState = ChomperState.LUNGE_SUCCESS_ANIMATION_COMPLETE;
			}
			else
			{
				chomperState = ChomperState.LUNGE_MISS_ANIMATION_COMPLETE;
			}
		}

		private void enablePenguinSwallow(bool isEnabled)
		{
			enablePenguinControl(isEnabled);
			enableSwimController(isEnabled);
			enablePenguinRenderers(isEnabled);
		}

		private void enablePenguinControl(bool isEnabled)
		{
			if (userControl == null)
			{
				userControl = penguinObj.GetComponent<PenguinUserControl>();
			}
			if (userControl != null)
			{
				userControl.enabled = isEnabled;
			}
			else
			{
				Log.LogError(this, "No PenguinControl component found");
			}
			enableUIControls(isEnabled);
		}

		private void enableSwimController(bool isActive)
		{
			if (swimControl == null)
			{
				swimControl = penguinObj.GetComponent<SwimController>();
			}
			if (swimControl != null)
			{
				swimControl.Active = isActive;
				dispatcher.DispatchEvent(new DivingEvents.FreeAirEffects(isActive, penguinObj.tag));
			}
			else
			{
				Log.LogError(this, "No SwimController component found");
			}
		}

		private void enablePenguinRenderers(bool isEnabled)
		{
			if (skinnedMeshRenderers == null)
			{
				skinnedMeshRenderers = penguinObj.GetComponentsInChildren<SkinnedMeshRenderer>();
			}
			if (skinnedMeshRenderers != null)
			{
				SkinnedMeshRenderer[] array = skinnedMeshRenderers;
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
				{
					skinnedMeshRenderer.enabled = isEnabled;
				}
			}
			else
			{
				Log.LogError(this, "No SkinnedMeshRenderer components found");
			}
			meshRenderers = penguinObj.GetComponentsInChildren<MeshRenderer>();
			if (meshRenderers != null)
			{
				MeshRenderer[] array2 = meshRenderers;
				foreach (MeshRenderer meshRenderer in array2)
				{
					meshRenderer.enabled = isEnabled;
				}
			}
			else
			{
				Log.LogError(this, "No MeshRenderer components found");
			}
		}

		private IEnumerator PauseChomper(float pauseSeconds)
		{
			yield return new WaitForSeconds(pauseSeconds);
			anim.SetFloat(HASH_PARAM_WARN_DISTANCE, 1f);
			IsPenguinGrabbed = false;
			chomperState = ChomperState.HUNGRY;
		}

		private void enableUIControls(bool isEnabled)
		{
			if (!penguinObj.CompareTag("Player"))
			{
				return;
			}
			GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root);
			if (isEnabled)
			{
				if (gameObject != null)
				{
					gameObject.GetComponent<StateMachineContext>().SendEvent(new ExternalEvent("Root", "exit_cinematic"));
				}
				Service.Get<EventDispatcher>().DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(true));
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElementGroup("MainNavButtons"));
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("DailyChallengesTab"));
			}
			else
			{
				if (gameObject != null)
				{
					gameObject.GetComponent<StateMachineContext>().SendEvent(new ExternalEvent("Root", "minnpc"));
				}
				Service.Get<EventDispatcher>().DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(false));
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElementGroup("MainNavButtons"));
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("DailyChallengesTab", true));
			}
		}

		private bool onPlayerResurfaced(DivingEvents.PlayerResurfaced evt)
		{
			if (penguinObj != null && (chomperState == ChomperState.LUNGE_SUCCESS_SPIT_WAIT || chomperState == ChomperState.LUNGE_SUCCESS_ANIMATION_WAIT))
			{
				enablePenguinSwallow(true);
				playerResurfaced = true;
			}
			return false;
		}
	}
}
