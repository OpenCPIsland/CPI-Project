using System;
using UnityEngine;

namespace SmoothieSmash
{
	public class SmoothieSmashInputObserver : MonoBehaviour
	{
		public Vector2 CurrentSteering;

		public event Action<Vector2, Vector2> SteeringChangedEvent;

		private void Update()
		{
			Vector2 vector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
			if (vector != CurrentSteering && this.SteeringChangedEvent != null)
			{
				this.SteeringChangedEvent(CurrentSteering, vector);
			}
			CurrentSteering = vector;
		}

		private void OnDestroy()
		{
			this.SteeringChangedEvent = null;
		}
	}
}
