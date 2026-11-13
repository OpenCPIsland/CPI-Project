using System;
using ClubPenguin;
using ClubPenguin.Adventure;
using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Manimal.Common.Util;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Assets.Game.MiniGames.Scripts.Fishing.FsmActions
{
    [ActionCategory("MiniGames")]
    public class CheckDailyLimitAction : FsmStateAction
    {
        public FsmString GameId = "fishing";

        [HutongGames.PlayMaker.Tooltip("Event to raise when the Daily limit is reached (or exceeded).")]
        public FsmEvent OutOfBaitEvent;

        [HutongGames.PlayMaker.Tooltip("Event to raise when the Daily limit has not yet been reached.")]
        public FsmEvent OkToPlayEvent;

        public override void OnEnter()
        {
            if (!string.IsNullOrEmpty(Service.Get<QuestService>().CurrentFishingPrize))
            {
                base.Fsm.Event(OkToPlayEvent);
            }
            else
            {
                int playCount = GetFishingPlayCount();
                int num = 10 - playCount;
                if (num <= 0)
                {
                    base.Fsm.Event(OutOfBaitEvent);
                }
                else
                {
                    base.Fsm.Event(OkToPlayEvent);
                }
            }
            Finish();
        }

        private int GetFishingPlayCount()
        {
            if (Service.Get<ICommonGameSettings>().OfflineMode)
            {
                string username = Service.Get<RememberMeService>().CurrentUsername;
                if (string.IsNullOrEmpty(username))
                {
                    return 0;
                }
                long num = DateTime.UtcNow.Date.GetTimeInMilliseconds();
                long storedDay = 0L;
                string storedDayStr = PlayerPrefs.GetString(GetPlatformKey("OfflineMinigameProgress_Day_" + username), "0");
                long.TryParse(storedDayStr, out storedDay);
                if (storedDay != num)
                {
                    return 0;
                }
                return PlayerPrefs.GetInt(GetPlatformKey("OfflineMinigameProgress_fishing_" + username), 0);
            }
            else
            {
                CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
                MiniGamePlayCountData component;
                if (!cPDataEntityCollection.LocalPlayerHandle.IsNull && cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
                {
                    if (component.MinigamePlayCounts.ContainsKey(GameId.Value))
                    {
                        return component.MinigamePlayCounts[GameId.Value];
                    }
                    else
                    {
                        component.SetMinigamePlayCount(GameId.Value, 0);
                        return 0;
                    }
                }
            }
            return 0;
        }

        private static string GetPlatformKey(string key)
        {
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
            if (UnityEngine.Application.isEditor)
            {
                return "Editor_" + key;
            }
#endif
            return key;
        }
    }
}