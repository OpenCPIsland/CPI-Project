using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fabric
{
	[AddComponentMenu("Fabric/Components/DialogAudioComponent")]
	public class DialogAudioComponent : AudioComponent
	{
		[HideInInspector]
		[SerializeField]
		public string _audioClipReference;

		[HideInInspector]
		[SerializeField]
		public DialogAudioLoadedFrom _dialogAudioLoadedFrom;

		private bool _loadPlayCoroutineActive;

		public string AudioClipReference
		{
			get
			{
				return _audioClipReference;
			}
			set
			{
				_audioClipReference = value;
			}
		}

		public override void Destroy()
		{
			UnLoad();
			base.Destroy();
		}

		private void Awake()
		{
			SceneManager.sceneLoaded += SceneLoaded;
		}

		private void SceneLoaded(Scene scene, LoadSceneMode m)
		{
			if (base.CurrentState != AudioComponentState.Playing)
			{
				UnLoad();
			}
		}

		protected void Load()
		{
			int languageIndex = FabricManager.Instance.GetLanguageIndex();
			if (languageIndex < 0)
			{
				return;
			}
			LanguageProperties languagePropertiesByIndex = FabricManager.Instance.GetLanguagePropertiesByIndex(languageIndex);
			if (!(base.AudioClip == null) || languagePropertiesByIndex == null)
			{
				return;
			}
			LoadLanguageAudioClip(languagePropertiesByIndex);
			if (base.AudioClip == null && FabricManager.Instance._defaultLanguage >= 0)
			{
				languagePropertiesByIndex = FabricManager.Instance.GetLanguagePropertiesByIndex(FabricManager.Instance._defaultLanguage);
				if (languagePropertiesByIndex != null)
				{
					LoadLanguageAudioClip(languagePropertiesByIndex);
				}
			}
		}

		protected void LoadLanguageAudioClip(LanguageProperties language)
		{
			string str = language._languageFolder + "/" + _audioClipReference.Replace(".wav", "");
			str += language._languagePrefix;
			str = str.Replace("\\", "/");
			if (_dialogAudioLoadedFrom == DialogAudioLoadedFrom.Resources)
			{
				base.AudioClip = (Resources.Load(str) as AudioClip);
			}
		}

		private IEnumerator LoadPlayCoroutine(ComponentInstance zComponentInstance, float target, float curve, bool dontPlayComponents)
		{
			int languageIndex = FabricManager.Instance.GetLanguageIndex();
			if (languageIndex >= 0)
			{
				LanguageProperties language = FabricManager.Instance.GetLanguagePropertiesByIndex(languageIndex);
				if (language != null)
				{
					_loadPlayCoroutineActive = true;
					yield return StartCoroutine(FabricManager.Instance._customAudioClipLoader.LoadAudioClip(_audioClipReference, language));
					base.AudioClip = FabricManager.Instance._customAudioClipLoader._audioClip;
					base.PlayInternal(zComponentInstance, target, curve, dontPlayComponents);
					_loadPlayCoroutineActive = false;
				}
			}
		}

		protected bool UnLoad()
		{
			if (_dialogAudioLoadedFrom == DialogAudioLoadedFrom.Resources || _dialogAudioLoadedFrom == DialogAudioLoadedFrom.CustomAudioClipLoader)
			{
				Resources.UnloadAsset(base.AudioClip);
			}
			base.AudioClip = null;
			if (base.AudioSource != null)
			{
				base.AudioSource.clip = null;
			}
			return true;
		}

		protected override void OnInitialise(bool inPreviewMode = false)
		{
			AudioSource component = base.gameObject.GetComponent<AudioSource>();
			if ((bool)component)
			{
				Debug.LogWarning("Fabric: Adding an AudioSource and AudioComponent [" + base.name + "] in the same gameObject will impact performance, move AudioSource in a new gameObject underneath");
			}
			base.OnInitialise(inPreviewMode);
		}

		protected override void Reset()
		{
			_loadPlayCoroutineActive = false;
			base.Reset();
		}

		public override void PlayInternal(ComponentInstance zComponentInstance, float target, float curve, bool dontPlayComponents)
		{
			if (CheckMIDI(zComponentInstance))
			{
				if (_dialogAudioLoadedFrom == DialogAudioLoadedFrom.CustomAudioClipLoader && FabricManager.Instance._customAudioClipLoader != null)
				{
					StartCoroutine(LoadPlayCoroutine(zComponentInstance, target, curve, dontPlayComponents));
					return;
				}
				Load();
				base.PlayInternal(zComponentInstance, target, curve, dontPlayComponents);
			}
		}

		protected override void StopAudioComponent(bool notifyParent)
		{
			base.StopAudioComponent();
			UnLoad();
		}

		public override void StopInternal(bool stopInstances, bool forceStop, float target, float curve, double scheduleEnd)
		{
			base.StopInternal(stopInstances, forceStop, target, curve, scheduleEnd);
			if (forceStop)
			{
				UnLoad();
			}
		}

		public override void SetAudioClip(AudioClip audioClip, GameObject parentGameObject)
		{
			UnLoad();
			base.SetAudioClip(audioClip, parentGameObject);
		}

		public void SetAudioClipReference(string audioClipReference)
		{
			_audioClipReference = audioClipReference;
		}

		public override bool IsPlaying()
		{
			if (_loadPlayCoroutineActive || (base.AudioClip != null && base.AudioClip.loadState == AudioDataLoadState.Loading))
			{
				return true;
			}
			return base.IsPlaying();
		}

		public override bool IsComponentActive()
		{
			if (_loadPlayCoroutineActive || (base.AudioClip != null && base.AudioClip.loadState == AudioDataLoadState.Loading))
			{
				return true;
			}
			return base.IsComponentActive();
		}

		public override EventStatus OnProcessEvent(Event zEvent, ComponentInstance zInstance)
		{
			EventStatus result = EventStatus.Failed_Uknown;
			if (zEvent.EventAction == EventAction.SetAudioClipReference)
			{
				List<ComponentInstance> list = FindInstances(zEvent.parentGameObject, false);
				if (list != null && list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						ComponentInstance componentInstance = list[i];
						((DialogAudioComponent)componentInstance._instance).SetAudioClipReference((string)zEvent._parameter);
						result = EventStatus.Handled;
					}
				}
				else
				{
					SetAudioClipReference((string)zEvent._parameter);
					result = EventStatus.Handled;
				}
			}
			return result;
		}
	}
}
