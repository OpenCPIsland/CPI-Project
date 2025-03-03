using UnityEngine;

namespace ClubPenguin.Actions
{
	public class RemoveFsmAction : Action
	{
		public PlayMakerFSM FSM;

		public string FSMName;

		protected override void CopyTo(Action _destAction)
		{
			RemoveFsmAction removeFsmAction = _destAction as RemoveFsmAction;
			removeFsmAction.FSM = FSM;
			removeFsmAction.FSMName = FSMName;
			base.CopyTo(_destAction);
		}

		protected override void Update()
		{
			if (FSM != null)
			{
				Object.Destroy(FSM);
			}
			else if (!string.IsNullOrEmpty(FSMName))
			{
				PlayMakerFSM[] components = GetTarget().GetComponents<PlayMakerFSM>();
				for (int i = 0; i < components.Length; i++)
				{
					if (components[i].FsmName == FSMName)
					{
						Object.Destroy(components[i]);
					}
				}
			}
			else if (IncomingUserData != null && IncomingUserData.GetType() == typeof(PlayMakerFSM))
			{
				Object.Destroy((PlayMakerFSM)IncomingUserData);
			}
			else
			{
				PlayMakerFSM[] components = GetTarget().GetComponents<PlayMakerFSM>();
				for (int i = 0; i < components.Length; i++)
				{
					Object.Destroy(components[i]);
				}
			}
			Completed();
		}
	}
}
