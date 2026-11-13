using System;

namespace ClubPenguin.Kelowna.Common.ImageCache
{
	[Serializable]
	public struct DImageCache
	{
		public string Hashname
		{
			get;
			set;
		}

		public int Filesize
		{
			get;
			set;
		}

		public long Timestamp
		{
			get;
			set;
		}

		public override string ToString()
		{
			return string.Format("[DImageCache: hashname={0}, filesize={1}, timestamp={2}]", Hashname, Filesize, Timestamp);
		}
	}
}
