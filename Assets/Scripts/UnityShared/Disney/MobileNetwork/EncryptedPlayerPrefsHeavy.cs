using UnityEngine;

namespace Disney.MobileNetwork
{
	internal class EncryptedPlayerPrefsHeavy : EncryptedPlayerPrefsBase
	{
		protected const int MAX_SECOND_LEVEL_KEYS = 5;

		protected const string MASTER_NAME = "__universe__";

		protected const string SECOND_NAME = "__second__";

		internal static bool isCurrentVersion
		{
			get
			{
				return PlayerPrefs.HasKey("__universe__");
			}
		}

		public EncryptedPlayerPrefsHeavy(bool fallbackUnencrypted)
			: base("__universe__", "__second__", 5, fallbackUnencrypted)
		{
		}

		public EncryptedPlayerPrefsHeavy()
			: this(true)
		{
		}

		public override void Cleanup()
		{
			if (!m_convertingFrom)
			{
				PlayerPrefs.DeleteAll();
			}
		}
	}
}
