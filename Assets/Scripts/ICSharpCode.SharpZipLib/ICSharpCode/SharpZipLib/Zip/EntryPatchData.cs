namespace ICSharpCode.SharpZipLib.Zip
{
	internal class EntryPatchData
	{
		private long sizePatchOffset_;

		private long crcPatchOffset_;

		public long SizePatchOffset
		{
			get
			{
				return sizePatchOffset_;
			}
			set
			{
				sizePatchOffset_ = value;
			}
		}

		public long CrcPatchOffset
		{
			get
			{
				return crcPatchOffset_;
			}
			set
			{
				crcPatchOffset_ = value;
			}
		}
	}
}
