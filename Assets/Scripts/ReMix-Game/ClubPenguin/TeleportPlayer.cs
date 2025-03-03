using Disney.MobileNetwork;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin
{
	public static class TeleportPlayer
	{
		[Invokable("SceneLoader.Teleport.SkyCafe", Description = "Teleports the player to the Sky Cafe only if the player is in the Boardwalk.")]
		[PublicTweak]
		public static void TeleportToSkyCafe()
		{
			string sceneName = Service.Get<ZoneTransitionService>().CurrentZone.SceneName;
			if (sceneName == "Boardwalk")
			{
				GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
				if (localPlayerGameObject != null)
				{
					localPlayerGameObject.transform.position = new Vector3(-3.21f, 18f, 3.4f);
				}
			}
		}

		private static void teleportToZone(string location)
		{
			string sceneName = Service.Get<ZoneTransitionService>().CurrentZone.SceneName;
			if (sceneName != location)
			{
				Service.Get<ZoneTransitionService>().LoadAsZoneOrScene(location, "Loading");
			}
		}

		[PublicTweak]
		[Invokable("SceneLoader.Teleport.Boardwalk", Description = "Teleport to the Boardwalk")]
		public static void TeleportToBoardwalk()
		{
			teleportToZone("Boardwalk");
		}

		[Invokable("SceneLoader.Teleport.Beach", Description = "Teleport to the Beach")]
		[PublicTweak]
		public static void TeleportToBeach()
		{
			teleportToZone("Beach");
		}

		[Invokable("SceneLoader.Teleport.Diving", Description = "Teleport to the Diving Cave")]
		[PublicTweak]
		public static void TeleportToDiving()
		{
			teleportToZone("Diving");
		}

		[Invokable("SceneLoader.Teleport.HerbertBase", Description = "Teleport to Herbert's Base")]
		[PublicTweak]
		public static void TeleportToHerbertBase()
		{
			teleportToZone("HerbertBase");
		}

		[PublicTweak]
		[Invokable("SceneLoader.Teleport.MtBlizzard", Description = "Teleport to Mt Blizzard")]
		public static void TeleportToMtBlizzard()
		{
			teleportToZone("MtBlizzard");
		}

		[PublicTweak]
		[Invokable("SceneLoader.Teleport.MtBlizzardSummit", Description = "Teleport to the Mt Blizzard Summit")]
		public static void TeleportToMtBlizzardSummit()
		{
			teleportToZone("MtBlizzardSummit");
		}

		[PublicTweak]
		[Invokable("SceneLoader.Teleport.Town", Description = "Teleport to the Town")]
		public static void TeleportToTown()
		{
			teleportToZone("Town");
		}

		[Invokable("SceneLoader.Teleport.BoxDimension", Description = "Teleport to the Box Dimension")]
		[PublicTweak]
		public static void TeleportToBoxDimension()
		{
			teleportToZone("BoxDimension");
		}

		[Invokable("SceneLoader.Teleport.Credits", Description = "Teleport to the Credits")]
		[PublicTweak]
		public static void TeleportToCredits()
		{
			teleportToZone("EndCredits");
		}
	}
}
