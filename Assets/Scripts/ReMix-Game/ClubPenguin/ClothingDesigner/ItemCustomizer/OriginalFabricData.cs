using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class OriginalFabricData
	{
		public Texture2D ActualChannelRed;

		public Texture2D ActualChannelGreen;

		public Texture2D ActualChannelBlue;

		public Texture2D OriginalChannelRed;

		public Texture2D OriginalChannelGreen;

		public Texture2D OriginalChannelBlue;

		public Texture2D UpdatedChannel;

		public Vector2 UVOffsetRed;

		public Vector2 UVOffsetGreen;

		public Vector2 UVOffsetBlue;

		public virtual void Clear()
		{
			ActualChannelRed = null;
			ActualChannelGreen = null;
			ActualChannelBlue = null;
			OriginalChannelRed = null;
			OriginalChannelGreen = null;
			OriginalChannelBlue = null;
			UpdatedChannel = null;
			UVOffsetRed = Vector2.zero;
			UVOffsetGreen = Vector2.zero;
			UVOffsetBlue = Vector2.zero;
		}
	}
}
