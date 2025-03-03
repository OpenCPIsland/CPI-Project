namespace ClubPenguin.Actions
{
	public class AddFsmAction : Action
	{
		public FsmTemplate Template;

		protected override void CopyTo(Action _destAction)
		{
			AddFsmAction addFsmAction = _destAction as AddFsmAction;
			addFsmAction.Template = Template;
			base.CopyTo(_destAction);
		}

		protected override void Update()
		{
			PlayMakerFSM playMakerFSM = GetTarget().AddComponent<PlayMakerFSM>();
			playMakerFSM.SetFsmTemplate(Template);
			playMakerFSM.FsmName = Template.name;
			Completed(playMakerFSM);
		}
	}
}
