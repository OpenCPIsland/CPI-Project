using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class PenguinStrafe : MonoBehaviour
	{
		private bool forwardAsUp;

		private bool stickRelative;

		private Transform cameraTransform;

		private Animator anim;

		private EventDispatcher dispatcher;

		public void Awake()
		{
			cameraTransform = Camera.main.transform;
			anim = GetComponent<Animator>();
			dispatcher = Service.Get<EventDispatcher>();
		}

		public void OnEnable()
		{
			dispatcher.AddListener<InputEvents.MoveEvent>(onMove);
		}

		public void OnDisable()
		{
			dispatcher.RemoveListener<InputEvents.MoveEvent>(onMove);
		}

		public void Init(bool _forwardAsUp, bool _stickRelative)
		{
			forwardAsUp = _forwardAsUp;
			stickRelative = _stickRelative;
		}

		private bool onMove(InputEvents.MoveEvent evt)
		{
			float num = 0f;
			float num2 = 0f;
			Vector3 vector;
			if (stickRelative)
			{
				num = evt.Direction.x;
				num2 = evt.Direction.y;
			}
			else if (forwardAsUp)
			{
				vector = evt.Direction.y * cameraTransform.up + evt.Direction.x * cameraTransform.right;
				vector.z = 0f;
				vector.Normalize();
				num = vector.x;
				num2 = vector.y;
			}
			else
			{
				Vector3 normalized = Vector3.Scale(cameraTransform.forward, new Vector3(1f, 0f, 1f)).normalized;
				vector = evt.Direction.y * normalized + evt.Direction.x * cameraTransform.right;
				vector.y = 0f;
				vector.Normalize();
				num = vector.x;
				num2 = vector.z;
			}
			anim.SetFloat(AnimationHashes.Params.StrafeX, num);
			anim.SetFloat(AnimationHashes.Params.StrafeY, num2);
			return false;
		}
	}
}
