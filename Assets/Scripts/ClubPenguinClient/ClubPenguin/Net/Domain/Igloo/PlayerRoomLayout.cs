using System;
using System.Collections.Generic;
using System.Text;

namespace ClubPenguin.Net.Domain.Igloo
{
	[Serializable]
	public class PlayerRoomLayout
	{
		public long playerRoomId;

		public string building = "NotNeeded";

		private List<CompoundRoomItem> _compoundRoomItems;

		public List<CompoundRoomItem> compoundRoomItems
		{
			get
			{
				if (_compoundRoomItems == null)
				{
					_compoundRoomItems = new List<CompoundRoomItem>();
				}
				return _compoundRoomItems;
			}
			set
			{
				_compoundRoomItems = value;
			}
		}

		public static PlayerRoomLayout FromPlaceableObjects(List<PlaceableObject> placeableObjects)
		{
			List<CompoundRoomItem> list = new List<CompoundRoomItem>(placeableObjects.Count);
			while (placeableObjects.Count > 0)
			{
				PlaceableObject placeableObject = placeableObjects[0];
				placeableObjects.Remove(placeableObject);
				if (placeableObject.PieceNumber == 1)
				{
					List<PlaceableObject> list2 = new List<PlaceableObject>();
					list2.Add(placeableObject);
					while (placeableObject.Next != null)
					{
						list2.Add(placeableObject.Next);
						placeableObjects.Remove(placeableObject.Next);
						placeableObject = placeableObject.Next;
					}
					list.Add(CompoundRoomItem.FromPlaceableObjects(list2));
				}
			}
			PlayerRoomLayout playerRoomLayout = new PlayerRoomLayout();
			playerRoomLayout._compoundRoomItems = list;
			return playerRoomLayout;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (CompoundRoomItem compoundRoomItem in compoundRoomItems)
			{
				stringBuilder.Append(compoundRoomItem.ToString());
			}
			return string.Format("Layout items: {0}", stringBuilder);
		}

		protected bool Equals(PlayerRoomLayout other)
		{
			return object.Equals(_compoundRoomItems, other._compoundRoomItems);
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(null, obj))
			{
				return false;
			}
			if (object.ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((PlayerRoomLayout)obj);
		}

		public override int GetHashCode()
		{
			return (_compoundRoomItems != null) ? _compoundRoomItems.GetHashCode() : 0;
		}
	}
}
