using System;

namespace Tweaker.Core
{
	public abstract class BaseTweakerAttribute : Attribute, ITweakerAttribute
	{
		public string Description = "";

		public string Name
		{
			get;
			private set;
		}

		public Guid Guid
		{
			get;
			private set;
		}

		protected BaseTweakerAttribute(string name)
		{
			Name = name;
			Guid = Guid.NewGuid();
		}
	}
}
