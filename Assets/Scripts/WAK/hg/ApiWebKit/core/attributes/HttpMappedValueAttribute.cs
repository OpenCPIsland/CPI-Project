using hg.ApiWebKit.core.http;
using System;
using System.Reflection;

namespace hg.ApiWebKit.core.attributes
{
	public abstract class HttpMappedValueAttribute : Attribute
	{
		public string VariableName;

		public string VariableValue;

		public string Value;

		public Type Converter;

		public Type[] Converters;

		public MappingDirection Direction
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		protected HttpMappedValueAttribute(MappingDirection direction, string name)
		{
			Direction = direction;
			Name = name;
		}

		public bool MapOnRequest()
		{
			return Direction == MappingDirection.REQUEST || Direction == MappingDirection.ALL;
		}

		public bool MapOnResponse()
		{
			return Direction == MappingDirection.RESPONSE || Direction == MappingDirection.ALL;
		}

		public void Initialize()
		{
			if (Converter != null)
			{
				if (Converters == null)
				{
					Converters = new Type[1]
					{
						Converter
					};
					return;
				}
				Array.Resize(ref Converters, Converters.Length + 1);
				Converters[Converters.Length - 1] = Converter;
			}
		}

		public virtual string OnResponseResolveName(HttpOperation operation, FieldInfo fi, HttpResponse response)
		{
			if (VariableName != null && Configuration.HasSetting(VariableName))
			{
				return Configuration.GetSetting<string>(VariableName);
			}
			if (!string.IsNullOrEmpty(Name))
			{
				return Name;
			}
			return fi.Name;
		}

		public virtual object OnResponseResolveValue(string name, HttpOperation operation, FieldInfo fi, HttpResponse response)
		{
			if (VariableValue != null && Configuration.HasSetting(VariableValue))
			{
				return Configuration.GetSetting(VariableValue);
			}
			if (!string.IsNullOrEmpty(Value))
			{
				return Value;
			}
			return fi.GetValue(operation);
		}

		public virtual object OnResponseApplyConverters(object value, HttpOperation operation, FieldInfo fi)
		{
			if (Converters != null)
			{
				Type[] converters = Converters;
				foreach (Type type in converters)
				{
					IValueConverter valueConverter = Activator.CreateInstance(type) as IValueConverter;
					if (valueConverter == null)
					{
						operation.Log("(HttpMappedValueAttribute)(OnResponseApplyConverters) Converter '" + type.FullName + "' must implement IValueConverter!", LogSeverity.ERROR);
						continue;
					}
					bool successful = false;
					value = valueConverter.Convert(value, fi, out successful, null);
					if (successful)
					{
						operation.Log("(HttpMappedValueAttribute)(OnResponseApplyConverters) Converter '" + type.FullName + "' applied.  Output type is " + ((value != null) ? ("'" + value.GetType().FullName + "'") : "(unknown) because @value is null") + ".", LogSeverity.VERBOSE);
					}
					else
					{
						operation.Log("(HttpMappedValueAttribute)(OnResponseApplyConverters) Converter '" + type.FullName + "' failed!", LogSeverity.WARNING);
					}
				}
			}
			return value;
		}

		public virtual void OnResponseResolveModel(object value, HttpOperation operation, FieldInfo fi)
		{
			value = ((value != null) ? value : (fi.FieldType.IsValueType ? Activator.CreateInstance(fi.FieldType) : ((fi.FieldType.GetConstructor(Type.EmptyTypes) == null) ? null : Activator.CreateInstance(fi.FieldType))));
			fi.SetValue(operation, Convert.ChangeType(value, fi.FieldType));
		}

		public virtual string OnRequestResolveName(HttpOperation operation, FieldInfo fi)
		{
			if (VariableName != null && Configuration.HasSetting(VariableName))
			{
				return Configuration.GetSetting<string>(VariableName);
			}
			if (!string.IsNullOrEmpty(Name))
			{
				return Name;
			}
			if (fi != null)
			{
				return fi.Name;
			}
			return null;
		}

		public virtual object OnRequestResolveValue(string name, HttpOperation operation, FieldInfo fi)
		{
			if (VariableValue != null && Configuration.HasSetting(VariableValue))
			{
				return Configuration.GetSetting(VariableValue);
			}
			if (!string.IsNullOrEmpty(Value))
			{
				return Value;
			}
			if (fi != null)
			{
				return fi.GetValue(operation);
			}
			return null;
		}

		public virtual object OnRequestApplyConverters(object value, HttpOperation operation, FieldInfo fi)
		{
			if (Converters != null)
			{
				Type[] converters = Converters;
				foreach (Type type in converters)
				{
					IValueConverter valueConverter = Activator.CreateInstance(type) as IValueConverter;
					if (valueConverter == null)
					{
						operation.Log("(HttpMappedValueAttribute)(OnRequestApplyConverters) Converter '" + type.FullName + "' must implement IValueConverter!", LogSeverity.ERROR);
						continue;
					}
					bool successful = false;
					value = valueConverter.Convert(value, fi, out successful, null);
					if (successful)
					{
						operation.Log("(HttpMappedValueAttribute)(OnRequestApplyConverters) Converter '" + type.FullName + "' applied.", LogSeverity.VERBOSE);
					}
					else
					{
						operation.Log("(HttpMappedValueAttribute)(OnRequestApplyConverters) Converter '" + type.FullName + "' failed!", LogSeverity.WARNING);
					}
				}
			}
			return value;
		}

		public virtual void OnRequestResolveModel(string name, object value, ref HttpRequestModel model, HttpOperation operation, FieldInfo fi)
		{
		}
	}
}
