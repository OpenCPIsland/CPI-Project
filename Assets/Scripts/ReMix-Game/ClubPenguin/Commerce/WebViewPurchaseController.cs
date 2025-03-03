using ClubPenguin.Analytics;
using ClubPenguin.ContentGates;
using ClubPenguin.Core;
using ClubPenguin.NativeWebViewer;
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using LitJson;
using System;
using System.Text;
using UnityEngine;

namespace ClubPenguin.Commerce
{
	public class WebViewPurchaseController
	{
		private const string purchaseTitle = "Membership.Purchase.CSG.WebviewerTitle";

		private const string purchaseURLParms = "?config={0}&planId={1}&pageType=checkout";

		private const string manageAccountTitle = "Membership.ManageAccount.CSG.WebviewerTitle";

		private const string manageAccountURLParms = "?config={0}&planId={1}&pageType=manageAccount";

		private const string javaScriptCommFunction = "CPI.Membership.WebviewEventComm";

		private const string javaScriptEnableCommFunction = "CPI.Membership.EnableWebviewEventComm";

		private GameObject webViewGameObject;

		private WebViewController webViewController;

		private readonly PrefabContentKey webViewPopupKey = new PrefabContentKey("MembershipPrefabs/MembershipBillingInfoScreen_PC");

		private bool closeUserInitiated = true;

		private CSGConfig csgConfig;

		private string planId;

		public void ShowPurchaseFlow(CSGConfig csgConfig, string planId)
		{
			this.csgConfig = csgConfig;
			this.planId = planId;
			string url = this.csgConfig.BaseUrl + "?config={0}&planId={1}&pageType=checkout";
			ShowFlow(url, "Membership.Purchase.CSG.WebviewerTitle");
		}

		public void ReloadPurchaseFlow(CSGConfig csgConfig, string planId)
		{
			this.csgConfig = csgConfig;
			string text = this.csgConfig.BaseUrl + "?config={0}&planId={1}&pageType=checkout";
			if (webViewController != null)
			{
				IContentInterruption gate = new ParentGate();
				webViewController.Show(text, gate, "Membership.Purchase.CSG.WebviewerTitle", true, false, 1.8f, 3f);
			}
			else
			{
				ShowFlow(text, "Membership.Purchase.CSG.WebviewerTitle");
			}
		}

		public void ReloadManageAccountFlow(CSGConfig csgConfig)
		{
			this.csgConfig = csgConfig;
			planId = "";
			string text = this.csgConfig.BaseUrl + "?config={0}&planId={1}&pageType=manageAccount";
			if (webViewController != null)
			{
				IContentInterruption gate = new ParentGate();
				webViewController.Show(text, gate, "Membership.ManageAccount.CSG.WebviewerTitle", true, false, 1.8f, 3f);
			}
			else
			{
				ShowFlow(text, "Membership.ManageAccount.CSG.WebviewerTitle");
			}
		}

		public void ShowManageAccountFlow(CSGConfig csgConfig)
		{
			this.csgConfig = csgConfig;
			planId = "";
			string url = this.csgConfig.BaseUrl + "?config={0}&planId={1}&pageType=manageAccount";
			ShowFlow(url, "Membership.ManageAccount.CSG.WebviewerTitle");
		}

		public void ShowFlow(string url, string title)
		{
			closeUserInitiated = true;
			webViewGameObject = new GameObject("WebView");
			webViewController = new WebViewController(webViewGameObject);
			webViewController.OnLoaded += delegate
			{
				if (!string.IsNullOrEmpty("CPI.Membership.WebviewEventComm"))
				{
					webViewController.RegisterJSFunction("CPI.Membership.WebviewEventComm");
					webViewController.CallFunction("CPI.Membership.EnableWebviewEventComm");
				}
			};
			webViewController.OnFailed += onWebViewFailed;
			webViewController.OnClosed += onWebViewClosed;
			webViewController.OnReceivedMessage += onReceivedMessage;
			WebViewController obj = webViewController;
			obj.FormatURL = (WebViewController.FormatURLDelegate)Delegate.Combine(obj.FormatURL, new WebViewController.FormatURLDelegate(setURLParameters));
			IContentInterruption gate = new ParentGate();
			webViewController.Show(url, gate, title, webViewPopupKey, true, false, 1.8f, 3f);
		}

