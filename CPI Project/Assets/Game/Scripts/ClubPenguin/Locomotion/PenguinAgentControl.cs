using ClubPenguin.Net.Client.Event;
using UnityEngine;
using UnityEngine.AI;

namespace ClubPenguin.Locomotion
{
	[RequireComponent(typeof(NavMeshAgent))]
	[RequireComponent(typeof(LocomotionTracker))]
	public class PenguinAgentControl : MonoBehaviour
	{
		public PlayerLocoStyle.Style LocoStyle;

		private NavMeshAgent agent;

		private LocomotionTracker tracker;

		private RunController runController;

		public void Awake()
		{
			agent = GetComponent<NavMeshAgent>();
			tracker = GetComponent<LocomotionTracker>();
			agent.updatePosition = false;
			agent.updateRotation = false;
			tracker.SetCurrentController<RunController>();
			runController = GetComponent<RunController>();
			runController.Behaviour.SetStyle(LocoStyle);
		}

		public void Update()
		{
			Vector2 steerInput = new Vector2(agent.desiredVelocity.x, agent.desiredVelocity.z);
			runController.Steer(steerInput);
		}

		public void LateUpdate()
		{
			agent.nextPosition = base.transform.position;
		}
	}
}
