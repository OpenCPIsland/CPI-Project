using ClubPenguin.Avatar;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	public static class AvatarRenderer
	{
		public const bool CONTINUE_PROCESSING = true;

		public const bool TERMINATE_PROCESSING = false;

		public static IEnumerator RenderOutfit(DCustomOutfit outfit, Color beakColor, Color bodyColor, Color bellyColor, ImageBuilderCameraData cameraData, GameObject avatarGO, Func<ModelRenderer, bool> onProcessModel, AvatarAnimationFrame animationFrame = null)
		{
			AvatarModel avatarModel = avatarGO.GetComponent<AvatarModel>();
			avatarModel.BeakColor = beakColor;
			avatarModel.BodyColor = bodyColor;
			avatarModel.BellyColor = bellyColor;
			avatarModel.ClearAllEquipment();
			avatarModel.ApplyOutfit(outfit);
			AvatarView avatarView = avatarGO.GetComponent<AvatarView>();
			yield return new WaitUntil(() => avatarView.IsReady);
			ModelRendererConfig renderConfig = new ModelRendererConfig(avatarGO.transform, cameraData.ModelOffset, new Vector2(cameraData.IconSize, cameraData.IconSize));
			avatarGO.transform.Rotate(cameraData.ModelRotation);
			renderConfig.FieldOfView = cameraData.FieldOfView;
			ModelRenderer modelRenderer = new ModelRenderer(renderConfig);
			modelRenderer.RotateCamera(cameraData.CameraRotation);
			if (animationFrame != null)
			{
				Animator component = avatarGO.GetComponent<Animator>();
                Debug.Log("StateName: "+ Convert.ToString(animationFrame.StateName) + " Layer: " + Convert.ToString(animationFrame.Layer) + " Time: " + Convert.ToString(animationFrame.Time));
				component.Play(animationFrame.StateName, animationFrame.Layer, animationFrame.Time);
			}
			do
			{
				yield return null;
			}
			while (onProcessModel(modelRenderer));
		}
	}
}
