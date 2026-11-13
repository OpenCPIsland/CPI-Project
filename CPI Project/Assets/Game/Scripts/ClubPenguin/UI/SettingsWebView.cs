using ClubPenguin.Configuration;
using ClubPenguin.ContentGates;
using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.Kelowna.Common.SEDFSM;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
    public class SettingsWebView : MonoBehaviour
    {
        public Button ScrollUp;
        public Button ScrollDown;
        public GameObject WebViewerPanel;
        public Text TitleText;
        public GameObject LoadingPanel;
        public GameObject WebViewFailedPanel;
        public string UrlToken;
        public string TitleToken;
        public bool IsParentGated;

        private void Start()
        {
            setWebPageSubPanelTitle(TitleToken);
            OpenCloseTweener componentInParent = GetComponentInParent<OpenCloseTweener>();
            if (componentInParent.IsOpen && !componentInParent.IsTransitioning)
            {
                StartCoroutine(waitForEndOfFrame());
            }
            else
            {
                componentInParent.OnComplete += onTweenComplete;
            }
        }

        private IEnumerator waitForEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            showURL(UrlToken, IsParentGated);
        }

        private void onTweenComplete()
        {
            GetComponentInParent<OpenCloseTweener>().OnComplete -= onTweenComplete;
            showURL(UrlToken, IsParentGated);
        }

        private void showURL(string urlToken, bool isParentGate)
        {
        }

        private void setWebPageSubPanelTitle(string titleToken)
        {
            TitleText.text = Service.Get<Localizer>().GetTokenTranslation(titleToken);
        }

        public void OnBackClicked()
        {
            closeSubScreen();
        }

        private void closeSubScreen()
        {
            GetComponentInParent<StateMachineContext>().SendEvent(new ExternalEvent("Settings", "back"));
        }

        private void OnDestroy()
        {
        }
    }
}