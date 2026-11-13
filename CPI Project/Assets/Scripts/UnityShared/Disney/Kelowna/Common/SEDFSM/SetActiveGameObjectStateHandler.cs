using UnityEngine;

namespace Disney.Kelowna.Common.SEDFSM
{
	public class SetActiveGameObjectStateHandler : PassiveStateHandler
	{
		public GameObject[] Targets;

		public string[] ActiveStates;

		public override void HandleStateChange(string newState)
		{
			bool active = false;
			for (int i = 0; i < ActiveStates.Length; i++)
			{
				if (ActiveStates[i] == newState)
				{
					active = true;
					break;
				}
			}
			for (int i = 0; i < Targets.Length; i++)
			{
				Targets[i].SetActive(active);
			}
		}
	}
}
