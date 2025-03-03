using ClubPenguin.Locomotion;
using UnityEngine;

namespace ClubPenguin.Actions
{
	public class PlayAnimAction : Action
	{
		[Tooltip("Full path to animation state (e.g. Base Layer.Idle)")]
		public string StateName;

		public int LayerIndex;

		public float Duration;

		public float NormStartTime;

		public bool AnimateTranslation;

		public bool AnimateRotation;

		public bool IgnoreCollisions;

		public bool MakeAnimRelative;

		public Transform IdealStartTransform;

		public float WarpStartTime;

		public float WarpEndTime;

		private Animator anim;

		private CharacterController controller;

		private RunController runController;

		private int stateID;

		private float elapsedTime;

		private bool animStarted;

		private float animDuration;

		private Transform thisTransform;

		private Vector3 firstAnimDeltaPos;

		private bool firstAnimDeltaPosSet;

		private RunController.ControllerBehaviour oldRunBehaviour;

		private bool runControllerBehaviourWasSet;

		private Vector3 startPos;

		private Quaternion startRot = Quaternion.identity;

		private Vector3 localAnimatedPosSoFar;

		private Quaternion localAnimatedRotSoFar = Quaternion.identity;

		private Vector3 lastValidPos;

		private Quaternion lastValidRot;

		private bool isWarping;

		private bool animFound;

		private float prevNormTime;

		private bool animLooped;

		protected override void CopyTo(Action _destWarper)
		{
			PlayAnimAction playAnimAction = _destWarper as PlayAnimAction;
			playAnimAction.Duration = Duration;
			playAnimAction.StateName = StateName;
			playAnimAction.LayerIndex = LayerIndex;
			playAnimAction.NormStartTime = NormStartTime;
			playAnimAction.AnimateTranslation = AnimateTranslation;
			playAnimAction.AnimateRotation = AnimateRotation;
			playAnimAction.IgnoreCollisions = IgnoreCollisions;
			playAnimAction.MakeAnimRelative = MakeAnimRelative;
			playAnimAction.IdealStartTransform = IdealStartTransform;
			playAnimAction.WarpStartTime = WarpStartTime;
			playAnimAction.WarpEndTime = WarpEndTime;
			base.CopyTo(_destWarper);
		}

		protected override void OnEnable()
		{
			anim = GetComponent<Animator>();
			stateID = Animator.StringToHash(StateName);
			controller = GetComponent<CharacterController>();
			runController = base.gameObject.GetComponent<RunController>();
			animDuration = Duration;
			thisTransform = base.transform;
			isWarping = (IdealStartTransform != null);
			lastValidPos = thisTransform.position;
			lastValidRot = thisTransform.rotation;
			animFound = anim.HasState(LayerIndex, stateID);
			if (animFound)
			{
				anim.Play(stateID, LayerIndex, NormStartTime);
				elapsedTime = 0f;
			}
			base.OnEnable();
		}

		protected override void Update()
		{
			if (!animFound)
			{
				Completed();
				return;
			}
			bool flag = false;
			if (!animStarted)
			{
				AnimatorStateInfo animatorStateInfo = anim.IsInTransition(LayerIndex) ? anim.GetNextAnimatorStateInfo(LayerIndex) : anim.GetCurrentAnimatorStateInfo(LayerIndex);
				if (animatorStateInfo.fullPathHash == stateID)
				{
					animDuration = ((Duration == 0f) ? animatorStateInfo.length : Duration);
					animStarted = true;
					if (runController != null && getImplementsOnAnimatorMove())
					{
						oldRunBehaviour = runController.Behaviour;
						runController.Behaviour = new RunController.ControllerBehaviour
						{
							IgnoreCollisions = true,
							IgnoreGravity = true,
							IgnoreRotation = true,
							IgnoreTranslation = true
						};
						runControllerBehaviourWasSet = true;
					}
					startRot = thisTransform.rotation;
					startPos = thisTransform.position;
				}
			}
			if (animStarted)
			{
				elapsedTime += Time.deltaTime;
				if (isWarping)
				{
					if (AnimateTranslation)
					{
						thisTransform.position = Vector3.zero;
					}
					if (AnimateRotation)
					{
						thisTransform.rotation = Quaternion.identity;
					}
				}
				if (animDuration < 0f)
				{
					if ((anim.IsInTransition(LayerIndex) ? anim.GetNextAnimatorStateInfo(LayerIndex) : anim.GetCurrentAnimatorStateInfo(LayerIndex)).fullPathHash != stateID)
					{
						flag = true;
					}
				}
				else if (elapsedTime >= animDuration)
				{
					flag = true;
				}
			}
			if (flag)
			{
				if (runController != null && runControllerBehaviourWasSet)
				{
					runController.Behaviour = oldRunBehaviour;
				}
				if (isWarping)
				{
					thisTransform.position = lastValidPos;
					thisTransform.rotation = lastValidRot;
				}
				Completed();
			}
		}

		private bool getImplementsOnAnimatorMove()
		{
			return AnimateRotation || AnimateTranslation || IgnoreCollisions || MakeAnimRelative;
		}

		protected override void OnAnimatorMove()
		{
			if (!animStarted)
			{
				return;
			}
			Quaternion quaternion = Quaternion.identity;
			float t = 0f;
			if (isWarping)
			{
				float normalizedTime = anim.GetCurrentAnimatorStateInfo(LayerIndex).normalizedTime;
				if (normalizedTime >= WarpStartTime && normalizedTime <= WarpEndTime)
				{
					float num = WarpEndTime - WarpStartTime;
					t = ((num != 0f) ? ((normalizedTime - WarpStartTime) / num) : 1f);
				}
				else if (normalizedTime > WarpEndTime || animLooped)
				{
					t = 1f;
				}
				else if (normalizedTime < prevNormTime)
				{
					animLooped = true;
					t = 1f;
				}
				prevNormTime = normalizedTime;
			}
			if (AnimateRotation)
			{
				if (isWarping)
				{
					quaternion = Quaternion.Slerp(startRot, IdealStartTransform.rotation, t) * Quaternion.Inverse(startRot);
					localAnimatedRotSoFar = anim.deltaRotation * localAnimatedRotSoFar;
					thisTransform.rotation = localAnimatedRotSoFar * quaternion * startRot;
				}
				else
				{
					thisTransform.rotation *= anim.deltaRotation;
				}
			}
			if (AnimateTranslation)
			{
				Vector3 deltaPosition = anim.deltaPosition;
				if (isWarping)
				{
					Vector3 b = Vector3.Lerp(Vector3.zero, IdealStartTransform.position - startPos, t);
					localAnimatedPosSoFar = localAnimatedRotSoFar * deltaPosition + localAnimatedPosSoFar;
					thisTransform.position = quaternion * startRot * localAnimatedPosSoFar + b + startPos;
				}
				else if (IgnoreCollisions || controller == null)
				{
					thisTransform.position += deltaPosition;
				}
				else
				{
					controller.Move(deltaPosition);
				}
			}
			lastValidPos = thisTransform.position;
			lastValidRot = thisTransform.rotation;
		}
	}
}
