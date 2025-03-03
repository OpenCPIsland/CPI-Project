using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems
{
	public static class JSONManager
	{
		public static event JSONErrorEventHandler ErrorEvent;

		static JSONManager()
		{
			JSONManager.ErrorEvent = delegate
			{
			};
			JsonMapper.RegisterExporter<Vector3>(Vector3Exporter);
			JsonMapper.RegisterExporter<float>(FloatExporter);
			JsonMapper.RegisterExporter<Quaternion>(QuaternionExporter);
			JsonMapper.RegisterImporter<Dictionary<string, float>, Vector3>(Vector3Importer);
			JsonMapper.RegisterImporter<double, float>(FloatImporter);
			JsonMapper.RegisterImporter<Dictionary<string, float>, Quaternion>(QuaternionImporter);
		}

		private static void OnErrorEvent(EventArgs eventArgs)
		{
			if (JSONManager.ErrorEvent != null)
			{
				JSONManager.ErrorEvent(null, eventArgs);
			}
		}

		public static IDictionary getDictionaryForJson(string json)
		{
			IDictionary result = null;
			try
			{
				result = JsonMapper.ToObject<Dictionary<string, object>>(json);
			}
			catch (JsonException ex)
			{
				Logger.LogFatal(typeof(JSONManager), "Error getting Dictionary for JSON string. " + ex.ToString(), Logger.TagFlags.ASSET);
				OnErrorEvent(EventArgs.Empty);
			}
			return result;
		}

		public static IDictionary getDictionaryForJson(StreamReader reader)
		{
			IDictionary result = null;
			try
			{
				result = JsonMapper.ToObject<Dictionary<string, object>>(reader);
			}
			catch (JsonException ex)
			{
				Logger.LogFatal(typeof(JSONManager), "Error getting Dictionary for JSON document. " + ex.ToString(), Logger.TagFlags.ASSET);
				OnErrorEvent(EventArgs.Empty);
			}
			return result;
		}

		public static IList getListForJson(string json)
		{
			IList result = null;
			try
			{
				result = JsonMapper.ToObject<List<object>>(json);
			}
			catch (JsonException ex)
			{
				Logger.LogFatal(typeof(JSONManager), "Error getting List for JSON string. " + ex.ToString(), Logger.TagFlags.ASSET);
				OnErrorEvent(EventArgs.Empty);
			}
			return result;
		}

		public static IList getListForJson(StreamReader reader)
		{
			IList result = null;
			try
			{
				result = JsonMapper.ToObject<List<object>>(reader);
			}
			catch (JsonException ex)
			{
				Logger.LogFatal(typeof(JSONManager), "Error getting List for JSON document. " + ex.ToString(), Logger.TagFlags.ASSET);
				OnErrorEvent(EventArgs.Empty);
			}
			return result;
		}

		public static T getTemplatedTypeForJson<T>(string json)
		{
			T result = default(T);
			try
			{
				result = JsonMapper.ToObject<T>(json);
				return result;
			}
			catch (JsonException ex)
			{
				Logger.LogFatal(typeof(JSONManager), "Error getting templated object for JSON string. " + ex.ToString(), Logger.TagFlags.ASSET);
				OnErrorEvent(EventArgs.Empty);
			}
			return result;
		}

		public static T getTemplatedTypeForJson<T>(StreamReader reader)
		{
			T result = default(T);
			try
			{
				result = JsonMapper.ToObject<T>(reader);
				return result;
			}
			catch (JsonException ex)
			{
				Logger.LogFatal(typeof(JSONManager), "Error getting templated object for JSON document. " + ex.ToString(), Logger.TagFlags.ASSET);
				OnErrorEvent(EventArgs.Empty);
			}
			return result;
		}

		public static string serializeToJson(object data)
		{
			string result = "";
			try
			{
				result = JsonMapper.ToJson(data);
			}
			catch (JsonException ex)
			{
				Logger.LogFatal(typeof(JSONManager), "Error serializing data to JSON. " + ex.ToString(), Logger.TagFlags.ASSET);
				OnErrorEvent(EventArgs.Empty);
			}
			return result;
		}

		public static string serializeToJson(object data, bool usePrintableFormatting)
		{
			string result = "";
			JsonWriter jsonWriter = new JsonWriter();
			jsonWriter.PrettyPrint = usePrintableFormatting;
			try
			{
				JsonMapper.ToJson(data, jsonWriter);
				result = jsonWriter.ToString();
			}
			catch (JsonException ex)
			{
				Logger.LogFatal(typeof(JSONManager), "Error serializing data to JSON. " + ex.ToString(), Logger.TagFlags.ASSET);
				OnErrorEvent(EventArgs.Empty);
			}
			return result;
		}

		public static void FloatExporter(float value, JsonWriter writer)
		{
			writer.Write(value);
		}

		public static float FloatImporter(double value)
		{
			return (float)value;
		}

		public static void Vector3Exporter(Vector3 vector, JsonWriter writer)
		{
			writer.WriteObjectStart();
			writer.WritePropertyName("x");
			writer.Write(vector.x);
			writer.WritePropertyName("y");
			writer.Write(vector.y);
			writer.WritePropertyName("z");
			writer.Write(vector.z);
			writer.WriteObjectEnd();
		}

		public static Vector3 Vector3Importer(Dictionary<string, float> floatDict)
		{
			float x = floatDict["x"];
			float y = floatDict["y"];
			float z = floatDict["z"];
			return new Vector3(x, y, z);
		}

		public static void QuaternionExporter(Quaternion quaternion, JsonWriter writer)
		{
			writer.WriteObjectStart();
			writer.WritePropertyName("x");
			writer.Write(quaternion.x);
			writer.WritePropertyName("y");
			writer.Write(quaternion.y);
			writer.WritePropertyName("z");
			writer.Write(quaternion.z);
			writer.WritePropertyName("w");
			writer.Write(quaternion.w);
			writer.WriteObjectEnd();
		}

		public static Quaternion QuaternionImporter(Dictionary<string, float> floatDict)
		{
			float x = floatDict["x"];
			float y = floatDict["y"];
			float z = floatDict["z"];
			float w = floatDict["w"];
			return new Quaternion(x, y, z, w);
		}
	}
}
