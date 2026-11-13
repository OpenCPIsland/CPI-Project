using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;
using System;
using UnityEngine;

namespace ClubPenguin.Net
{
	public class CaptchaService : BaseNetworkService, ICaptchaService, INetworkService
	{
		public class CaptchaErrorResponse : ErrorResponse
		{
			public string captcha;
		}

		public bool BypassCaptcha
		{
			get;
			set;
		}

		protected override void setupListeners()
		{
		}

		public void GetCaptcha(CaptchaType type, int? width = null, int? height = null)
		{
			APICall<GetCaptchaOperation> captcha = clubPenguinClient.CaptchaApi.GetCaptcha(type, width, height);
			captcha.OnResponse += onGetCaptcha;
			captcha.OnError += handleCPResponseError;
			captcha.Execute();
		}

		public void PostCaptchaSolution(CaptchaType type, CaptchaSolution solution)
		{
			APICall<PostCaptchaSolutionOperation> aPICall = clubPenguinClient.CaptchaApi.PostCaptchaSolution(type, solution);
			aPICall.OnResponse += onPostCaptchaSolutionSuccess;
			aPICall.OnError += onPostCaptchaSolutionFailed;
			aPICall.Execute();
		}

		private void onGetCaptcha(GetCaptchaOperation operation, HttpResponse httpResponse)
		{
			CaptchaData responseBody = operation.ResponseBody;
			Service.Get<EventDispatcher>().DispatchEvent(new CaptchaServiceEvents.CaptchaLoaded(responseBody));
		}

		private void onPostCaptchaSolutionSuccess(PostCaptchaSolutionOperation operation, HttpResponse httpResponse)
		{
			string id = operation.CaptchaSolution.id;
			Service.Get<EventDispatcher>().DispatchEvent(new CaptchaServiceEvents.CaptchaSolutionAccepted(id));
		}

		public void onPostCaptchaSolutionFailed(PostCaptchaSolutionOperation operation, HttpResponse httpResponse)
		{
			Debug.LogWarningFormat("onPostCaptchaSolutionFailed. httpResponse.Text = {0}", httpResponse.Text);
			if (!httpResponse.Is2XX)
			{
				if (!string.IsNullOrEmpty(httpResponse.Text))
				{
					CaptchaErrorResponse captchaErrorResponse = new CaptchaErrorResponse();
					try
					{
						captchaErrorResponse = Service.Get<JsonService>().Deserialize<CaptchaErrorResponse>(httpResponse.Text);
						if (captchaErrorResponse.code == 1023)
						{
							string captcha = captchaErrorResponse.captcha;
							CaptchaData newCaptchaData = Service.Get<JsonService>().Deserialize<CaptchaData>(captcha);
							Service.Get<EventDispatcher>().DispatchEvent(new CaptchaServiceEvents.CaptchaSolutionDeclined(newCaptchaData));
						}
						else
						{
							handleCPResponseError(httpResponse);
						}
					}
					catch (Exception)
					{
						handleCPResponseError(httpResponse);
					}
				}
				else
				{
					handleCPResponseError(httpResponse);
				}
			}
		}
	}
}
