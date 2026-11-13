namespace ClubPenguin.UI
{
	public class MembershipFlowButtonDisabler : UIElementDisabler
	{
		public ShowMembershipFlowButton MembershipFlowButton;

		public override void DisableElement(bool hide)
		{
			base.DisableElement(hide);
			if (MembershipFlowButton != null)
			{
				MembershipFlowButton.enabled = false;
			}
		}

		public override void EnableElement()
		{
			base.EnableElement();
			if (MembershipFlowButton != null)
			{
				MembershipFlowButton.enabled = true;
			}
		}
	}
}
