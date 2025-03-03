namespace Disney.Kelowna.Common
{
	public class DevCacheableType<T> : CacheableType<T>
	{
		public DevCacheableType(string playerPrefsKey, T defaultValue)
			: base(playerPrefsKey, defaultValue)
		{
		}

		public override void SetValue(T value)
		{
		}

		public override T GetValue()
		{
			return defaultValue;
		}
	}
}
