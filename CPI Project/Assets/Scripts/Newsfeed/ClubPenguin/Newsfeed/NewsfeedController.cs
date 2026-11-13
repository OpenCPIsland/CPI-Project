using ClubPenguin.Core;
using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Newsfeed
{
    public class NewsfeedController : MonoBehaviour
    {
        private const string NEWSFEED_LOGIN_TIMESTAMP_PLAYERPREFS_KEY = "newsfeed_login_timestamp";

        private CPDataEntityCollection dataEntityCollection;

        private Localizer localizer;

        private static string GetPlatformKey(string key)
        {
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
            if (Application.isEditor)
            {
                return "Editor_" + key;
            }
#endif
            return key;
        }

#pragma warning disable 0067
        public event System.Action NewsfeedClosed;

        public event System.Action NewsfeedLoaded;

        public event System.Action NewsfeedFailed;

        public event System.Action NewsfeedLoginSucceeded;
#pragma warning restore 0067

        private void Start()
        {
            dataEntityCollection = Service.Get<CPDataEntityCollection>();
            localizer = Service.Get<Localizer>();
            Service.Get<EventDispatcher>().AddListener<NewsfeedServiceEvents.LatestPostTime>(onLatestPostTime);
            newPostCheck();
        }

        private void OnDestroy()
        {
            Service.Get<EventDispatcher>().RemoveListener<NewsfeedServiceEvents.LatestPostTime>(onLatestPostTime);
        }

        private void OnApplicationPause(bool isPaused)
        {
            if (!isPaused)
            {
                newPostCheck();
            }
        }

        public void ShowOnPanel(string urlToken, Button scrollUp, Button scrollDown, GameObject viewerPanel, string javaScriptLoginFunction = "")
        {
        }

        public void Close()
        {
        }

        private void newPostCheck()
        {
            NewPostData component;
            if (!dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
            {
                if (PlayerPrefs.GetInt(GetPlatformKey(NEWSFEED_LOGIN_TIMESTAMP_PLAYERPREFS_KEY)) <= 0)
                {
                    dataEntityCollection.AddComponent<NewPostData>(dataEntityCollection.LocalPlayerHandle);
                }
                else
                {
                    Service.Get<INetworkServicesManager>().NewsfeedService.GetLatestPostTime(localizer.Language.ToString());
                }
            }
        }

        private bool onLatestPostTime(NewsfeedServiceEvents.LatestPostTime evt)
        {
            if (evt.Timestamp > PlayerPrefs.GetInt(GetPlatformKey(NEWSFEED_LOGIN_TIMESTAMP_PLAYERPREFS_KEY)))
            {
                dataEntityCollection.AddComponent<NewPostData>(dataEntityCollection.LocalPlayerHandle);
            }
            return false;
        }
    }
}