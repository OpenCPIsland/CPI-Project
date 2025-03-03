using ClubPenguin.Net.Client.Smartfox.SFSObject;
using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct AFKEvent
	{
		public readonly long SessionId;

		public readonly int AFKValue;

		public readonly EquippedObject EquippedObject;

		public AFKEvent(long sessionId, int afkValue, EquippedObject equippedObject)
		{
			SessionId = sessionId;
			AFKValue = afkValue;
			EquippedObject = equippedObject;
		}
	}
}
