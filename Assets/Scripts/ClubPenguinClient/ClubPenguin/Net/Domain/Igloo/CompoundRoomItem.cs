using System;
using System.Collections.Generic;
using System.Text;

namespace ClubPenguin.Net.Domain.Igloo
{
	[Serializable]
	public class CompoundRoomItem
	{
		public string furnitureTemplateId;

		private List<RoomItem> _roomItems;

		public List<RoomItem> roomItems
		{
			get
			{
				if (_roomItems == null)
				{
					_roomItems = new List<RoomItem>();
				}
				return _roomItems;
			}
			set
			{
				_roomItems = value;
			}
		}

		public static CompoundRoomItem FromPlaceableObjects(List<PlaceableObject> placeableObjects)
		{
			List<RoomItem> list = new List<RoomItem>(placeableObjects.Count);
			CompoundRoomItem compoundRoomItem = new CompoundRoomItem();
			foreach (PlaceableObject placeableObject in placeableObjects)
			{
				if (!string.IsNullOrEmpty(placeableObject.FurnitureTemplateID))
				{
					compoundRoomItem.furnitureTemplateId = placeableObject.FurnitureTemplateID;
				}
				list.Add(RoomItem.FromPlaceableObject(placeableObject));
			}
			compoundRoomItem.roomItems = list;
			return compoundRoomItem;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (RoomItem roomItem in roomItems)
			{
				stringBuilder.Append(roomItem.ToString());
				stringBuilder.Append("\r\n");
			}
			return stringBuilder.ToString();
		}

		protected bool Equals(CompoundRoomItem other)
		{
			return object.Equals(_roomItems, other._roomItems);
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
			return Equals((CompoundRoomItem)obj);
		}

		public override int GetHashCode()
		{
			return (_roomItems != null) ? _roomItems.GetHashCode() : 0;
		}
	}
}
