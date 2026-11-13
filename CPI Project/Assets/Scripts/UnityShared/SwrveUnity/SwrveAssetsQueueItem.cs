namespace SwrveUnity
{
	public class SwrveAssetsQueueItem
	{
		public string Name
		{
			get;
			private set;
		}

		public string Digest
		{
			get;
			private set;
		}

		public bool IsImage
		{
			get;
			private set;
		}

		public SwrveAssetsQueueItem(string name, string digest, bool isImage)
		{
			Name = name;
			Digest = digest;
			IsImage = isImage;
		}

		public override bool Equals(object obj)
		{
			SwrveAssetsQueueItem swrveAssetsQueueItem = obj as SwrveAssetsQueueItem;
			return swrveAssetsQueueItem != null && swrveAssetsQueueItem.Name == Name && swrveAssetsQueueItem.Digest == Digest && swrveAssetsQueueItem.IsImage == IsImage;
		}

		public override int GetHashCode()
		{
			int num = 17;
			num = num * 23 + Name.GetHashCode();
			num = num * 23 + Digest.GetHashCode();
			return num * 23 + IsImage.GetHashCode();
		}
	}
}
