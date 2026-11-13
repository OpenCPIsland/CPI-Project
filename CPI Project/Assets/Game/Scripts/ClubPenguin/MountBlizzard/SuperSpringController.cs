using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.Net.Domain;
using ClubPenguin.Props;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.MountBlizzard
{
	public class SuperSpringController : MonoBehaviour
	{
		public PropDefinition[] PropDefinition;

		public ParticleSystem ParticlesSnow;

		public GameObject[] SpringObjects;

		public TextMesh TextCounter;

		public int JumpsRequired = 10;

		public Vector3 shakeMagnitude = new Vector3(0.1f, 0.05f, 0.05f);

		public float shakeTime = 3f;

		public float shakeDelay = 1f;

		private int jumpsRemaining;

		private bool isActivating;

		private LocomotionEventBroadcaster locomotionEventBroadcaster;

		private AvatarLocomotionStateSetter locomotionSetter;

		private LocomotionEventBroadcaster LocomotionEventBroadcaster
		{
			get
			{
				if (locomotionEventBroadcaster == null)
				{
					locomotionEventBroadcaster = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<LocomotionEventBroadcaster>();
				}
				return locomotionEventBroadcaster;
			}
		}

		private AvatarLocomotionStateSetter LocomotionSetter
		{
			get
			{
				if (locomotionSetter == null)
				{
					locomotionSetter = GetComponent<AvatarLocomotionStateSetter>();
				}
				return locomotionSetter;
			}
		}

		private void Awake()
		{
			EnableSprings(false);
			jumpsRemaining = JumpsRequired;
			setText(string.Format("Jumps Left:\n{0}", jumpsRemaining));
			isActivating = false;
		}

		private void OnTriggerEnter()
		{
			if (LocomotionEventBroadcaster != null)
			{
				locomotionEventBroadcaster.OnDoActionEvent += OnDoAction;
			}
			if (LocomotionSetter != null)
			{
				LocomotionSetter.ActionButtonInvoked += OnActionButtonClicked;
			}
			Service.Get<EventDispatcher>().AddListener<InputEvents.ActionEvent>(onInputEventActionEvent);
		}

		private void OnTriggerExit()
		{
			if (LocomotionEventBroadcaster != null)
			{
				locomotionEventBroadcaster.OnDoActionEvent -= OnDoAction;
			}
			if (LocomotionSetter != null)
			{
				LocomotionSetter.ActionButtonInvoked -= OnActionButtonClicked;
			}
			Service.Get<EventDispatcher>().RemoveListener<InputEvents.ActionEvent>(onInputEventActionEvent);
		}

		private bool onInputEventActionEvent(InputEvents.ActionEvent evt)
		{
			switch (evt.Action)
			{
			default:
				return false;
			}
		}

		private void OnActionButtonClicked(LocomotionAction action)
		{
			switch (action)
			{
			case LocomotionAction.Jump:
				break;
			case LocomotionAction.Interact:
				break;
			case LocomotionAction.ChargeThrow:
			case LocomotionAction.LaunchThrow:
			case LocomotionAction.Torpedo:
			case LocomotionAction.SlideTrick:
				break;
			case LocomotionAction.Action1:
				break;
			case LocomotionAction.Action2:
				break;
			case LocomotionAction.Action3:
				break;
			}
		}

		private void OnDoAction(LocomotionController.LocomotionAction action, object userData)
		{
			switch (action)
			{
			case LocomotionController.LocomotionAction.Torpedo:
			case LocomotionController.LocomotionAction.SlideTrick:
			case LocomotionController.LocomotionAction.ChargeThrow:
			case LocomotionController.LocomotionAction.LaunchThrow:
				break;
			case LocomotionController.LocomotionAction.Interact:
				break;
			case LocomotionController.LocomotionAction.Jump:
			case LocomotionController.LocomotionAction.Action1:
			case LocomotionController.LocomotionAction.Action2:
			case LocomotionController.LocomotionAction.Action3:
				if (!isActivating)
				{
					jumpsRemaining--;
					setText(string.Format("Jumps Left:\n{0}", jumpsRemaining));
					if (jumpsRemaining <= 0)
					{
						isActivating = true;
						StartCoroutine(ActivatePlatform());
					}
				}
				break;
			}
		}

		private int hasEquippedProp()
		{
			bool flag = false;
			int result = -1;
			PropUser component = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<PropUser>();
			if (component != null && component.Prop != null)
			{
				int num = PropDefinition.Length;
				for (int i = 0; i < num; i++)
				{
					flag = (component.Prop.PropId == PropDefinition[i].GetNameOnServer());
					if (flag)
					{
						result = i;
						break;
					}
				}
				if (flag)
				{
					Animator componentInParent = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponentInParent<Animator>();
					if (componentInParent != null)
					{
						AnimatorStateInfo animatorStateInfo = LocomotionUtils.GetAnimatorStateInfo(componentInParent, 1);
						flag = (LocomotionUtils.IsHolding(animatorStateInfo) || LocomotionUtils.IsRetrieving(animatorStateInfo));
					}
				}
			}
			if (!flag)
			{
				result = -1;
			}
			return result;
		}

		private void EnableSprings(bool isEnabled)
		{
			int num = SpringObjects.Length;
			for (int i = 0; i < num; i++)
			{
				SpringObjects[i].SetActive(isEnabled);
			}
		}

		private void setText(string newText)
		{
			if (TextCounter != null)
			{
				TextCounter.text = newText;
			}
		}

		private IEnumerator ActivatePlatform()
		{
			yield return new WaitForSeconds(0.5f);
			iTween.ShakePosition(base.gameObject, iTween.Hash("delay", shakeDelay, "isLocal", true, "time", shakeTime, "amount", shakeMagnitude));
			setText("Activating in\n...3...");
			yield return new WaitForSeconds(1f);
			setText("Activating in\n ..2..");
			yield return new WaitForSeconds(1f);
			setText("Activating in\n .1.");
			yield return new WaitForSeconds(1f);
			setText("Going up!");
			if (ParticlesSnow != null)
			{
				Object.Instantiate(ParticlesSnow, base.transform.position, Quaternion.identity);
			}
			EnableSprings(true);
			yield return new WaitForSeconds(2f);
			EnableSprings(false);
			jumpsRemaining = JumpsRequired;
			setText(string.Format("Jumps Left:\n{0}", jumpsRemaining));
			isActivating = false;
		}
	}
}
