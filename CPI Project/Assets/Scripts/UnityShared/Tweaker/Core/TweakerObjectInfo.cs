namespace Tweaker.Core
{
	public class TweakerObjectInfo
	{
		public string Name
		{
			get;
			private set;
		}

		public string Description
		{
			get;
			private set;
		}

		public uint InstanceId
		{
			get;
			private set;
		}

		public ICustomTweakerAttribute[] CustomAttributes
		{
			get;
			set;
		}

		public TweakerObjectInfo(string name, uint instanceId = 0u, ICustomTweakerAttribute[] customAttributes = null, string description = "")
		{
			Name = name;
			Description = description;
			InstanceId = instanceId;
			if (customAttributes == null)
			{
				customAttributes = new ICustomTweakerAttribute[0];
			}
			CustomAttributes = customAttributes;
		}
	}
}
