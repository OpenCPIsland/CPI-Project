#if UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE
using ClubPenguin.NativeWebViewer;
using LitJson;
using System;
using UnityEngine;

namespace ClubPenguin.NativeWebViewer
{
public abstract class AbstractWebViewComponentController
{
	public string Url
	{
		get;
		set;
	}

	public abstract bool SupportsAddJavascript
	{
		get;
	}

	public event Action<AbstractWebViewComponentController, bool, string> OnLoadComplete;

	public event Action<AbstractWebViewComponentController> OnWebViewShouldClose;

	public event Action<string> OnEvalJavaScriptFinished;

	public event Action<JsonData> OnReceivedMessage;

	public event Action OnSetupComplete;

	public abstract void SetUp(GameObject webViewPanel, bool isDownsampled, bool allowPopups, bool openPopupInNewBrowser, float zoomLevel, float newBrowserZoomLevel);

	public abstract void Load();

	public abstract void Show();

	public abstract void CleanCache();

	public abstract void Close();

	public abstract void EvaluateJavaScript(string javaScript);

	public abstract void AddJavaScript(string javaScript);

	public abstract void RegisterJSFunction(string jsFunctionName);

	public abstract void CallFunction(string name, params JsonData[] arguments);

	protected void raiseOnSetupComplete()
	{
		if (this.OnSetupComplete != null)
		{
			this.OnSetupComplete();
		}
	}

	protected void raiseOnLoadComplete(bool success, string errorMessage)
	{
		if (this.OnLoadComplete != null)
		{
			this.OnLoadComplete(this, success, errorMessage);
		}
	}

	protected void raiseOnWebViewShouldClose()
	{
		if (this.OnWebViewShouldClose != null)
		{
			this.OnWebViewShouldClose(this);
		}
	}

	protected void raiseOnEvalJavaScriptFinished(string result)
	{
		if (this.OnEvalJavaScriptFinished != null)
		{
			this.OnEvalJavaScriptFinished(result);
		}
	}

	protected void raiseOnReceivedMessage(JsonData message)
	{
		if (this.OnReceivedMessage != null)
		{
			this.OnReceivedMessage(message);
		}
	}
}
}
#else
using LitJson;
using System;
using UnityEngine;

namespace ClubPenguin.NativeWebViewer
{
	public abstract class AbstractWebViewComponentController
	{
		public string Url
		{
			get;
			set;
		}

		public abstract bool SupportsAddJavascript
		{
			get;
		}

		public event Action<AbstractWebViewComponentController, bool, string> OnLoadComplete;

		public event Action<AbstractWebViewComponentController> OnWebViewShouldClose;

		public event Action<string> OnEvalJavaScriptFinished;

		public event Action<JsonData> OnReceivedMessage;

		public event Action OnSetupComplete;

		public abstract void SetUp(GameObject webViewPanel, bool isDownsampled, bool allowPopups, bool openPopupInNewBrowser, float zoomLevel, float newBrowserZoomLevel);

		public abstract void Load();

		public abstract void Show();

		public abstract void CleanCache();

		public abstract void Close();

		public abstract void EvaluateJavaScript(string javaScript);

		public abstract void AddJavaScript(string javaScript);

		public abstract void RegisterJSFunction(string jsFunctionName);

		public abstract void CallFunction(string name, params JsonData[] arguments);

		protected void raiseOnSetupComplete()
		{
			if (this.OnSetupComplete != null)
			{
				this.OnSetupComplete();
			}
		}

		protected void raiseOnLoadComplete(bool success, string errorMessage)
		{
			if (this.OnLoadComplete != null)
			{
				this.OnLoadComplete(this, success, errorMessage);
			}
		}

		protected void raiseOnWebViewShouldClose()
		{
			if (this.OnWebViewShouldClose != null)
			{
				this.OnWebViewShouldClose(this);
			}
		}

		protected void raiseOnEvalJavaScriptFinished(string result)
		{
			if (this.OnEvalJavaScriptFinished != null)
			{
				this.OnEvalJavaScriptFinished(result);
			}
		}

		protected void raiseOnReceivedMessage(JsonData message)
		{
			if (this.OnReceivedMessage != null)
			{
				this.OnReceivedMessage(message);
			}
		}
	}
}
#endif