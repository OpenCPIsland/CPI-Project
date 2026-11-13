using System;
using System.Reflection;
using Tweaker.AssemblyScanner;

namespace Tweaker.Core
{
	public class AutoTweakableProcessor : IAttributeScanProcessor<TweakableAttribute, AutoTweakableResult>, IScanProcessor<TweakableAttribute, AutoTweakableResult>, IScanResultProvider<AutoTweakableResult>
	{
		public event EventHandler<ScanResultArgs<AutoTweakableResult>> ResultProvided;

		public void ProcessAttribute(TweakableAttribute input, Type type, IBoundInstance instance = null)
		{
		}

		public void ProcessAttribute(TweakableAttribute input, MemberInfo memberInfo, IBoundInstance instance = null)
		{
			ITweakable tweakable = null;
			AutoTweakableResult autoTweakableResult = null;
			if (memberInfo.MemberType == MemberTypes.Field)
			{
				FieldInfo fieldInfo = (FieldInfo)memberInfo;
				if (fieldInfo.FieldType.IsSubclassOf(typeof(AutoTweakable)))
				{
					AutoTweakable instance2 = fieldInfo.GetValue(instance.Instance) as AutoTweakable;
					FieldInfo field = fieldInfo.FieldType.GetField("value", BindingFlags.Instance | BindingFlags.Public);
					IBoundInstance boundInstance = BoundInstanceFactory.Create(instance2, instance.UniqueId);
					tweakable = TweakableFactory.MakeTweakable(input, field, boundInstance, memberInfo);
					autoTweakableResult = new AutoTweakableResult();
					autoTweakableResult.autoTweakable = (boundInstance.Instance as AutoTweakable);
					autoTweakableResult.tweakble = tweakable;
					autoTweakableResult.uniqueId = boundInstance.UniqueId;
				}
				if (autoTweakableResult != null)
				{
					ProvideResult(autoTweakableResult);
				}
				return;
			}
			throw new ProcessorException("AutoTweakableProcessor cannot process non FieldInfo types");
		}

		private void ProvideResult(AutoTweakableResult result)
		{
			if (this.ResultProvided != null)
			{
				this.ResultProvided(this, new ScanResultArgs<AutoTweakableResult>(result));
			}
		}
	}
}
