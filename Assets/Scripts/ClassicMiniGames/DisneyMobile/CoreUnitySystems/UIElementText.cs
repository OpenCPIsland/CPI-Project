namespace DisneyMobile.CoreUnitySystems
{
	public class UIElementText : UIElementBase
	{
		public void SetText(string t, bool iskey)
		{
			if (iskey)
			{
				SetTextByKey(t);
			}
			else
			{
				SetText(t);
			}
		}

		public virtual void SetText(string t)
		{
			Logger.LogFatal(this, "Should not use base class UIElementText, use UIElementTextNGUI instead");
		}

		public void SetTextByKey(string key)
		{
			if (TextManager.Instance == null)
			{
				Logger.LogWarning(this, "TextManager not initialized ");
				SetText(key);
			}
			else
			{
				SetText(key);
			}
		}
	}
}
