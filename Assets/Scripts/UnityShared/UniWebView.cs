#if UNITY_ANDROID //|| UNITY_IOS || UNITY_IPHONE
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class UniWebView : MonoBehaviour
{
    public delegate void LoadCompleteDelegate(UniWebView webView, bool success, string errorMessage);

    public delegate void LoadBeginDelegate(UniWebView webView, string loadingUrl);

    public delegate void ReceivedMessageDelegate(UniWebView webView, UniWebViewMessage message);

    public delegate void EvalJavaScriptFinishedDelegate(UniWebView webView, string result);

    public delegate bool WebViewShouldCloseDelegate(UniWebView webView);

    public delegate void ReceivedKeyCodeDelegate(UniWebView webView, int keyCode);

    public delegate UniWebViewEdgeInsets InsetsForScreenOreitationDelegate(UniWebView webView, UniWebViewOrientation orientation);

    [SerializeField]
    private UniWebViewEdgeInsets _insets = new UniWebViewEdgeInsets(0, 0, 0, 0);

    public string url;

    public bool loadOnStart;

    public bool autoShowWhenLoadComplete;

    private bool _backButtonEnable = true;

    private bool _bouncesEnable;

    private bool _zoomEnable;

    private string _currentGUID;

    private int _lastScreenHeight;

    private bool _immersiveMode = true;

    private Action _showTransitionAction = null;

    private Action _hideTransitionAction = null;

    public bool toolBarShow = false;

    public static bool UseWebKitIfAvailable = true;

    public UniWebViewEdgeInsets insets
    {
        get
        {
            return _insets;
        }
        set
        {
            if (_insets != value)
            {
                ForceUpdateInsetsInternal(value);
            }
        }
    }

    public string currentUrl
    {
        get
        {
            return UniWebViewPlugin.GetCurrentUrl(base.gameObject.name);
        }
    }

    public bool backButtonEnable
    {
        get
        {
            return _backButtonEnable;
        }
        set
        {
            if (_backButtonEnable != value)
            {
                _backButtonEnable = value;
                UniWebViewPlugin.SetBackButtonEnable(base.gameObject.name, _backButtonEnable);
            }
        }
    }

    public bool bouncesEnable
    {
        get
        {
            return _bouncesEnable;
        }
        set
        {
            if (_bouncesEnable != value)
            {
                _bouncesEnable = value;
                UniWebViewPlugin.SetBounces(base.gameObject.name, _bouncesEnable);
            }
        }
    }

    public bool zoomEnable
    {
        get
        {
            return _zoomEnable;
        }
        set
        {
            if (_zoomEnable != value)
            {
                _zoomEnable = value;
                UniWebViewPlugin.SetZoomEnable(base.gameObject.name, _zoomEnable);
            }
        }
    }

    public string userAgent
    {
        get
        {
            return UniWebViewPlugin.GetUserAgent(base.gameObject.name);
        }
    }

    public float alpha
    {
        get
        {
            return UniWebViewPlugin.GetAlpha(base.gameObject.name);
        }
        set
        {
            UniWebViewPlugin.SetAlpha(base.gameObject.name, Mathf.Clamp01(value));
        }
    }

    public bool openLinksInExternalBrowser
    {
        get
        {
            return UniWebViewPlugin.GetOpenLinksInExternalBrowser(base.gameObject.name);
        }
        set
        {
            UniWebViewPlugin.SetOpenLinksInExternalBrowser(base.gameObject.name, value);
        }
    }

    public bool immersiveMode
    {
        get
        {
            return _immersiveMode;
        }
        set
        {
            _immersiveMode = value;
            UniWebViewPlugin.SetImmersiveModeEnabled(base.gameObject.name, _immersiveMode);
        }
    }

    public event LoadCompleteDelegate OnLoadComplete;

    public event LoadBeginDelegate OnLoadBegin;

    public event ReceivedMessageDelegate OnReceivedMessage;

    public event EvalJavaScriptFinishedDelegate OnEvalJavaScriptFinished;

    public event WebViewShouldCloseDelegate OnWebViewShouldClose;

    public event ReceivedKeyCodeDelegate OnReceivedKeyCode;

    public event InsetsForScreenOreitationDelegate InsetsForScreenOreitation;

    private void ForceUpdateInsetsInternal(UniWebViewEdgeInsets insets)
    {
        _insets = insets;
        UniWebViewPlugin.ChangeInsets(base.gameObject.name, this.insets.top, this.insets.left, this.insets.bottom, this.insets.right);
    }

    public static void SetUserAgent(string value)
    {
        UniWebViewPlugin.SetUserAgent(value);
    }

    public static void ResetUserAgent()
    {
        SetUserAgent(null);
    }

    public static void SetDoneButtonText(string text)
    {
    }

    public void Load()
    {
        string text = (!string.IsNullOrEmpty(url)) ? url.Trim() : "about:blank";
        UniWebViewPlugin.Load(base.gameObject.name, text);
    }

    public void Load(string aUrl)
    {
        url = aUrl;
        Load();
    }

    public void LoadHTMLString(string htmlString, string baseUrl)
    {
        UniWebViewPlugin.LoadHTMLString(base.gameObject.name, htmlString, baseUrl);
    }

    public void Reload()
    {
        UniWebViewPlugin.Reload(base.gameObject.name);
    }

    public void Stop()
    {
        UniWebViewPlugin.Stop(base.gameObject.name);
    }

    public void Show(bool fade = false, UniWebViewTransitionEdge direction = UniWebViewTransitionEdge.None, float duration = 0.4f, Action finishAction = null)
    {
        _lastScreenHeight = UniWebViewHelper.screenHeight;
        ResizeInternal();
        UniWebViewPlugin.Show(base.gameObject.name, fade, (int)direction, duration);
        _showTransitionAction = finishAction;
        if (toolBarShow)
        {
            ShowToolBar(true);
        }
    }

    public void Hide(bool fade = false, UniWebViewTransitionEdge direction = UniWebViewTransitionEdge.None, float duration = 0.4f, Action finishAction = null)
    {
        UniWebViewPlugin.Hide(base.gameObject.name, fade, (int)direction, duration);
        _hideTransitionAction = finishAction;
    }

    public void EvaluatingJavaScript(string javaScript)
    {
        UniWebViewPlugin.EvaluatingJavaScript(base.gameObject.name, javaScript);
    }

    public void AddJavaScript(string javaScript)
    {
        UniWebViewPlugin.AddJavaScript(base.gameObject.name, javaScript);
    }

    public void CleanCache()
    {
        UniWebViewPlugin.CleanCache(base.gameObject.name);
    }

    [Obsolete("CleanCookie is deprecated, please use the staic SetCookie method instead.")]
    public void CleanCookie(string key = null)
    {
        UniWebViewPlugin.CleanCookie(base.gameObject.name, key);
    }

    public static void SetCookie(string url, string cookie)
    {
        UniWebViewPlugin.SetCookie(url, cookie);
    }

    public static string GetCookie(string url, string key)
    {
        return UniWebViewPlugin.GetCookie(url, key);
    }

    [Obsolete("SetTransparentBackground is deprecated, please use SetBackgroundColor instead.")]
    public void SetTransparentBackground(bool transparent = true)
    {
        UniWebViewPlugin.TransparentBackground(base.gameObject.name, transparent);
    }

    public void SetBackgroundColor(Color color)
    {
        UniWebViewPlugin.SetBackgroundColor(base.gameObject.name, color.r, color.g, color.b, color.a);
    }

    public void ShowToolBar(bool animate)
    {
    }

    public void HideToolBar(bool animate)
    {
    }

    public void SetShowSpinnerWhenLoading(bool show)
    {
        UniWebViewPlugin.SetSpinnerShowWhenLoading(base.gameObject.name, show);
    }

    public void SetSpinnerLabelText(string text)
    {
        UniWebViewPlugin.SetSpinnerText(base.gameObject.name, text);
    }

    public void SetUseWideViewPort(bool use)
    {
        UniWebViewPlugin.SetUseWideViewPort(base.gameObject.name, use);
    }

    public void LoadWithOverviewMode(bool overview)
    {
        UniWebViewPlugin.LoadWithOverviewMode(base.gameObject.name, overview);
    }

    public bool CanGoBack()
    {
        return UniWebViewPlugin.CanGoBack(base.gameObject.name);
    }

    public bool CanGoForward()
    {
        return UniWebViewPlugin.CanGoForward(base.gameObject.name);
    }

    public void GoBack()
    {
        UniWebViewPlugin.GoBack(base.gameObject.name);
    }

    public void GoForward()
    {
        UniWebViewPlugin.GoForward(base.gameObject.name);
    }

    public void AddPermissionRequestTrustSite(string url)
    {
        UniWebViewPlugin.AddPermissionRequestTrustSite(base.gameObject.name, url);
    }

    public void AddCertTrustedHost(string host)
    {
        UniWebViewPlugin.AddCertTrustedHost(base.gameObject.name, host);
    }

    public void AddUrlScheme(string scheme)
    {
        UniWebViewPlugin.AddUrlScheme(base.gameObject.name, scheme);
    }

    public void RemoveUrlScheme(string scheme)
    {
        UniWebViewPlugin.RemoveUrlScheme(base.gameObject.name, scheme);
    }

    public void SetHeaderField(string key, string value)
    {
        UniWebViewPlugin.SetHeaderField(base.gameObject.name, key, value);
    }

    public void SetVerticalScrollBarShow(bool show)
    {
        UniWebViewPlugin.SetVerticalScrollBarShow(base.gameObject.name, show);
    }

    public void SetHorizontalScrollBarShow(bool show)
    {
        UniWebViewPlugin.SetHorizontalScrollBarShow(base.gameObject.name, show);
    }

    public void SetAllowAutoPlay(bool allowed)
    {
        UniWebViewPlugin.SetAllowAutoPlay(base.gameObject.name, allowed);
    }

    public void SetAllowInlinePlay(bool allowed)
    {
    }

    public static void SetWebContentsDebuggingEnabled(bool enabled)
    {
        UniWebViewPlugin.SetWebContentsDebuggingEnabled(enabled);
    }

    public static void SetAllowThirdPartyCookies(bool allowed)
    {
        UniWebViewPlugin.SetAllowThirdPartyCookies(allowed);
    }

    private bool OrientationChanged()
    {
        int screenHeight = UniWebViewHelper.screenHeight;
        if (_lastScreenHeight != screenHeight)
        {
            _lastScreenHeight = screenHeight;
            return true;
        }
        return false;
    }

    private void ResizeInternal()
    {
        int screenHeight = UniWebViewHelper.screenHeight;
        int screenWidth = UniWebViewHelper.screenWidth;
        UniWebViewEdgeInsets insets = this.insets;
        if (this.InsetsForScreenOreitation != null)
        {
            UniWebViewOrientation orientation = (screenHeight < screenWidth) ? UniWebViewOrientation.LandScape : UniWebViewOrientation.Portrait;
            insets = this.InsetsForScreenOreitation(this, orientation);
        }
        ForceUpdateInsetsInternal(insets);
    }

    [Conditional("UNITY_IOS")]
    public void UseWebKit(bool useWebKit)
    {
    }

    private void LoadComplete(string message)
    {
        bool flag = string.Equals(message, "");
        bool flag2 = this.OnLoadComplete != null;
        if (flag)
        {
            if (flag2)
            {
                this.OnLoadComplete(this, true, null);
            }
            if (autoShowWhenLoadComplete)
            {
                Show();
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("Web page load failed: " + base.gameObject.name + "; url: " + url + "; error:" + message);
            if (flag2)
            {
                this.OnLoadComplete(this, false, message);
            }
        }
    }

    private void LoadBegin(string url)
    {
        if (this.OnLoadBegin != null)
        {
            this.OnLoadBegin(this, url);
        }
    }

    private void ReceivedMessage(string rawMessage)
    {
        UniWebViewMessage message = new UniWebViewMessage(rawMessage);
        if (this.OnReceivedMessage != null)
        {
            this.OnReceivedMessage(this, message);
        }
    }

    private void WebViewDone(string message)
    {
        bool flag = true;
        if (this.OnWebViewShouldClose != null)
        {
            flag = this.OnWebViewShouldClose(this);
        }
        if (flag)
        {
            Hide();
            UnityEngine.Object.Destroy(this);
        }
    }

    private void WebViewKeyDown(string message)
    {
        int keyCode = Convert.ToInt32(message);
        if (this.OnReceivedKeyCode != null)
        {
            this.OnReceivedKeyCode(this, keyCode);
        }
    }

    private void EvalJavaScriptFinished(string result)
    {
        if (this.OnEvalJavaScriptFinished != null)
        {
            this.OnEvalJavaScriptFinished(this, result);
        }
    }

    private void AnimationFinished(string identifier)
    {
    }

    private void ShowTransitionFinished(string message)
    {
        if (_showTransitionAction != null)
        {
            _showTransitionAction();
            _showTransitionAction = null;
        }
    }

    private void HideTransitionFinished(string message)
    {
        if (_hideTransitionAction != null)
        {
            _hideTransitionAction();
            _hideTransitionAction = null;
        }
    }

    private IEnumerator LoadFromJarPackage(string jarFilePath)
    {
        WWW stream = new WWW(jarFilePath);
        yield return stream;
        if (stream.error != null)
        {
            if (this.OnLoadComplete != null)
            {
                this.OnLoadComplete(this, false, stream.error);
            }
        }
        else
        {
            LoadHTMLString(stream.text, "");
        }
    }

    private void Awake()
    {
        _currentGUID = Guid.NewGuid().ToString();
        base.gameObject.name = base.gameObject.name + _currentGUID;
        UniWebViewPlugin.Init(base.gameObject.name, insets.top, insets.left, insets.bottom, insets.right, UseWebKitIfAvailable);
        _lastScreenHeight = UniWebViewHelper.screenHeight;
    }

    private void Start()
    {
        if (loadOnStart)
        {
            Load();
        }
    }

    private void OnDestroy()
    {
        RemoveAllListeners();
        UniWebViewPlugin.Destroy(base.gameObject.name);
        base.gameObject.name = base.gameObject.name.Replace(_currentGUID, "");
    }

    private void RemoveAllListeners()
    {
        this.OnLoadBegin = null;
        this.OnLoadComplete = null;
        this.OnReceivedMessage = null;
        this.OnReceivedKeyCode = null;
        this.OnEvalJavaScriptFinished = null;
        this.OnWebViewShouldClose = null;
        this.InsetsForScreenOreitation = null;
    }

    private void Update()
    {
        if (OrientationChanged())
        {
            ResizeInternal();
        }
    }
}
#endif