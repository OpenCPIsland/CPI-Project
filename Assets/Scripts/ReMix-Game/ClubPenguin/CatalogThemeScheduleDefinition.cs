using ClubPenguin.Core.StaticGameData;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/CatalogThemeSchedule")]
	public class CatalogThemeScheduleDefinition : StaticGameDataDefinition
	{
		[StaticGameDataDefinitionId]
		public int Id;

		public int Day;

		public int Month;

		public int Year;

		public int CatalogThemeId;

		public override string ToString()
		{
			return string.Format("[CatalogThemeScheduleDefinition] Id: {0}, Day: {1}, Month: {2}, Year: {3}, Catalog Theme Id: {4}", Id, Day, Month, Year, CatalogThemeId);
		}
	}
}
