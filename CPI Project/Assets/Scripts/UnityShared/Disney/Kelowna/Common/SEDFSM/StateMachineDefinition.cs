using UnityEngine;

namespace Disney.Kelowna.Common.SEDFSM
{
	[CreateAssetMenu]
	public class StateMachineDefinition : ScriptableObject
	{
		public bool PersistState;

		public ExternalEvent[] OnEnableEvents;

		public State[] States;

		public ExternalEvent[] OnDisableEvents;

		public State GetState(string name)
		{
			for (int i = 0; i < States.Length; i++)
			{
				if (States[i].Name == name)
				{
					return States[i];
				}
			}
			return null;
		}
	}
}
