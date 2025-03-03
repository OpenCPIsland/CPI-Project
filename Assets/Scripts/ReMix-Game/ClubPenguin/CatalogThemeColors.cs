using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin
{
	public class CatalogThemeColors
	{
		private DateTime startDate;

		public Color[] foreground
		{
			get;
			private set;
		}

		public Color[] background
		{
			get;
			private set;
		}

		public int indexOffset
		{
			get;
			private set;
		}

		public CatalogThemeColors()
		{
			startDate = new DateTime(2017, 2, 21, 0, 0, 0);
			foreground = new Color[7]
			{
				new Color(0.647058845f, 0.1764706f, 49f / 85f),
				new Color(172f / 255f, 4f / 85f, 19f / 255f),
				new Color(184f / 255f, 82f / 255f, 0f),
				new Color(0.294117659f, 142f / 255f, 0.0196078438f),
				new Color(32f / 255f, 0.4117647f, 146f / 255f),
				new Color(43f / 255f, 53f / 85f, 1f),
				new Color(131f / 255f, 101f / 255f, 239f / 255f)
			};
			background = new Color[7]
			{
				new Color(72f / 85f, 74f / 255f, 0.7647059f),
				new Color(47f / 51f, 67f / 255f, 0.294117659f),
				new Color(47f / 51f, 134f / 255f, 2f / 85f),
				new Color(166f / 255f, 211f / 255f, 19f / 85f),
				new Color(38f / 85f, 0.7647059f, 1f),
				new Color(0f, 36f / 85f, 66f / 85f),
				new Color(77f / 255f, 47f / 255f, 63f / 85f)
			};
		}

		public void SetIndex()
		{
			calculateIndexOffset(Service.Get<ContentSchedulerService>().PresentTime());
		}

		public void SetIndex(DateTime overrideDateTime)
		{
			calculateIndexOffset(overrideDateTime);
		}

		private void calculateIndexOffset(DateTime dateTime)
		{
			for (indexOffset = (dateTime - startDate).Days; indexOffset >= foreground.Length; indexOffset -= foreground.Length)
			{
			}
		}

		public Color[] GetColorsByIndex(int index)
		{
			if (index < 0)
			{
				index = 0;
			}
			while (index >= foreground.Length)
			{
				index -= foreground.Length;
			}
			index -= indexOffset;
			if (index < 0)
			{
				index = foreground.Length + index;
			}
			return new Color[2]
			{
				foreground[index],
				background[index]
			};
		}
	}
}
