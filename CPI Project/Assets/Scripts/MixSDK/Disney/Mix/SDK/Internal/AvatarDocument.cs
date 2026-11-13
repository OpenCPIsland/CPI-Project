using DeviceDB;

namespace Disney.Mix.SDK.Internal
{
	[Serialized(103, new byte[]
	{

	})]
	public class AvatarDocument : AbstractDocument
	{
		public static readonly string AvatarIdFieldName = FieldNameGetter.Get((AvatarDocument m) => m.AvatarId);

		[Serialized(0, new byte[]
		{

		})]
		public string AccessoryPropertySelectionKey;

		[Serialized(1, new byte[]
		{

		})]
		public int AccessoryPropertyTintIndex;

		[Serialized(2, new byte[]
		{

		})]
		public double AccessoryPropertyXOffset;

		[Serialized(3, new byte[]
		{

		})]
		public double AccessoryPropertyYOffset;

		[Serialized(4, new byte[]
		{

		})]
		public string BrowPropertySelectionKey;

		[Serialized(5, new byte[]
		{

		})]
		public int BrowPropertyTintIndex;

		[Serialized(6, new byte[]
		{

		})]
		public double BrowPropertyXOffset;

		[Serialized(7, new byte[]
		{

		})]
		public double BrowPropertyYOffset;

		[Serialized(8, new byte[]
		{

		})]
		public string CostumePropertySelectionKey;

		[Serialized(9, new byte[]
		{

		})]
		public int CostumePropertyTintIndex;

		[Serialized(10, new byte[]
		{

		})]
		public double CostumePropertyXOffset;

		[Serialized(11, new byte[]
		{

		})]
		public double CostumePropertyYOffset;

		[Serialized(12, new byte[]
		{

		})]
		public string EyesPropertySelectionKey;

		[Serialized(13, new byte[]
		{

		})]
		public int EyesPropertyTintIndex;

		[Serialized(14, new byte[]
		{

		})]
		public double EyesPropertyXOffset;

		[Serialized(15, new byte[]
		{

		})]
		public double EyesPropertyYOffset;

		[Serialized(16, new byte[]
		{

		})]
		public string HairPropertySelectionKey;

		[Serialized(17, new byte[]
		{

		})]
		public int HairPropertyTintIndex;

		[Serialized(18, new byte[]
		{

		})]
		public double HairPropertyXOffset;

		[Serialized(19, new byte[]
		{

		})]
		public double HairPropertyYOffset;

		[Serialized(20, new byte[]
		{

		})]
		public string NosePropertySelectionKey;

		[Serialized(21, new byte[]
		{

		})]
		public int NosePropertyTintIndex;

		[Serialized(22, new byte[]
		{

		})]
		public double NosePropertyXOffset;

		[Serialized(23, new byte[]
		{

		})]
		public double NosePropertyYOffset;

		[Serialized(24, new byte[]
		{

		})]
		public string MouthPropertySelectionKey;

		[Serialized(25, new byte[]
		{

		})]
		public int MouthPropertyTintIndex;

		[Serialized(26, new byte[]
		{

		})]
		public double MouthPropertyXOffset;

		[Serialized(27, new byte[]
		{

		})]
		public double MouthPropertyYOffset;

		[Serialized(28, new byte[]
		{

		})]
		public string SkinPropertySelectionKey;

		[Serialized(29, new byte[]
		{

		})]
		public int SkinPropertyTintIndex;

		[Serialized(30, new byte[]
		{

		})]
		public double SkinPropertyXOffset;

		[Serialized(31, new byte[]
		{

		})]
		public double SkinPropertyYOffset;

		[Indexed]
		[Serialized(32, new byte[]
		{

		})]
		public long AvatarId;

		[Serialized(33, new byte[]
		{

		})]
		public int SlotId;

		[Serialized(34, new byte[]
		{

		})]
		public string HatPropertySelectionKey;

		[Serialized(35, new byte[]
		{

		})]
		public int HatPropertyTintIndex;

		[Serialized(36, new byte[]
		{

		})]
		public double HatPropertyXOffset;

		[Serialized(37, new byte[]
		{

		})]
		public double HatPropertyYOffset;
	}
}
