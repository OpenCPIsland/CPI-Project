using System;

namespace JsonFx.Json
{
	public class JsonReaderSettings
	{
		public TypeCoercionUtility Coercion = new TypeCoercionUtility();

		private bool allowUnquotedObjectKeys;

		private string typeHintName;

		public bool AllowNullValueTypes
		{
			get
			{
				return Coercion.AllowNullValueTypes;
			}
			set
			{
				Coercion.AllowNullValueTypes = value;
			}
		}

		public bool AllowUnquotedObjectKeys
		{
			get
			{
				return allowUnquotedObjectKeys;
			}
			set
			{
				allowUnquotedObjectKeys = value;
			}
		}

		public string TypeHintName
		{
			get
			{
				return typeHintName;
			}
			set
			{
				typeHintName = value;
			}
		}

		public bool IsTypeHintName(string name)
		{
			return !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(typeHintName) && StringComparer.Ordinal.Equals(typeHintName, name);
		}
	}
}
