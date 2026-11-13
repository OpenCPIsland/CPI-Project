namespace DisneyMobile.CoreUnitySystems
{
	public class UIElementTextNGUI : UIElementText
	{
		public UILabel TextUILabel = null;

		protected override void Awake()
		{
			if (TextUILabel == null)
			{
				TextUILabel = base.gameObject.GetComponentInChildren<UILabel>();
			}
		}

		public override void SetText(string t)
		{
			if (TextUILabel == null)
			{
				Logger.LogWarning(this, "UIElementText should have UILabel : " + base.name);
			}
			else
			{
				TextUILabel.text = t;
			}
		}
	}
}
