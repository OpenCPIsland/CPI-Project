namespace ClubPenguin.FeatureToggle
{
	internal class FeatureToggleDisabler : AbstractFeatureToggler
	{
		protected override void onFeatureEnabled()
		{
			base.gameObject.SetActive(true);
		}

		protected override void onFeatureDisabled()
		{
			base.gameObject.SetActive(false);
		}
	}
}
