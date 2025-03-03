namespace Tweaker
{
	public class TweakerOptions
	{
		public TweakerOptionFlags Flags = GetDefaultFlags();

		public static TweakerOptionFlags GetDefaultFlags()
		{
			TweakerOptionFlags tweakerOptionFlags = (TweakerOptionFlags)2147483647;
			tweakerOptionFlags &= ~TweakerOptionFlags.ScanInEverything;
			tweakerOptionFlags &= ~TweakerOptionFlags.DoNotAutoScan;
			return tweakerOptionFlags & ~TweakerOptionFlags.IncludeTests;
		}

		public static TweakerOptions GetDefault()
		{
			TweakerOptions tweakerOptions = new TweakerOptions();
			tweakerOptions.Flags = GetDefaultFlags();
			return tweakerOptions;
		}

		public static TweakerOptions GetDefaultWithAdditionalFlags(TweakerOptionFlags flags)
		{
			TweakerOptions @default = GetDefault();
			@default.Flags |= flags;
			return @default;
		}
	}
}
