using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fabric
{
	public abstract class AssetBundleAudioComponent : AudioComponent
	{
		private GameObject m_streamLoader;

		private AudioResourceLoadedFrom m_loadedFrom;

		[SerializeField]
		[HideInInspector]
		private string _audioClipReference;

		[SerializeField]
		[HideInInspector]
		private string _fallbackLocation;

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

		protected abstract AssetBundle AssetBundle
		{
			get;
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
			if (base.AudioClip == null)
			{
				string str = _audioClipReference.Replace(".wav", "") + "_SL";
				GameObject gameObject = null;
				AssetBundle assetBundle = AssetBundle;
				m_loadedFrom = AudioResourceLoadedFrom.None;
				bool flag = assetBundle != null;
				if (!gameObject)
				{
					string path = _fallbackLocation + str;
					gameObject = (GameObject)Resources.Load(path, typeof(GameObject));
					if (gameObject != null)
					{
						m_loadedFrom = AudioResourceLoadedFrom.Resources;
					}
				}
				if ((bool)gameObject)
				{
					m_streamLoader = Object.Instantiate(gameObject);
					AudioSource component = m_streamLoader.GetComponent<AudioSource>();
					base.AudioClip = component.clip;
				}
			}
			if (base.AudioSource != null)
			{
				base.AudioSource.clip = base.AudioClip;
			}
		}

		protected bool UnLoad()
		{
			bool result = false;
			if (m_loadedFrom == AudioResourceLoadedFrom.Resources && base.AudioClip != null)
			{
				AudioClip audioClip = base.AudioClip;
				base.AudioClip = null;
				result = true;
				Resources.UnloadAsset(audioClip);
			}
			if (m_streamLoader != null)
			{
				Object.Destroy(m_streamLoader);
			}
			m_streamLoader = null;
			if (m_loadedFrom != 0)
			{
				base.AudioClip = null;
			}
			return result;
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
			base.Reset();
		}

		public override void PlayInternal(ComponentInstance zComponentInstance, float target, float curve, bool dontPlayComponents)
		{
			Load();
			base.PlayInternal(zComponentInstance, target, curve, dontPlayComponents);
		}

		protected override void StopAudioComponent(bool notifyParent)
		{
			base.StopAudioComponent();
			UnLoad();
		}

		public override void StopInternal(bool stopInstances, bool forceStop, float target, float curve, double scheduleEnd = 0.0)
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
			m_loadedFrom = AudioResourceLoadedFrom.None;
		}
	}
}
