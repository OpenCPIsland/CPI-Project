using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class UnityInput : MonoBehaviour
	{
		private EventDispatcher dispatcher;

		private Vector2 prevAxis;

		private bool isWalking;

		public void Awake()
		{
			dispatcher = Service.Get<EventDispatcher>();
		}

		public void Update()
		{
			Vector2 lhs = new Vector2(UnityEngine.Input.GetAxis("Horizontal"), UnityEngine.Input.GetAxis("Vertical"));
			if (lhs != prevAxis)
			{
				dispatcher.DispatchEvent(new InputEvents.MoveEvent(lhs.normalized));
				prevAxis = lhs;
			}
			if (UnityEngine.Input.GetButtonDown("Jump"))
			{
				dispatcher.DispatchEvent(new InputEvents.ActionEvent(InputEvents.Actions.Jump));
			}
			if (UnityEngine.Input.GetButtonDown("Fire3"))
			{
				isWalking = !isWalking;
				dispatcher.DispatchEvent(new InputEvents.SwitchChangeEvent(InputEvents.Switches.Tube, isWalking));
			}
		}
	}
}
