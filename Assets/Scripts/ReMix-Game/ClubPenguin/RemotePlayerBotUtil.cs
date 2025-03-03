using System.Collections.Generic;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin
{
	public static class RemotePlayerBotUtil
	{
		private static Transform botContainer;

		public static Transform GetBotContainer()
		{
			if (botContainer == null || botContainer.IsDestroyed())
			{
				botContainer = new GameObject("BotContainer").transform;
			}
			return botContainer;
		}

		private static List<RemotePlayerBot> findBots()
		{
			return new List<RemotePlayerBot>(GetBotContainer().GetComponentsInChildren<RemotePlayerBot>());
		}

		[Invokable("Bots.EquipConsumable")]
		private static void equip([ArgDescription("The consumable to equip.")] string consumable = "PartyBlaster")
		{
			List<RemotePlayerBot> list = findBots();
			list.ForEach(delegate(RemotePlayerBot bot)
			{
				bot.EquipConsumable(consumable);
			});
		}

		[Invokable("Bots.UseConsumable")]
		private static void use([ArgDescription("The number of frames to delay consumable use over.")] int frames = 0)
		{
			List<RemotePlayerBot> bots = findBots();
			RemotePlayerBotConsumableHelper.Initialize(bots, frames);
		}

		[Invokable("Bots.StartRandomChatter")]
		private static void startRandomChatter([ArgDescription("Per bot random delay before starting chatter.")] float maxRandomStartDelay = 7f)
		{
			List<RemotePlayerBot> list = findBots();
			list.ForEach(delegate(RemotePlayerBot b)
			{
				b.StartRandomChatter(maxRandomStartDelay);
			});
		}
	}
}
