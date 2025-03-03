using System;
using System.Collections.Generic;
using System.Reflection;

namespace Disney.LaunchPadFramework
{
	public class AutoConfigurable : IConfigurable
	{
		public static void AutoConfigureObject(object objectToBeConfigured, IDictionary<string, object> dictionary)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			IDictionary<string, object> dictionary2 = dictionary["values"] as IDictionary<string, object>;
			foreach (string key in dictionary2.Keys)
			{
				string name = "m_" + char.ToLowerInvariant(key[0]) + key.Substring(1);
				try
				{
					Type type = objectToBeConfigured.GetType();
					FieldInfo field = type.GetField(name, bindingAttr);
					if (field == null)
					{
						throw new TargetException();
					}
					Type fieldType = field.FieldType;
					object value = dictionary2[key];
					field.SetValue(objectToBeConfigured, Convert.ChangeType(value, fieldType));
				}
				catch (TargetException)
				{
				}
			}
		}

		public virtual void Configure(IDictionary<string, object> dictionary)
		{
			if (dictionary != null)
			{
				AutoConfigureObject(this, dictionary);
			}
		}
	}
}
