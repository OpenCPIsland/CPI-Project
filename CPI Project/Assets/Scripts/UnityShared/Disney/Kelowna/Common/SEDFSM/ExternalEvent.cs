using System;

namespace Disney.Kelowna.Common.SEDFSM
{
	[Serializable]
	public struct ExternalEvent
	{
		public string Target;

		public string Event;

		public ExternalEvent(string target, string evt)
		{
			Target = target;
			Event = evt;
		}

		public override string ToString()
		{
			return string.Format("(Target: {0}, Event: {1})", Target, Event);
		}
	}
}