		private string setURLParameters(string urlFormat)
		{
			return string.Format(urlFormat, base64Encode(JsonMapper.ToJson(csgConfig)), planId);
		}

		private string base64Encode(string plainText)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(plainText);
			return Convert.ToBase64String(bytes);
		}

		private string setURLManageAccountParameters(string urlFormat)
		{
			return string.Format(urlFormat, base64Encode(JsonMapper.ToJson(csgConfig)), planId, "manageAccount");
		}

		private void onWebViewFailed()
		{
			JsonData purchaseError = JsonMapper.ToObject("{ 'status' : '0', 'msg' : 'webview-failed' }");
			Service.Get<EventDispatcher>().DispatchEvent(new IAPServiceErrors.PCPurchaseError(purchaseError));
			UnityEngine.Object.Destroy(webViewGameObject);
		}

		private void onWebViewClosed()
		{
			if (closeUserInitiated)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(IAPServiceEvents.PCPurchaseCancelled));
			}
			UnityEngine.Object.Destroy(webViewGameObject);
		}

		private void onManagaAccountWebViewFailed()
		{
			UnityEngine.Object.Destroy(webViewGameObject);
		}

		private void onManagaAccountWebViewClosed()
		{
			UnityEngine.Object.Destroy(webViewGameObject);
		}

		private void onReceivedMessage(JsonData message)
		{
			string text = "";
			if (message.Count >= 2 && message[0].ToString().StartsWith("ecommerce"))
			{
				text = message[0].ToString();
				JsonData jsonData = message[1];
				switch (text)
				{
				case "ecommerce-success":
					Service.Get<EventDispatcher>().DispatchEvent(new IAPServiceEvents.PCPurchaseSuccess(jsonData));
					closeUserInitiated = false;
					webViewController.Close();
					break;
				case "ecommerce-fatal":
					Service.Get<EventDispatcher>().DispatchEvent(new IAPServiceErrors.PCPurchaseError(jsonData));
					closeUserInitiated = false;
					webViewController.Close();
					break;
				case "ecommerce-session-expired":
					Service.Get<EventDispatcher>().DispatchEvent(default(IAPServiceErrors.SessionExpired));
					closeUserInitiated = false;
					webViewController.Close();
					break;
				case "ecommerce-bi":
					sendBI(jsonData);
					break;
				}
			}
		}

		private void sendBI(JsonData biContent)
		{
			if (biContent.Contains("bi-method"))
			{
				switch ((string)biContent["bi-method"])
				{
				case "Action":
					Service.Get<ICPSwrveService>().Action(getJsonString(biContent, "tier1"), getJsonString(biContent, "tier2"), getJsonString(biContent, "tier3"), getJsonString(biContent, "tier4"), getJsonString(biContent, "context"), getJsonString(biContent, "message"), getJsonString(biContent, "level"));
					break;
				case "Funnel":
					Service.Get<ICPSwrveService>().Funnel(getJsonString(biContent, "type"), getJsonString(biContent, "step-number"), getJsonString(biContent, "step-name"), getJsonString(biContent, "message"));
					break;
				case "Error":
					Service.Get<ICPSwrveService>().Error("csg_error", getJsonString(biContent, "type"), getJsonString(biContent, "context"), getJsonString(biContent, "message"));
					break;
				}
			}
		}

		private string getJsonString(JsonData json, string key)
		{
			return json.Contains(key) ? json[key].ToString() : null;
		}
	}
}
