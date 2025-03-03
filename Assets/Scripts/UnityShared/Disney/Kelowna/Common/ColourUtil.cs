using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public static class ColourUtil
	{
		public static Color32 GetColor32FromInt(int colour)
		{
			byte[] bytes = BitConverter.GetBytes(colour);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes);
			}
			return new Color32(bytes[0], bytes[1], bytes[2], bytes[3]);
		}

		public static int GetIntFromColor32(Color32 colour)
		{
			byte[] array = new byte[4]
			{
				colour.r,
				colour.g,
				colour.b,
				colour.a
			};
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(array);
			}
			return BitConverter.ToInt32(array, 0);
		}
	}
}
