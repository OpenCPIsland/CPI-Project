namespace DisneyMobile.CoreUnitySystems
{
	public class BaseEvent
	{
		public string GetName()
		{
			return GetType().ToString();
		}
	}
}
