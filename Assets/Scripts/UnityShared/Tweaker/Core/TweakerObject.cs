using System;
using System.Reflection;

namespace Tweaker.Core
{
	public abstract class TweakerObject : ITweakerObject
	{
		protected readonly bool isPublic;

		protected readonly WeakReference instance;

		protected readonly Assembly assembly;

		private string shortName;

		protected TweakerObjectInfo Info
		{
			get;
			set;
		}

		public string Description
		{
			get
			{
				return Info.Description;
			}
		}

		public string Name
		{
			get
			{
				return Info.Name;
			}
		}

		public string ShortName
		{
			get
			{
				return shortName;
			}
		}

		public bool IsPublic
		{
			get
			{
				return isPublic;
			}
		}

		public Assembly Assembly
		{
			get
			{
				return assembly;
			}
		}

		public virtual WeakReference WeakInstance
		{
			get
			{
				return instance;
			}
		}

		public virtual object StrongInstance
		{
			get
			{
				if (WeakInstance == null)
				{
					return null;
				}
				object target = null;
				instance.TryGetTarget(out target);
				return target;
			}
		}

		public virtual bool IsValid
		{
			get
			{
				return WeakInstance == null || StrongInstance != null;
			}
		}

		public ICustomTweakerAttribute[] CustomAttributes
		{
			get
			{
				return Info.CustomAttributes;
			}
		}

		public TweakerObject(TweakerObjectInfo info, Assembly assembly, WeakReference instance, bool isPublic)
		{
			Info = info;
			this.assembly = assembly;
			this.instance = instance;
			this.isPublic = isPublic;
			int num = Name.LastIndexOf('.');
			if (num < 0)
			{
				shortName = Name;
			}
			else
			{
				shortName = Name.Substring(num + 1);
			}
		}

		protected virtual bool CheckInstanceIsValid()
		{
			return IsValid;
		}

		public TAttribute GetCustomAttribute<TAttribute>() where TAttribute : Attribute, ICustomTweakerAttribute
		{
			for (int i = 0; i < CustomAttributes.Length; i++)
			{
				ICustomTweakerAttribute customTweakerAttribute = CustomAttributes[i];
				if (typeof(TAttribute) == customTweakerAttribute.GetType())
				{
					return customTweakerAttribute as TAttribute;
				}
			}
			return null;
		}
	}
}
