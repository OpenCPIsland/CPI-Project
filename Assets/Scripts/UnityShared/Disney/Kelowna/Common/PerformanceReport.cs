using LitJson;
using System.Collections;
using System.Collections.Generic;

namespace Disney.Kelowna.Common
{
	public class PerformanceReport : Performance.IJsonSerializable
	{
		public string Platform;

		public string OperatingSystem;

		public string Device;

		public string BundleId;

		public string Version;

		public string UnityVersion;

		public string Date;

		public float StartTime;

		public float EndTime;

		public List<PerformanceZoneReport> Zones = new List<PerformanceZoneReport>();

		public JsonData ToJson()
		{
			JsonData jsonData = new JsonData();
			jsonData["platform"] = Platform;
			jsonData["operatingSystem"] = OperatingSystem;
			jsonData["device"] = Device;
			jsonData["bundleId"] = BundleId;
			jsonData["version"] = Version;
			jsonData["unity"] = UnityVersion;
			jsonData["date"] = Date;
			jsonData["startTime"] = StartTime;
			jsonData["endTime"] = EndTime;
			JsonData jsonData2 = new JsonData();
			jsonData2.SetJsonType(JsonType.Array);
			foreach (PerformanceZoneReport zone in Zones)
			{
				jsonData2.Add(zone.ToJson());
			}
			jsonData["zones"] = jsonData2;
			return jsonData;
		}

		public void FromJson(JsonData json)
		{
			Platform = (string)json["platform"];
			OperatingSystem = (string)json["operatingSystem"];
			Device = (string)json["device"];
			BundleId = (string)json["bundleId"];
			Version = (string)json["version"];
			UnityVersion = (string)json["unity"];
			Date = (string)json["date"];
			StartTime = (long)json["startTime"];
			EndTime = (long)json["endTime"];
			JsonData jsonData = json["zones"];
			foreach (JsonData item in (IEnumerable)jsonData)
			{
				PerformanceZoneReport performanceZoneReport = new PerformanceZoneReport();
				performanceZoneReport.FromJson(item);
				Zones.Add(performanceZoneReport);
			}
		}
	}
}
