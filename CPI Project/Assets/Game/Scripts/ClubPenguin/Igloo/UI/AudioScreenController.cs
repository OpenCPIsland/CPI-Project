using ClubPenguin.ClothingDesigner;
using ClubPenguin.Core;
using ClubPenguin.Progression;
using ClubPenguin.Rewards;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ClubPenguin.Igloo.UI
{
	public class AudioScreenController : AbstractIglooScreenController<MusicTrackDefinition, int>
	{
		[SerializeField]
		private ClothingDesignerFilterBar genreFilterBar;

		private int currentlyPreviewedTrack;

		private bool isReloadingForGenre;

		protected override void Awake()
		{
			base.Awake();
			currentlyPreviewedTrack = 0;
			eventDispatcher = Service.Get<EventDispatcher>();
			if (genreFilterBar != null)
			{
				genreFilterBar.FilterChanged += onGenreFilterChanged;
			}
		}

		protected override void OnDestroy()
		{
			if (genreFilterBar != null)
			{
				genreFilterBar.FilterChanged -= onGenreFilterChanged;
			}
			base.OnDestroy();
		}

		private void onGenreFilterChanged()
		{
			if (!displayListRetrieved || isReloadingForGenre || isDestroyed)
			{
				return;
			}
			isReloadingForGenre = true;
			Reload();
			isReloadingForGenre = false;
		}

		private void applyGenreFilter(List<KeyValuePair<MusicTrackDefinition, int>> list)
		{
			if (genreFilterBar == null)
			{
				return;
			}
			Dictionary<int, MusicGenreDefinition> genres = Service.Get<IGameData>().Get<Dictionary<int, MusicGenreDefinition>>();
			Dictionary<string, Color> genreColors = new Dictionary<string, Color>();
			for (int i = 0; i < list.Count; i++)
			{
				string name = getGenreName(genres, list[i].Key);
				if (!string.IsNullOrEmpty(name) && !genreColors.ContainsKey(name))
				{
					genreColors.Add(name, genres[list[i].Key.MusicGenre.Id].GenreColor);
				}
			}
			genreFilterBar.SetTagColors(genreColors);
			genreFilterBar.SetAvailableTags(genreColors.Keys);
			string selected = genreFilterBar.SelectedTag;
			if (!string.IsNullOrEmpty(selected))
			{
				list.RemoveAll((KeyValuePair<MusicTrackDefinition, int> pair) => getGenreName(genres, pair.Key) != selected);
			}
		}

		private static string getGenreName(Dictionary<int, MusicGenreDefinition> genres, MusicTrackDefinition track)
		{
			MusicGenreDefinition genre;
			if (genres == null || track == null || !genres.TryGetValue(track.MusicGenre.Id, out genre) || genre == null)
			{
				return null;
			}
			return genre.DisplayName;
		}

		protected override List<ProgressionUtils.ParsedProgression<MusicTrackDefinition>> GetProgressionList()
		{
			return ProgressionUtils.RetrieveProgressionLockedItems<MusicTrackDefinition, MusicTrackRewardDefinition>(ProgressionUnlockCategory.musicTracks, AbstractStaticGameDataRewardDefinition<MusicTrackDefinition>.ToDefinitionArray);
		}

		protected override List<KeyValuePair<MusicTrackDefinition, int>> GetAvailableItems()
		{
			List<ProgressionUtils.ParsedProgression<MusicTrackDefinition>> progressionList = GetProgressionList();
			List<KeyValuePair<MusicTrackDefinition, int>> list = new List<KeyValuePair<MusicTrackDefinition, int>>();
			for (int i = 0; i < progressionList.Count; i++)
			{
				list.Add(new KeyValuePair<MusicTrackDefinition, int>(progressionList[i].Definition, 1));
			}
			list.Sort((a, b) => a.Key.MusicGenre.Id.CompareTo(b.Key.MusicGenre.Id));
			applyGenreFilter(list);
			return list;
		}

		protected override void onObjectAdded(RectTransform item, int index)
		{
			base.onObjectAdded(item, index);
			SetupAudioButton(item.GetComponent<AudioScreenButton>(), index);
		}

		protected override void onObjectRemoved(RectTransform item, int index)
		{
			AudioScreenButton component = item.GetComponent<AudioScreenButton>();
			component.Apply -= onApplyButton;
			component.Preview -= onPreviewButton;
			component.Stop -= onStopButton;
			component.Pause -= onPauseButton;
		}

		private void SetupAudioButton(AudioScreenButton button, int index)
		{
			MusicTrackDefinition key = inventoryCountPairs[index - numberOfStaticButtons].Key;
			AudioScreenButton.ButtonState state = AudioScreenButton.ButtonState.Normal;
			if (sceneLayoutData != null)
			{
				if (sceneLayoutData.MusicTrackId == key.Id)
				{
					state = AudioScreenButton.ButtonState.Selected;
				}
				else if (currentlyPreviewedTrack == key.Id)
				{
					state = AudioScreenButton.ButtonState.Preview;
				}
			}
			button.Init(key, state, index);
			button.Apply += onApplyButton;
			button.Preview += onPreviewButton;
			button.Stop += onStopButton;
			button.Pause += onPauseButton;
			ProgressionUtils.ParsedProgression<MusicTrackDefinition> value;
			inventoryProgressionStatus.TryGetValue(key.Id, out value);
			SetLockableButtonLockedStatus(button, key, value);
		}

		protected override int GetIntegerDefinitionId(MusicTrackDefinition definition)
		{
			return definition.Id;
		}

		public void OnStopAll()
		{
			currentlyPreviewedTrack = 0;
			eventDispatcher.DispatchEvent(default(IglooMusicEvents.StopAllMusic));
		}

		private void onPreviewButton(int index)
		{
			MusicTrackDefinition key = inventoryCountPairs[index].Key;
			currentlyPreviewedTrack = key.Id;
			eventDispatcher.DispatchEvent(new IglooMusicEvents.PreviewMusicTrack(key));
		}

		private void onApplyButton(int index)
		{
			MusicTrackDefinition key = inventoryCountPairs[index].Key;
			currentlyPreviewedTrack = 0;
			eventDispatcher.DispatchEvent(new IglooMusicEvents.SetMusicTrack(key));
		}

		private void onStopButton()
		{
			currentlyPreviewedTrack = 0;
			eventDispatcher.DispatchEvent(default(IglooMusicEvents.StopAllMusic));
		}

		private void onPauseButton()
		{
			currentlyPreviewedTrack = 0;
			eventDispatcher.DispatchEvent(default(IglooMusicEvents.StopPreviewMusic));
		}
	}
}
