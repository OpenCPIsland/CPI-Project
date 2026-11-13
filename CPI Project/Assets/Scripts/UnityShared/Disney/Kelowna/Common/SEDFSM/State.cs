using System;

namespace Disney.Kelowna.Common.SEDFSM
{
	[Serializable]
	public class State
	{
		public string Name;

		public ExternalEvent[] OnEntryEvents;

		public Transition[] Transitions;

		public ExternalEvent[] OnExitEvents;

		public string GetTargetStateName(string evt)
		{
			for (int i = 0; i < Transitions.Length; i++)
			{
				if (Transitions[i].Event == evt)
				{
					return Transitions[i].TargetState;
				}
			}
			return null;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
