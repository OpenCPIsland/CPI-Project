using ClubPenguin.Net.Domain;
using UnityEngine;

namespace ClubPenguin
{
	public class RoomPopulationDisplay : MonoBehaviour
	{
		private const int POPULATION_ICON_ON_INDEX = 0;

		private const int POPULATION_ICON_OFF_INDEX = 1;

		public TintSelector[] PopulationBarIcons;

		public Transform FullIcon;

		public void UpdatePopulationDisplay(RoomPopulationScale populationScale)
		{
			int num = PopulationBarIcons.Length;
			for (int i = 0; i < num; i++)
			{
				if (i < (int)populationScale)
				{
					PopulationBarIcons[i].SelectColor(0);
				}
				else
				{
					PopulationBarIcons[i].SelectColor(1);
				}
			}
			if (FullIcon != null)
			{
				FullIcon.gameObject.SetActive((int)populationScale >= PopulationBarIcons.Length);
			}
		}
	}
}
