using SwrveUnity;
using SwrveUnity.Messaging;
using SwrveUnity.ResourceManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwrveEmpty : SwrveSDK
{
	public override void Init(MonoBehaviour container, int appId, string apiKey, string userId)
	{
		SwrveConfig swrveConfig = new SwrveConfig();
		swrveConfig.UserId = userId;
		Init(container, 0, "", swrveConfig);
	}

	public override void Init(MonoBehaviour container, int appId, string apiKey, string userId, SwrveConfig config)
	{
		config.UserId = userId;
		Init(container, 0, "", config);
	}

	public override void Init(MonoBehaviour container, int appId, string apiKey, SwrveConfig config)
	{
		Container = container;
		ResourceManager = new SwrveResourceManager();
		prefabName = container.name;
		base.appId = appId;
		base.apiKey = apiKey;
		base.config = config;
		userId = config.UserId;
		Language = config.Language;
		Initialised = true;
	}

	public override bool SendQueuedEvents()
	{
		return true;
	}

	public override void GetUserResources(Action<Dictionary<string, Dictionary<string, string>>, string> onResult, Action<Exception> onError)
	{
	}

	public override void GetUserResourcesDiff(Action<Dictionary<string, Dictionary<string, string>>, Dictionary<string, Dictionary<string, string>>, string> onResult, Action<Exception> onError)
	{
	}

	public override void FlushToDisk(bool saveEventsBeingSent = false)
	{
	}

	public override bool IsMessageDisplaying()
	{
		return false;
	}

	public override SwrveMessage GetMessageForEvent(string eventName, IDictionary<string, string> payload)
	{
		return null;
	}

	public override SwrveConversation GetConversationForEvent(string eventName, IDictionary<string, string> payload = null)
	{
		return null;
	}

	public override IEnumerator ShowMessageForEvent(string eventName, SwrveMessage message, ISwrveInstallButtonListener installButtonListener = null, ISwrveCustomButtonListener customButtonListener = null, ISwrveMessageListener messageListener = null)
	{
		yield return null;
	}

	public override IEnumerator ShowConversationForEvent(string eventName, SwrveConversation conversation)
	{
		yield return null;
	}

	public override void DismissMessage()
	{
	}

	public override void RefreshUserResourcesAndCampaigns()
	{
	}

	public override void SessionStart()
	{
	}

	public override void NamedEvent(string name, Dictionary<string, string> payload = null)
	{
	}

	public override void UserUpdate(Dictionary<string, string> attributes)
	{
	}

	public override void UserUpdate(string name, DateTime date)
	{
	}

	public override void Purchase(string item, string currency, int cost, int quantity)
	{
	}

	public override void Iap(int quantity, string productId, double productPrice, string currency)
	{
	}

	public override void Iap(int quantity, string productId, double productPrice, string currency, IapRewards rewards)
	{
	}

	public override void CurrencyGiven(string givenCurrency, double amount)
	{
	}

	public override void LoadFromDisk()
	{
	}

	public override Dictionary<string, string> GetDeviceInfo()
	{
		return new Dictionary<string, string>();
	}

	public override void OnSwrvePause()
	{
	}

	public override void OnSwrveResume()
	{
	}

	public override void OnSwrveDestroy()
	{
	}

	public override List<SwrveBaseCampaign> GetCampaigns()
	{
		return new List<SwrveBaseCampaign>();
	}

	public override void ButtonWasPressedByUser(SwrveButton button)
	{
	}

	public override void MessageWasShownToUser(SwrveMessageFormat messageFormat)
	{
	}

	public override void ShowMessageCenterCampaign(SwrveBaseCampaign campaign)
	{
	}

	public override void ShowMessageCenterCampaign(SwrveBaseCampaign campaign, SwrveOrientation orientation)
	{
	}

	public override List<SwrveBaseCampaign> GetMessageCenterCampaigns()
	{
		return new List<SwrveBaseCampaign>();
	}

	public override List<SwrveBaseCampaign> GetMessageCenterCampaigns(SwrveOrientation orientation)
	{
		return new List<SwrveBaseCampaign>();
	}

	public override void RemoveMessageCenterCampaign(SwrveBaseCampaign campaign)
	{
	}

	public override SwrveMessage GetMessageForId(int messageId)
	{
		return null;
	}
}
