namespace ClubPenguin.UI
{
	public class OnOffSpriteSelector : SpriteSelector
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
					SelectSprite(1);
				}
				else
				{
					SelectSprite(0);
				}
			}
		}
	}
}
