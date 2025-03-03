using ClubPenguin.Analytics;
using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Igloo.UI
{
	public class IglooNowPlayingController : MonoBehaviour
	{
		private const string ANIM_TRIGGER_IN = "In";

		private const string ANIM_TRIGGER_OUT = "Out";

		public Image Icon;

		public Text SongName;

		public Image GenreIconBackgroundToTint;

		public Animator IntroExitAnimator;

		public Animator SongAnimationAnimator;

		public Animator SongGenreAnimationAnimator;

		private Dictionary<int, MusicTrackDefinition> musicDefinitions;

		private Dictionary<int, MusicGenreDefinition> musicGenres;

		private EventChannel eventChannel;

		private DataEventListener sceneLayoutListener;

		private Sprite previousIconSprite;

		private bool isPlaying;

		public void OnStopButton()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(IglooMusicEvents.StopAllMusic));
		}

		private void Start()
		{
			musicDefinitions = Service.Get<IGameData>().Get<Dictionary<int, MusicTrackDefinition>>();
			musicGenres = Service.Get<IGameData>().Get<Dictionary<int, MusicGenreDefinition>>();
			setupListeners();
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			sceneLayoutListener = cPDataEntityCollection.When<SceneLayoutData>("ActiveSceneData", onLayoutAdded);
		}

		private void setupListeners()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<IglooMusicEvents.SetMusicTrack>(onSetMusicTrack);
			eventChannel.AddListener<IglooMusicEvents.PreviewMusicTrack>(onPreviewMusicTrack);
			eventChannel.AddListener<IglooMusicEvents.StopPreviewMusic>(onStopPreviewMusic);
			eventChannel.AddListener<IglooMusicEvents.StopAllMusic>(onStopAllMusic);
		}

		private void onLayoutAdded(SceneLayoutData obj)
		{
			if (obj.MusicTrackId != 0)
			{
				setMusicTrackData(obj.MusicTrackId);
				showNowPlaying();
			}
		}

		private bool onSetMusicTrack(IglooMusicEvents.SetMusicTrack evt)
		{
			setMusicTrackData(evt.Definition.Id);
			showNowPlaying();
			SongAnimationAnimator.enabled = true;
			SongGenreAnimationAnimator.enabled = true;
			MusicTrackDefinition value;
			if (evt.Definition.Id == 0)
			{
				Service.Get<ICPSwrveService>().Action("igloo", "music_selection", "none");
			}
			else if (musicDefinitions.TryGetValue(evt.Definition.Id, out value))
			{
				Service.Get<ICPSwrveService>().Action("igloo", "music_selection", value.InternalName);
			}
			return false;
		}

		private void setMusicTrackData(int id)
		{
			MusicTrackDefinition value;
			if (id == 0)
			{
				hideNowPlaying();
			}
			else if (musicDefinitions.TryGetValue(id, out value))
			{
				SongName.text = Localizer.Instance.GetTokenTranslation(value.Name);
				if (musicGenres.ContainsKey(value.MusicGenre.Id))
				{
					GenreIconBackgroundToTint.color = musicGenres[value.MusicGenre.Id].GenreColor;
				}
				resetSprite();
				if (value.Icon != null && !string.IsNullOrEmpty(value.Icon.Key))
				{
					Content.LoadAsync(onTrackIconLoaded, value.Icon);
				}
			}
		}

		private void onTrackIconLoaded(string path, Texture2D newIcon)
		{
			previousIconSprite = Icon.sprite;
			Icon.sprite = Sprite.Create(newIcon, new Rect(0f, 0f, newIcon.width, newIcon.height), Vector2.zero);
		}

		private void resetSprite()
		{
			if (previousIconSprite != null)
			{
				Object.Destroy(Icon.sprite);
				Icon.sprite = previousIconSprite;
				previousIconSprite = null;
			}
		}

		private bool onPreviewMusicTrack(IglooMusicEvents.PreviewMusicTrack evt)
		{
			SongAnimationAnimator.enabled = false;
			SongGenreAnimationAnimator.enabled = false;
			return false;
		}

		private bool onStopPreviewMusic(IglooMusicEvents.StopPreviewMusic evt)
		{
			SongAnimationAnimator.enabled = true;
			SongGenreAnimationAnimator.enabled = true;
			return false;
		}

		private bool onStopAllMusic(IglooMusicEvents.StopAllMusic evt)
		{
			hideNowPlaying();
			return false;
		}

		private void showNowPlaying()
		{
			if (!isPlaying)
			{
				isPlaying = true;
				if (IntroExitAnimator != null)
				{
					IntroExitAnimator.ResetTrigger("Out");
					IntroExitAnimator.SetTrigger("In");
				}
			}
		}

		private void hideNowPlaying()
		{
			if (isPlaying)
			{
				isPlaying = false;
				if (IntroExitAnimator != null)
				{
					IntroExitAnimator.ResetTrigger("In");
					IntroExitAnimator.SetTrigger("Out");
				}
			}
		}

		private void OnDestroy()
		{
			resetSprite();
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
			if (sceneLayoutListener != null)
			{
				sceneLayoutListener.StopListening();
			}
		}
	}
}
