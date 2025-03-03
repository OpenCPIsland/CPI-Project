using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public class ScriptedCamera : MonoBehaviour
	{
		public struct AnimState
		{
			public ScriptedCameraSource Anchor;

			public ScriptedCameraSource Target;

			public Vector3 FirstFrameCameraPos;

			public Vector3 FirstFrameAnchorPos;

			public Quaternion FirstFrameAnchorRot;

			public Vector3 FinalFrameAnchorPos;

			public AnimationCurve BlendCurve;

			public float BlendDuration;

			public float ElapsedBlendTime;

			public bool IsBlendingAnchor;

			public bool IsBlendingTarget;

			public void FullReset()
			{
				Anchor.Reset();
				Target.Reset();
				PartialReset();
			}

			public void PartialReset()
			{
				BlendDuration = 0f;
				ElapsedBlendTime = 0f;
				BlendCurve = null;
				IsBlendingAnchor = false;
				IsBlendingTarget = false;
			}
		}

		private EventDispatcher dispatcher;

		private AnimState animState;

		private bool signalCompletion;

		private Transform cameraTransform;

		private bool justEnabled;

		public AnimState GetAnimState
		{
			get
			{
				return animState;
			}
		}

		public void Awake()
		{
			cameraTransform = base.transform;
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<CinematographyEvents.ChangeCameraContextEvent>(onChangeCameraContextEvent);
			dispatcher.AddListener<ScriptedCameraEvents.SetCameraEvent>(onAnimateCameraEvent);
			animState.Anchor = new ScriptedCameraSource();
			animState.Target = new ScriptedCameraSource();
			animState.FullReset();
			base.enabled = false;
			justEnabled = false;
		}

		public void OnDestroy()
		{
			if (dispatcher != null)
			{
				dispatcher.RemoveListener<CinematographyEvents.ChangeCameraContextEvent>(onChangeCameraContextEvent);
				dispatcher.RemoveListener<ScriptedCameraEvents.SetCameraEvent>(onAnimateCameraEvent);
			}
		}

		public void OnEnable()
		{
			animState.FullReset();
			signalCompletion = false;
		}

		public bool onChangeCameraContextEvent(CinematographyEvents.ChangeCameraContextEvent evt)
		{
			if (evt.Context == Director.CameraContext.Cinematic)
			{
				justEnabled = !base.enabled;
				base.enabled = true;
			}
			else
			{
				base.enabled = false;
			}
			return false;
		}

		private bool onAnimateCameraEvent(ScriptedCameraEvents.SetCameraEvent evt)
		{
			signalCompletion = evt.WaitForCompletion;
			animState.PartialReset();
			if (evt.Anchor != null)
			{
				animState.Anchor.Set(evt.Anchor, evt.PosAnimTrigger, Vector3.zero);
			}
			if (evt.Target != null)
			{
				animState.Target.Set(evt.Target, evt.TargetAnimTrigger, evt.TargetOffset);
			}
			animState.Anchor.Activate();
			animState.Target.Activate();
			if (justEnabled || evt.BlendDuration <= 0f)
			{
				justEnabled = false;
				if (animState.Anchor.IsValid())
				{
					cameraTransform.position = animState.Anchor.GetPosition();
				}
				if (animState.Target.IsValid())
				{
					cameraTransform.rotation = Quaternion.LookRotation(animState.Target.GetPosition() - cameraTransform.position);
				}
				checkForCompletion();
			}
			else
			{
				animState.BlendCurve = evt.BlendCurve;
				animState.BlendDuration = evt.BlendDuration;
				animState.FirstFrameCameraPos = cameraTransform.position;
				animState.FirstFrameAnchorRot = cameraTransform.rotation;
				if (animState.Anchor.IsValid())
				{
					animState.FirstFrameAnchorPos = animState.Anchor.GetPosition();
					animState.FinalFrameAnchorPos = animState.Anchor.GetPosition();
					animState.IsBlendingAnchor = true;
				}
				else
				{
					animState.FinalFrameAnchorPos = cameraTransform.position;
				}
				if (animState.Target.IsValid())
				{
					animState.IsBlendingTarget = true;
				}
			}
			return false;
		}

		public void LateUpdate()
		{
			bool flag = animState.Anchor.IsValid();
			if (animState.IsBlendingAnchor || animState.IsBlendingTarget)
			{
				bool flag2 = false;
				animState.ElapsedBlendTime += Time.deltaTime;
				if (animState.ElapsedBlendTime > animState.BlendDuration)
				{
					animState.ElapsedBlendTime = animState.BlendDuration;
					flag2 = true;
				}
				float t = animState.BlendCurve.Evaluate(animState.ElapsedBlendTime / animState.BlendDuration);
				if (flag)
				{
					if (animState.IsBlendingAnchor)
					{
						cameraTransform.position = Vector3.Lerp(animState.FirstFrameCameraPos, animState.FinalFrameAnchorPos, t);
						cameraTransform.position += animState.Anchor.GetPosition() - animState.FirstFrameAnchorPos;
					}
					else
					{
						cameraTransform.position = animState.Anchor.GetPosition();
					}
				}
				if (animState.Target.IsValid())
				{
					if (animState.IsBlendingTarget)
					{
						Quaternion b = Quaternion.LookRotation(animState.Target.GetPosition() - cameraTransform.position);
						cameraTransform.rotation = Quaternion.Slerp(animState.FirstFrameAnchorRot, b, t);
					}
					else
					{
						cameraTransform.rotation = Quaternion.LookRotation(animState.Target.GetPosition() - cameraTransform.position);
					}
				}
				else if (flag && animState.Anchor.Type == ScriptedCameraSource.SourceType.Anim)
				{
					cameraTransform.rotation = Quaternion.Slerp(animState.FirstFrameAnchorRot, animState.Anchor.GetRotation(), t);
				}
				if (flag2)
				{
					animState.IsBlendingAnchor = false;
					animState.IsBlendingTarget = false;
				}
			}
			else
			{
				if (flag)
				{
					cameraTransform.position = animState.Anchor.GetPosition();
				}
				if (animState.Target.IsValid())
				{
					cameraTransform.rotation = Quaternion.LookRotation(animState.Target.GetPosition() - cameraTransform.position);
				}
				else if (flag && animState.Anchor.Type == ScriptedCameraSource.SourceType.Anim)
				{
					cameraTransform.rotation = animState.Anchor.GetRotation();
				}
			}
			checkForCompletion();
		}

		private void checkForCompletion()
		{
			if (!signalCompletion)
			{
				return;
			}
			bool flag = true;
			if (animState.Anchor.IsValid())
			{
				if (animState.Anchor.Type == ScriptedCameraSource.SourceType.Transform)
				{
					if (animState.IsBlendingAnchor)
					{
						flag = false;
					}
				}
				else if (!animState.Anchor.IsFinished())
				{
					flag = false;
				}
			}
			if (animState.Target.IsValid())
			{
				if (animState.Target.Type == ScriptedCameraSource.SourceType.Transform)
				{
					if (animState.IsBlendingTarget)
					{
						flag = false;
					}
				}
				else if (!animState.Target.IsFinished())
				{
					flag = false;
				}
			}
			if (flag)
			{
				signalCompletion = false;
				dispatcher.DispatchEvent(default(ScriptedCameraEvents.CameraCompleted));
			}
		}
	}
}
