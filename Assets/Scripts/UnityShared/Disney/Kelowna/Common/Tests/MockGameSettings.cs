using Disney.Kelowna.Common.Environment;

namespace Disney.Kelowna.Common.Tests
{
	public class MockGameSettings : ICommonGameSettings
	{
		public CacheableType<bool> SfxEnabled
		{
			get;
			set;
		}

		public CacheableType<bool> MusicEnabled
		{
			get;
			set;
		}

		public CacheableType<float> SfxVolume
		{
			get;
			set;
		}

		public CacheableType<float> MusicVolume
		{
			get;
			set;
		}

		public CacheableType<string> SavedLanguage
		{
			get;
			set;
		}

		public bool OfflineMode
		{
			get;
			set;
		}

		public DevCacheableType<Disney.Kelowna.Common.Environment.Environment> DevelopmentEnvironment
		{
			get;
			set;
		}

		public CacheableType<string> GameServerHost
		{
			get;
			set;
		}

		public CacheableType<string> CPAPIServicehost
		{
			get;
			set;
		}

		public CacheableType<string> CDN
		{
			get;
			set;
		}

		public MockGameSettings()
		{
			SfxEnabled = new CacheableType<bool>("cp.tests.SfxEnabled", true);
			MusicEnabled = new CacheableType<bool>("cp.tests.MusicEnabled", true);
			SfxVolume = new CacheableType<float>("cp.tests.SfxVolume", 1f);
			MusicVolume = new CacheableType<float>("cp.tests.MusicVolume", 1f);
			SavedLanguage = new CacheableType<string>("cp.tests.SavedLanguage", "en_US");
			DevelopmentEnvironment = new DevCacheableType<Disney.Kelowna.Common.Environment.Environment>("cp.tests.DevelopmentEnvironment", Disney.Kelowna.Common.Environment.Environment.QA);
		}

		public void RegisterSetting(ICachableType setting, bool canBeReset)
		{
		}
	}
}
