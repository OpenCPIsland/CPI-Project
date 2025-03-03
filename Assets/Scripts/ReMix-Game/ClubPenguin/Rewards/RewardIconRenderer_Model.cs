using ClubPenguin.Cinematography;
using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardIconRenderer_Model : IRewardIconRenderer
	{
		private const float ICON_SIZE = 256f;

		private static Vector3 MODEL_RENDER_OFFSET = new Vector3(0f, 0f, 10f);

		private static Vector3 ITEM_MODEL_ROTATION = new Vector3(8f, 160f, 0f);

		private RewardIconRenderComplete callback;

		private Shader unlitTextureShader;

		public void RenderReward(DReward reward, RewardIconRenderComplete callback)
		{
			this.callback = callback;
			CameraCullingMaskHelper.HideLayer(Camera.main, "IconRender");
			CoroutineRunner.Start(renderSprite((string)reward.UnlockID), this, "RewardIconRenderer_Sprite.renderSprite");
		}

		private IEnumerator renderSprite(string unlockID)
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync<GameObject>(unlockID);
			yield return assetRequest;
			GameObject itemModel = Object.Instantiate(assetRequest.Asset);
			itemModel.transform.rotation = Quaternion.Euler(ITEM_MODEL_ROTATION);
			CameraCullingMaskHelper.SetLayerIncludingChildren(itemModel.transform, "IconRender");
			if (unlitTextureShader == null)
			{
				unlitTextureShader = Shader.Find("Unlit/Texture");
			}
			Material mat = new Material(unlitTextureShader)
			{
				mainTexture = itemModel.GetComponent<Renderer>().sharedMaterial.mainTexture
			};
			itemModel.GetComponent<Renderer>().material = mat;
			itemModel.AddComponent<ResourceCleaner>();
			ModelRenderer modelRenderer = new ModelRenderer(new ModelRendererConfig(itemModel.transform, MODEL_RENDER_OFFSET, new Vector2(256f, 256f))
			{
				FrameObjectInCamera = true
			});
			yield return new WaitForEndOfFrame();
			callback(Sprite.Create(modelRenderer.Image, new Rect(0f, 0f, modelRenderer.Image.width, modelRenderer.Image.height), Vector2.zero), null);
			modelRenderer.Destroy();
		}
	}
}
