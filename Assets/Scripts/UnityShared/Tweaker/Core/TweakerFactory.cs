using System;
using System.Reflection;
using Tweaker.AssemblyScanner;

namespace Tweaker.Core
{
	public class TweakerFactory : ITweakerFactory
	{
		private IScanner scanner;

		public TweakerFactory(IScanner scanner)
		{
			this.scanner = scanner;
		}

		public T Create<T>(params object[] constructorArgs)
		{
			Type typeFromHandle = typeof(T);
			Type[] array = new Type[constructorArgs.Length];
			for (int i = 0; i < constructorArgs.Length; i++)
			{
				array[i] = constructorArgs[i].GetType();
			}
			ConstructorInfo constructor = typeFromHandle.GetConstructor(array);
			if (constructor == null)
			{
				throw new Exception("Could not find constructor that matches input arguments.");
			}
			T val = (T)Activator.CreateInstance(typeFromHandle, constructorArgs);
			if (val != null)
			{
				scanner.ScanInstance(val);
			}
			return val;
		}
	}
}
