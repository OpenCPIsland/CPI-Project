using LitJson;
using System;

namespace Disney.Kelowna.Common
{
	public class PerformanceMetricReport<T> : Performance.IJsonSerializable where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
	{
		public string Name;

		public T Value;

		public double ExpMovingAverage;

		public double Average;

		public T Min;

		public T Max;

		public ulong UpdateCount;

		public PerformanceMetricReport()
		{
		}

		public PerformanceMetricReport(Performance.Metric<T> metric)
		{
			Name = metric.Name;
			Value = metric.Value;
			ExpMovingAverage = metric.ExpMovingAverage;
			Average = metric.Average;
			Min = metric.Min;
			Max = metric.Max;
			UpdateCount = metric.UpdateCount;
		}

		public JsonData ToJson()
		{
			JsonData jsonData = new JsonData();
			jsonData["name"] = Name;
			jsonData["value"] = new JsonData(Value);
			jsonData["expMovingAverage"] = ExpMovingAverage;
			jsonData["average"] = Average;
			jsonData["min"] = new JsonData(Min);
			jsonData["max"] = new JsonData(Max);
			jsonData["updateCount"] = UpdateCount;
			return jsonData;
		}

		public void FromJson(JsonData json)
		{
			Name = (string)json["name"];
			Value = Performance.Metric<T>.ConvertJsonData(json["value"]);
			ExpMovingAverage = (double)json["expMovingAverage"];
			Average = (double)json["average"];
			Min = Performance.Metric<T>.ConvertJsonData(json["min"]);
			Max = Performance.Metric<T>.ConvertJsonData(json["max"]);
			UpdateCount = (ulong)(double)json["updateCount"];
		}
	}
}
