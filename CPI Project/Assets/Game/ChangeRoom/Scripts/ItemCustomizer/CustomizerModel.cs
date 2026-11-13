namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class CustomizerModel
	{
		private CustomizerState state;

		private DItemCustomization itemCustomization;

		public DItemCustomization ItemCustomization
		{
			get
			{
				return itemCustomization;
			}
		}

		public CustomizerState State
		{
			get
			{
				return state;
			}
			set
			{
				if (state != value)
				{
					state = value;
					CustomizationContext.EventBus.DispatchEvent(new CustomizerModelEvents.CustomizerStateChangedEvent(state));
				}
			}
		}

		public bool IsChanged
		{
			get
			{
				return itemCustomization.IsChanged;
			}
		}

		public CustomizerModel(DItemCustomization itemCustomization)
		{
			this.itemCustomization = itemCustomization;
			state = CustomizerState.FABRIC;
		}
	}
}
