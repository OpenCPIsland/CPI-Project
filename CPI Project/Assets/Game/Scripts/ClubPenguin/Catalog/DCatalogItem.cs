using ClubPenguin.Avatar;

namespace ClubPenguin.Catalog
{
	public class DCatalogItem
	{
		public DCustomEquipment Equipment;

		public int LikeCount
		{
			get;
			set;
		}

		public string PenguinName
		{
			get;
			set;
		}

		public int ItemCost
		{
			get;
			set;
		}

		public bool DoesOwn
		{
			get;
			set;
		}
	}
}
