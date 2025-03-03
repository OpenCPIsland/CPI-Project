using System;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class RawTaggedData : ITaggedData
	{
		private short _tag;

		private byte[] _data;

		public short TagID
		{
			get
			{
				return _tag;
			}
			set
			{
				_tag = value;
			}
		}

		public byte[] Data
		{
			get
			{
				return _data;
			}
			set
			{
				_data = value;
			}
		}

		public RawTaggedData(short tag)
		{
			_tag = tag;
		}

		public void SetData(byte[] data, int offset, int count)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			_data = new byte[count];
			Array.Copy(data, offset, _data, 0, count);
		}

		public byte[] GetData()
		{
			return _data;
		}
	}
}
