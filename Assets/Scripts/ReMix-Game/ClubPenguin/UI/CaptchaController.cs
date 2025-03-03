using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class CaptchaController : MonoBehaviour
	{
		private struct SelectedImageInfo
		{
			public string ToggleId;

			public Vector2 ImageLocalPosition;
		}

		public GameObject LoadingSpinner;

		public GameObject ImagesGridContainer;

		public Text ChallengeText;

		public GameObject ChallengeFail;

		public Image CaptchaImage;

		public RectTransform CaptchaImageRectTransform;

		private string captchaId;

		private CaptchaData captchaData;

		private CaptchaType captchaType;

		private ToggleInspector[] imageToggles;

		private List<SelectedImageInfo> selectedImagesList;

		private Camera guiCamera;

		private Localizer localizer;

		private EventDispatcher eventDispatcher;

		private void OnDestroy()
		{
			eventDispatcher.RemoveListener<CaptchaServiceEvents.CaptchaLoaded>(onCaptchaLoaded);
			eventDispatcher.RemoveListener<CaptchaServiceEvents.CaptchaSolutionAccepted>(onCaptchaSolutionAccepted);
			eventDispatcher.RemoveListener<CaptchaServiceEvents.CaptchaSolutionDeclined>(onCaptchaSolutionDeclined);
			for (int i = 0; i < imageToggles.Length; i++)
			{
				imageToggles[i].ToggleClicked -= onToggleOnClickedPosition;
			}
		}

		public void SetupCaptcha(CaptchaType captchaType, int? width = null, int? height = null)
		{
			this.captchaType = captchaType;
			localizer = Service.Get<Localizer>();
			eventDispatcher = Service.Get<EventDispatcher>();
			imageToggles = GetComponentsInChildren<ToggleInspector>();
			for (int i = 0; i < imageToggles.Length; i++)
			{
				imageToggles[i].ToggleClicked += onToggleOnClickedPosition;
			}
			setLoadingSpinnerVisibility(true);
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowTopPopup(base.gameObject));
			guiCamera = base.gameObject.GetComponentInParent<Canvas>().GetComponent<Camera>();
			eventDispatcher.AddListener<CaptchaServiceEvents.CaptchaLoaded>(onCaptchaLoaded);
			eventDispatcher.AddListener<CaptchaServiceEvents.CaptchaSolutionAccepted>(onCaptchaSolutionAccepted);
			eventDispatcher.AddListener<CaptchaServiceEvents.CaptchaSolutionDeclined>(onCaptchaSolutionDeclined);
			Service.Get<INetworkServicesManager>().CaptchaService.GetCaptcha(captchaType, width, height);
		}

		private void setLoadingSpinnerVisibility(bool show)
		{
			LoadingSpinner.SetActive(show);
			ImagesGridContainer.SetActive(!show);
			CaptchaImage.enabled = !show;
		}

		private bool onCaptchaLoaded(CaptchaServiceEvents.CaptchaLoaded evt)
		{
			captchaId = evt.CaptchaData.id;
			captchaData = evt.CaptchaData;
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.LoadImage(captchaData.CaptchaImageBytes);
			Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
			CaptchaImage.sprite = sprite;
			string challengeToken = captchaData.challengeToken;
			challengeToken += ".Desktop";
			string tokenTranslation = localizer.GetTokenTranslation(challengeToken);
			try
			{
				ChallengeText.text = string.Format(tokenTranslation, captchaData.solutionSize);
			}
			catch (Exception)
			{
				Log.LogErrorFormatted(this, "An error occurred trying to format the challenge token {0}.", challengeToken);
			}
			selectedImagesList = new List<SelectedImageInfo>();
			setLoadingSpinnerVisibility(false);
			return false;
		}

		private bool onCaptchaSolutionAccepted(CaptchaServiceEvents.CaptchaSolutionAccepted evt)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return false;
		}

		private bool onCaptchaSolutionDeclined(CaptchaServiceEvents.CaptchaSolutionDeclined evt)
		{
			ChallengeFail.SetActive(true);
			DateTime retryDate = DateTimeUtils.DateTimeFromUnixTime(evt.NewCaptchaData.retryTimestamp);
			CoroutineRunner.Start(showNextCaptcha(retryDate, evt.NewCaptchaData), this, "showNextCaptcha");
			return false;
		}

		private IEnumerator showNextCaptcha(DateTime retryDate, CaptchaData captchaData)
		{
			while (retryDate > DateTime.Now)
			{
				yield return null;
			}
			Service.Get<EventDispatcher>().DispatchEvent(new CaptchaServiceEvents.CaptchaLoaded(captchaData));
		}

		private void onToggleOnClickedPosition(string toggleId, bool isToggleOn, Vector2 toggleClickPosition)
		{
			if (!isToggleOn)
			{
				int num = 0;
				SelectedImageInfo item;
				while (true)
				{
					if (num < selectedImagesList.Count)
					{
						item = selectedImagesList[num];
						if (item.ToggleId == toggleId)
						{
							break;
						}
						num++;
						continue;
					}
					return;
				}
				selectedImagesList.Remove(item);
				return;
			}
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(CaptchaImageRectTransform, toggleClickPosition, guiCamera, out localPoint);
			SelectedImageInfo item2 = default(SelectedImageInfo);
			item2.ToggleId = toggleId;
			item2.ImageLocalPosition = localPoint;
			selectedImagesList.Add(item2);
			if (selectedImagesList.Count == captchaData.solutionSize)
			{
				CaptchaSolution captchaSolution = new CaptchaSolution();
				captchaSolution.id = captchaId;
				captchaSolution.captchaDimensions = captchaData.captchaDimensions;
				List<SolutionPoint> list = new List<SolutionPoint>();
				for (int num = 0; num < selectedImagesList.Count; num++)
				{
					SolutionPoint solutionPoint = new SolutionPoint();
					solutionPoint.x = (int)selectedImagesList[num].ImageLocalPosition.x;
					solutionPoint.y = (int)selectedImagesList[num].ImageLocalPosition.y;
					list.Add(solutionPoint);
				}
				captchaSolution.solution = list;
				CoroutineRunner.Start(showLoadingSpinnerDelay(captchaSolution), this, "showLoadingSpinnerDelay");
			}
		}

		private IEnumerator showLoadingSpinnerDelay(CaptchaSolution solution)
		{
			yield return new WaitForFrame(5);
			setLoadingSpinnerVisibility(true);
			for (int i = 0; i < imageToggles.Length; i++)
			{
				imageToggles[i].Toggle.isOn = false;
			}
			Service.Get<INetworkServicesManager>().CaptchaService.PostCaptchaSolution(captchaType, solution);
		}
	}
}
