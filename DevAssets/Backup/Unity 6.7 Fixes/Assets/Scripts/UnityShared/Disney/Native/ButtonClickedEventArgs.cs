using System;
using UnityEngine;

namespace Disney.Native
{
	public class ButtonClickedEventArgs : EventArgs
	{
		public EntityId Id;

		public ButtonClickedEventArgs(EntityId aId)
		{
			Id = aId;
		}
	}
}
