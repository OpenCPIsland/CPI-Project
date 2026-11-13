using System;
using System.Collections.Generic;
using System.Reflection;

namespace DisneyMobile.CoreUnitySystems
{
	public class AutoConfigurable : IConfigurable
	{
		public static void AutoConfigureObject(object objectToBeConfigured, IDictionary<string, object> dictionary)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			if (dictionary != null)
			{
				IDictionary<string, object> dictionary2 = dictionary["values"] as IDictionary<string, object>;
				foreach (string key in dictionary2.Keys)
				{
					string text = key;
					try
					{
						Type type = objectToBeConfigured.GetType();
						FieldInfo field = type.GetField(text, bindingAttr);
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
						Logger.LogFatal(objectToBeConfigured, "AutoConfiguration failed for class: " + objectToBeConfigured.GetType().ToString() + " cannot set value on property " + text + " as no member variable or property exists in that class. Please verify that the variable in the configuration file matches the variable name in the class.", Logger.TagFlags.INIT);
					}
				}
			}
		}

		public virtual void Configure(IDictionary<string, object> dictionary)
		{
			AutoConfigureObject(this, dictionary);
		}

		public virtual void Reconfigure(IDictionary<string, object> dictionary)
		{
			Configure(dictionary);
		}
	}
}
