using System;
using System.Globalization;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.Utility
{
	public class Utilities
	{
		public static TYPE ParseForEnum<TYPE>(string enumname, TYPE defaultval)
		{
			TYPE[] array = (TYPE[])Enum.GetValues(typeof(TYPE));
			TYPE[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				TYPE result = array2[i];
				if (enumname == result.ToString())
				{
					return result;
				}
			}
			return defaultval;
		}

		public static TYPE ParseForNumber<TYPE>(string inString, TYPE inDefault)
		{
			try
			{
				Type typeFromHandle = typeof(TYPE);
				if (typeFromHandle.Equals(typeof(byte)))
				{
					return (TYPE)(object)byte.Parse(inString, NumberStyles.Number, CultureInfo.InvariantCulture);
				}
				if (typeFromHandle.Equals(typeof(int)))
				{
					return (TYPE)(object)int.Parse(inString, NumberStyles.Number, CultureInfo.InvariantCulture);
				}
				if (typeFromHandle.Equals(typeof(uint)))
				{
					return (TYPE)(object)uint.Parse(inString, NumberStyles.Number, CultureInfo.InvariantCulture);
				}
				if (typeFromHandle.Equals(typeof(float)))
				{
					return (TYPE)(object)float.Parse(inString, NumberStyles.Number, CultureInfo.InvariantCulture);
				}
				if (typeFromHandle.Equals(typeof(double)))
				{
					return (TYPE)(object)double.Parse(inString, NumberStyles.Number, CultureInfo.InvariantCulture);
				}
				if (typeFromHandle.Equals(typeof(decimal)))
				{
					return (TYPE)(object)decimal.Parse(inString, NumberStyles.Number, CultureInfo.InvariantCulture);
				}
				return inDefault;
			}
			catch (Exception)
			{
				return inDefault;
			}
		}

		public static bool IsNumeric(string Expression)
		{
			double result;
			return double.TryParse(Convert.ToString(Expression), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out result);
		}

		public static string DateToString(DateTime date)
		{
			return date.Year + "#" + date.Month + "#" + date.Day;
		}

		public static string DateTimeToString(DateTime date)
		{
			return DateToString(date) + "#" + date.Hour + "#" + date.Minute + "#" + date.Second;
		}

		public static DateTime StringToDateTime(string s, bool isUTC = false)
		{
			DateTime now = DateTime.Now;
			string[] array = s.Split('#');
			int num = array.Length;
			int year = 1;
			int month = 1;
			int day = 1;
			int hour = 0;
			int minute = 0;
			int second = 0;
			if (num > 0)
			{
				year = ParseForNumber(array[0], 0);
			}
			if (num > 1)
			{
				month = ParseForNumber(array[1], 0);
			}
			if (num > 2)
			{
				day = ParseForNumber(array[2], 0);
			}
			if (num > 3)
			{
				hour = ParseForNumber(array[3], 0);
			}
			if (num > 4)
			{
				minute = ParseForNumber(array[4], 0);
			}
			if (num > 5)
			{
				second = ParseForNumber(array[5], 0);
			}
			try
			{
				if (isUTC)
				{
					return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
				}
				return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Local);
			}
			catch (Exception ex)
			{
				Logger.LogWarning(null, "Utilities: DateTime parsing error" + ex.Message);
				return DateTime.Now;
			}
		}

		public static string formatVector3(Vector3 vector)
		{
			return string.Format("{0}, {1}, {2}", vector.x, vector.y, vector.z);
		}

		public static Vector3 stringToVector3(string s)
		{
			Vector3 result = new Vector3(0f, 0f, 0f);
			string[] array = s.Split(',');
			for (int i = 0; i < 3; i++)
			{
				array[i] = array[i].TrimStart(' ');
				array[i] = array[i].TrimEnd(' ');
				result[i] = float.Parse(array[i], CultureInfo.InvariantCulture);
			}
			return result;
		}

		public static string AddCommas(float value)
		{
			float num = Mathf.Abs(value);
			int num2 = Mathf.FloorToInt(num);
			float num3 = num - (float)num2;
			string text = AddCommas(num2);
			num3 *= 100000f;
			num2 = Mathf.FloorToInt(num3);
			while (num2 % 10 == 0 && num2 >= 10)
			{
				num2 /= 10;
			}
			if (num2 > 0)
			{
				text = text + "." + AddCommas(num2);
			}
			if (value < 0f)
			{
				text = "-" + text;
			}
			return text;
		}

		public static string AddCommas(int value)
		{
			if (value == 0)
			{
				return "0";
			}
			int num = (int)Mathf.Abs((float)value);
			string text = "";
			while (num > 0)
			{
				string text2 = (num % 1000).ToString();
				while (text2.Length < 3 && num >= 1000)
				{
					text2 = "0" + text2;
				}
				text = ((text.Length <= 0) ? text2 : (text2 + "," + text));
				num /= 1000;
			}
			if (value < 0)
			{
				return "-" + text;
			}
			return text;
		}

		public static bool IsURL(string path)
		{
			string text = path.ToLower();
			return text.StartsWith("http://") || text.StartsWith("https://");
		}

		public static Texture SetObjectTexture(GameObject obj, string partName, int idx, Texture t)
		{
			Component[] componentsInChildren = obj.GetComponentsInChildren(typeof(Renderer));
			Component[] array = componentsInChildren;
			foreach (Component component in array)
			{
				if (component.transform.name == partName)
				{
					Texture mainTexture = ((Renderer)component).materials[idx].mainTexture;
					((Renderer)component).materials[idx].mainTexture = t;
					return mainTexture;
				}
			}
			return null;
		}

		public static void SetObjectTexture(GameObject obj, int idx, Texture t)
		{
			Component componentInChildren = obj.GetComponentInChildren(typeof(Renderer));
			if (componentInChildren != null)
			{
				((Renderer)componentInChildren).materials[idx].mainTexture = t;
			}
		}

		public static void SetObjectTexture(GameObject obj, int idx, string propertyName, Texture t)
		{
			Component componentInChildren = obj.GetComponentInChildren(typeof(Renderer));
			if (componentInChildren != null && propertyName != null && ((Renderer)componentInChildren).materials[idx].HasProperty(propertyName))
			{
				((Renderer)componentInChildren).materials[idx].SetTexture(propertyName, t);
			}
		}

		public static Texture GetObjectTexture(GameObject obj, int idx)
		{
			Component componentInChildren = obj.GetComponentInChildren(typeof(Renderer));
			return ((Renderer)componentInChildren).materials[idx].mainTexture;
		}

		public static void SetLayers(GameObject obj, int layer)
		{
			Component[] componentsInChildren = obj.GetComponentsInChildren<Transform>();
			Component[] array = componentsInChildren;
			foreach (Component component in array)
			{
				component.gameObject.layer = layer;
			}
		}
	}
}
