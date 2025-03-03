using ClubPenguin.Net.Domain;
using System.Collections.Generic;

namespace ClubPenguin.Net.Offline
{
	public struct CustomEquipmentCollection : IOfflineData
	{
		public List<CustomEquipment> Equipment;

		public void Init()
		{
			Equipment = new List<CustomEquipment>();
		}
	}
}
