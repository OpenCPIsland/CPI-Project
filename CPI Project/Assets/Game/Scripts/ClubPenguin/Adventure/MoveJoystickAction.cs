using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("GUI")]
	public class MoveJoystickAction : FsmStateAction
	{
		public string JoystickName;

		private GameObject joystickObject;

		private VirtualJoystick joystick;

		private EventDispatcher dispatcher;

		public override void OnEnter()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<InputEvents.MoveEvent>(onMove);
		}

		public override void OnExit()
		{
			dispatcher.RemoveListener<InputEvents.MoveEvent>(onMove);
		}

		private bool onMove(InputEvents.MoveEvent evt)
		{
			if (evt.Direction.magnitude > 0.1f && evt.Direction != Vector2.zero)
			{
				Finish();
			}
			return false;
		}
	}
}
