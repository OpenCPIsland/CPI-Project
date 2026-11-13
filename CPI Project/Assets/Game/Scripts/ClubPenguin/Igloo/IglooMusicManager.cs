using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Igloo
{
	public class IglooMusicManager : MonoBehaviour
	{
		private Dictionary<int, MusicTrackDefinition> musicDefs;

		private SceneLayoutData layout;

		private DataEventListener sceneLayoutListener;

		private EventChannel eventChannel;

		private MusicTrackDefinition currentMusicTrackDefinition;

		private GameObject musicTrackPrefabInstance;

		private MusicTrackDefinition currentPreviewingTrackDefinition;

		private GameObject previewMusicTrackInstance;

		private int currentlyLoadingTrack;

		private void Awake()
		{
			musicDefs = Service.Get<IGameData>().Get<Dictionary<int, MusicTrackDefinition>>();
			setupEventListeners();
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			sceneLayoutListener = cPDataEntityCollection.Whenever<SceneLayoutData>("ActiveSceneData", onLayoutAdded, onLayoutRemoved);
		}

		private void setupEventListeners()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<SceneTransitionEvents.MusicTrackPrefabLoaded>(onMusicTrackPrefabLoaded);
			eventChannel.AddListener<IglooMusicEvents.PreviewMusicTrack>(onPreviewMusicTrack);
			eventChannel.AddListener<IglooMusicEvents.SetMusicTrack>(onSetMusicTrack);
			eventChannel.AddListener<IglooMusicEvents.StopPreviewMusic>(onStopPreviewMusic);
			eventChannel.AddListener<IglooMusicEvents.StopAllMusic>(onStopAllMusic);
		}

		private bool onMusicTrackPrefabLoaded(SceneTransitionEvents.MusicTrackPrefabLoaded evt)
		{
			musicTrackPrefabInstance = evt.MusicTrackPrefab;
			return false;
		}

		private void onLayoutAdded(SceneLayoutData sceneLayoutData)
		{
			layout = sceneLayoutData;
			if (sceneLayoutData.MusicTrackId != 0)
			{
				MusicTrackDefinition value;
				if (musicDefs.TryGetValue(sceneLayoutData.MusicTrackId, out value))
				{
					currentMusicTrackDefinition = value;
				}
				else
				{
					Log.LogErrorFormatted(this, "Unable to find MusicTrackDefinition with id {0}", sceneLayoutData.MusicTrackId);
				}
			}
		}

		private void onLayoutRemoved(SceneLayoutData sceneLayoutData)
		{
			currentMusicTrackDefinition = null;
			currentPreviewingTrackDefinition = null;
			stopMusicTrack();
			stopPreviewTrack();
		}

		private bool onPreviewMusicTrack(IglooMusicEvents.PreviewMusicTrack evt)
		{
			int id = evt.Definition.Id;
			if (currentPreviewingTrackDefinition != null && id == currentPreviewingTrackDefinition.Id)
			{
				return false;
			}
			currentlyLoadingTrack = id;
			MusicTrackDefinition music;
			if (id == 0)
			{
				stopPreviewTrack();
			}
			else if (musicDefs.TryGetValue(id, out music))
			{
				Content.LoadAsync(delegate(string path, GameObject prefab)
				{
					onPreviewMusicLoaded(prefab, music);
				}, music.Music);
			}
			return false;
		}

		private bool onStopPreviewMusic(IglooMusicEvents.StopPreviewMusic evt)
		{
			if (currentPreviewingTrackDefinition == null && previewMusicTrackInstance == null)
			{
				return false;
			}
			currentPreviewingTrackDefinition = null;
			stopPreviewTrack();
			if (currentMusicTrackDefinition != null && currentMusicTrackDefinition.Id != 0)
			{
				currentlyLoadingTrack = currentMusicTrackDefinition.Id;
				playSong(currentMusicTrackDefinition.InternalName);
			}
			else
			{
				currentlyLoadingTrack = 0;
			}
			return false;
		}

		private bool onStopAllMusic(IglooMusicEvents.StopAllMusic evt)
		{
			layout.MusicTrackId = 0;
			currentlyLoadingTrack = 0;
			stopMusicTrack();
			stopPreviewTrack();
			return false;
		}

		private bool onSetMusicTrack(IglooMusicEvents.SetMusicTrack evt)
		{
			int id = evt.Definition.Id;
			if (currentMusicTrackDefinition != null && id == currentMusicTrackDefinition.Id)
			{
				return false;
			}
			currentlyLoadingTrack = id;
			MusicTrackDefinition music;
			if (id == 0)
			{
				stopMusicTrack();
			}
			else if (currentPreviewingTrackDefinition != null && id == currentPreviewingTrackDefinition.Id)
			{
				stopMusicTrack();
				layout.MusicTrackId = currentPreviewingTrackDefinition.Id;
				currentMusicTrackDefinition = currentPreviewingTrackDefinition;
				musicTrackPrefabInstance = previewMusicTrackInstance;
				musicTrackPrefabInstance.name = "Playing_" + evt.Definition.InternalName;
				if (!musicTrackPrefabInstance.activeSelf)
				{
					musicTrackPrefabInstance.SetActive(true);
				}
				currentPreviewingTrackDefinition = null;
				previewMusicTrackInstance = null;
			}
			else if (musicDefs.TryGetValue(id, out music))
			{
				Content.LoadAsync(delegate(string path, GameObject prefab)
				{
					onMusicLoaded(prefab, music);
				}, music.Music);
			}
			return false;
		}

		private void stopMusicTrack()
		{
			if (currentMusicTrackDefinition != null)
			{
				stopSong(currentMusicTrackDefinition.InternalName);
			}
			currentMusicTrackDefinition = null;
			if (musicTrackPrefabInstance != null)
			{
				Object.Destroy(musicTrackPrefabInstance);
			}
		}

		private void stopPreviewTrack()
		{
			if (currentMusicTrackDefinition != null)
			{
				stopSong(currentMusicTrackDefinition.InternalName);
			}
			currentPreviewingTrackDefinition = null;
			if (previewMusicTrackInstance != null)
			{
				Object.Destroy(previewMusicTrackInstance);
			}
		}

		private void onMusicLoaded(GameObject prefab, MusicTrackDefinition definition)
		{
			if (!(base.gameObject == null) && base.gameObject.activeSelf && currentlyLoadingTrack == definition.Id)
			{
				stopMusicTrack();
				stopPreviewTrack();
				musicTrackPrefabInstance = Object.Instantiate(prefab);
				musicTrackPrefabInstance.name = "Playing_" + definition.InternalName;
				layout.MusicTrackId = definition.Id;
				currentMusicTrackDefinition = definition;
				playSong(definition.InternalName);
			}
		}

		private void onPreviewMusicLoaded(GameObject prefab, MusicTrackDefinition definition)
		{
			if (!(base.gameObject == null) && base.gameObject.activeSelf && currentlyLoadingTrack == definition.Id)
			{
				if (currentMusicTrackDefinition != null && currentMusicTrackDefinition.Id != 0)
				{
					pauseSong(currentMusicTrackDefinition.InternalName);
				}
				stopPreviewTrack();
				previewMusicTrackInstance = Object.Instantiate(prefab);
				previewMusicTrackInstance.name = "Preview_" + definition.InternalName;
				currentPreviewingTrackDefinition = definition;
				playSong(definition.InternalName);
			}
		}

		private void playSong(string trackName)
		{
			string eventName = string.Format("Play/{0}", trackName);
			EventManager.Instance.PostEvent(eventName, EventAction.PlaySound, base.gameObject);
		}

		private void stopSong(string trackName)
		{
			string eventName = string.Format("Play/{0}", trackName);
			EventManager.Instance.PostEvent(eventName, EventAction.StopSound, base.gameObject);
		}

		private void pauseSong(string trackName)
		{
			string eventName = string.Format("Play/{0}", trackName);
			EventManager.Instance.PostEvent(eventName, EventAction.PauseSound, base.gameObject);
		}

		private void OnDestroy()
		{
			sceneLayoutListener.StopListening();
			stopPreviewTrack();
			stopMusicTrack();
		}
	}
}
