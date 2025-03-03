using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	internal class OrbitalImpulseGoalPlanner : GoalPlanner
	{
		public float Height;

		private EventDispatcher dispatcher;

		private float swipeDelta;

		public void Awake()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<InputEvents.SwipeEvent>(OnSwipeEvent);
		}

		public override void Plan(ref Setup setup)
		{
			Vector3 vector = setup.Focus.position - setup.Goal;
			vector.y = 0f;
			float magnitude = vector.magnitude;
			Vector3 normalized = vector.normalized;
			Vector3 a = new Vector3(normalized.z, 0f, 0f - normalized.x);
			setup.Goal += swipeDelta * magnitude * a;
			swipeDelta = 0f;
			setup.Goal.y = setup.Focus.position.y + Height;
		}

		private bool OnSwipeEvent(InputEvents.SwipeEvent evt)
		{
			swipeDelta = evt.Delta;
			Dirty = true;
			return false;
		}
	}
}
