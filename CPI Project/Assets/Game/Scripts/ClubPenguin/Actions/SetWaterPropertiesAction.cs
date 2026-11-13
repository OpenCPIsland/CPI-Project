using ClubPenguin.Locomotion;

namespace ClubPenguin.Actions
{
	public class SetWaterPropertiesAction : Action
	{
		public float SurfaceHeight;

		protected override void CopyTo(Action _destAction)
		{
			SetWaterPropertiesAction setWaterPropertiesAction = _destAction as SetWaterPropertiesAction;
			setWaterPropertiesAction.SurfaceHeight = SurfaceHeight;
			base.CopyTo(_destAction);
		}

		protected override void Update()
		{
			SwimController component = GetTarget().GetComponent<SwimController>();
			if (component != null)
			{
				component.SetSurfaceHeight(SurfaceHeight);
			}
			Completed();
		}
	}
}
