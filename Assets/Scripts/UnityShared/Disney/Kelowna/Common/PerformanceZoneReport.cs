using LitJson;
using System.Collections;
using System.Collections.Generic;

namespace Disney.Kelowna.Common
{
	public class PerformanceZoneReport : Performance.IJsonSerializable
	{
		public string Name;

		public float StartTime;

		public float Length;

		public long Frame;

		public List<Performance.IJsonSerializable> Metrics = new List<Performance.IJsonSerializable>();

		public JsonData ToJson()
		{
			JsonData jsonData = new JsonData();
			jsonData["name"] = Name;
			jsonData["start"] = StartTime;
			jsonData["length"] = Length;
			jsonData["frame"] = Frame;
			JsonData jsonData2 = new JsonData();
			jsonData2.SetJsonType(JsonType.Array);
			foreach (Performance.IJsonSerializable metric in Metrics)
			{
				jsonData2.Add(metric.ToJson());
			}
			jsonData["metrics"] = jsonData2;
			return jsonData;
		}

		public void FromJson(JsonData json)
		{
			Name = (string)json["name"];
			StartTime = (long)json["start"];
			Length = (long)json["length"];
			Frame = (long)json["frame"];
			JsonData jsonData = json["metrics"];
			foreach (JsonData item in (IEnumerable)jsonData)
			{
				JsonData jsonData3 = item["value"];
				Performance.IJsonSerializable jsonSerializable;
				switch (jsonData3.GetJsonType())
				{
				default:
					return;
				case JsonType.Double:
					jsonSerializable = new PerformanceMetricReport<double>();
					break;
				case JsonType.Int:
					jsonSerializable = new PerformanceMetricReport<int>();
					break;
				case JsonType.Long:
					jsonSerializable = new PerformanceMetricReport<long>();
					break;
				}
				jsonSerializable.FromJson(item);
				Metrics.Add(jsonSerializable);
			}
		}
	}
}
