using ClubPenguin.ClothingDesigner.ItemCustomizer;
using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(ActiveSwatchWidget))]
	public class ActiveSwatchWidgetDisabler : UIElementDisabler
	{
		private ActiveSwatchWidget widget;

		private void Awake()
		{
			widget = GetComponent<ActiveSwatchWidget>();
		}

		public override void DisableElement(bool hide)
		{
			isEnabled = false;
			widget.SetInteractable(false);
		}

		public override void EnableElement()
		{
			isEnabled = true;
			widget.SetInteractable(true);
		}
	}
}
