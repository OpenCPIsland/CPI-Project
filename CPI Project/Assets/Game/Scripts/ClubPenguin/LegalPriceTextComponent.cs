using ClubPenguin.Commerce;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class LegalPriceTextComponent : MonoBehaviour
	{
		public Text LegalPriceText;

		private MembershipController membershipController;

		private void Start()
		{
			membershipController = GetComponentInParent<MembershipController>();
			LegalPriceText.text = membershipController.GetLegalText();
			membershipController.OnProductsReady += updateLegalText;
			membershipController.GetProduct(true);
		}

		public void updateLegalText(Product product, List<Product> productsToOffer)
		{
			membershipController.OnProductsReady -= updateLegalText;
			string token = "Membership.MembershipTerms.TermsTitle." + product.sku_duration;
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(token);
			LegalPriceText.text = membershipController.GetLegalText(product.price, tokenTranslation);
		}

		private void OnDestroy()
		{
			if (membershipController != null)
			{
				membershipController.OnProductsReady -= updateLegalText;
			}
		}
	}
}
