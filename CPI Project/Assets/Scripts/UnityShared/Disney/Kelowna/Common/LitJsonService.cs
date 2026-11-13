using LitJson;
using System;
using System.Reflection;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class LitJsonService : JsonService
	{
		private static readonly MethodInfo toObjectMethod = typeof(JsonMapper).GetMethod("ReadValue", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[2]
		{
			typeof(Type),
			typeof(JsonReader)
		}, null);

		private readonly JsonWriter writer = new JsonWriter();

		public bool PrettyPrint
		{
			get;
			set;
		}

		public LitJsonService()
		{
			JsonMapper.RegisterExporter<float>(exportFloatAsDecimal);
			JsonMapper.RegisterExporter<Vector3>(exportVector3AsDecimal);
			JsonMapper.RegisterExporter<Vector2>(exportVector2AsDecimal);
			JsonMapper.RegisterImporter<double, float>(importFloatFromDouble);
		}

		private static void exportFloatAsDecimal(float value, JsonWriter writer)
		{
			writer.Write((decimal)value);
		}

		private static void exportVector3AsDecimal(Vector3 value, JsonWriter writer)
		{
			writer.WriteObjectStart();
			writer.WritePropertyName("x");
			exportFloatAsDecimal(value.x, writer);
			writer.WritePropertyName("y");
			exportFloatAsDecimal(value.y, writer);
			writer.WritePropertyName("z");
			exportFloatAsDecimal(value.z, writer);
			writer.WriteObjectEnd();
		}

		private static void exportVector2AsDecimal(Vector2 value, JsonWriter writer)
		{
			writer.WriteObjectStart();
			writer.WritePropertyName("x");
			exportFloatAsDecimal(value.x, writer);
			writer.WritePropertyName("y");
			exportFloatAsDecimal(value.y, writer);
			writer.WriteObjectEnd();
		}

		private float importFloatFromDouble(double value)
		{
			return (float)value;
		}

		public override string Serialize<T>(T objectToSerialize)
		{
			writer.Reset();
			writer.PrettyPrint = PrettyPrint;
			JsonMapper.ToJson(objectToSerialize, writer);
			return writer.ToString();
		}

		public override T Deserialize<T>(string stringToDeserialize)
		{
			return JsonMapper.ToObject<T>(stringToDeserialize);
		}

		public override object Deserialize(string stringToDeserialize, Type type)
		{
			JsonReader jsonReader = new JsonReader(stringToDeserialize);
			return toObjectMethod.Invoke(null, new object[2]
			{
				type,
				jsonReader
			});
		}
	}
}
