namespace ClubPenguin.UI
{
	public class OnOffTintSelector : TintSelector
	{
		private const int OFF = 0;

		private const int ON = 1;

		private bool isOn;

		public bool IsOn
		{
			get
			{
				return isOn;
			}
			set
			{
				isOn = value;
				if (isOn)
				{
					SelectColor(1);
				}
				else
				{
					SelectColor(0);
				}
			}
		}
	}
}
