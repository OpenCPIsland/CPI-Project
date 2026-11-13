using System;
using UnityEngine.Events;

namespace ClubPenguin
{
	[Serializable]
	public class SnowballHitTargetEvent : UnityEvent<long, int>
	{
	}
}
