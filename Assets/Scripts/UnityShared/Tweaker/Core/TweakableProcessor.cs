using Disney.LaunchPadFramework;
using System;
using System.Reflection;
using Tweaker.AssemblyScanner;

namespace Tweaker.Core
{
	public class TweakableProcessor : IAttributeScanProcessor<TweakableAttribute, ITweakable>, IScanProcessor<TweakableAttribute, ITweakable>, IScanResultProvider<ITweakable>
	{
		private readonly TweakerOptions options;

		public event EventHandler<ScanResultArgs<ITweakable>> ResultProvided;

		public TweakableProcessor(TweakerOptions options)
		{
			this.options = options;
		}

		public void ProcessAttribute(TweakableAttribute input, Type type, IBoundInstance instance = null)
		{
			try
			{
				MemberInfo[] members = type.GetMembers(ReflectionUtil.GetBindingFlags(instance));
				foreach (MemberInfo memberInfo in members)
				{
					if ((memberInfo.MemberType == MemberTypes.Property || memberInfo.MemberType == MemberTypes.Field) && memberInfo.GetCustomAttributes(typeof(TweakableAttribute), false).Length == 0)
					{
						TweakableAttribute input2 = new TweakableAttribute(input.Name + "." + memberInfo.Name);
						ProcessAttribute(input2, memberInfo, instance);
					}
				}
			}
			catch (ProcessorException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				Log.LogException(this, ex2);
			}
		}

		public void ProcessAttribute(TweakableAttribute input, MemberInfo memberInfo, IBoundInstance instance = null)
		{
			ITweakable tweakable = null;
			try
			{
				if (memberInfo.MemberType == MemberTypes.Property)
				{
					PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
					tweakable = TweakableFactory.MakeTweakable(input, propertyInfo, instance);
				}
				else
				{
					if (memberInfo.MemberType != MemberTypes.Field)
					{
						throw new ProcessorException("TweakableProcessor cannot process non PropertyInfo or FieldInfo types");
					}
					FieldInfo fieldInfo = (FieldInfo)memberInfo;
					tweakable = TweakableFactory.MakeTweakable(input, fieldInfo, instance);
				}
			}
			catch (ProcessorException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				Log.LogException(this, ex2);
			}
			if (tweakable != null)
			{
				ProvideResult(tweakable);
			}
		}

		private void ProvideResult(ITweakable tweakable)
		{
			IsTestAttribute customAttribute = tweakable.GetCustomAttribute<IsTestAttribute>();
			if (((options.Flags & TweakerOptionFlags.IncludeTests) != 0 || customAttribute == null) && this.ResultProvided != null)
			{
				this.ResultProvided(this, new ScanResultArgs<ITweakable>(tweakable));
			}
		}
	}
}
