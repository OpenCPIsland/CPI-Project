using ClubPenguin.Net;
using Disney.Kelowna.Common.Environment;
using UnityEngine;

namespace ClubPenguin.Benchmarking
{
	internal class PuppetLoader : MonoBehaviour
	{
		public Environment Environment = Environment.DEV;

		private int loadingPuppet = -1;

		private int runningPuppet = -1;

		private LocalPuppet[] puppets;

		private void Awake()
		{
			LocalPuppet.SwappableNetworkServicesManager.NetworkServicesManager = new NetworkServicesManager(this, NetworkController.GenerateNetworkServiceConfig(Environment), false);
		}

		private void Start()
		{
			puppets = GetComponentsInChildren<LocalPuppet>();
			loadNextPuppet();
		}

		private void loadNextPuppet()
		{
			if (loadingPuppet >= 0)
			{
				puppets[loadingPuppet].OnLoaded -= loadNextPuppet;
			}
			loadingPuppet++;
			if (puppets.Length > loadingPuppet)
			{
				LocalPuppet localPuppet = puppets[loadingPuppet];
				localPuppet.OnLoaded += loadNextPuppet;
				localPuppet.Load();
			}
			else
			{
				startSimulation();
			}
		}

		private void startSimulation()
		{
			runNextPuppet();
		}

		public static void IgnoreEvent(byte eventCode, object content, int senderId)
		{
		}

		private void runNextPuppet()
		{
			if (runningPuppet >= 0)
			{
				puppets[runningPuppet].OnTurnComplete -= runNextPuppet;
			}
			runningPuppet++;
			runningPuppet %= puppets.Length;
			LocalPuppet localPuppet = puppets[runningPuppet];
			localPuppet.OnTurnComplete += runNextPuppet;
			localPuppet.Simulate();
		}
	}
}
