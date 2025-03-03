using ClubPenguin.Net.Client.Event;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(RunController))]
	public class JumpTestScript : MonoBehaviour
	{
		private RunController controller;

		public float RunTime = 2f;

		public float JumpTime = 1f;

		public float Jump2Time = 1f;

		public float WalkTime = 1f;

		public float Jump3Time = 1f;

		public float StopTime = 0.5f;

		private Vector3 startPos;

		private Quaternion startRot;

		public void Awake()
		{
			controller = GetComponent<RunController>();
		}

		public void Start()
		{
			StartCoroutine(TestRoutine());
		}

		private IEnumerator TestRoutine()
		{
			startPos = base.transform.position;
			startRot = base.transform.rotation;
			RunController.ControllerBehaviour behaviourRun = controller.Behaviour;
			behaviourRun.Style = PlayerLocoStyle.Style.Run;
			RunController.ControllerBehaviour behaviourWalk = controller.Behaviour;
			behaviourWalk.Style = PlayerLocoStyle.Style.Walk;
			while (true)
			{
				controller.Behaviour.SetStyle(PlayerLocoStyle.Style.Run);
				controller.Steer(Vector2.up);
				yield return new WaitForSeconds(RunTime);
				controller.DoAction(LocomotionController.LocomotionAction.Jump);
				yield return new WaitForSeconds(JumpTime);
				controller.DoAction(LocomotionController.LocomotionAction.Jump);
				yield return new WaitForSeconds(Jump2Time);
				controller.Behaviour.SetStyle(PlayerLocoStyle.Style.Walk);
				yield return new WaitForSeconds(WalkTime);
				controller.DoAction(LocomotionController.LocomotionAction.Jump);
				yield return new WaitForSeconds(Jump3Time);
				controller.Steer(Vector2.zero);
				yield return new WaitForSeconds(StopTime);
				base.transform.position = startPos;
				base.transform.rotation = startRot;
			}
		}
	}
}
