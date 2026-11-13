using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityTest.IntegrationTestRunner;

namespace UnityTest
{
	public class TestRunnerConfigurator
	{
		public static string integrationTestsNetwork = "networkconfig.txt";

		public static string batchRunFileMarker = "batchrun.txt";

		public static string testScenesToRun = "testscenes.txt";

		public static string oldBuildScenes = "oldscenes.txt";

		private readonly List<IPEndPoint> m_IPEndPointList = new List<IPEndPoint>();

		public bool isBatchRun
		{
			get;
			private set;
		}

		public bool sendResultsOverNetwork
		{
			get;
			private set;
		}

		public TestRunnerConfigurator()
		{
			CheckForBatchMode();
			CheckForSendingResultsOverNetwork();
		}

		public string GetIntegrationTestScenes(int testSceneNum)
		{
			string text = (!Application.isEditor) ? GetTextFromTextAsset(testScenesToRun) : GetTextFromTempFile(testScenesToRun);
			List<string> list = new List<string>();
			string[] array = text.Split(new char[1]
			{
				'\n'
			}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text2 in array)
			{
				list.Add(text2.ToString());
			}
			if (testSceneNum < list.Count)
			{
				return list.ElementAt(testSceneNum);
			}
			return null;
		}

		private void CheckForSendingResultsOverNetwork()
		{
			string text = (!Application.isEditor) ? GetTextFromTextAsset(integrationTestsNetwork) : GetTextFromTempFile(integrationTestsNetwork);
			if (text == null)
			{
				return;
			}
			sendResultsOverNetwork = true;
			m_IPEndPointList.Clear();
			string[] array = text.Split(new char[1]
			{
				'\n'
			}, StringSplitOptions.RemoveEmptyEntries);
			int num = 0;
			string text2;
			while (true)
			{
				if (num < array.Length)
				{
					text2 = array[num];
					int num2 = text2.IndexOf(':');
					if (num2 == -1)
					{
						break;
					}
					string ipString = text2.Substring(0, num2);
					string s = text2.Substring(num2 + 1);
					m_IPEndPointList.Add(new IPEndPoint(IPAddress.Parse(ipString), int.Parse(s)));
					num++;
					continue;
				}
				return;
			}
			throw new Exception(text2);
		}

		public static string GetTextFromTextAsset(string fileName)
		{
			string path = fileName.Substring(0, fileName.LastIndexOf('.'));
			TextAsset textAsset = Resources.Load(path) as TextAsset;
			return (textAsset != null) ? textAsset.text : null;
		}

		public static string GetTextFromTempFile(string fileName)
		{
			string result = null;
			try
			{
			}
			catch
			{
				return null;
			}
			return result;
		}

		private void CheckForBatchMode()
		{
			if (GetTextFromTextAsset(batchRunFileMarker) != null)
			{
				isBatchRun = true;
			}
		}

		public static List<string> GetAvailableNetworkIPs()
		{
			if (!NetworkInterface.GetIsNetworkAvailable())
			{
				List<string> list = new List<string>();
				list.Add(IPAddress.Loopback.ToString());
				return list;
			}
			List<UnicastIPAddressInformation> list2 = new List<UnicastIPAddressInformation>();
			List<UnicastIPAddressInformation> list3 = new List<UnicastIPAddressInformation>();
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface networkInterface in allNetworkInterfaces)
			{
				if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
				{
					IEnumerable<UnicastIPAddressInformation> collection = networkInterface.GetIPProperties().UnicastAddresses.Where((UnicastIPAddressInformation a) => a.Address.AddressFamily == AddressFamily.InterNetwork);
					list3.AddRange(collection);
					if (networkInterface.OperationalStatus == OperationalStatus.Up)
					{
						list2.AddRange(collection);
					}
				}
			}
			if (!list2.Any())
			{
				return list3.Select((UnicastIPAddressInformation i) => i.Address.ToString()).ToList();
			}
			list2.Sort(delegate(UnicastIPAddressInformation ip1, UnicastIPAddressInformation ip2)
			{
				int value = BitConverter.ToInt32(ip1.IPv4Mask.GetAddressBytes().Reverse().ToArray(), 0);
				return BitConverter.ToInt32(ip2.IPv4Mask.GetAddressBytes().Reverse().ToArray(), 0).CompareTo(value);
			});
			if (list2.Count == 0)
			{
				List<string> list4 = new List<string>();
				list4.Add(IPAddress.Loopback.ToString());
				return list4;
			}
			return list2.Select((UnicastIPAddressInformation i) => i.Address.ToString()).ToList();
		}

		public ITestRunnerCallback ResolveNetworkConnection()
		{
			List<NetworkResultSender> list = m_IPEndPointList.Select((IPEndPoint ipEndPoint) => new NetworkResultSender(ipEndPoint.Address.ToString(), ipEndPoint.Port)).ToList();
			TimeSpan t = TimeSpan.FromSeconds(30.0);
			DateTime now = DateTime.Now;
			while (DateTime.Now - now < t)
			{
				foreach (NetworkResultSender item in list)
				{
					try
					{
						if (!item.Ping())
						{
							continue;
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						sendResultsOverNetwork = false;
						return null;
					}
					return item;
				}
				Thread.Sleep(500);
			}
			Debug.LogError("Couldn't connect to the server: " + string.Join(", ", m_IPEndPointList.Select((IPEndPoint ipep) => string.Concat(ipep.Address, ":", ipep.Port)).ToArray()));
			sendResultsOverNetwork = false;
			return null;
		}
	}
}
