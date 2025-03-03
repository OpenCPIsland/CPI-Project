using ClubPenguin.Net.Domain;
using System.Collections.Generic;

namespace ClubPenguin.Net.Offline
{
	public struct PlayerOutfitDetails : IOfflineData
	{
		public List<CustomEquipment> Parts;

		public void Init()
		{
			Parts = new List<CustomEquipment>();
		}
	}
}
