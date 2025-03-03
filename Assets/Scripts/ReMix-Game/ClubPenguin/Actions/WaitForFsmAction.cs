namespace ClubPenguin.Actions
{
	public class WaitForFsmAction : Action
	{
		public PlayMakerFSM FSM;

		public string FSMName;

		private PlayMakerFSM fsm;

		protected override void CopyTo(Action _destAction)
		{
			WaitForFsmAction waitForFsmAction = _destAction as WaitForFsmAction;
			waitForFsmAction.FSM = FSM;
			waitForFsmAction.FSMName = FSMName;
			base.CopyTo(_destAction);
		}

		protected override void OnEnable()
		{
			fsm = FSM;
			if (fsm == null)
			{
				if (!string.IsNullOrEmpty(FSMName))
				{
					PlayMakerFSM[] components = GetTarget().GetComponents<PlayMakerFSM>();
					for (int i = 0; i < components.Length; i++)
					{
						if (components[i].FsmName == FSMName)
						{
							fsm = components[i];
							break;
						}
					}
				}
				else if (IncomingUserData != null && IncomingUserData.GetType() == typeof(PlayMakerFSM))
				{
					fsm = (PlayMakerFSM)IncomingUserData;
				}
			}
			base.OnEnable();
		}

		protected override void Update()
		{
			if (fsm != null)
			{
				if (fsm.Fsm.Finished)
				{
					Completed(fsm);
				}
			}
			else
			{
				Completed();
			}
		}
	}
}
