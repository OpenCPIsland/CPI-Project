using ClubPenguin.CellPhone;
using ClubPenguin.Core;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;

namespace ClubPenguin.UI
{
	public static class MarketPlaceUtils
	{
		public delegate bool IsItemInSaleDelegate<T>(CellPhoneSaleActivityDefinition sale, T item);

		public const int DEFAULT_ITEM_DISCOUNT_PERCENTAGE = 0;

		public static List<CellPhoneSaleActivityDefinition> GetCurrentSales()
		{
			DateTime dateTime = Service.Get<ContentSchedulerService>().ScheduledEventDate();
			Dictionary<int, CellPhoneSaleActivityDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<int, CellPhoneSaleActivityDefinition>>();
			List<CellPhoneSaleActivityDefinition> list = new List<CellPhoneSaleActivityDefinition>();
			foreach (CellPhoneSaleActivityDefinition value in dictionary.Values)
			{
				if (dateTime.CompareTo(value.GetStartingDate().Date) >= 0 && dateTime.CompareTo(value.GetEndingDate().Date) < 0)
				{
					list.Add(value);
				}
			}
			return list;
		}

		public static int GetItemCost<T>(T item, int defaultCost, IsItemInSaleDelegate<T> isItemInSale)
		{
			int num = defaultCost;
			int itemDiscountPercentage = GetItemDiscountPercentage(item, isItemInSale);
			if (itemDiscountPercentage != 0)
			{
				num = (int)((double)(float)num - Math.Ceiling((float)num * ((float)itemDiscountPercentage / 100f)));
			}
			return num;
		}

		public static int GetItemDiscountPercentage<T>(T item, IsItemInSaleDelegate<T> isItemInSale)
		{
			List<CellPhoneSaleActivityDefinition> currentSales = GetCurrentSales();
			for (int i = 0; i < currentSales.Count; i++)
			{
				if (isItemInSale(currentSales[i], item))
				{
					return currentSales[i].DiscountPercentage;
				}
			}
			return 0;
		}
	}
}
