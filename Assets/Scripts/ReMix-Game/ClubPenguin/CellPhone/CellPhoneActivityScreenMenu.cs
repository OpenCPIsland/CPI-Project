using ClubPenguin.UI;
using Disney.Manimal.Common.Util;
using Disney.MobileNetwork;
using Fabric;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.CellPhone
{
	[RequireComponent(typeof(OpenCloseTweener))]
	public class CellPhoneActivityScreenMenu : MonoBehaviour
	{
		private const int ARROW_SPRITE_OPEN_INDEX = 0;

		private const int ARROW_SPRITE_CLOSED_INDEX = 1;

		private const long AUTO_OPEN_OFFSET_MILLISEONDS = 3000L;

		public LayoutElement ScrollViewMenuOffset;

		public SpriteSelector MenuArrowSelector;

		private OpenCloseTweener tweener;

		private RectTransform rect;

		private float openPosition;

		private void Awake()
		{
			rect = GetComponent<RectTransform>();
		}

		private void Start()
		{
			initOpenCloseTweener();
		}

		private void OnDestroy()
		{
			if (tweener != null)
			{
				tweener.OnComplete -= onTweenComplete;
				tweener.OnPositionChanged -= onPositionChanged;
			}
		}

		public void OnToggleButtonClick()
		{
			if (!tweener.IsTransitioning)
			{
				if (tweener.IsOpen)
				{
					tweener.Close();
					EventManager.Instance.PostEvent("SFX/UI/Store/TrayClose", EventAction.PlaySound, this);
				}
				else
				{
					tweener.Open();
					EventManager.Instance.PostEvent("SFX/UI/Store/TrayOpen", EventAction.PlaySound, this);
				}
			}
		}

		private void initOpenCloseTweener()
		{
			openPosition = rect.anchoredPosition.y;
			float closedPosition = openPosition - rect.rect.height;
			tweener = GetComponent<OpenCloseTweener>();
			tweener.OnComplete += onTweenComplete;
			tweener.OnPositionChanged += onPositionChanged;
			tweener.Init(openPosition, closedPosition);
			if (wasCellPhoneAutoOpened())
			{
				tweener.SetClosed();
			}
			else
			{
				tweener.SetOpen();
			}
			onTweenComplete();
		}

		private void onPositionChanged(float value)
		{
			Vector2 anchoredPosition = rect.anchoredPosition;
			anchoredPosition.y = value;
			rect.anchoredPosition = anchoredPosition;
		}

		private void onTweenComplete()
		{
			MenuArrowSelector.SelectSprite((!tweener.IsOpen) ? 1 : 0);
			if (tweener.IsOpen)
			{
				ScrollViewMenuOffset.preferredHeight = rect.rect.height;
			}
			else
			{
				ScrollViewMenuOffset.preferredHeight = 0f;
			}
		}

		private bool wasCellPhoneAutoOpened()
		{
			bool result = false;
			if (PlayerPrefs.HasKey("DailyChallengesLastOpen"))
			{
				long num = long.Parse(PlayerPrefs.GetString("DailyChallengesLastOpen"));
				long timeInMilliseconds = Service.Get<ContentSchedulerService>().PresentTime().GetTimeInMilliseconds();
				long num2 = timeInMilliseconds - num;
				if (num2 <= 3000)
				{
					result = true;
				}
			}
			return result;
		}
	}
}
