using ClubPenguin.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using System.Collections.Generic;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin
{
	public class RemotePlayerBotRemover : MonoBehaviour
	{
		public int BotsToRemove;

		private int botsLeftToRemove;

		public int RemoveDelay;

		private int removeDelayCounter;

		private List<RemotePlayerBot> remoteBotPlayers = new List<RemotePlayerBot>();

		private static RemotePlayerBotRemover remover;

		public int BotsLeftToRemove
		{
			get
			{
				return botsLeftToRemove;
			}
		}

		public int RemoveDelayCounter
		{
			get
			{
				return removeDelayCounter;
			}
		}

		public void Awake()
		{
			base.enabled = false;
		}

		public void OnEnable()
		{
			botsLeftToRemove = BotsToRemove;
			removeDelayCounter = RemoveDelay;
			remoteBotPlayers.Clear();
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle[] remotePlayerHandles = cPDataEntityCollection.GetRemotePlayerHandles();
			int num = remotePlayerHandles.Length;
			for (int i = 0; i < num; i++)
			{
				DisplayNameData component;
				if (cPDataEntityCollection.TryGetComponent(remotePlayerHandles[i], out component))
				{
					RemotePlayerBot component2 = RemotePlayerBotUtil.GetBotContainer().Find(component.DisplayName).GetComponent<RemotePlayerBot>();
					remoteBotPlayers.Add(component2);
				}
			}
		}

		public void Update()
		{
			if (botsLeftToRemove > 0)
			{
				removeDelayCounter--;
				if (removeDelayCounter <= 0)
				{
					removeBot();
					removeDelayCounter = RemoveDelay;
				}
			}
			else
			{
				base.enabled = false;
			}
		}

		private void removeBot()
		{
			if (remoteBotPlayers.Count > 0)
			{
				RemotePlayerBot remotePlayerBot = remoteBotPlayers[0];
				remotePlayerBot.Remove(0f);
				remoteBotPlayers.RemoveAt(0);
			}
			botsLeftToRemove--;
		}

		private static void initialize()
		{
			if (remover == null)
			{
				GameObject gameObject = new GameObject("RemoteBotRemover");
				remover = gameObject.AddComponent<RemotePlayerBotRemover>();
			}
		}

		[Invokable("Bots.Remove", Description = "Remove remote player bots in a first in, first out policy (Order of data game objects)")]
		private static void removeBots([ArgDescription("The number of remote bots to remove.")] int number, [ArgDescription("The number of frames delay before each remote penguin is removed")] int delay)
		{
			initialize();
			remover.BotsToRemove = number;
			remover.RemoveDelay = delay;
			remover.enabled = true;
		}
	}
}
