using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(OpenCloseTweener))]
	public class ChatBarDisabler : UIElementDisabler
	{
		public RectTransform animationContainer;

		private OpenCloseTweener openCloseTweener;

		private bool isHidden;

		private Button[] childButtons;

		private void Awake()
		{
			setChildrenActive(false);
		}

		protected override void start()
		{
			base.start();
			if (isEnabled)
			{
				setChildrenActive(true);
			}
		}

		public override void DisableElement(bool hide)
		{
			isEnabled = false;
			if (hide)
			{
				isHidden = true;
				CanvasGroup canvasGroup = base.gameObject.AddComponent<CanvasGroup>();
				canvasGroup.alpha = 0f;
				canvasGroup.blocksRaycasts = false;
				return;
			}
			if (openCloseTweener == null)
			{
				initOpenCloseTweener();
			}
			if (openCloseTweener.IsOpen)
			{
				openCloseTweener.Close();
			}
			setButtonsEnabled(false);
		}

		public override void EnableElement()
		{
			isEnabled = true;
			setChildrenActive(true);
			if (isHidden)
			{
				isHidden = false;
				if (base.gameObject.GetComponent<CanvasGroup>() != null)
				{
					Object.Destroy(base.gameObject.GetComponent<CanvasGroup>());
				}
				return;
			}
			if (openCloseTweener == null)
			{
				initOpenCloseTweener();
			}
			if (!openCloseTweener.IsOpen)
			{
				openCloseTweener.Open();
			}
			setButtonsEnabled(true);
		}

		private void initOpenCloseTweener()
		{
			animationContainer = (base.transform.Find("AnimationContainer") as RectTransform);
			float y = animationContainer.anchoredPosition.y;
			float closedPosition = y - animationContainer.sizeDelta.y;
			openCloseTweener = GetComponent<OpenCloseTweener>();
			openCloseTweener.OnPositionChanged += onPositionChanged;
			openCloseTweener.OnComplete += onTweenComplete;
			openCloseTweener.Init(y, closedPosition);
			openCloseTweener.SetOpen();
		}

		private void onPositionChanged(float value)
		{
			if (animationContainer != null)
			{
				Vector2 anchoredPosition = animationContainer.anchoredPosition;
				anchoredPosition.y = value;
				animationContainer.anchoredPosition = anchoredPosition;
			}
		}

		private void onTweenComplete()
		{
			if (!openCloseTweener.IsOpen)
			{
				setChildrenActive(false);
			}
		}

		private void setChildrenActive(bool isActive)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				base.transform.GetChild(i).gameObject.SetActive(isActive);
			}
		}

		private void setButtonsEnabled(bool isEnabled)
		{
			if (childButtons == null)
			{
				childButtons = GetComponentsInChildren<Button>();
			}
			for (int i = 0; i < childButtons.Length; i++)
			{
				childButtons[i].interactable = isEnabled;
			}
		}
	}
}
