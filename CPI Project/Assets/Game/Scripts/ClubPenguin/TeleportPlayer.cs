using ClubPenguin.Core;
using Disney.MobileNetwork;
using Tweaker.Core;
using UnityEngine;
using System;

namespace ClubPenguin
{
    public static class TeleportPlayer
    {
        private static bool IsScheduledEventActive(int eventId)
        {
            ScheduledEventDateDefinition definition =
                Service.Get<IGameData>()
                    .Get<System.Collections.Generic.Dictionary<int, ScheduledEventDateDefinition>>()[eventId];

            DateTime now = DateTime.UtcNow;

            return now >= definition.Dates.StartDate.Date &&
                   now < definition.Dates.EndDate.Date;
        }

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
                    localPlayerGameObject.transform.position = new Vector3(-3.21f, 18.012f, 3.4f);
                    Physics.SyncTransforms();
                }
            }
        }

        [PublicTweak]
        [Invokable("SceneLoader.Teleport.SummerSplashdownSkyCafe", Description = "Teleports the player to the Sky Cafe only if the player is in the Town during the Summer Splashdown.")]
        public static void TeleportToTownSkyCafe()
        {
            string sceneName = Service.Get<ZoneTransitionService>().CurrentZone.SceneName;

            if (sceneName == "Town" && IsScheduledEventActive(20))
            {
                GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;

                if (localPlayerGameObject != null)
                {
                    localPlayerGameObject.transform.position = new Vector3(11.182f, 18.017f, -5.991f);
                    Physics.SyncTransforms();
                }
            }
        }

        [PublicTweak]
        [Invokable("SceneLoader.Teleport.PenglantisRuins", Description = "Teleports the player to the Penglantis Ruins if the player is in the Beach.")]
        public static void TeleportToBeachRuins()
        {
            string sceneName = Service.Get<ZoneTransitionService>().CurrentZone.SceneName;

            if (sceneName == "Beach")
            {
                GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;

                if (localPlayerGameObject != null)
                {
                    localPlayerGameObject.transform.position = new Vector3(-35.14183f, 1.130408f, 8.027508f);
                    Physics.SyncTransforms();
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

        [Invokable("SceneLoader.Teleport.PenglantianVault", Description = "Teleport to the Penglantian Vault if the Pirate Party is active.")]
        [PublicTweak]
        public static void TeleportToPenglantianVault()
        {
            if (IsScheduledEventActive(18))
            {
                teleportToZone("EventPirateParty");
            }
        }

        [Invokable("SceneLoader.Teleport.Dungeon", Description = "Teleport to the Dungeon if the Medieval Party is active.")]
        [PublicTweak]
        public static void TeleportToMedievalDungeon()
        {
            if (IsScheduledEventActive(21))
            {
                teleportToZone("EventMedievalDungeon1");
            }
        }

        [Invokable("SceneLoader.Teleport.DotsShop", Description = "Teleport to Dot's Shop (The Disney Store)")]
        [PublicTweak]
        public static void TeleportToDotsShop()
        {
            teleportToZone("DotsShop");
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

        [Invokable("SceneLoader.Teleport.TownInterior", Description = "Teleport to the Town Interior")]
        [PublicTweak]
        public static void TeleportToTownInterior()
        {
            teleportToZone("TownInterior");
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