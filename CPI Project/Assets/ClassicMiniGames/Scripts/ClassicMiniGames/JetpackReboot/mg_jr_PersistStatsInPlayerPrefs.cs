using UnityEngine;
using NUnit.Framework;

namespace JetpackReboot
{
	public class mg_jr_PersistStatsInPlayerPrefs : mg_jr_IStatPersistenceStrategy
	{
		private const string DISTANCE_KEY = "mg_jr_BestDistance";

		private const string COINS_KEY = "mg_jr_MostCoins";

		public void Save(mg_jr_PlayerStats _toSave)
		{
			Assert.NotNull(_toSave, "Can't save a null object");
			PlayerPrefs.SetInt(GetKey(DISTANCE_KEY), _toSave.BestDistance);
			PlayerPrefs.SetInt(GetKey(COINS_KEY), _toSave.MostCoins);
		}

		public mg_jr_PlayerStats LoadOrCreate()
		{
			int @int = PlayerPrefs.GetInt(GetKey(DISTANCE_KEY));
			int int2 = PlayerPrefs.GetInt(GetKey(COINS_KEY));
			return new mg_jr_PlayerStats(@int, int2, this);
		}

		private static string GetKey(string key)
		{
			string scopedKey = mg_jr_PlayerPrefsKey.Get(key);
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
			if (Application.isEditor)
			{
				return "Editor_" + scopedKey;
			}
#endif
			return scopedKey;
		}
	}
}
