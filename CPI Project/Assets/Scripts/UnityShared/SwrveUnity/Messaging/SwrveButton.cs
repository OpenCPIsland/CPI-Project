using UnityEngine;

namespace SwrveUnity.Messaging
{
	public class SwrveButton : SwrveWidget
	{
		public string Image;

		public string Action;

		public SwrveActionType ActionType;

		public SwrveMessage Message;

		public int AppId;

		public string Name;

		public bool Pressed = false;

		public Rect PointerRect;
	}
}
