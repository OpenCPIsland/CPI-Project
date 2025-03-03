using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer.MockData
{
	public class FurnitureTemplatesMockData : MonoBehaviour
	{
		public static Dictionary<FurnitureTemplateCategory, string[]> GetMockData()
		{
			Dictionary<FurnitureTemplateCategory, string[]> dictionary = new Dictionary<FurnitureTemplateCategory, string[]>();
			dictionary.Add(FurnitureTemplateCategory.Chair, new string[3]
			{
				"kitchenChair",
				"armChair",
				"Ottoman"
			});
			dictionary.Add(FurnitureTemplateCategory.Table, new string[2]
			{
				"KitchenTable",
				"coffeeTable"
			});
			dictionary.Add(FurnitureTemplateCategory.Wall, new string[0]);
			dictionary.Add(FurnitureTemplateCategory.Floor, new string[0]);
			dictionary.Add(FurnitureTemplateCategory.Decor, new string[0]);
			dictionary.Add(FurnitureTemplateCategory.Interactive, new string[1]
			{
				"Clock"
			});
			return dictionary;
		}
	}
}
