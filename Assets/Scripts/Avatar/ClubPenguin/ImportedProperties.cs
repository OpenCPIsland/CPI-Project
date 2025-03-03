using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class ImportedProperties : MonoBehaviour
	{
		[Serializable]
		public struct StringProperty
		{
			public string Key;

			public string Value;

			public StringProperty(string key, object value)
			{
				Key = key;
				Value = (string)value;
			}
		}

		[Serializable]
		public struct FloatProperty
		{
			public string Key;

			public float Value;

			public FloatProperty(string key, object value)
			{
				Key = key;
				Value = (float)value;
			}
		}

		[Serializable]
		public struct ColorProperty
		{
			public string Key;

			public Color Value;

			public ColorProperty(string key, object value)
			{
				Key = key;
				Value = new Color(((Vector4)value).x, ((Vector4)value).y, ((Vector4)value).z);
			}
		}

		[Serializable]
		public struct BooleanProperty
		{
			public string Key;

			public bool Value;

			public BooleanProperty(string key, object value)
			{
				Key = key;
				Value = (bool)value;
			}
		}

		public List<StringProperty> Strings;

		public List<FloatProperty> Floats;

		public List<ColorProperty> Colors;

		public List<BooleanProperty> Booleans;

		public void Parse(string[] keys, object[] values)
		{
			Strings = new List<StringProperty>();
			Floats = new List<FloatProperty>();
			Colors = new List<ColorProperty>();
			Booleans = new List<BooleanProperty>();
			int num = 0;
			string text;
			object obj;
			Type type;
			while (true)
			{
				if (num >= keys.Length)
				{
					return;
				}
				text = keys[num];
				obj = values[num];
				type = obj.GetType();
				if (type == typeof(string))
				{
					Strings.Add(new StringProperty(text, obj));
				}
				else if (type == typeof(float))
				{
					Floats.Add(new FloatProperty(text, obj));
				}
				else if (type == typeof(bool))
				{
					Booleans.Add(new BooleanProperty(text, obj));
				}
				else
				{
					if (type != typeof(Vector4))
					{
						break;
					}
					Colors.Add(new ColorProperty(text, obj));
				}
				num++;
			}
			throw new UnityException(string.Format("Imported property {0} has invalid type {1}. Value: {2}!", text, type, obj));
		}
	}
}
