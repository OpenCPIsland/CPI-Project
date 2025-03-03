using ClubPenguin.Commerce;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(Image), typeof(Button))]
	public class SubscriptionOptionButton : MonoBehaviour
	{
		[Header("Status")]
		public bool IsSelected;

		[Header("Button Look/Feel")]
		public GameObject SelectedOutline;

		public Color SelectedTextColor;

		public Color DefaultTextColor;

		[Header("Elements")]
		public GameObject SelectedCheck;

		[Header("Text")]
		public Text PlanTitle;

		public Text TrialText;

		public Text PriceText;

		public Text DurationText;

		public Text RecurringText;

		public Text CurrencyCode;

		private SubscriptionOptionButton[] siblings;

		private Product product;

		private void Start()
		{
			base.gameObject.GetComponent<Button>().onClick.AddListener(onOptionClick);
		}

		private void setupSiblings()
		{
			siblings = base.gameObject.transform.parent.gameObject.GetComponentsInChildren<SubscriptionOptionButton>();
		}

		public void SetProduct(Product product, bool isSelected)
		{
			this.product = product;
			string token = "Membership.MembershipTerms.TermsTitle." + product.sku_duration;
			PlanTitle.text = Service.Get<Localizer>().GetTokenTranslation(token);
			TrialText.text = (product.IsTrial() ? ("+ " + Service.Get<Localizer>().GetTokenTranslation("Membership.MembershipTerms.TrialText")) : "");
			PriceText.text = product.price;
			DurationText.text = "/" + Service.Get<Localizer>().GetTokenTranslation(token);
			CurrencyCode.text = product.currencyCode;
			if (product.IsRecurring())
			{
				string token2 = "Membership.MembershipTerms.RecurringText." + product.sku_duration;
				RecurringText.text = Service.Get<Localizer>().GetTokenTranslation(token2);
			}
			if (isSelected)
			{
				SelectButton();
			}
			else
			{
				UnselectButton();
			}
		}

		public Product GetProduct()
		{
			return product;
		}

		public void onOptionClick()
		{
			if (siblings == null)
			{
				setupSiblings();
			}
			if (!IsSelected)
			{
				SubscriptionOptionButton[] array = siblings;
				foreach (SubscriptionOptionButton subscriptionOptionButton in array)
				{
					subscriptionOptionButton.UnselectButton();
				}
				SelectButton();
			}
		}

		public void UnselectButton()
		{
			IsSelected = false;
			SelectedOutline.SetActive(false);
			SelectedCheck.SetActive(false);
			PlanTitle.color = DefaultTextColor;
			TrialText.color = DefaultTextColor;
		}

		public void SelectButton()
		{
			IsSelected = true;
			SelectedOutline.SetActive(true);
			SelectedCheck.SetActive(true);
			PlanTitle.color = SelectedTextColor;
			TrialText.color = SelectedTextColor;
		}
	}
}
