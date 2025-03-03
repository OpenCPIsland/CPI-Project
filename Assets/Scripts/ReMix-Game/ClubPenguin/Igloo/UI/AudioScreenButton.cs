using ClubPenguin.Breadcrumbs;
using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Igloo.UI
{
	public class AudioScreenButton : MonoBehaviour, ILockableButton
	{
		public enum ButtonState
		{
			Normal,
			Selected,
			Preview
		}

		public Text SongName;

		public Image GenreIcon;

		public GameObject SelectedHighlight;

		[Tooltip("The image will be tinted based on the genre.")]
		public Image GenreIconBackgroundToTint;

		public Button PreviewButton;

		public Button ApplyButton;

		public Button StopButton;

		public Button PauseButton;

		public GameObject LockedPanel;

		public GameObject ProgressionStar;

		public Text ProgressionBadgeText;

		public GameObject MascotBadges;

		public GameObject MemberBadge;

		public PersistentBreadcrumbTypeDefinitionKey audioTypeBreadcrumb;

		public StaticBreadcrumbDefinitionKey audioBreadcrumb;

		public NotificationBreadcrumb breadcrumb;

		private int definitionId;

		private ButtonState currentState;

		private Dictionary<int, MusicGenreDefinition> genres;

		private Sprite previousIconSprite;

		private EventChannel eventChannel;

		private bool isBreadcrumbShowing;

		private TintSelector previewButtonTintSelector;

		private int buttonIndex;

		public event Action<int> Preview;

		public event Action<int> Apply;

		public event System.Action Stop;

		public event System.Action Pause;

		private void Awake()
		{
			genres = Service.Get<IGameData>().Get<Dictionary<int, MusicGenreDefinition>>();
			previewButtonTintSelector = PreviewButton.GetComponent<TintSelector>();
			setupListeners();
		}

		private void setupListeners()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<IglooMusicEvents.PreviewMusicTrack>(onPreviewMusicTrack);
			eventChannel.AddListener<IglooMusicEvents.SetMusicTrack>(onSetMusicTrack);
			eventChannel.AddListener<IglooMusicEvents.StopAllMusic>(onMuteAllMusic);
			eventChannel.AddListener<IglooMusicEvents.StopPreviewMusic>(onStopPreviewMusic);
		}

		public void Init(MusicTrackDefinition trackDefinition, ButtonState state, int buttonIndex)
		{
			definitionId = trackDefinition.Id;
			SongName.text = Service.Get<Localizer>().GetTokenTranslation(trackDefinition.Name);
			setTrackIcon(trackDefinition);
			if (genres.ContainsKey(trackDefinition.MusicGenre.Id))
			{
				GenreIconBackgroundToTint.color = genres[trackDefinition.MusicGenre.Id].GenreColor;
			}
			switch (state)
			{
			case ButtonState.Normal:
				setToNormalState();
				break;
			case ButtonState.Selected:
				setToSelectedState();
				break;
			case ButtonState.Preview:
				setToPreviewingState();
				break;
			}
			this.buttonIndex = buttonIndex;
			breadcrumb.SetBreadcrumbId(audioTypeBreadcrumb, trackDefinition.Id.ToString());
			isBreadcrumbShowing = (breadcrumb.Count > 0);
		}

		public void SetLevelLocked(int level)
		{
			LockedPanel.SetActive(true);
			ProgressionStar.SetActive(true);
			MascotBadges.SetActive(false);
			MemberBadge.SetActive(false);
			ProgressionBadgeText.text = level.ToString();
		}

		public void SetProgressionLocked(string mascotName)
		{
			LockedPanel.SetActive(true);
			MascotBadges.SetActive(true);
			ProgressionStar.SetActive(false);
			MemberBadge.SetActive(false);
			IList<Transform> children = MascotBadges.GetChildren();
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i].name.Equals(mascotName))
				{
					children[i].gameObject.SetActive(true);
				}
				else
				{
					children[i].gameObject.SetActive(false);
				}
			}
		}

		public void SetMemberLocked()
		{
			LockedPanel.SetActive(true);
			MemberBadge.SetActive(true);
			MascotBadges.SetActive(false);
			ProgressionStar.SetActive(false);
		}

		public void SetUnlocked()
		{
			LockedPanel.SetActive(false);
		}

		private void setTrackIcon(MusicTrackDefinition trackDefinition)
		{
			resetSprite();
			if (trackDefinition.Icon != null && !string.IsNullOrEmpty(trackDefinition.Icon.Key))
			{
				Content.LoadAsync(onTrackIconLoaded, trackDefinition.Icon);
			}
		}

		private void setToNormalState()
		{
			currentState = ButtonState.Normal;
			SelectedHighlight.SetActive(false);
			PreviewButton.interactable = true;
			PreviewButton.gameObject.SetActive(true);
			previewButtonTintSelector.SelectColor(0);
			ApplyButton.gameObject.SetActive(true);
			PauseButton.gameObject.SetActive(false);
			StopButton.gameObject.SetActive(false);
		}

		private void setToSelectedState()
		{
			currentState = ButtonState.Selected;
			SelectedHighlight.SetActive(true);
			PreviewButton.interactable = false;
			PreviewButton.gameObject.SetActive(true);
			previewButtonTintSelector.SelectColor(1);
			StopButton.gameObject.SetActive(true);
			PauseButton.gameObject.SetActive(false);
			ApplyButton.gameObject.SetActive(false);
		}

		private void setToPreviewingState()
		{
			currentState = ButtonState.Preview;
			SelectedHighlight.SetActive(false);
			PreviewButton.gameObject.SetActive(false);
			PauseButton.gameObject.SetActive(true);
			ApplyButton.gameObject.SetActive(true);
			StopButton.gameObject.SetActive(false);
		}

		private void RemoveBreadcrumb()
		{
			if (isBreadcrumbShowing)
			{
				Service.Get<NotificationBreadcrumbController>().RemovePersistentBreadcrumb(audioTypeBreadcrumb, definitionId.ToString());
				isBreadcrumbShowing = false;
				Service.Get<NotificationBreadcrumbController>().RemoveBreadcrumb(audioBreadcrumb);
			}
		}

		public void OnPreviewButton()
		{
			if (this.Preview != null)
			{
				RemoveBreadcrumb();
				this.Preview(buttonIndex);
			}
		}

		public void OnApplyButton()
		{
			if (this.Apply != null)
			{
				RemoveBreadcrumb();
				this.Apply(buttonIndex);
			}
		}

		public void OnStopButton()
		{
			if (this.Stop != null)
			{
				this.Stop();
			}
		}

		public void OnPauseButton()
		{
			if (this.Pause != null)
			{
				this.Pause();
			}
		}

		private bool onPreviewMusicTrack(IglooMusicEvents.PreviewMusicTrack evt)
		{
			if (definitionId == evt.Definition.Id)
			{
				setToPreviewingState();
			}
			else if (currentState == ButtonState.Preview)
			{
				setToNormalState();
			}
			return false;
		}

		private bool onSetMusicTrack(IglooMusicEvents.SetMusicTrack evt)
		{
			if (definitionId == evt.Definition.Id)
			{
				setToSelectedState();
			}
			else if (currentState != 0)
			{
				setToNormalState();
			}
			return false;
		}

		private bool onMuteAllMusic(IglooMusicEvents.StopAllMusic evt)
		{
			setToNormalState();
			return false;
		}

		private bool onStopPreviewMusic(IglooMusicEvents.StopPreviewMusic evt)
		{
			if (currentState == ButtonState.Preview)
			{
				setToNormalState();
			}
			return false;
		}

		private void onTrackIconLoaded(string path, Texture2D newIcon)
		{
			previousIconSprite = GenreIcon.sprite;
			GenreIcon.sprite = Sprite.Create(newIcon, new Rect(0f, 0f, newIcon.width, newIcon.height), Vector2.zero);
		}

		private void resetSprite()
		{
			if (previousIconSprite != null)
			{
				UnityEngine.Object.Destroy(GenreIcon.sprite);
				GenreIcon.sprite = previousIconSprite;
				previousIconSprite = null;
			}
		}

		private void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
			resetSprite();
			this.Preview = null;
			this.Apply = null;
			this.Stop = null;
			this.Pause = null;
		}
	}
}
