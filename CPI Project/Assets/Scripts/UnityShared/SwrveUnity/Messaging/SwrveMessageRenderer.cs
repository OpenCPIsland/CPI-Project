using UnityEngine;

namespace SwrveUnity.Messaging
{
	public class SwrveMessageRenderer
	{
		protected static readonly Color ButtonPressedColor = new Color(0.5f, 0.5f, 0.5f);

		protected static Texture2D blankTexture;

		protected static Rect WholeScreen = default(Rect);

		public static ISwrveMessageAnimator Animator;

		protected static Texture2D GetBlankTexture()
		{
			if (blankTexture == null)
			{
				blankTexture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
				blankTexture.SetPixel(0, 0, Color.white);
				blankTexture.SetPixel(1, 0, Color.white);
				blankTexture.SetPixel(0, 1, Color.white);
				blankTexture.SetPixel(1, 1, Color.white);
				blankTexture.Apply(false, true);
			}
			return blankTexture;
		}

		public static void InitMessage(SwrveMessageFormat format, SwrveOrientation deviceOrientation)
		{
			if (Animator != null)
			{
				Animator.InitMessage(format);
				return;
			}
			format.Init(deviceOrientation);
			format.InitAnimation(new Point(0, 0), new Point(0, 0));
		}

		public static void AnimateMessage(SwrveMessageFormat format)
		{
			if (Animator != null)
			{
				Animator.AnimateMessage(format);
			}
		}

		public static void DrawMessage(SwrveMessageFormat format, int centerx, int centery)
		{
			if (Animator != null)
			{
				AnimateMessage(format);
			}
			if (format.BackgroundColor.HasValue && GetBlankTexture() != null)
			{
				Color value = format.BackgroundColor.Value;
				value.a *= format.Message.BackgroundAlpha;
				GUI.color = value;
				WholeScreen.width = Screen.width;
				WholeScreen.height = Screen.height;
				GUI.DrawTexture(WholeScreen, blankTexture, ScaleMode.StretchToFill, true, 10f);
				GUI.color = Color.white;
			}
			bool rotate = format.Rotate;
			if (rotate)
			{
				Vector2 pivotPoint = new Vector2(centerx, centery);
				GUIUtility.RotateAroundPivot(90f, pivotPoint);
			}
			float num = format.Scale * format.Message.AnimationScale;
			GUI.color = Color.white;
			Point centeredPosition;
			for (int i = 0; i < format.Images.Count; i++)
			{
				SwrveImage swrveImage = format.Images[i];
				if (swrveImage.Texture != null)
				{
					float num2 = num * swrveImage.AnimationScale;
					centeredPosition = swrveImage.GetCenteredPosition(swrveImage.Texture.width, swrveImage.Texture.height, num2, num);
					centeredPosition.X += centerx;
					centeredPosition.Y += centery;
					swrveImage.Rect.x = centeredPosition.X;
					swrveImage.Rect.y = centeredPosition.Y;
					swrveImage.Rect.width = (float)swrveImage.Texture.width * num2;
					swrveImage.Rect.height = (float)swrveImage.Texture.height * num2;
					GUI.DrawTexture(swrveImage.Rect, swrveImage.Texture, ScaleMode.StretchToFill, true, 10f);
				}
				else
				{
					GUI.Box(swrveImage.Rect, swrveImage.File);
				}
			}
			for (int j = 0; j < format.Buttons.Count; j++)
			{
				SwrveButton swrveButton = format.Buttons[j];
				if (swrveButton.Texture != null)
				{
					float num2 = num * swrveButton.AnimationScale;
					centeredPosition = swrveButton.GetCenteredPosition(swrveButton.Texture.width, swrveButton.Texture.height, num2, num);
					swrveButton.Rect.x = centeredPosition.X + centerx;
					swrveButton.Rect.y = centeredPosition.Y + centery;
					swrveButton.Rect.width = (float)swrveButton.Texture.width * num2;
					swrveButton.Rect.height = (float)swrveButton.Texture.height * num2;
					if (rotate)
					{
						Point center = swrveButton.GetCenter(swrveButton.Texture.width, swrveButton.Texture.height, num2);
						swrveButton.PointerRect.x = (float)centerx - (float)swrveButton.Position.Y * num + (float)center.Y;
						swrveButton.PointerRect.y = (float)centery + (float)swrveButton.Position.X * num + (float)center.X;
						swrveButton.PointerRect.width = swrveButton.Rect.height;
						swrveButton.PointerRect.height = swrveButton.Rect.width;
					}
					else
					{
						swrveButton.PointerRect = swrveButton.Rect;
					}
					if (Animator != null)
					{
						Animator.AnimateButtonPressed(swrveButton);
					}
					else
					{
						GUI.color = (swrveButton.Pressed ? ButtonPressedColor : Color.white);
					}
					GUI.DrawTexture(swrveButton.Rect, swrveButton.Texture, ScaleMode.StretchToFill, true, 10f);
				}
				else
				{
					GUI.Box(swrveButton.Rect, swrveButton.Image);
				}
				GUI.color = Color.white;
			}
			if ((Animator == null && format.Closing) || (Animator != null && Animator.IsMessageDismissed(format)))
			{
				format.Dismissed = true;
				format.UnloadAssets();
			}
		}
	}
}
