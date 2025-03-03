using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class DecalScreenController : MonoBehaviour
	{
		private CustomizerModel customizerModel;

		private EventChannel eventChannel;

		public void Init(CustomizerModel customizerModel)
		{
			this.customizerModel = customizerModel;
		}

		private void OnEnable()
		{
			if (eventChannel == null)
			{
				eventChannel = new EventChannel(CustomizationContext.EventBus);
			}
			eventChannel.AddListener<CustomizerUIEvents.UpdateChannelDecal>(onChangeDecal);
		}

		private void OnDisable()
		{
			eventChannel.RemoveAllListeners();
		}

		private bool onChangeDecal(CustomizerUIEvents.UpdateChannelDecal evt)
		{
			switch (evt.ChannelToUpdate)
			{
			case CustomizationChannel.RED:
				customizerModel.ItemCustomization.SetChannelDecal(customizerModel.ItemCustomization.RedChannel, evt.NewDecal, evt.UVOffset, evt.ChosenRenderer);
				break;
			case CustomizationChannel.GREEN:
				customizerModel.ItemCustomization.SetChannelDecal(customizerModel.ItemCustomization.GreenChannel, evt.NewDecal, evt.UVOffset, evt.ChosenRenderer);
				break;
			case CustomizationChannel.BLUE:
				customizerModel.ItemCustomization.SetChannelDecal(customizerModel.ItemCustomization.BlueChannel, evt.NewDecal, evt.UVOffset, evt.ChosenRenderer);
				break;
			}
			return false;
		}
	}
}
