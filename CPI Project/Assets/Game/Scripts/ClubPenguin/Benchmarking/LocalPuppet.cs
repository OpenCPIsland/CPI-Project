using ClubPenguin.Locomotion;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Locomotion;
using Disney.Kelowna.Common.Environment;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Benchmarking
{
	internal class LocalPuppet : MonoBehaviour
	{
		public PlayerOutfit outfit = null;

		public string Username = "test";

		public string Password = "test";

		public int RoomId = 1;

		public Disney.Kelowna.Common.Environment.Environment Environment = Disney.Kelowna.Common.Environment.Environment.DEV;

		private NetworkServicesManager networkServicesManager;

		private PuppetNetworkController networkController;

		public static SwappableNetworkServicesManager SwappableNetworkServicesManager;

		public event Action OnLoaded;

		public event Action OnTurnComplete;

		static LocalPuppet()
		{
			SwappableNetworkServicesManager = new SwappableNetworkServicesManager();
			Service.Set((INetworkServicesManager)SwappableNetworkServicesManager);
		}

		public void Load()
		{
			networkServicesManager = new NetworkServicesManager(this, NetworkController.GenerateNetworkServiceConfig(Environment), false);
			SwappableNetworkServicesManager.NetworkServicesManager = networkServicesManager;
			base.tag = "Player";
			if (this.OnLoaded != null)
			{
				this.OnLoaded();
			}
		}

		private void OnApplicationQuit()
		{
		}

		public virtual void Simulate()
		{
			SwappableNetworkServicesManager.NetworkServicesManager = networkServicesManager;
			move(Vector2.right);
		}

		protected void move(Vector2 direction)
		{
			GetComponent<RunController>().Steer(direction);
			StartCoroutine(waitForMovement());
		}

		private IEnumerator waitForMovement()
		{
			yield return null;
			yield return new WaitForSeconds(1f);
			GetComponent<RunController>().Steer(Vector2.zero);
			yield return StartCoroutine(checkForMoveComplete());
		}

		private IEnumerator checkForMoveComplete()
		{
			LocomotionBroadcaster lcb = GetComponent<LocomotionBroadcaster>();
			while (lcb.MovementDirty())
			{
				yield return null;
			}
			if (this.OnTurnComplete != null)
			{
				this.OnTurnComplete();
			}
		}
	}
}
