namespace ClubPenguin.Gui
{
	public class TrayEnabledNotifier : TrayNotifier
	{
		public void OnEnable()
		{
			controller.OnTrayElementEnabled(base.gameObject);
		}

		public void OnDisable()
		{
			controller.OnTrayElementDisabled(base.gameObject);
		}
	}
}
