using ClubPenguin.Actions;
using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	[RequireComponent(typeof(MotionTracker))]
	public class SitController : LocomotionController
	{
		public enum Mode
		{
			Jumping,
			Sitting,
			Exiting
		}

		private readonly InputButtonGroupContentKey sitButtonsDefinitionContentKey = new InputButtonGroupContentKey("Definitions/ControlsScreen/Locomotion/SitGroupDefinition");

		private readonly InputButtonGroupContentKey sitSwimButtonsDefinitionContentKey = new InputButtonGroupContentKey("Definitions/ControlsScreen/Locomotion/SitSwimGroupDefinition");

		public Transform ChestBone;

		public SitControllerData MasterData;

		private Mode mode;

		private bool exitingViaJump;

		private ChairProperties chair;

		private LocomotionTracker tracker;

		private LocomotionController prevLocoController;

		private Vector3 chestBoneRotation = new Vector3(0f, 0f, 0f);

		public bool IsUnderwater
		{
			get
			{
				return prevLocoController is SwimController;
			}
		}

		public void OnValidate()
		{
		}

		protected override void awake()
		{
			tracker = GetComponent<LocomotionTracker>();
			base.enabled = false;
		}

		private void OnEnable()
		{
			if (canSitFromCurrentState() || !base.gameObject.CompareTag("Player"))
			{
				prevLocoController = tracker.GetCurrentController();
				animator.SetBool(AnimationHashes.Params.Sit, true);
				mode = Mode.Jumping;
				exitingViaJump = false;
				base.Broadcaster.OnInteractionPreStartedEvent += onInteractionPreStartedEvent;
				base.Broadcaster.BroadcastOnControlsLocked();
			}
			else
			{
				base.enabled = false;
			}
			if (base.gameObject.CompareTag("Player"))
			{
				LoadControlsLayout();
			}
		}

		public void SetChair(ChairProperties _chair)
		{
			if (chair != null)
			{
				chair.gameObject.SetActive(true);
			}
			chair = _chair;
			if (chair != null)
			{
				chestBoneRotation = chair.Fields.ChestBoneRotation;
				if (animator != null)
				{
					animator.SetFloat(AnimationHashes.Params.EnterSitAnimIndex, chair.Fields.EnterSitAnimIndex);
					animator.SetFloat(AnimationHashes.Params.SitAnimIndex, chair.Fields.SitAnimIndex);
				}
				chair.gameObject.SetActive(false);
			}
			else
			{
				Log.LogError(this, "Attempted to sit penguin without chair properties!");
			}
		}

		private void onInteractionPreStartedEvent(GameObject trigger)
		{
		}

		public override void LoadControlsLayout()
		{
			if (IsUnderwater)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new ControlsScreenEvents.SetRightOption(sitSwimButtonsDefinitionContentKey));
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(new ControlsScreenEvents.SetRightOption(sitButtonsDefinitionContentKey));
			}
		}

		private void OnDisable()
		{
			base.Broadcaster.OnInteractionPreStartedEvent -= onInteractionPreStartedEvent;
			if (animator != null)
			{
				animator.SetBool(AnimationHashes.Params.Sit, false);
			}
			RunController component = GetComponent<RunController>();
			if (component != null)
			{
				component.Behaviour.Reset();
			}
			if (chair != null)
			{
				chair.gameObject.SetActive(true);
			}
		}

		private bool canSitFromCurrentState()
		{
			AnimatorStateInfo animatorStateInfo = LocomotionUtils.GetAnimatorStateInfo(animator);
			return LocomotionUtils.IsIdling(animatorStateInfo) || LocomotionUtils.IsLocomoting(animatorStateInfo) || LocomotionUtils.IsInAir(animatorStateInfo) || LocomotionUtils.IsLanding(animatorStateInfo) || LocomotionUtils.IsSwimming(animatorStateInfo);
		}

		public override bool IsFullbodyLocked()
		{
			return true;
		}

		public override void Steer(Vector2 steerInput)
		{
			if (steerInput != Vector2.zero)
			{
				Steer(new Vector3(steerInput.x, steerInput.y, 0f));
			}
			base.Broadcaster.BroadcastOnStickDirectionEvent(steerInput);
		}

		public override void Steer(Vector3 wsSteerInput)
		{
			if (mode == Mode.Sitting && wsSteerInput != Vector3.zero)
			{
				mode = Mode.Exiting;
			}
		}

		public override void DoAction(LocomotionAction action, object userData = null)
		{
			switch (action)
			{
			case LocomotionAction.Torpedo:
			case LocomotionAction.SlideTrick:
				break;
			case LocomotionAction.ChargeThrow:
				DefaultDoAction(action, userData);
				break;
			case LocomotionAction.LaunchThrow:
				DefaultDoAction(action, userData);
				break;
			case LocomotionAction.Interact:
				DefaultDoAction(action, userData);
				break;
			case LocomotionAction.Action1:
			case LocomotionAction.Action2:
			case LocomotionAction.Action3:
				DefaultDoAction(action, userData);
				break;
			case LocomotionAction.Jump:
				if (mode == Mode.Sitting)
				{
					mode = Mode.Exiting;
					exitingViaJump = true;
				}
				break;
			}
		}

		public void RemoteSit(ChairProperties _chair)
		{
			if (_chair == null)
			{
				_chair = getClosestChair(base.transform.position);
			}
			if (_chair != null)
			{
				SetChair(_chair);
				PlayAnimAction component = _chair.gameObject.GetComponent<PlayAnimAction>();
				if (component != null && component.IdealStartTransform != null)
				{
					base.transform.rotation = component.IdealStartTransform.rotation;
				}
			}
			animator.SetBool(AnimationHashes.Params.Sit, true);
			animator.Play(AnimationHashes.States.Interactions.Sit, 0);
			mode = Mode.Sitting;
			base.Broadcaster.BroadcastOnControlsUnLocked();
		}

		private ChairProperties getClosestChair(Vector3 position)
		{
			ChairProperties chairProperties = null;
			Collider[] array = Physics.OverlapSphere(position, 0f, -1, QueryTriggerInteraction.Collide);
			for (int i = 0; i < array.Length; i++)
			{
				ChairProperties component = array[i].gameObject.GetComponent<ChairProperties>();
				if (!(component != null))
				{
					continue;
				}
				if (chairProperties != null)
				{
					if ((position - component.transform.position).sqrMagnitude < (position - chairProperties.transform.position).sqrMagnitude)
					{
						chairProperties = component;
					}
				}
				else
				{
					chairProperties = component;
				}
			}
			return chairProperties;
		}

		private void TryExitSit()
		{
			if (!SceneRefs.ActionSequencer.GetTrigger(base.gameObject))
			{
				tracker.SetCurrentController<RunController>();
				if (exitingViaJump && tracker.GetCurrentController() is RunController && !IsUnderwater)
				{
					tracker.GetCurrentController().DoAction(LocomotionAction.Jump);
				}
			}
		}

		private void LateUpdate()
		{
			if (mode == Mode.Jumping)
			{
				if (!SceneRefs.ActionSequencer.GetTrigger(base.gameObject))
				{
					mode = Mode.Sitting;
					base.Broadcaster.BroadcastOnControlsUnLocked();
				}
				applyIK();
			}
			else if (mode == Mode.Exiting)
			{
				TryExitSit();
			}
			else
			{
				base.transform.rotation *= animator.deltaRotation;
				characterController.Move(animator.deltaPosition);
				applyIK();
			}
		}

		private void applyIK()
		{
			ChestBone.Rotate(chestBoneRotation);
		}
	}
}
