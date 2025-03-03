using ClubPenguin.Net.Client.Smartfox.SFSObject;
using System;

namespace ClubPenguin
{
	[Serializable]
	public class AwayFromKeyboardState
	{
		public AwayFromKeyboardStateType Type;

		public EquippedObject EquippedObject;

		public AwayFromKeyboardState(AwayFromKeyboardStateType type, EquippedObject equippedObject)
		{
			Type = type;
			EquippedObject = equippedObject;
		}
	}
}
