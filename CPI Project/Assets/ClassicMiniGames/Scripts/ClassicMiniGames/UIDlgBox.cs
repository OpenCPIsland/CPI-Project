using DisneyMobile.CoreUnitySystems;
using System.Collections.Generic;
using UnityEngine;

public class UIDlgBox : UIControlBase
{
	public const string YES = "yes";

	public const string NO = "no";

	public const string OK = "ok";

	public const string TITLE = "title";

	public const string DESC = "desc";

	public const string TEXTKEY = "tkey";

	protected UIElementText m_YesBtn;

	protected UIElementText m_NoBtn;

	protected UIElementText m_OkBtn;

	protected UIElementText m_Title;

	protected UIElementText m_Desc;

	protected GameObject m_MessageObject = null;

	protected string m_MessageTag = "";

	public void SetMessageObject(GameObject obj, string tag)
	{
		m_MessageObject = obj;
		m_MessageTag = tag;
	}

	private void Start()
	{
		if (!IsLoaded)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["yes"] = "Accept";
			dictionary["no"] = "Reject";
			dictionary["title"] = "Network Error";
			dictionary["desc"] = "Please connect to wifi";
			dictionary["tkey"] = "t";
			LoadUI(dictionary);
		}
	}

	private string GetStringFromDict(Dictionary<string, string> plist, string key)
	{
		string result = "";
		if (plist.ContainsKey(key))
		{
			result = plist[key];
		}
		return result;
	}

	public void InitDlgBox(string title, string desc, string yesText, string noText, string okText, bool textiskey)
	{
		InitElement(m_YesBtn, yesText, textiskey);
		InitElement(m_NoBtn, noText, textiskey);
		InitElement(m_OkBtn, okText, textiskey);
		InitElement(m_Title, title, textiskey);
		InitElement(m_Desc, desc, textiskey);
	}

	public override void LoadUI(Dictionary<string, string> plist = null)
	{
		base.LoadUI(plist);
		m_YesBtn = (FindElement("PfDlgBtnYes") as UIElementText);
		m_NoBtn = (FindElement("PfDlgBtnNo") as UIElementText);
		m_OkBtn = (FindElement("PfDlgBtnOk") as UIElementText);
		m_Title = (FindElement("PfDlgTitle") as UIElementText);
		m_Desc = (FindElement("PfDlgDesc") as UIElementText);
		if (plist != null)
		{
			bool textiskey = plist.ContainsKey("tkey");
			InitDlgBox(GetStringFromDict(plist, "title"), GetStringFromDict(plist, "desc"), GetStringFromDict(plist, "yes"), GetStringFromDict(plist, "no"), GetStringFromDict(plist, "ok"), textiskey);
		}
	}

	public void OnOK()
	{
		if (!IsInTransition && m_MessageObject != null)
		{
			m_MessageObject.SendMessage("OnDlgOk", m_MessageTag);
		}
	}

	public void OnYes()
	{
		if (!IsInTransition && m_MessageObject != null)
		{
			m_MessageObject.SendMessage("OnDlgYes", m_MessageTag);
		}
	}

	public void OnNo()
	{
		if (!IsInTransition && m_MessageObject != null)
		{
			m_MessageObject.SendMessage("OnDlgNo", m_MessageTag);
		}
	}

	public void OnEscape()
	{
		if (!IsInTransition && m_MessageObject != null)
		{
			m_MessageObject.SendMessage("OnDlgEscape", m_MessageTag);
		}
	}

	public override bool HandleEscape()
	{
		OnEscape();
		return true;
	}

	private void InitElement(UIElementText element, string val, bool textiskey)
	{
		element.Show(!string.IsNullOrEmpty(val));
		if (!string.IsNullOrEmpty(val))
		{
			element.SetText(val, textiskey);
		}
	}
}
