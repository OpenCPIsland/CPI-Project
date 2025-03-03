using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardIconRenderer_EquipmentInstance : IRewardIconRenderer
	{
		private DReward reward;

		private RewardIconRenderComplete callback;

		private ItemImageBuilder itemImageBuilder;

		public void RenderReward(DReward reward, RewardIconRenderComplete callback)
		{
			this.reward = reward;
			this.callback = callback;
			renderClothingInstance();
		}

		private void renderClothingInstance()
		{
			itemImageBuilder = ItemImageBuilder.acquire();
			itemImageBuilder.RequestImage(CustomEquipmentResponseAdaptor.ConvertResponseToCustomEquipment(reward.EquipmentRequest), onEquipmentInstanceLoaded);
		}

		private void onEquipmentInstanceLoaded(bool success, Texture2D texture, AbstractImageBuilder.CallbackToken callbackToken)
		{
			if (success && reward.EquipmentRequest.equipmentId == callbackToken.Id && reward.EquipmentRequest.definitionId == callbackToken.DefinitionId)
			{
				callback(Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero), null);
			}
			if (itemImageBuilder != null)
			{
				ItemImageBuilder.release();
				itemImageBuilder = null;
			}
		}
	}
}
