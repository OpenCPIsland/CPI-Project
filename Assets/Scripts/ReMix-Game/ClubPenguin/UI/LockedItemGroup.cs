using Disney.Kelowna.Common;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class LockedItemGroup : LockedItemTag
	{
		private static Color32 LOCKED_THEME_NAME_TEXT_COLOR = new Color32(55, 8, 112, byte.MaxValue);

		private static Color32 UNLOCKED_THEME_NAME_TEXT_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		public Transform ItemsContainer;

		public GameObject MemberLock;

		public GameObject LevelLock;

		public Text LevelText;

		public GameObject MascotLock;

		public GameObject UnlockedBG;

		public bool ShowUnlockedBG = true;

		public Image MascotIcon;

		public GameObject CustomLock;

		[Header("Grouping")]
		public Image BackgroundImage;

		public Sprite DefaultSprite;

		public Sprite StartSprite;

		public Sprite MiddleSprite;

		public Sprite EndSprite;

		[Header("Theme With Icon")]
		public GameObject ThemeIconContainer;

		public Image ThemeIcon;

		public Text ThemeNameWithIcon;

		[Header("Theme Without Icon")]
		public GameObject ThemeWithoutIconContainer;

		public Text ThemeNameWithoutIcon;

		private bool isLocked = false;

		private LayoutGroup layoutGroup;

		private void Awake()
		{
			layoutGroup = GetComponent<LayoutGroup>();
		}

		private void SetThemeIconActive(bool state)
		{
			if (ThemeIconContainer != null)
			{
				ThemeIconContainer.SetActive(state);
			}
		}

		private void SetThemeWithoutIconActive(bool state)
		{
			if (ThemeWithoutIconContainer != null)
			{
				ThemeWithoutIconContainer.SetActive(state);
			}
		}

		public void GoToMemberLockState()
		{
			isLocked = true;
			MemberLock.SetActive(true);
			LevelLock.SetActive(false);
			MascotLock.SetActive(false);
			UnlockedBG.SetActive(false);
			setCustomLockVisibility(false);
			SetThemeIconActive(false);
		}

		public void GoToLevelLockState(int level)
		{
			isLocked = true;
			MemberLock.SetActive(false);
			LevelLock.SetActive(true);
			LevelText.text = level.ToString();
			MascotLock.SetActive(false);
			UnlockedBG.SetActive(false);
			setCustomLockVisibility(false);
			SetThemeIconActive(false);
		}

		public void GoToMascotLockState(SpriteContentKey mascotIconContentKey)
		{
			isLocked = true;
			MemberLock.SetActive(false);
			LevelLock.SetActive(false);
			MascotLock.SetActive(true);
			MascotIcon.enabled = false;
			UnlockedBG.SetActive(false);
			setCustomLockVisibility(false);
			SetThemeIconActive(false);
			Content.LoadAsync(onMascotIconLoaded, mascotIconContentKey);
		}

		public void GoToUnlockedState()
		{
			isLocked = false;
			MemberLock.SetActive(false);
			LevelLock.SetActive(false);
			MascotLock.SetActive(false);
			UnlockedBG.SetActive(ShowUnlockedBG);
			setCustomLockVisibility(false);
			SetThemeIconActive(false);
		}

		public void GoToCustomLockState()
		{
			isLocked = true;
			MemberLock.SetActive(false);
			LevelLock.SetActive(false);
			MascotLock.SetActive(false);
			UnlockedBG.SetActive(true);
			setCustomLockVisibility(true);
			SetThemeIconActive(false);
		}

		private void setCustomLockVisibility(bool visibility)
		{
			if (CustomLock != null)
			{
				CustomLock.SetActive(visibility);
			}
		}

		public void SetBackgroundImage(BGImage bgImage)
		{
			switch (bgImage)
			{
			case BGImage.Default:
				setBackgroundSprite(DefaultSprite);
				break;
			case BGImage.Start:
				setBackgroundSprite(StartSprite);
				break;
			case BGImage.Middle:
				setBackgroundSprite(MiddleSprite);
				break;
			case BGImage.End:
				setBackgroundSprite(EndSprite);
				break;
			}
		}

		private void setBackgroundSprite(Sprite sprite)
		{
			if (sprite != null)
			{
				BackgroundImage.sprite = sprite;
			}
		}

		public void SetThemeState(string localizedThemeName, SpriteContentKey packIconContentKey = null)
		{
			if (packIconContentKey == null)
			{
				SetThemeWithoutIconActive(true);
				SetThemeIconActive(false);
				if (ThemeNameWithoutIcon != null)
				{
					ThemeNameWithoutIcon.text = "";
					if (!string.IsNullOrEmpty(localizedThemeName))
					{
						ThemeNameWithoutIcon.text = localizedThemeName;
					}
					if (isLocked)
					{
						ThemeNameWithoutIcon.color = LOCKED_THEME_NAME_TEXT_COLOR;
					}
					else
					{
						ThemeNameWithoutIcon.color = UNLOCKED_THEME_NAME_TEXT_COLOR;
					}
				}
				return;
			}
			SetThemeIconActive(true);
			SetThemeWithoutIconActive(false);
			if (ThemeNameWithIcon != null)
			{
				ThemeNameWithIcon.text = "";
				if (!string.IsNullOrEmpty(localizedThemeName))
				{
					ThemeNameWithIcon.text = localizedThemeName;
				}
				if (isLocked)
				{
					ThemeNameWithIcon.color = LOCKED_THEME_NAME_TEXT_COLOR;
				}
				else
				{
					ThemeNameWithIcon.color = UNLOCKED_THEME_NAME_TEXT_COLOR;
				}
			}
			if (ThemeIcon != null)
			{
				Content.LoadAsync(onThemeIconLoaded, packIconContentKey);
			}
		}

		private void onMascotIconLoaded(string path, Sprite mascotIcon)
		{
			MascotIcon.sprite = mascotIcon;
			MascotIcon.enabled = true;
		}

		private void onThemeIconLoaded(string path, Sprite themeIcon)
		{
			ThemeIcon.sprite = themeIcon;
			ThemeIcon.enabled = true;
		}

		public void SetChildAlignment(TextAnchor alignment)
		{
			if (layoutGroup != null)
			{
				layoutGroup.childAlignment = alignment;
			}
		}
	}
}
