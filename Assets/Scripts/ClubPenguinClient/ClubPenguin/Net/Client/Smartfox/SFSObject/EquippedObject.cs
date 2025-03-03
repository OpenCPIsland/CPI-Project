using Sfs2X.Entities.Data;
using System;

namespace ClubPenguin.Net.Client.Smartfox.SFSObject
{
	[Serializable]
	public class EquippedObject
	{
		private const string EQUIPPED_OBJECT_TYPE_KEY = "type";

		private const string EQUIPPED_OBJECT_ID_KEY = "name";

		private const string CONSUMABLE_ITEM = "consum";

		private const string DISPENSABLE_ITEM = "dispns";

		private const string DURABLE_ITEM = "durable";

		private const string PARTYGAME_ITEM = "pgame";

		private string equippedObjectType;

		public string EquippedObjectId
		{
			get;
			private set;
		}

		public string PropertiesJSON
		{
			get;
			private set;
		}

		public bool IsConsumable()
		{
			return equippedObjectType == "consum";
		}

		public bool IsDispensable()
		{
			return equippedObjectType == "dispns";
		}

		public bool IsDurable()
		{
			return equippedObjectType == "durable";
		}

		public bool isPartyGame()
		{
			return equippedObjectType == "pgame";
		}

		public static EquippedObject CreateConsumableObject(string consumableId)
		{
			EquippedObject equippedObject = new EquippedObject();
			equippedObject.equippedObjectType = "consum";
			equippedObject.EquippedObjectId = consumableId;
			return equippedObject;
		}

		public static EquippedObject CreateDispensableObject(string dispensableId)
		{
			EquippedObject equippedObject = new EquippedObject();
			equippedObject.equippedObjectType = "dispns";
			equippedObject.EquippedObjectId = dispensableId;
			return equippedObject;
		}

		public static EquippedObject CreateDurableObject(string value)
		{
			EquippedObject equippedObject = new EquippedObject();
			equippedObject.equippedObjectType = "durable";
			equippedObject.EquippedObjectId = value;
			return equippedObject;
		}

		public static EquippedObject FromSFSData(ISFSObject obj, string properties = null)
		{
			EquippedObject equippedObject = new EquippedObject();
			equippedObject.equippedObjectType = obj.GetUtfString("type");
			equippedObject.EquippedObjectId = obj.GetUtfString("name");
			equippedObject.PropertiesJSON = properties;
			return equippedObject;
		}

		public ISFSObject ToSFSObject()
		{
			ISFSObject iSFSObject = new Sfs2X.Entities.Data.SFSObject();
			iSFSObject.PutUtfString("type", equippedObjectType);
			iSFSObject.PutUtfString("name", EquippedObjectId);
			return iSFSObject;
		}
	}
}
