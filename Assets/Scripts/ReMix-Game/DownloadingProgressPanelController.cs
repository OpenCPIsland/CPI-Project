using ClubPenguin;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

public class DownloadingProgressPanelController : MonoBehaviour
{
	public Text HeadlineText;

	public Text DetailsText;

	public GameObject Emojis;

	[Header("Set specific emoji to use.")]
	public GameObject SetEmoji;

	public GameObject ProgressBarFill;

	[Range(0f, 10f)]
	public float fillStripesRepeatSeconds = 2f;

	public RectTransform fillStripes;

	private RectTransform fillStripesParent;

	private RectTransform emojiRectTransform;

	private RectTransform progressBarFillRectTransform;

	private TintSelector tintSelector;

	private LoadingController loadingController;

	private BundlePrecacheManager bundleManager;

	private float milestoneProgress;

	private float progressbarWidth;

	private bool isGenericProgressBar = true;

	private float currentTime;

	private bool isPlayingSpinner = false;

	[Range(0f, 1f)]
	public float FakeProgress = 0f;

	public bool FullProgressCompleted = false;

	private float gradualProgress = 0f;

	private void OnValidate()
	{
	}

	private void OnEnable()
	{
		if (ProgressBarFill != null)
		{
			progressBarFillRectTransform = ProgressBarFill.GetComponent<RectTransform>();
			fillStripesParent = (RectTransform)fillStripes.parent;
			tintSelector = ProgressBarFill.GetComponentInChildren<TintSelector>();
			milestoneProgress = progressBarFillRectTransform.rect.width / (float)tintSelector.Colors.Length;
			progressbarWidth = progressBarFillRectTransform.rect.width;
			gradualProgress = 0f;
		}
		if (Emojis != null)
		{
			emojiRectTransform = Emojis.GetComponent<RectTransform>();
			setSpecificEmoji();
		}
		bundleManager = Service.Get<BundlePrecacheManager>();
		currentTime = 0f;
		isPlayingSpinner = false;
		Update();
	}

	private void Update()
	{
		if (loadingController == null && Service.IsSet<LoadingController>())
		{
			loadingController = Service.Get<LoadingController>();
		}
		float num = 0f;
		float num2 = 0f;
		num = ((loadingController == null) ? 1f : (loadingController.DownloadProgress.HasValue ? loadingController.DownloadProgress.Value : 1f));
		if (bundleManager == null && Service.IsSet<BundlePrecacheManager>())
		{
			bundleManager = Service.Get<BundlePrecacheManager>();
		}
		num2 = ((bundleManager == null) ? 1f : ((!bundleManager.IsCaching) ? 1f : bundleManager.CompleteRatio));
		num2 = num2 / 2f + num / 2f;
		num2 = Mathf.Lerp(0f, num2, currentTime);
		Vector2 anchoredPosition = fillStripes.anchoredPosition;
		currentTime += Time.deltaTime;
		currentTime %= fillStripesRepeatSeconds;
		float num3 = currentTime / fillStripesRepeatSeconds;
		float num4 = anchoredPosition.x = num3 * fillStripesParent.rect.width;
		fillStripes.anchoredPosition = anchoredPosition;
		if (num2 >= 1f - Mathf.Epsilon)
		{
			FullProgressCompleted = true;
		}
		else if (FullProgressCompleted)
		{
			setSpecificEmoji();
			FullProgressCompleted = false;
			isPlayingSpinner = false;
		}
		if (!FullProgressCompleted || !isPlayingSpinner)
		{
			progressBarFillRectTransform.anchorMax = new Vector2(num2, progressBarFillRectTransform.anchorMax.y);
			float x = num2 * progressbarWidth;
			emojiRectTransform.anchoredPosition = new Vector2(x, emojiRectTransform.anchoredPosition.y);
			int a = (int)Mathf.Max(0f, (float)tintSelector.Colors.Length * num2);
			uint num5 = (uint)Mathf.Min(a, tintSelector.Colors.Length - 1);
			tintSelector.SelectColor((int)num5);
			if (isGenericProgressBar && SetEmoji == null)
			{
				setEmojiIndex(num5 + 1);
			}
			if (FullProgressCompleted)
			{
				setEmojiIndex(0u);
				isPlayingSpinner = true;
			}
		}
	}

	private void setSpecificEmoji()
	{
		if (SetEmoji != null)
		{
			setEmoji(SetEmoji);
			isGenericProgressBar = false;
		}
	}

	private void setEmoji(GameObject emojiGO)
	{
		if (!emojiGO.activeSelf)
		{
			for (int i = 0; i < Emojis.transform.childCount; i++)
			{
				Transform child = Emojis.transform.GetChild(i);
				child.gameObject.SetActive(emojiGO == child.gameObject);
			}
		}
	}

	private void setEmojiIndex(uint index)
	{
		if (index >= Emojis.transform.childCount)
		{
			Log.LogError(this, "The index is higher than the number of emojis available.  Setting to max allowed");
			index = (uint)Mathf.Min(index, Emojis.transform.childCount - 1);
		}
		if (!Emojis.transform.GetChild((int)index).gameObject.activeSelf)
		{
			for (int i = 0; i < Emojis.transform.childCount; i++)
			{
				Transform child = Emojis.transform.GetChild(i);
				child.gameObject.SetActive(index == i);
			}
		}
	}
}
