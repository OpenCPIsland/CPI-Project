using ClubPenguin.Net;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class RemotePlayerBotConsumableHelper : MonoBehaviour
	{
		private List<long>[] currentBotArray;

		private List<RemotePlayerBot> remoteBotPlayers = new List<RemotePlayerBot>();

		private int delayFrames;

		private EventDispatcher eventDispatcher;

		private static RemotePlayerBotConsumableHelper helper;

		public static void Initialize(List<RemotePlayerBot> bots, int frames = 0)
		{
			if (helper == null)
			{
				GameObject gameObject = new GameObject("RemoteBotHelper");
				helper = gameObject.AddComponent<RemotePlayerBotConsumableHelper>();
			}
			helper.delayFrames = ((frames == 0) ? 1 : frames);
			helper.remoteBotPlayers = bots;
			helper.enabled = true;
		}

		public void Awake()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			base.enabled = false;
		}

		private void OnEnable()
		{
			currentBotArray = CalculateArray(delayFrames);
			StartCoroutine(ProcessBots());
		}

		private List<long>[] CalculateArray(int frames)
		{
			List<long>[] array = new List<long>[frames];
			for (int i = 0; i < remoteBotPlayers.Count; i++)
			{
				int num = Random.Range(0, array.Length);
				if (array[num] == null)
				{
					array[num] = new List<long>();
				}
				array[num].Add(remoteBotPlayers[i].SessionId);
			}
			return array;
		}

		private IEnumerator ProcessBots()
		{
			for (int i = 0; i < currentBotArray.Length; i++)
			{
				if (currentBotArray[i] != null)
				{
					currentBotArray[i].ForEach(delegate(long bot)
					{
						eventDispatcher.DispatchEvent(new ConsumableServiceEvents.ConsumableUsed(bot, bot.ToString()));
						eventDispatcher.DispatchEvent(new PlayerStateServiceEvents.HeldObjectDequipped(bot));
					});
				}
				yield return new WaitForEndOfFrame();
			}
			Dispose();
		}

		private void Dispose()
		{
			base.enabled = false;
		}
	}
}
