namespace ClubPenguin
{
	public class SilentClosePopupStateHandler : AbstractAccountStateHandler
	{
		public void OnStateChanged(string state)
		{
			if (state == HandledState)
			{
				AccountPopupController componentInParent = GetComponentInParent<AccountPopupController>();
				componentInParent.OnClosePopup();
			}
		}
	}
}
