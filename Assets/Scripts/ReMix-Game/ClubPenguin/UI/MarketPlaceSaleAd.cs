using ClubPenguin.CellPhone;
using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class MarketPlaceSaleAd : MonoBehaviour
	{
		public Text TimeRemainingText;

		public Text SalePercentageText;

		private CellPhoneSaleActivityDefinition saleData;

		private DateTime endTime;

		private Localizer localizer;

		private void Awake()
		{
			localizer = Service.Get<Localizer>();
		}

		public void setSaleData(CellPhoneSaleActivityDefinition saleData)
		{
			this.saleData = saleData;
			setEndTime(this.saleData);
			setSaleDiscount(this.saleData);
			CoroutineRunner.Start(updateTimer(), this, "updateTimer");
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}

		private void setEndTime(CellPhoneSaleActivityDefinition saleData)
		{
			ScheduledEventDateDefinition availableDates = saleData.AvailableDates;
			endTime = availableDates.Dates.EndDate.Date;
		}

		private IEnumerator updateTimer()
		{
			WaitForSeconds waitForOneSecond = new WaitForSeconds(1f);
			while (true)
			{
				TimeSpan remainingTime = endTime - Service.Get<ContentSchedulerService>().ScheduledEventDate();
				showTimerTime(remainingTime);
				if (remainingTime.TotalSeconds <= 0.0)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				yield return waitForOneSecond;
			}
		}

		private void showTimerTime(TimeSpan time)
		{
			TimeRemainingText.text = string.Format(localizer.GetTokenTranslation("GoGuide.ShopSale.Timer"), Mathf.Floor((float)time.TotalHours), time.Minutes, time.Seconds);
		}

		private void setSaleDiscount(CellPhoneSaleActivityDefinition saleData)
		{
			SalePercentageText.text = string.Format(Service.Get<Localizer>().GetTokenTranslation("GoGuide.ShopSale.Discount"), saleData.DiscountPercentage);
		}
	}
}
