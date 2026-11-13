using System;
using System.Collections.Generic;

namespace Tweaker.Core
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
	public class NamedToggleValueAttribute : Attribute
	{
		public interface NamedToggleValueGenerator
		{
			IEnumerable<NamedToggleValue> GetNameToggleValues();
		}

		public struct NamedToggleValue
		{
			public readonly string Name;

			public readonly object Value;

			public NamedToggleValue(string name, object value)
			{
				Name = name;
				Value = value;
			}
		}

		public string Name;

		public object Value;

		public uint Order;

		public Type ValueGeneratorType;

		public NamedToggleValueAttribute(string name, object value, uint order = 0u)
		{
			Name = name;
			Value = value;
			Order = order;
		}

		public NamedToggleValueAttribute(Type valueGeneratorType, uint order = 0u)
		{
			ValueGeneratorType = valueGeneratorType;
			Order = order;
		}
	}
}
