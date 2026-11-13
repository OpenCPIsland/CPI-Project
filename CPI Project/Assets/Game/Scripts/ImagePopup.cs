using ClubPenguin;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImagePopup : AnimatedPopup
{
	public Image Image;

	public Button CloseButton;

	public Button FullScreenButton;

	public Text DisplayText;

	public RectTransform ImagePanel;

	protected override void awake()
	{
		AutoOpen = false;
	}

	protected override void start()
	{
		if (CloseButton != null)
		{
			CloseButton.gameObject.AddComponent<BackButton>();
		}
	}

	public void SetData(DImagePopup imagePopupData)
	{
		CoroutineRunner.Start(loadImage(imagePopupData.ImageContentKey), this, "loadPopupImage");
		if (DisplayText == null)
		{
			return;
		}
		ImagePanel.anchoredPosition = imagePopupData.ImageOffset;
		ImagePanel.localScale = new Vector3(imagePopupData.ImageScale.x, imagePopupData.ImageScale.y, 0f);
		if (string.IsNullOrEmpty(imagePopupData.Text))
		{
			DisplayText.gameObject.SetActive(false);
			return;
		}
		DisplayText.text = imagePopupData.Text;
		DisplayText.color = ColorUtils.HexToColor(imagePopupData.TextStyle.ColorHex);
		DisplayText.fontSize = imagePopupData.TextStyle.FontSize;
		DisplayText.rectTransform.position += new Vector3(imagePopupData.TextOffset.x, imagePopupData.TextOffset.y, 0f);
		if (!string.IsNullOrEmpty(imagePopupData.TextStyle.FontContentKey))
		{
			string fontContentKey = formatFontPath(imagePopupData.TextStyle.FontContentKey);
			CoroutineRunner.Start(loadFont(fontContentKey), this, "ImagePopup.loadFont");
		}
		DisplayText.alignment = imagePopupData.TextAlignment;
	}

	private IEnumerator loadFont(string fontContentKey)
	{
		AssetRequest<Font> assetRequest = Content.LoadAsync<Font>(fontContentKey);
		yield return assetRequest;
		DisplayText.font = assetRequest.Asset;
	}

	public void EnableCloseButtons(bool closeButtonEnabled, bool fullScreenButtonEnabled)
	{
		CloseButton.gameObject.SetActive(closeButtonEnabled);
		FullScreenButton.gameObject.SetActive(fullScreenButtonEnabled);
	}

	private IEnumerator loadImage(SpriteContentKey imageContentKey)
	{
		AssetRequest<Sprite> assetRequest = Content.LoadAsync(imageContentKey);
		yield return assetRequest;
		Image.sprite = assetRequest.Asset;
		if (OpenDelay == 0f || openDelayComplete)
		{
			OpenPopup();
		}
	}

	private string formatFontPath(string fontPath)
	{
		int num = fontPath.IndexOf("/Resources/");
		if (num == -1)
		{
			num = fontPath.IndexOf("/AssetBundles/");
		}
		int num2 = fontPath.LastIndexOf('.');
		int num3 = num + 11;
		return fontPath.Substring(num3, num2 - num3);
	}
}
