using ClubPenguin.Commerce;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

public class IAPRunnerService : MonoBehaviour
{
	private CommerceService cs;

	private void Start()
	{
		cs = Service.Get<CommerceService>();
		if (cs != null)
		{
			Service.Get<EventDispatcher>().AddListener<CommerceServiceEvents.BillingEnabled>(BillingWasEnabled);
			Service.Get<EventDispatcher>().AddListener<CommerceServiceErrors.BillingEnabledError>(BillingWasEnabledError);
			Service.Get<EventDispatcher>().AddListener<CommerceServiceEvents.PurchaseVerified>(PurchaseWasVerified);
			Service.Get<EventDispatcher>().AddListener<CommerceServiceErrors.PurchaseVerifiedError>(PurchaseWasVerifiedError);
			Service.Get<EventDispatcher>().AddListener<CommerceServiceEvents.UnexpectedPurchase>(UnexpectedPurchaseEvent);
			Service.Get<EventDispatcher>().AddListener<CommerceServiceEvents.ProductsLoaded>(ProducstWereLoadedVerified);
			Service.Get<EventDispatcher>().AddListener<CommerceServiceErrors.ProductsLoadedError>(ProducstWereLoadedError);
			Service.Get<EventDispatcher>().AddListener<CommerceServiceEvents.RestoreVerified>(RestoreWasVerified);
			Service.Get<EventDispatcher>().AddListener<CommerceServiceErrors.RestoreVerifiedError>(RestoreWasVerifiedError);
		}
	}

	private void Update()
	{
	}

	public bool BillingWasEnabled(CommerceServiceEvents.BillingEnabled be)
	{
		cs.LoadProductInformation();
		return false;
	}

	public bool BillingWasEnabledError(CommerceServiceErrors.BillingEnabledError bee)
	{
		return false;
	}

	public bool ProducstWereLoadedVerified(CommerceServiceEvents.ProductsLoaded pl)
	{
		Product productByKey = cs.GetProductByKey("First_Time_User_Offer");
		if (productByKey != null && productByKey.IsPurchasable())
		{
			string sku_duration = productByKey.sku_duration;
			string price = productByKey.price;
			string currencyCode = productByKey.currencyCode;
			string title = productByKey.title;
			string description = productByKey.description;
			string text = "";
			if (productByKey.IsTrial())
			{
				text = productByKey.sku_trial_duration;
			}
		}
		return false;
	}

	public bool ProducstWereLoadedError(CommerceServiceErrors.ProductsLoadedError ple)
	{
		return false;
	}

	public bool PurchaseWasVerified(CommerceServiceEvents.PurchaseVerified pv)
	{
		return false;
	}

	public bool PurchaseWasVerifiedError(CommerceServiceErrors.PurchaseVerifiedError pve)
	{
		return false;
	}

	public bool RestoreWasVerified(CommerceServiceEvents.RestoreVerified rv)
	{
		if (rv.Success)
		{
		}
		return false;
	}

	public bool RestoreWasVerifiedError(CommerceServiceErrors.RestoreVerifiedError rve)
	{
		return false;
	}

	public bool UnexpectedPurchaseEvent(CommerceServiceEvents.UnexpectedPurchase up)
	{
		cs.VerifyUnexpectedPurchase(up.PI);
		return false;
	}

	private void OnDestroy()
	{
		Service.Get<EventDispatcher>().RemoveListener<CommerceServiceEvents.BillingEnabled>(BillingWasEnabled);
		Service.Get<EventDispatcher>().RemoveListener<CommerceServiceErrors.BillingEnabledError>(BillingWasEnabledError);
		Service.Get<EventDispatcher>().RemoveListener<CommerceServiceEvents.PurchaseVerified>(PurchaseWasVerified);
		Service.Get<EventDispatcher>().RemoveListener<CommerceServiceErrors.PurchaseVerifiedError>(PurchaseWasVerifiedError);
		Service.Get<EventDispatcher>().RemoveListener<CommerceServiceEvents.UnexpectedPurchase>(UnexpectedPurchaseEvent);
		Service.Get<EventDispatcher>().RemoveListener<CommerceServiceEvents.ProductsLoaded>(ProducstWereLoadedVerified);
		Service.Get<EventDispatcher>().RemoveListener<CommerceServiceErrors.ProductsLoadedError>(ProducstWereLoadedError);
		Service.Get<EventDispatcher>().RemoveListener<CommerceServiceEvents.RestoreVerified>(RestoreWasVerified);
		Service.Get<EventDispatcher>().RemoveListener<CommerceServiceErrors.RestoreVerifiedError>(RestoreWasVerifiedError);
	}
}
