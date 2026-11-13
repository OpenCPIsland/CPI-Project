using System;

namespace Disney.Kelowna.Common.SEDFSM
{
	[Serializable]
	public struct Transition
	{
		public string Event;

		public string TargetState;
	}
}
