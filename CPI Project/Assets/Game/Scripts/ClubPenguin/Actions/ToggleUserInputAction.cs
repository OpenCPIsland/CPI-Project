using ClubPenguin.Locomotion;

namespace ClubPenguin.Actions
{
	public class ToggleUserInputAction : Action
	{
		public bool On = true;

		protected override void CopyTo(Action _destWarper)
		{
			ToggleUserInputAction toggleUserInputAction = _destWarper as ToggleUserInputAction;
			toggleUserInputAction.On = On;
			base.CopyTo(_destWarper);
		}

		protected override void Update()
		{
			PenguinUserControl component = GetTarget().GetComponent<PenguinUserControl>();
			if (component != null)
			{
				component.enabled = On;
			}
			Completed();
		}
	}
}
