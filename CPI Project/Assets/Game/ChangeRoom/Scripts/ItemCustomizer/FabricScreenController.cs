using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class FabricScreenController : MonoBehaviour
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
			eventChannel.AddListener<CustomizerUIEvents.UpdateChannelFabric>(onChangeFabric);
		}

		private void OnDisable()
		{
			eventChannel.RemoveAllListeners();
		}

		private bool onChangeFabric(CustomizerUIEvents.UpdateChannelFabric evt)
		{
			switch (evt.ChannelToUpdate)
			{
			case CustomizationChannel.RED:
				customizerModel.ItemCustomization.SetChannelFabric(customizerModel.ItemCustomization.RedChannel, evt.NewFabric, evt.UVOffset);
				break;
			case CustomizationChannel.GREEN:
				customizerModel.ItemCustomization.SetChannelFabric(customizerModel.ItemCustomization.GreenChannel, evt.NewFabric, evt.UVOffset);
				break;
			case CustomizationChannel.BLUE:
				customizerModel.ItemCustomization.SetChannelFabric(customizerModel.ItemCustomization.BlueChannel, evt.NewFabric, evt.UVOffset);
				break;
			}
			return false;
		}
	}
}
