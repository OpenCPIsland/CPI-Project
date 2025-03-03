using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public class Director : MonoBehaviour
	{
		public enum CameraContext
		{
			Gameplay,
			Cinematic,
			Chase,
			Count
		}

		private EventDispatcher dispatcher;

		public Transform DefaultTarget;

		public BaseCamera ActiveCamera;

		public Transform GameplayCameraLeaf;

		public Transform CinematicCameraLeaf;

		public Transform ChaseCameraLeaf;

		public bool IsStartUpComplete;

		private AnimationCurve blendCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		private float blendDuration = 0f;

		private float curBlendTime = 0f;

		private bool isBlending;

		private bool isBlendingFromPrevFrame;

		private Transform[] nodes;

		private CameraContext curNode;

		private CameraContext nextNode;

		private Transform mainCamera;

		private Vector3 prevCamPos;

		private Quaternion prevCamRot;

		[Tweakable("Debug.ShowCameraControllers")]
		public static bool ShowCameraControllers;

		public bool InCinematicContext
		{
			get
			{
				return curNode == CameraContext.Cinematic || nextNode == CameraContext.Cinematic;
			}
		}

		public void Awake()
		{
			SceneRefs.Set(this);
			dispatcher = Service.Get<EventDispatcher>();
			mainCamera = Camera.main.transform;
			nodes = new Transform[3];
			nodes[0] = GameplayCameraLeaf;
			nodes[1] = CinematicCameraLeaf;
			nodes[2] = ChaseCameraLeaf;
			curNode = CameraContext.Gameplay;
			nextNode = curNode;
		}

		public void Start()
		{
			ResetCamera();
			dispatcher.DispatchEvent(default(CinematographyEvents.DirectorStartComplete));
			IsStartUpComplete = true;
		}

		public void OnEnable()
		{
			dispatcher.AddListener<CinematographyEvents.ChangeCameraContextEvent>(onChangeCameraContextEvent);
			dispatcher.AddListener<CinematographyEvents.CameraLogicChangeEvent>(OnCameraLogicChangeEvent);
			dispatcher.AddListener<CinematographyEvents.CameraLogicResetEvent>(OnCameraLogicResetEvent);
			dispatcher.AddListener<CinematographyEvents.ChangeCameraTarget>(OnCameraTargetChangeEvent);
			dispatcher.AddListener<CinematographyEvents.CameraSnapLockEvent>(OnCameraSnapLockEvent);
			dispatcher.AddListener<SwitchEvents.SwitchChange>(OnSwitchChange);
		}

		public void OnDisable()
		{
			dispatcher.RemoveListener<CinematographyEvents.ChangeCameraContextEvent>(onChangeCameraContextEvent);
			dispatcher.RemoveListener<CinematographyEvents.CameraLogicChangeEvent>(OnCameraLogicChangeEvent);
			dispatcher.RemoveListener<CinematographyEvents.CameraLogicResetEvent>(OnCameraLogicResetEvent);
			dispatcher.RemoveListener<CinematographyEvents.ChangeCameraTarget>(OnCameraTargetChangeEvent);
			dispatcher.RemoveListener<CinematographyEvents.CameraSnapLockEvent>(OnCameraSnapLockEvent);
			dispatcher.RemoveListener<SwitchEvents.SwitchChange>(OnSwitchChange);
		}

		public void ResetCamera()
		{
			if (ActiveCamera != null)
			{
				ActiveCamera.Target = DefaultTarget;
				ActiveCamera.Reset();
			}
		}

		public void SoftResetCamera()
		{
			if (ActiveCamera != null)
			{
				ActiveCamera.Target = DefaultTarget;
				ActiveCamera.SoftReset();
			}
		}

		private bool onChangeCameraContextEvent(CinematographyEvents.ChangeCameraContextEvent evt)
		{
			if (nextNode != evt.Context)
			{
				blendCurve = evt.BlendCurve;
				blendDuration = evt.BlendDuration;
				if (curNode == evt.Context)
				{
					isBlendingFromPrevFrame = true;
					prevCamPos = mainCamera.position;
					prevCamRot = mainCamera.rotation;
				}
				nextNode = evt.Context;
				curBlendTime = 0f;
				isBlending = true;
			}
			return false;
		}

		public void LateUpdate()
		{
			if (!isBlending)
			{
				mainCamera.position = nodes[(int)curNode].position;
				mainCamera.rotation = nodes[(int)curNode].rotation;
				return;
			}
			curBlendTime += Time.deltaTime / Mathf.Max(blendDuration, 0.01f);
			float t = 0f;
			if (curBlendTime > 1f)
			{
				curBlendTime = 1f;
				t = 1f;
			}
			else if (blendCurve != null && blendCurve.keys.Length > 0)
			{
				t = blendCurve.Evaluate(curBlendTime);
			}
			if (isBlendingFromPrevFrame)
			{
				mainCamera.position = Vector3.Lerp(prevCamPos, nodes[(int)nextNode].position, t);
				mainCamera.rotation = Quaternion.Slerp(prevCamRot, nodes[(int)nextNode].rotation, t);
			}
			else
			{
				mainCamera.position = Vector3.Lerp(nodes[(int)curNode].position, nodes[(int)nextNode].position, t);
				mainCamera.rotation = Quaternion.Slerp(nodes[(int)curNode].rotation, nodes[(int)nextNode].rotation, t);
			}
			if (curBlendTime >= 1f)
			{
				CameraContext fromCamera = curNode;
				curNode = nextNode;
				isBlending = false;
				isBlendingFromPrevFrame = false;
				dispatcher.DispatchEvent(new CinematographyEvents.CameraBlendComplete(fromCamera, curNode));
			}
		}

		private bool OnSwitchChange(SwitchEvents.SwitchChange evt)
		{
			CameraController component = evt.Owner.GetComponent<CameraController>();
			if (component != null)
			{
				if (evt.Value)
				{
					ActiveCamera.Set(component);
				}
				else
				{
					ActiveCamera.Clear(component);
				}
			}
			return false;
		}

		private bool OnCameraLogicChangeEvent(CinematographyEvents.CameraLogicChangeEvent evt)
		{
			ActiveCamera.Set(evt.Controller, evt.ForceCutTransition);
			return false;
		}

		private bool OnCameraLogicResetEvent(CinematographyEvents.CameraLogicResetEvent evt)
		{
			ActiveCamera.Clear(evt.Controller);
			return false;
		}

		private bool OnCameraTargetChangeEvent(CinematographyEvents.ChangeCameraTarget evt)
		{
			ActiveCamera.Target = evt.Target;
			return false;
		}

		private bool OnCameraSnapLockEvent(CinematographyEvents.CameraSnapLockEvent evt)
		{
			ActiveCamera.LockSnap(evt.SnapPosition, evt.SnapAim);
			return false;
		}
	}
}
