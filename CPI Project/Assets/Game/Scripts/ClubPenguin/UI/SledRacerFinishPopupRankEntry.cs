using ClubPenguin.SledRacer;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class SledRacerFinishPopupRankEntry : MonoBehaviour
	{
		private const string PLATINUM_TEXT_TOKEN = "Global.Racing.Rank.Platinum";

		private const string GOLD_TEXT_TOKEN = "Global.Racing.Rank.Gold";

		private const string SILVER_TEXT_TOKEN = "Global.Racing.Rank.Silver";

		private const string BRONZE_TEXT_TOKEN = "Global.Racing.Rank.Bronze";

		private const string UNDER_TEXT_TOKEN = "Global.Racing.Time.Under";

		public SpriteSelector TrophySpriteSelector;

		public Text RankText;

		public Text TimeText;

		public Text YourTimeText;

		public void Initialize(long raceTime, RaceResults.RaceResultsCategory category, long[] rankTimes, bool isSelected)
		{
			int index = (int)(3 - (category - 1));
			TrophySpriteSelector.SelectSprite(index);
			string token = "Global.Racing.Rank.Platinum";
			switch (category)
			{
			case RaceResults.RaceResultsCategory.Gold:
				token = "Global.Racing.Rank.Gold";
				break;
			case RaceResults.RaceResultsCategory.Silver:
				token = "Global.Racing.Rank.Silver";
				break;
			case RaceResults.RaceResultsCategory.Bronze:
				token = "Global.Racing.Rank.Bronze";
				break;
			}
			RankText.text = Service.Get<Localizer>().GetTokenTranslation(token);
			int num = (int)(category - 2);
			if (num < 0)
			{
				TimeText.gameObject.SetActive(false);
			}
			else
			{
				string arg = default(DateTime).AddMilliseconds(rankTimes[num]).ToString("m:ss.ff");
				TimeText.text = string.Format(Service.Get<Localizer>().GetTokenTranslation("Global.Racing.Time.Under"), arg);
			}
			if (YourTimeText != null)
			{
				DateTime dateTime = default(DateTime).AddMilliseconds(raceTime);
				YourTimeText.text = dateTime.ToString("m:ss.ff");
			}
		}
	}
}
