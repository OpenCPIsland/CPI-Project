using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tweaker.AssemblyScanner
{
	public class Scanner : IScanner
	{
		private class BaseProcessorWrapper
		{
			public readonly object Processor;

			private HashSet<object> processedObjects;

			protected BaseProcessorWrapper(object processor)
			{
				Processor = processor;
				processedObjects = new HashSet<object>();
			}

			private bool CheckAlreadyProcessed(object obj, IBoundInstance instance)
			{
				if (instance != null)
				{
					return false;
				}
				if (!processedObjects.Contains(obj))
				{
					processedObjects.Add(obj);
					return false;
				}
				return true;
			}

			public void ProcessAttribute(Attribute attribute, Type type, IBoundInstance instance = null)
			{
				if (!CheckAlreadyProcessed(attribute.GetType().FullName + type.FullName, instance))
				{
					DoProcessAttribute(attribute, type, instance);
				}
			}

			public void ProcessAttribute(Attribute attribute, MemberInfo memberInfo, IBoundInstance instance = null)
			{
				if (!CheckAlreadyProcessed(attribute.GetType().FullName + memberInfo.ReflectedType.FullName + memberInfo.Name, instance))
				{
					DoProcessAttribute(attribute, memberInfo, instance);
				}
			}

			public void ProcessType(Type type, IBoundInstance instance = null)
			{
				if (!CheckAlreadyProcessed(type.FullName, instance))
				{
					DoProcessType(type, instance);
				}
			}

			public void ProcessMember(MemberInfo memberInfo, Type type, IBoundInstance instance = null)
			{
				if (!CheckAlreadyProcessed(memberInfo.ReflectedType.FullName + memberInfo.Name, instance))
				{
					DoProcessMember(memberInfo, type, instance);
				}
			}

			protected virtual void DoProcessAttribute(Attribute attribute, Type type, IBoundInstance instance = null)
			{
			}

			protected virtual void DoProcessAttribute(Attribute attribute, MemberInfo memberInfo, IBoundInstance instance = null)
			{
			}

			protected virtual void DoProcessType(Type type, IBoundInstance instance = null)
			{
			}

			protected virtual void DoProcessMember(MemberInfo memberInfo, Type type, IBoundInstance instance = null)
			{
			}
		}

		private class ProcessorWrapper<TInput, TResult> : BaseProcessorWrapper where TInput : class
		{
			public ProcessorWrapper(IAttributeScanProcessor<TInput, TResult> processor)
				: base(processor)
			{
			}

			public ProcessorWrapper(ITypeScanProcessor<TInput, TResult> processor)
				: base(processor)
			{
			}

			public ProcessorWrapper(IMemberScanProcessor<TInput, TResult> processor)
				: base(processor)
			{
			}

			protected override void DoProcessAttribute(Attribute attribute, MemberInfo memberInfo, IBoundInstance instance = null)
			{
				IAttributeScanProcessor<TInput, TResult> attributeScanProcessor = Processor as IAttributeScanProcessor<TInput, TResult>;
				if (attributeScanProcessor != null)
				{
					attributeScanProcessor.ProcessAttribute(attribute as TInput, memberInfo, instance);
				}
			}

			protected override void DoProcessAttribute(Attribute attribute, Type type, IBoundInstance instance = null)
			{
				IAttributeScanProcessor<TInput, TResult> attributeScanProcessor = Processor as IAttributeScanProcessor<TInput, TResult>;
				if (attributeScanProcessor != null)
				{
					attributeScanProcessor.ProcessAttribute(attribute as TInput, type, instance);
				}
			}

			protected override void DoProcessType(Type type, IBoundInstance instance = null)
			{
				ITypeScanProcessor<TInput, TResult> typeScanProcessor = Processor as ITypeScanProcessor<TInput, TResult>;
				if (typeScanProcessor != null)
				{
					typeScanProcessor.ProcessType(type, instance);
				}
			}

			protected override void DoProcessMember(MemberInfo memberInfo, Type type, IBoundInstance instance = null)
			{
				IMemberScanProcessor<TInput, TResult> memberScanProcessor = Processor as IMemberScanProcessor<TInput, TResult>;
				if (memberScanProcessor != null)
				{
					memberScanProcessor.ProcessMember(memberInfo as TInput, type, instance);
				}
			}
		}

		private interface IResultGroup
		{
			Type TResult
			{
				get;
			}

			void AddProcessor(object processor);

			void RemoveProcessor(object processor);
		}

		private class ResultGroup<T> : IScanResultProvider<T>, IResultGroup
		{
			public Type TResult
			{
				get
				{
					return typeof(T);
				}
			}

			public event EventHandler<ScanResultArgs<T>> ResultProvided;

			public void AddProcessor(object processor)
			{
				if (processor is IScanResultProvider<T>)
				{
					((IScanResultProvider<T>)processor).ResultProvided += OnResultProvided;
				}
			}

			public void RemoveProcessor(object processor)
			{
				if (processor is IScanResultProvider<T>)
				{
					((IScanResultProvider<T>)processor).ResultProvided -= OnResultProvided;
				}
			}

			private void OnResultProvided(object sender, ScanResultArgs<T> args)
			{
				if (this.ResultProvided != null)
				{
					this.ResultProvided(sender, args);
				}
			}
		}

		private class ProcessorDictionary : Dictionary<Type, List<BaseProcessorWrapper>>
		{
		}

		private class ResultProviderDictionary : Dictionary<Type, IResultGroup>
		{
		}

		private static Scanner instance;

		private ProcessorDictionary processors;

		private ResultProviderDictionary resultProviders;

		public static Scanner Global
		{
			get
			{
				if (instance == null)
				{
					instance = new Scanner();
				}
				return instance;
			}
		}

		public Scanner()
		{
			processors = new ProcessorDictionary();
			resultProviders = new ResultProviderDictionary();
		}

		public void Scan(ScanOptions options = null)
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly[] array = assemblies;
			foreach (Assembly assembly in array)
			{
				if (options == null || options.CheckMatch(assembly))
				{
					ScanAssembly(assembly, options);
				}
			}
		}

		public void ScanAssembly(Assembly assembly, ScanOptions options = null)
		{
			Type[] types = assembly.GetTypes();
			Type[] array = types;
			foreach (Type type in array)
			{
				Attribute[] customAttributes = Attribute.GetCustomAttributes(type, false);
				foreach (Attribute attribute in customAttributes)
				{
					if (options == null || options.CheckMatch(attribute))
					{
						ScanAttribute(attribute, type, options);
					}
				}
				if (options == null || options.CheckMatch(type))
				{
					if (type.ContainsGenericParameters)
					{
						ScanGenericType(type, options);
					}
					else
					{
						ScanType(type, options);
					}
				}
			}
		}

		public void ScanType(Type type, ScanOptions options = null)
		{
			ScanType(type, null, options);
		}

		public void ScanGenericType(Type type, ScanOptions options = null)
		{
			ScanGenericType(type, null, options);
		}

		public void ScanMember(MemberInfo member, ScanOptions options = null)
		{
			ScanMember(member, null, options);
		}

		public void ScanAttribute(Attribute attribute, object reflectedObject, ScanOptions options = null)
		{
			ScanAttribute(attribute, reflectedObject, null, options);
		}

		public IBoundInstance ScanInstance(object instance, ScanOptions options = null)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance", "Cannot scan null instance.");
			}
			IBoundInstance result = BoundInstanceFactory.Create(instance);
			ScanType(instance.GetType(), result, options);
			return result;
		}

		public void ScanType(Type type, IBoundInstance instance, ScanOptions options = null)
		{
			try
			{
				BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
				bindingFlags = ((options != null && options.ScanStaticAndInstanceMembers) ? (bindingFlags | (BindingFlags.Instance | BindingFlags.Static)) : ((instance != null) ? (bindingFlags | BindingFlags.Instance) : (bindingFlags | BindingFlags.Static)));
				MemberInfo[] members = type.GetMembers(bindingFlags);
				MemberInfo[] array = members;
				foreach (MemberInfo member in array)
				{
					if (options == null || options.CheckMatch(member))
					{
						ScanMember(member, instance, options);
					}
				}
				if (options == null || options.CheckMatch(type))
				{
					foreach (Type key in processors.Keys)
					{
						if (type.IsSubclassOf(key))
						{
							List<BaseProcessorWrapper> list = processors[key];
							foreach (BaseProcessorWrapper item in list)
							{
								item.ProcessType(type, instance);
							}
						}
					}
				}
			}
			catch (TypeLoadException)
			{
			}
		}

		public void ScanGenericType(Type type, IBoundInstance instance, ScanOptions options = null)
		{
		}

		public void ScanMember(MemberInfo member, IBoundInstance instance, ScanOptions options = null)
		{
			object[] customAttributes = member.GetCustomAttributes(false);
			foreach (object obj in customAttributes)
			{
				Attribute attribute = obj as Attribute;
				if ((options == null || options.CheckMatch(attribute)) && member.MemberType != MemberTypes.NestedType)
				{
					ScanAttribute(attribute, member, instance, options);
				}
			}
			Type baseType = member.GetType().BaseType;
			if (processors.ContainsKey(baseType))
			{
				List<BaseProcessorWrapper> list = processors[baseType];
				foreach (BaseProcessorWrapper item in list)
				{
					item.ProcessMember(member, member.ReflectedType, instance);
				}
			}
		}

		public void ScanAttribute(Attribute attribute, object reflectedObject, IBoundInstance instance, ScanOptions options = null)
		{
			Type type = attribute.GetType();
			if (processors.ContainsKey(type))
			{
				List<BaseProcessorWrapper> list = processors[type];
				foreach (BaseProcessorWrapper item in list)
				{
					if (reflectedObject is MemberInfo)
					{
						item.ProcessAttribute(attribute, (MemberInfo)reflectedObject, instance);
					}
					else if (reflectedObject is Type)
					{
						item.ProcessAttribute(attribute, (Type)reflectedObject, instance);
					}
				}
			}
		}

		public IScanResultProvider<TResult> GetResultProvider<TResult>()
		{
			if (!resultProviders.ContainsKey(typeof(TResult)))
			{
				resultProviders.Add(typeof(TResult), new ResultGroup<TResult>());
			}
			return (IScanResultProvider<TResult>)resultProviders[typeof(TResult)];
		}

		public void AddProcessor<TInput, TResult>(IAttributeScanProcessor<TInput, TResult> processor) where TInput : class
		{
			AddBaseProcessor(processor);
		}

		public void AddProcessor<TInput, TResult>(ITypeScanProcessor<TInput, TResult> processor) where TInput : class
		{
			AddBaseProcessor(processor);
		}

		public void AddProcessor<TInput, TResult>(IMemberScanProcessor<TInput, TResult> processor) where TInput : class
		{
			AddBaseProcessor(processor);
		}

		private void AddBaseProcessor<TInput, TResult>(IScanProcessor<TInput, TResult> processor) where TInput : class
		{
			if (!processors.ContainsKey(typeof(TInput)))
			{
				processors.Add(typeof(TInput), new List<BaseProcessorWrapper>());
			}
			List<BaseProcessorWrapper> list = processors[typeof(TInput)];
			if (!list.Exists((BaseProcessorWrapper wrapper) => wrapper.Processor == processor))
			{
				list.Add(MakeWrapper(processor));
			}
			if (!resultProviders.ContainsKey(typeof(TResult)))
			{
				resultProviders.Add(typeof(TResult), new ResultGroup<TResult>());
			}
			ResultGroup<TResult> resultGroup = (ResultGroup<TResult>)resultProviders[typeof(TResult)];
			resultGroup.AddProcessor(processor);
		}

		public void RemoveProcessor<TInput, TResult>(IScanProcessor<TInput, TResult> processor) where TInput : class
		{
			if (processors.ContainsKey(typeof(TInput)))
			{
				List<BaseProcessorWrapper> list = processors[typeof(TInput)];
				list.RemoveAll((BaseProcessorWrapper wrapper) => wrapper.Processor == processor);
			}
			if (resultProviders.ContainsKey(typeof(TResult)))
			{
				ResultGroup<TResult> resultGroup = (ResultGroup<TResult>)resultProviders[typeof(TResult)];
				resultGroup.RemoveProcessor(processor);
			}
		}

		private BaseProcessorWrapper MakeWrapper<TInput, TResult>(IScanProcessor<TInput, TResult> processor) where TInput : class
		{
			if (processor is IAttributeScanProcessor<TInput, TResult>)
			{
				return new ProcessorWrapper<TInput, TResult>((IAttributeScanProcessor<TInput, TResult>)processor);
			}
			if (processor is ITypeScanProcessor<TInput, TResult>)
			{
				return new ProcessorWrapper<TInput, TResult>((ITypeScanProcessor<TInput, TResult>)processor);
			}
			if (processor is IMemberScanProcessor<TInput, TResult>)
			{
				return new ProcessorWrapper<TInput, TResult>((IMemberScanProcessor<TInput, TResult>)processor);
			}
			return null;
		}
	}
}
