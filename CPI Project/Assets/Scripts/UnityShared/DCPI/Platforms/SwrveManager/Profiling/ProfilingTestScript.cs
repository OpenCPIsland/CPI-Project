using SwrveUnity;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace DCPI.Platforms.SwrveManager.Profiling
{
	public class ProfilingTestScript : MonoBehaviour
	{
		private const float NumIterations = 5f;

		private string report;

		public GameObject swrveManager;

		public static readonly string SWRVE_API_KEY = "NCpGE1rI4fH1UvTKlxcP";

		public static readonly int SWRVE_API_ID = 3901;

		private void Start()
		{
			Stopwatch stopwatch = new Stopwatch();
			long num = stopwatch.RunTest(delegate
			{
				for (long num3 = 0L; (float)num3 < 5f; num3++)
				{
					if (SwrveManager.instance == null)
					{
						swrveManager = (GameObject)Object.Instantiate(Resources.Load("SwrveManager"));
					}
					SwrveConfig customConfig = new SwrveConfig
					{
						PushNotificationEnabled = true,
						GCMSenderId = "1234567890",
						AndroidPushNotificationTitle = "Profiler notifications",
						UserId = "profiler"
					};
					Dictionary<string, string> customData = new Dictionary<string, string>
					{
						{
							"customData1",
							"fancy-custom-data-1"
						},
						{
							"customData2",
							"fancy-custom-data-2"
						},
						{
							"customData3",
							"fancy-custom-data-3"
						},
						{
							"lat.is_lat",
							"this-should-not-overwrite"
						}
					};
					SwrveManager.DebugLogging = true;
					SwrveManager.instance.InitWithAnalyticsKeySecretConfigAndCustomData(SWRVE_API_ID, SWRVE_API_KEY, customConfig, customData);
					Object.Destroy(SwrveManager.instance.gameObject);
					SwrveManager.instance = null;
					swrveManager = null;
				}
			});
			float num2 = (float)num / 5f;
			report = "Test, Total Test Time (milliseconds), Avg Test Time (milliseconds)\nSwrveManagerInit, " + num + ", " + num2 + "\n";
		}

		private void OnGUI()
		{
			Rect position = new Rect(0f, 0f, Screen.width, Screen.height);
			GUI.TextArea(position, report);
		}
	}
}
