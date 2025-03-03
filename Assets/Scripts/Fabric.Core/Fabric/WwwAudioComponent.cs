using System.Collections;
using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Components/WwwAudioComponent")]
	public class WwwAudioComponent : AudioComponent
	{
		[SerializeField]
		[HideInInspector]
		public wwwFileLocation _fileLocation;

		[SerializeField]
		[HideInInspector]
		public AudioType _audioType = AudioType.WAV;

		[HideInInspector]
		public bool _is3D;

		[SerializeField]
		[HideInInspector]
		public bool _isStreaming = true;

		[SerializeField]
		[HideInInspector]
		public bool _languageSupported;

		[HideInInspector]
		[SerializeField]
		public bool _loadOnStart;

		[SerializeField]
		[HideInInspector]
		public bool _ignoreUnloadUnusedAssets;

		[HideInInspector]
		[SerializeField]
		private string _audioClipReference;

		private WWW www;

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

		protected override void OnInitialise(bool inPreviewMode = false)
		{
			AudioSource component = base.gameObject.GetComponent<AudioSource>();
			if ((bool)component)
			{
				Debug.LogWarning("Fabric: Adding an AudioSource and AudioComponent [" + base.name + "] in the same gameObject will impact performance, move AudioSource in a new gameObject underneath");
			}
			base.OnInitialise(inPreviewMode);
			if (_loadOnStart)
			{
				Load();
			}
		}

		private void Load()
		{
			string text = GetAudioClipReferenceFilename();
			if (_languageSupported)
			{
				int languageIndex = FabricManager.Instance.GetLanguageIndex();
				if (languageIndex >= 0)
				{
					LanguageProperties languagePropertiesByIndex = FabricManager.Instance.GetLanguagePropertiesByIndex(languageIndex);
					if (languagePropertiesByIndex != null)
					{
						text = text.Replace("LANGUAGE", languagePropertiesByIndex._languageFolder);
					}
				}
			}
			www = new WWW(text);
			base.AudioClip = www.GetAudioClip(_is3D, _isStreaming, _audioType);
		}

		private IEnumerator LoadPlayCoroutine(ComponentInstance zComponentInstance, float target, float curve, bool dontPlayComponents)
		{
			string filename = GetAudioClipReferenceFilename();
			if (_languageSupported)
			{
				int languageIndex = FabricManager.Instance.GetLanguageIndex();
				if (languageIndex >= 0)
				{
					LanguageProperties languagePropertiesByIndex = FabricManager.Instance.GetLanguagePropertiesByIndex(languageIndex);
					if (languagePropertiesByIndex != null)
					{
						filename = filename.Replace("LANGUAGE", languagePropertiesByIndex._languageFolder);
					}
				}
			}
			www = new WWW(filename);
			while (!www.isDone)
			{
				yield return new WaitForSeconds(0.1f);
			}
			base.AudioClip = www.GetAudioClip(_is3D, _isStreaming, _audioType);
			PlayInternalWait(zComponentInstance, target, curve, dontPlayComponents);
		}

		protected bool UnLoad()
		{
			base.AudioClip = null;
			if (base.AudioSource != null)
			{
				base.AudioSource.clip = null;
			}
			if (www != null)
			{
				www.Dispose();
			}
			if (!_ignoreUnloadUnusedAssets)
			{
				Resources.UnloadUnusedAssets();
			}
			return true;
		}

		private string GetAudioClipReferenceFilename()
		{
			string str = "";
			if (_fileLocation == wwwFileLocation.DataPath)
			{
				str = GetDataPath();
			}
			else if (_fileLocation == wwwFileLocation.PersistentDataPath)
			{
				str = GetPersistentPath();
			}
			else if (_fileLocation == wwwFileLocation.StreamingAssetsPath)
			{
				str = GetStreamingPath();
			}
			else if (_fileLocation == wwwFileLocation.Http)
			{
				return _audioClipReference;
			}
			return str + "//" + _audioClipReference;
		}

		private string GetPersistentPath()
		{
			return "file://" + Application.persistentDataPath;
		}

		private string GetStreamingPath()
		{
			return "file://" + Application.streamingAssetsPath;
		}

		private string GetDataPath()
		{
			return "file://" + Application.dataPath;
		}

		public override void PlayInternal(ComponentInstance zComponentInstance, float target, float curve, bool dontPlayComponents)
		{
			if (CheckMIDI(zComponentInstance))
			{
				if (!_loadOnStart)
				{
					SetComponentActive(true);
					_currentState = AudioComponentState.WaitingToPlay;
					StartCoroutine(LoadPlayCoroutine(zComponentInstance, target, curve, dontPlayComponents));
				}
				else
				{
					PlayInternalWait(zComponentInstance, target, curve, dontPlayComponents);
				}
			}
		}

		protected override void StopAudioComponent(bool notifyParent)
		{
			base.StopAudioComponent();
			if (!_loadOnStart)
			{
				UnLoad();
			}
		}

		public override void StopInternal(bool stopInstances, bool forceStop, float target, float curve, double scheduledEnd)
		{
			base.StopInternal(stopInstances, forceStop, target, curve, scheduledEnd);
			if (forceStop && !_loadOnStart)
			{
				UnLoad();
			}
		}

		public override void SetAudioClip(AudioClip audioClip, GameObject parentGameObject)
		{
			if (!_loadOnStart)
			{
				UnLoad();
			}
			base.SetAudioClip(audioClip, parentGameObject);
		}
	}
}
