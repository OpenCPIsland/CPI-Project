using hg.LitJson;
using UnityEngine;

namespace ClubPenguin.Net.Utils
{
	public class WAKLitJsonExtensions
	{
		public static void Register()
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

		private static float importFloatFromDouble(double value)
		{
			return (float)value;
		}
	}
}
