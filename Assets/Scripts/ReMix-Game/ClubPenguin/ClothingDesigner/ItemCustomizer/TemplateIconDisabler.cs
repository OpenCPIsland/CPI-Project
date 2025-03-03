using ClubPenguin.UI;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	[RequireComponent(typeof(TemplateIcon))]
	public class TemplateIconDisabler : UIElementDisabler
	{
		private TemplateIcon templateIcon;

		private void Awake()
		{
			templateIcon = GetComponent<TemplateIcon>();
		}

		protected override void start()
		{
			disablerManager = Service.Get<UIElementDisablerManager>();
			disablerManager.RegisterDisabler(this);
		}

		public override void DisableElement(bool hide)
		{
			templateIcon.IsEnabled = false;
			Button component = GetComponent<Button>();
			if (component != null)
			{
				component.interactable = false;
			}
		}

		public override void EnableElement()
		{
			templateIcon.IsEnabled = true;
			Button component = GetComponent<Button>();
			if (component != null)
			{
				component.interactable = true;
			}
		}
	}
}
