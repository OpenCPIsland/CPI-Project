using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Components/AudioComponent")]
	public class AudioComponent : Component
	{
		protected AudioSource _audioSource;

		protected AudioSource[] _loopRegionAudioSources;

		protected int _loopIndex;

		private GameObject _audioSourceGameObject;

		protected AudioComponentState _currentState = AudioComponentState.Stopped;

		private DynamicLoadAsyncState _dynamicLoadAsyncState;

		[SerializeField]
		private AudioClip _audioClip;

		[SerializeField]
		[HideInInspector]
		public AudioClipHandle _audioClipHandle = new AudioClipHandle();

		[HideInInspector]
		[SerializeField]
		public bool _dynamicAudioClipLoading;

		[HideInInspector]
		[SerializeField]
		public float _dynamicAudioClipUnloadDelay;

		[SerializeField]
		[HideInInspector]
		public bool _dynamicAsyncAudioClipLoading;

		[HideInInspector]
		[SerializeField]
		public AudioClipAssetPath _audioClipAssetPath;

		[SerializeField]
		[HideInInspector]
		private double _delay;

		[HideInInspector]
		private double _delayRuntime;

		[HideInInspector]
		[SerializeField]
		private int _delayInBeats;

		[SerializeField]
		[HideInInspector]
		private bool _loop;

		[NonSerialized]
		private bool _playToEnd;

		[SerializeField]
		[HideInInspector]
		private bool _ignoreNativeLoop;

		[HideInInspector]
		[SerializeField]
		private bool _randomizeStart;

		[SerializeField]
		[Range(0f, 100f)]
		[HideInInspector]
		private float _randomizeStartPercentage = 100f;

		[SerializeField]
		[HideInInspector]
		public int _numLoops = -1;

		[NonSerialized]
		[HideInInspector]
		public int _numLoopsLeft;

		[HideInInspector]
		[SerializeField]
		public bool _loopMarkersLoaded;

		[SerializeField]
		[HideInInspector]
		public bool _useLoopMarkers = true;

		[HideInInspector]
		[SerializeField]
		public int _startLoopMarkerIndex;

		[HideInInspector]
		[SerializeField]
		public int _endLoopMarkerIndex = 1;

		[HideInInspector]
		[SerializeField]
		public int _loopRegionIndex;

		[HideInInspector]
		[SerializeField]
		public bool _randomizeStartLoopMarkerIndex;

		[SerializeField]
		[HideInInspector]
		public bool _randomizeEndLoopMarkerIndex;

		[HideInInspector]
		[SerializeField]
		public bool _randomizeRegionIndex;

		[HideInInspector]
		[SerializeField]
		private bool _dontPlay;

		[SerializeField]
		[HideInInspector]
		private bool _dontStopOnDestroy;

		[HideInInspector]
		[SerializeField]
		private bool _ignoreVirtualization;

		[HideInInspector]
		[SerializeField]
		public bool _randomizePosition;

		[SerializeField]
		[HideInInspector]
		public float _randomizeMinPosition;

		[HideInInspector]
		[SerializeField]
		public float _randomizeMaxPosition = 10f;

		[HideInInspector]
		[SerializeField]
		public bool _syncWithMusicTime;

		[NonSerialized]
		[HideInInspector]
		private MusicSyncType _musicSyncType = MusicSyncType.OnBar;

		[HideInInspector]
		[SerializeField]
		public List<Marker> _markers = new List<Marker>();

		[SerializeField]
		[HideInInspector]
		public bool _syncRegionsWithTempo;

		[HideInInspector]
		[SerializeField]
		public float _audioClipTempo = 120f;

		[NonSerialized]
		[HideInInspector]
		public Region _activeTempoRegion;

		[NonSerialized]
		[HideInInspector]
		public int _activeTempoRegionIndex;

		[NonSerialized]
		private int _lastMarkerIndex;

		[HideInInspector]
		[SerializeField]
		public List<Region> _regions = new List<Region>();

		[NonSerialized]
		[HideInInspector]
		private Region _loopRegion;

		private bool _hasAudioSource;

		private float _pauseAudioSouceTimeInSecs;

		private double _pauseTimeInSec;

		private Vector3 _cachedPosition;

		protected double _dspTimeTriggered;

		private float _updateContextPitch = 1f;

		private float _audioClipLength;

		private bool _triggerPlayingHandlerOnce = true;

		protected bool _disableTriggerPlayingHandlerOnce;

		internal List<DSPComponent> _audioSourcePoolDSPComponents;

		public GameObject AudioSourceGameObject
		{
			get
			{
				return _audioSourceGameObject;
			}
		}

		public AudioComponentState CurrentState
		{
			get
			{
				return _currentState;
			}
		}

		public DynamicLoadAsyncState LoadAsyncState
		{
			get
			{
				return _dynamicLoadAsyncState;
			}
		}

		public AudioClip AudioClip
		{
			get
			{
				return _audioClip;
			}
			set
			{
				_audioClip = value;
			}
		}

		public double Delay
		{
			get
			{
				return _delay;
			}
			set
			{
				_delay = value;
			}
		}

		public bool Loop
		{
			get
			{
				return _loop;
			}
			set
			{
				_loop = value;
			}
		}

		public bool DontPlay
		{
			get
			{
				return _dontPlay;
			}
			set
			{
				_dontPlay = value;
			}
		}

		public bool DontStopOnDestroy
		{
			get
			{
				return _dontStopOnDestroy;
			}
			set
			{
				_dontStopOnDestroy = value;
			}
		}

		public AudioSource AudioSource
		{
			get
			{
				return _audioSource;
			}
			set
			{
				_audioSource = value;
			}
		}

		public AudioSource[] LoopRegions
		{
			get
			{
				return _loopRegionAudioSources;
			}
		}

		public int LoopIndex
		{
			get
			{
				return _loopIndex;
			}
			set
			{
				_loopIndex = value;
			}
		}

		public double DSPTimeTriggered
		{
			get
			{
				return _dspTimeTriggered;
			}
		}

		internal void AddDSPComponent(DSPComponent dspComponent)
		{
			if (_audioSourcePoolDSPComponents == null)
			{
				_audioSourcePoolDSPComponents = new List<DSPComponent>();
			}
			_audioSourcePoolDSPComponents.Add(dspComponent);
		}

		private UnityEngine.Component GetHeavyPlugin(GameObject go)
		{
			UnityEngine.Component[] components = go.GetComponents<UnityEngine.Component>();
			for (int i = 0; i < components.Length; i++)
			{
				if (components[i].GetType().FullName == "Heavy.Plugin")
				{
					return components[i];
				}
			}
			return null;
		}

		private void RemoveDSPInstances()
		{
			if (_audioSourcePoolDSPComponents == null || _audioSourcePoolDSPComponents.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < _audioSourcePoolDSPComponents.Count; i++)
			{
				DSPComponent dSPComponent = _audioSourcePoolDSPComponents[i];
				UnityEngine.Component component = _audioSource.gameObject.GetComponent(dSPComponent.Name);
				if (component == null)
				{
					component = GetHeavyPlugin(_audioSource.gameObject);
				}
				if (component != null)
				{
					dSPComponent.RemoveDSPInstance(component);
					UnityEngine.Object.DestroyImmediate(component);
				}
			}
		}

		private void SetupLoopRegionAudioSources()
		{
			if (_loopRegionAudioSources != null && _loopRegionAudioSources.Length >= 1)
			{
				if (_hasAudioSource)
				{
					_loopRegionAudioSources[0] = _audioSource;
					_loopRegionAudioSources[1] = UnityEngine.Object.Instantiate(_audioSource);
					_loopRegionAudioSources[1].name = _loopRegionAudioSources[1].name.Replace("_1", "_2");
					_loopRegionAudioSources[1].name = _loopRegionAudioSources[1].name.Replace("(Clone)", "");
					_loopRegionAudioSources[1].transform.parent = base.gameObject.transform;
					_loopRegionAudioSources[1].gameObject.hideFlags = HideFlags.DontSave;
				}
				else
				{
					_loopRegionAudioSources[0] = _audioSource;
					_loopRegionAudioSources[1] = FabricManager.Instance.AudioSourcePoolManager.Alloc(this);
				}
				_loopRegionAudioSources[1].loop = (_numLoops <= 0 && !_ignoreNativeLoop && _loop);
				_loopRegionAudioSources[1].playOnAwake = false;
				_loopRegionAudioSources[1].timeSamples = 0;
				_loopRegionAudioSources[1].time = 0f;
				_loopRegionAudioSources[1].clip = AudioClip;
			}
		}

		protected override void OnInitialise(bool inPreviewMode = false)
		{
			InitializeAudioComponent(inPreviewMode);
		}

		public void InitializeAudioComponent(bool inPreviewMode = false, AudioSource audioSource = null)
		{
			bool flag = true;
			_audioSource = base.gameObject.GetComponent<AudioSource>();
			if ((bool)_audioSource)
			{
				Debug.LogWarning("Fabric: Adding an AudioSource and AudioComponent [" + base.name + "] in the same gameObject will impact performance, move AudioSource in a new gameObject underneath");
				_hasAudioSource = true;
			}
			else
			{
				bool flag2 = false;
				if (audioSource != null)
				{
					_audioSource = audioSource;
					_audioSourceGameObject = audioSource.gameObject;
					flag2 = true;
				}
				else
				{
					int childCount = base.transform.childCount;
					for (int i = 0; i < childCount; i++)
					{
						GameObject gameObject = base.transform.GetChild(i).gameObject;
						AudioSource component = gameObject.GetComponent<AudioSource>();
						if (component != null)
						{
							_audioSourceGameObject = gameObject;
							_audioSource = component;
							flag2 = true;
							break;
						}
					}
				}
				if (FabricManager.Instance.AudioSourcePoolManager == null)
				{
					flag = false;
				}
				if (!flag2 && !flag)
				{
					_audioSourceGameObject = new GameObject("AudioSource_1");
					_audioSourceGameObject.hideFlags = HideFlags.DontSave;
					_audioSource = _audioSourceGameObject.AddComponent<AudioSource>();
				}
				if (_audioSource != null)
				{
					_audioSource.clip = null;
					_audioSource.playOnAwake = false;
					_audioSource.loop = (_numLoops <= 0 && !_ignoreNativeLoop && _loop);
					Generic.SetGameObjectActive(_audioSourceGameObject, false);
					if (_loopRegionAudioSources != null && _loopRegionAudioSources.Length > 0 && _loopRegionAudioSources[1] != null)
					{
						Generic.SetGameObjectActive(_loopRegionAudioSources[1].gameObject, false);
					}
					if (inPreviewMode)
					{
						FabricManager.AddPreviewGameObject(_audioSourceGameObject);
					}
					else
					{
						_audioSourceGameObject.transform.parent = base.gameObject.transform;
					}
					_hasAudioSource = true;
				}
			}
			if (_loop && ((_markers.Count > 1 && _useLoopMarkers) || _ignoreNativeLoop || _numLoops > 0))
			{
				SetupLoopRegion();
				_loopIndex = 1;
			}
			if (_syncRegionsWithTempo && _regions.Count > 0)
			{
				_loopRegionAudioSources = new AudioSource[2];
			}
			if (_hasAudioSource || !flag)
			{
				SetupLoopRegionAudioSources();
			}
			InitialiseAudioBus();
			InitialiseCustomCurves();
			InitialiseDSPWrappers();
			_name = base.name;
			if (_dynamicAudioClipLoading && !base.IsInstance && !_audioClipHandle.IsAudioClipPathSet())
			{
				_audioClipHandle.AudioClip = _audioClip;
			}
			_currentState = AudioComponentState.Stopped;
		}

		private void Start()
		{
		}

		private void SetupLoopRegion()
		{
			if (_loopRegion == null)
			{
				_loopRegionAudioSources = new AudioSource[2];
				_loopRegion = new Region();
				_loopRegion.name = "LoopRegion";
			}
			if (_markers.Capacity > 1 && _useLoopMarkers)
			{
				if (_randomizeStartLoopMarkerIndex)
				{
					_startLoopMarkerIndex = base._random.Next(0, _endLoopMarkerIndex);
				}
				if (_randomizeEndLoopMarkerIndex)
				{
					_endLoopMarkerIndex = base._random.Next(_startLoopMarkerIndex, _markers.Count);
				}
				_loopRegion.offsetTime = _markers[_startLoopMarkerIndex].offsetTime;
				_loopRegion.lengthTime = _markers[_endLoopMarkerIndex].offsetTime;
			}
			else if (_regions.Capacity > 0)
			{
				if (_randomizeRegionIndex)
				{
					_loopRegionIndex = base._random.Next(0, _regions.Count);
				}
				_loopRegion.offsetTime = _regions[_loopRegionIndex].offsetTime;
				_loopRegion.lengthTime = _regions[_loopRegionIndex].offsetTime + _regions[0].lengthTime;
			}
			else
			{
				_loopRegion.offsetTime = 0f;
				_loopRegion.lengthTime = _audioClip.length;
			}
		}

		private void OnDestroy()
		{
			Destroy();
		}

		public override void Destroy()
		{
			if (!_quitting)
			{
				StopAudioComponent(false);
				_audioClip = null;
				base.Destroy();
			}
		}

		public override void SetMusicTimeSettings(MusicTimeSittings musicTimeSettings, MusicSyncType musicSyncType)
		{
			if (!_overrideMusicTimeSettings)
			{
				_activeMusicTimeSettings = musicTimeSettings;
			}
			_musicSyncType = musicSyncType;
			_syncWithMusicTime = false;
		}

		protected override void Reset()
		{
			base.Reset();
			_numLoopsLeft = _numLoops;
			_lastMarkerIndex = 0;
			_delayRuntime = 0.0;
			_currentState = AudioComponentState.Stopped;
			_audioClipLength = 0f;
			_updateContextPitch = 1f;
			_playToEnd = false;
			_activeTempoRegionIndex = 0;
		}

		protected override void InitialiseMusicTimingSettings()
		{
			if (_syncWithMusicTime)
			{
				_activeMusicTimeSettings = FabricManager.Instance.GetMusicSettingByIndex(_musicTimeSettingsIndex + 1);
			}
			else
			{
				base.InitialiseMusicTimingSettings();
			}
		}

		private IEnumerator DynamicAudioClipLoad(AudioClipHandle audioClipHandle)
		{
			_dynamicLoadAsyncState = DynamicLoadAsyncState.Loading;
			if (audioClipHandle.RefCount == 0)
			{
				string path = "";
				if (_audioClipAssetPath == AudioClipAssetPath.DataPath)
				{
					path = Application.dataPath;
				}
				else if (_audioClipAssetPath == AudioClipAssetPath.StreamingAssetsPath)
				{
					path = Application.streamingAssetsPath;
				}
				else if (_audioClipAssetPath == AudioClipAssetPath.PersistentDataPath)
				{
					path = Application.persistentDataPath;
				}
				WWW www = new WWW("file://" + path + "//" + audioClipHandle.GetAudioClipPath());
				while (!www.isDone)
				{
					yield return new WaitForSeconds(0.1f);
				}
				audioClipHandle.AudioClip = www.GetAudioClip(Convert.ToBoolean(www));
			}
			_dynamicLoadAsyncState = DynamicLoadAsyncState.Loaded;
			audioClipHandle.IncRef(false);
			_audioClip = audioClipHandle.AudioClip;
		}

		private IEnumerator DynamicAudioClipLoadAndPlay(AudioClipHandle audioClipHandle, ComponentInstance zComponentInstance, float target, float curve, bool dontPlayComponents)
		{
			_dynamicLoadAsyncState = DynamicLoadAsyncState.Loading;
			if (audioClipHandle.RefCount == 0)
			{
				string path = "";
				if (_audioClipAssetPath == AudioClipAssetPath.DataPath)
				{
					path = Application.dataPath;
				}
				else if (_audioClipAssetPath == AudioClipAssetPath.StreamingAssetsPath)
				{
					path = Application.streamingAssetsPath;
				}
				else if (_audioClipAssetPath == AudioClipAssetPath.PersistentDataPath)
				{
					path = Application.persistentDataPath;
				}
				WWW www = new WWW("file://" + path + "//" + audioClipHandle.GetAudioClipPath());
				while (!www.isDone)
				{
					yield return new WaitForSeconds(0.1f);
				}
				audioClipHandle.AudioClip = www.GetAudioClip(Convert.ToBoolean(www));
			}
			_dynamicLoadAsyncState = DynamicLoadAsyncState.Loaded;
			audioClipHandle.IncRef(false);
			_audioClip = audioClipHandle.AudioClip;
			PlayInternalWait(zComponentInstance, target, curve, dontPlayComponents);
		}

		public override void PlayInternal(ComponentInstance zComponentInstance, float target, float curve, bool dontPlayComponents)
		{
			if ((zComponentInstance._instance.UpdateContext._audioBus != null && !zComponentInstance._instance.UpdateContext._audioBus.IncrementAudioComponent()) || !CheckMIDI(zComponentInstance))
			{
				return;
			}
			if (_dynamicAudioClipLoading)
			{
				AudioComponent audioComponent = base.ComponentHolder as AudioComponent;
				if ((bool)audioComponent)
				{
					if (_dynamicAsyncAudioClipLoading)
					{
						StartCoroutine(DynamicAudioClipLoadAndPlay(audioComponent._audioClipHandle, zComponentInstance, target, curve, dontPlayComponents));
						return;
					}
					_dynamicLoadAsyncState = DynamicLoadAsyncState.Loading;
					_audioClip = audioComponent._audioClipHandle.IncRef();
					if (_loopRegionAudioSources != null)
					{
						_loopRegionAudioSources[1].clip = AudioClip;
					}
					_dynamicLoadAsyncState = DynamicLoadAsyncState.Loaded;
				}
			}
			if (_audioClip == null)
			{
				DebugLog.Print("AudioComponent [ " + base.name + " ] doesn't have a valid audio clip so it will be treated as a source");
			}
			_componentInstance = zComponentInstance;
			if (_randomizePosition && _componentInstance != null)
			{
				_componentInstance._randomisePosition = RandomisePosition(_randomizeMinPosition, _randomizeMaxPosition);
			}
			UpdatePosition();
			if (_dontPlay || CheckIsVirtual())
			{
				return;
			}
			base.PlayInternal(zComponentInstance, _fadeInTime, _fadeInCurve, dontPlayComponents);
			if (_audioSource == null)
			{
				_audioSource = FabricManager.Instance.AudioSourcePoolManager.Alloc(this);
				SetupLoopRegionAudioSources();
				if (_audioSource == null)
				{
					DebugLog.Print("AudioComponent [ " + base.name + " ] failed to create an AudioSource", DebugLevel.Error);
					return;
				}
				_audioSourceGameObject = _audioSource.gameObject;
				if (_audioSourcePoolDSPComponents != null && _audioSourcePoolDSPComponents.Count > 0)
				{
					for (int i = 0; i < _audioSourcePoolDSPComponents.Count; i++)
					{
						_audioSourcePoolDSPComponents[i].AddDSPInstance(_audioSourcePoolDSPComponents[i].CreateComponent(_audioSourceGameObject));
					}
				}
			}
			UpdateParentProperties();
			_audioSource.clip = _audioClip;
			_audioSource.playOnAwake = false;
			_audioSource.loop = (_numLoops <= 0 && !_ignoreNativeLoop && _loop);
			_audioSource.timeSamples = 0;
			_audioSource.time = 0f;
			if (_randomizeStart)
			{
				_audioSource.time = UnityEngine.Random.Range(0f, _audioClip.length * (_randomizeStartPercentage / 100f));
			}
			else if (_randomizeStartLoopMarkerIndex && !_useLoopMarkers)
			{
				_startLoopMarkerIndex = base._random.Next(0, _markers.Count);
				_audioSource.time = _markers[_startLoopMarkerIndex].offsetTime;
			}
			_triggerPlayingHandlerOnce = true;
			if (_componentInstance != null && _componentInstance._parentGameObject != null)
			{
				_cachedPosition = _componentInstance._parentGameObject.transform.position;
			}
			SetComponentActive(true);
			if (_audioSourceGameObject != null)
			{
				Generic.SetGameObjectActive(_audioSourceGameObject, true);
				if (_loopRegionAudioSources != null && _loopRegionAudioSources.Length > 0 && _loopRegionAudioSources[1] != null)
				{
					Generic.SetGameObjectActive(_loopRegionAudioSources[1].gameObject, true);
				}
			}
			if (_componentInstance != null)
			{
				double playScheduled = 0.0;
				double playScheduledDelay = 0.0;
				_componentInstance._instance.GetPlayScheduled(ref playScheduled, ref playScheduledDelay);
				base.PlayScheduled = playScheduled;
				base.PlayScheduledDelay = playScheduledDelay;
			}
			double num = 0.0;
			if (base.PlayScheduled > 0.0)
			{
				num = base.PlayScheduled;
			}
			if (_syncWithMusicTime && _activeMusicTimeSettings != null)
			{
				_delayRuntime = _activeMusicTimeSettings.GetDelay(this, _delayInBeats) + _delay;
				double playScheduledTime = 0.0;
				GetPlayScheduleTime(ref playScheduledTime);
				SetTime((float)playScheduledTime);
			}
			else
			{
				_delayRuntime = _delay;
				double playScheduledTime2 = 0.0;
				_componentInstance._instance.GetPlayScheduleTime(ref playScheduledTime2);
				if (playScheduledTime2 > 0.0)
				{
					SetTime((float)playScheduledTime2);
				}
			}
			if (_componentInstance._instance._scheduledDspTime == 0.0)
			{
				_componentInstance._instance._scheduledDspTime = AudioSettings.dspTime;
			}
			_dspTimeTriggered = _componentInstance._instance._scheduledDspTime + num + _delayRuntime;
			_audioClipLength = 0f;
			_updateContextPitch = 1f;
			if (_syncRegionsWithTempo && _regions.Count > 0)
			{
				_activeTempoRegion = _regions[_activeTempoRegionIndex];
			}
			_audioSource.PlayScheduled(_dspTimeTriggered);
			_currentState = AudioComponentState.Playing;
			if (HasValidEventNotifier())
			{
				NotifyEvent(EventNotificationType.OnAudioComponentPlay, this);
			}
		}

		protected void PlayInternalWait(ComponentInstance zComponentInstance, float target, float curve, bool dontPlayComponents)
		{
			if (_dontPlay)
			{
				return;
			}
			base.PlayInternal(zComponentInstance, _fadeInTime, _fadeInCurve, dontPlayComponents);
			Reset();
			_componentInstance = zComponentInstance;
			if (_componentInstance != null)
			{
				double playScheduled = 0.0;
				double playScheduledDelay = 0.0;
				_componentInstance._instance.GetPlayScheduled(ref playScheduled, ref playScheduledDelay);
				base.PlayScheduled = playScheduled;
				base.PlayScheduledDelay = playScheduledDelay;
			}
			if (_audioSource == null)
			{
				_audioSource = FabricManager.Instance.AudioSourcePoolManager.Alloc(this);
				SetupLoopRegionAudioSources();
				if (_audioSource == null)
				{
					DebugLog.Print("AudioComponent [ " + base.name + " ] failed to create an AudioSource", DebugLevel.Error);
					return;
				}
				if (_audioSourcePoolDSPComponents != null && _audioSourcePoolDSPComponents.Count > 0)
				{
					for (int i = 0; i < _audioSourcePoolDSPComponents.Count; i++)
					{
						_audioSourcePoolDSPComponents[i].AddDSPInstance(_audioSourcePoolDSPComponents[i].CreateComponent(_audioSourceGameObject));
					}
				}
			}
			if (_audioClip == null)
			{
				DebugLog.Print("AudioComponent [ " + base.name + " ] doesn't have a valid audio clip so it will be treated as a source");
			}
			_audioSource.clip = _audioClip;
			_audioSource.playOnAwake = false;
			_audioSource.loop = (_numLoops <= 0 && !_ignoreNativeLoop && _loop);
			_audioSource.timeSamples = 0;
			_audioSource.time = 0f;
			_dspTimeTriggered = AudioSettings.dspTime;
			_triggerPlayingHandlerOnce = true;
			if (_componentInstance != null && _componentInstance._parentGameObject != null)
			{
				_cachedPosition = _componentInstance._parentGameObject.transform.position;
			}
			SetComponentActive(true);
			_currentState = AudioComponentState.WaitingToPlay;
		}

		public override bool IsPlaying()
		{
			_fadeParameter.Get(FabricTimer.Get());
			if (_fadeParameter.HasReachedTarget() && _currentState == AudioComponentState.Stopped)
			{
				return false;
			}
			return _isComponentActive;
		}

		public override bool IsComponentActive()
		{
			_fadeParameter.Get(FabricTimer.Get());
			if ((!_fadeParameter.HasReachedTarget() && _currentState != AudioComponentState.Stopped) || IsVirtualizationActive())
			{
				return true;
			}
			return _isComponentActive;
		}

		public override bool IsOneShot()
		{
			if (!_loop)
			{
				return true;
			}
			return false;
		}

		public override bool IsLooped()
		{
			if (!_loop)
			{
				return false;
			}
			return true;
		}

		public override int GetTimeSamples()
		{
			if (_audioClip != null && _audioSource != null)
			{
				return _audioClip.samples - _audioSource.timeSamples;
			}
			return -1;
		}

		public override double GetTime()
		{
			if (_audioClip != null)
			{
				return _dspTimeTriggered + (double)_audioClip.length;
			}
			return 0.0;
		}

		public override double GetCurrentTime(bool returnFirst)
		{
			if (_audioSource != null)
			{
				return _audioSource.time;
			}
			return 0.0;
		}

		public override float GetLength()
		{
			if (_audioClip != null)
			{
				return _audioClip.length;
			}
			return 0f;
		}

		public override int GetSampleRate()
		{
			if (_audioClip != null)
			{
				return _audioClip.frequency;
			}
			return -1;
		}

		public override bool HasReachedEnd()
		{
			if (_audioSource != null && _audioClip != null && _audioSource.isPlaying)
			{
				double num = _audioClip.length;
				if (base.PlayScheduledDelay < 0.0)
				{
					num += base.PlayScheduledDelay;
				}
				if ((double)_audioSource.time >= num)
				{
					return true;
				}
			}
			return false;
		}

		private bool CheckIsVirtual()
		{
			if (!_ignoreVirtualization && FabricManager.Instance._enableVirtualization && _audioSource != null)
			{
				float num = 0f;
				if (FabricManager.Instance._audioListener != null)
				{
					num = Vector3.Distance(_cachedPosition, FabricManager.Instance._audioListener.transform.position);
				}
				else
				{
					if (!(Camera.main != null))
					{
						return false;
					}
					num = Vector3.Distance(_cachedPosition, Camera.main.transform.position);
				}
				if (num > _audioSource.maxDistance || (double)_updateContext._volume <= FabricManager.Instance._volumeThreshold)
				{
					return true;
				}
			}
			return false;
		}

		private void UpdateVirtual(bool isVirtual)
		{
			if (isVirtual && (bool)_audioSource)
			{
				if (_currentState == AudioComponentState.Virtual)
				{
					return;
				}
				_pauseAudioSouceTimeInSecs = _audioSource.time;
				_audioSource.volume = 0f;
				_audioSource.Stop();
				if (_loop && _loopRegionAudioSources != null && (_loopMarkersLoaded || _ignoreNativeLoop || _numLoops > 0))
				{
					_loopRegionAudioSources[1].Stop();
				}
				if (_audioSourceGameObject != null)
				{
					Generic.SetGameObjectActive(_audioSourceGameObject, false);
					if (_loopRegionAudioSources != null && _loopRegionAudioSources[1] != null)
					{
						Generic.SetGameObjectActive(_loopRegionAudioSources[1].gameObject, false);
					}
				}
				_currentState = AudioComponentState.Virtual;
			}
			else
			{
				if (_currentState != AudioComponentState.Virtual)
				{
					return;
				}
				if (_audioSourceGameObject != null)
				{
					Generic.SetGameObjectActive(_audioSourceGameObject, true);
					if (_loopRegionAudioSources != null && _loopRegionAudioSources[1] != null)
					{
						Generic.SetGameObjectActive(_loopRegionAudioSources[1].gameObject, true);
					}
				}
				_audioSource.time = Math.Min(_pauseAudioSouceTimeInSecs, AudioClip.length);
				_pauseAudioSouceTimeInSecs = 0f;
				_audioSource.Play();
				_currentState = AudioComponentState.Playing;
			}
		}

		public override bool UpdateInternal(ref Context context)
		{
			profiler.Begin();
			if (_componentInstance != null && _componentInstance._parentGameObject == null && !_dontStopOnDestroy && _currentState == AudioComponentState.Playing)
			{
				if (AudioClip == null)
				{
					WwwAudioComponent x = this as WwwAudioComponent;
					if (x != null)
					{
						return true;
					}
				}
				Stop();
			}
			UpdateProperties(context);
			UpdateVirtualization2(_updateContext);
			bool flag = CheckIsVirtual();
			bool flag2 = false;
			switch (_currentState)
			{
			case AudioComponentState.WaitingToPlay:
				if (!(AudioClip != null) || AudioClip.loadState != AudioDataLoadState.Loaded)
				{
					break;
				}
				UpdatePosition();
				if (flag)
				{
					break;
				}
				if (_audioSourceGameObject != null)
				{
					Generic.SetGameObjectActive(_audioSourceGameObject, true);
					if (_loopRegionAudioSources != null && _loopRegionAudioSources[1] != null)
					{
						Generic.SetGameObjectActive(_loopRegionAudioSources[1].gameObject, true);
					}
				}
				if (base.PlayScheduled > 0.0)
				{
					double playScheduled = base.PlayScheduled;
				}
				if (_syncWithMusicTime && _activeMusicTimeSettings != null)
				{
					_delayRuntime = _activeMusicTimeSettings.GetDelay(_delayInBeats) + _delay;
				}
				else
				{
					_delayRuntime = _delay;
				}
				_dspTimeTriggered = AudioSettings.dspTime + _delayRuntime;
				_audioSource.PlayScheduled(_dspTimeTriggered);
				_currentState = AudioComponentState.Playing;
				break;
			case AudioComponentState.Playing:
				if (_audioSource == null)
				{
					StopAudioComponent();
					break;
				}
				if (AudioClip != null)
				{
					if (_activeMusicTimeSettings != null && _musicSyncType != MusicSyncType.OnEnd)
					{
						double offset = 0.0;
						if (_activeMusicTimeSettings.CheckIfNextEventIsWithinRange(_musicSyncType, ref offset))
						{
							OnFinishPlaying(offset);
						}
					}
					else
					{
						if (_loop && (double)_audioSource.time <= 0.1)
						{
							_triggerPlayingHandlerOnce = true;
						}
						if (_audioClipLength == 0f)
						{
							_audioClipLength = _audioClip.length;
						}
						if (_updateContext._pitch != _updateContextPitch)
						{
							float num = _audioClip.length - _audioSource.time;
							_audioClipLength = num / _updateContext._pitch;
							_updateContextPitch = _updateContext._pitch;
							_dspTimeTriggered = AudioSettings.dspTime;
						}
						double num2 = _dspTimeTriggered + (double)_audioClipLength;
						if (base.PlayScheduledDelay < 0.0)
						{
							num2 += base.PlayScheduledDelay;
						}
						double num3 = FabricManager.Instance._transitionThreshold;
						if ((double)_audioClipLength < num3)
						{
							num3 = 0.0;
						}
						double num4 = AudioSettings.dspTime + num3;
						if (num4 > num2 && _triggerPlayingHandlerOnce)
						{
							double time = num2 - AudioSettings.dspTime;
							OnFinishPlaying(time);
							_triggerPlayingHandlerOnce = false;
						}
					}
				}
				if (_syncRegionsWithTempo && _regions.Count > 0 && _activeMusicTimeSettings != null)
				{
					bool flag3 = false;
					if (_activeTempoRegionIndex >= _regions.Count)
					{
						if (_loop)
						{
							_activeTempoRegionIndex = 0;
							_dspTimeTriggered = AudioSettings.dspTime;
						}
						else
						{
							flag3 = true;
						}
					}
					if (!flag3)
					{
						flag2 = true;
						float num5 = _audioClipTempo / _activeMusicTimeSettings._bpm;
						double num6 = AudioSettings.dspTime - _dspTimeTriggered;
						int num7 = _activeTempoRegionIndex + 1;
						if (num7 > _regions.Count - 1)
						{
							num7 = 0;
						}
						Region region = _regions[num7];
						if (region != null && num6 + 0.05000000074505806 > (double)(region.offsetTime * num5))
						{
							double num8 = (double)region.lengthTime - num6;
							_audioSource.SetScheduledEndTime(AudioSettings.dspTime + num8);
							_audioSource = _loopRegionAudioSources[_loopIndex++];
							_audioSource.time = region.offsetTime;
							_audioSource.PlayScheduled(AudioSettings.dspTime + num8);
							if (_loopIndex >= _loopRegionAudioSources.Length)
							{
								_loopIndex = 0;
							}
							_activeTempoRegion = _regions[_activeTempoRegionIndex++];
						}
						if (_activeTempoRegion != null && _audioSource.isPlaying && num6 > (double)((_activeTempoRegion.offsetTime + _activeTempoRegion.lengthTime) * num5))
						{
							_audioSource.Stop();
						}
					}
					else
					{
						StopAudioComponent();
					}
				}
				if (_loop && _loopRegion != null && (_loopMarkersLoaded || _ignoreNativeLoop || _numLoops > 0) && AudioClip != null)
				{
					float lengthTime = _loopRegion.lengthTime;
					if (_audioSource.time + 0.1f > lengthTime && (_numLoopsLeft > 0 || _numLoopsLeft == -1))
					{
						double num9 = lengthTime - _audioSource.time;
						_audioSource.SetScheduledEndTime(AudioSettings.dspTime + num9);
						_audioSource = _loopRegionAudioSources[_loopIndex++];
						_audioSource.time = Mathf.Min(_loopRegion.offsetTime, AudioClip.length - 0.01f);
						_audioSource.PlayScheduled(AudioSettings.dspTime + num9);
						if (_loopIndex >= _loopRegionAudioSources.Length)
						{
							_loopIndex = 0;
						}
						if (_numLoopsLeft > 0)
						{
							_numLoopsLeft--;
						}
						SetupLoopRegion();
						_lastMarkerIndex = 0;
						if (_playToEnd)
						{
							StopAudioComponent();
						}
					}
				}
				if (_markers != null && _markers.Count > 0)
				{
					double num10 = _audioSource.time + 0.1f;
					if (num10 < (double)_markers[0].offsetTime && _lastMarkerIndex > 0)
					{
						_lastMarkerIndex = 0;
					}
					for (int i = 0; i < _markers.Count; i++)
					{
						Marker marker = _markers[i];
						if (!(num10 > (double)marker.offsetTime) || i < _lastMarkerIndex || marker.type == MarkerType.Ignore)
						{
							continue;
						}
						double num11 = num10 - (double)marker.offsetTime;
						if (OnMarker(num11))
						{
							if (marker.type == MarkerType.NotifyAndPlayToEnd)
							{
								_playToEnd = true;
							}
							else if (marker.type == MarkerType.NotifyAndStop)
							{
								StopInternal(false, false, 0f, 0.5f, num11);
							}
						}
						if (HasValidEventNotifier())
						{
							MarkerNotficationData markerNotficationData = new MarkerNotficationData();
							markerNotficationData._offset = num11;
							markerNotficationData._label = marker.name;
							NotifyEvent(EventNotificationType.OnMarker, markerNotficationData);
						}
						_lastMarkerIndex = i + 1;
						break;
					}
				}
				if (!flag2 && !_audioSource.isPlaying && (_audioClip == null || _audioClip.loadState != AudioDataLoadState.Loading))
				{
					StopAudioComponent();
				}
				break;
			case AudioComponentState.WaitingToStop:
				if (_updateContext._fadeParameter == 0f)
				{
					StopAudioComponent();
				}
				break;
			case AudioComponentState.ScheduledToStop:
				if (_updateContext._fadeParameter == 0f || !_audioSource.isPlaying)
				{
					StopAudioComponent();
				}
				break;
			}
			UpdateVirtual(flag);
			_isComponentActive = false;
			if (_componentInstances != null)
			{
				for (int j = 0; j < _componentInstances.Length; j++)
				{
					ComponentInstance componentInstance = _componentInstances[j];
					if (componentInstance._instance.IsInstance && componentInstance._instance.IsComponentActive())
					{
						_isComponentActive = true;
						componentInstance._instance.UpdateInternal(ref context);
					}
				}
			}
			if (_currentState != AudioComponentState.Stopped || IsVirtualizationActive())
			{
				bool isComponentActive = _isComponentActive;
				_isComponentActive = true;
			}
			_updatePropertiesFlag = false;
			profiler.End();
			return _isComponentActive;
		}

		internal override bool UpdateProperties(Context context)
		{
			if (_audioSource == null)
			{
				return false;
			}
			base.UpdateProperties(context);
			if (_overrideParentVolume || _parentComponent == null)
			{
				_updateContext._volume = _volume;
			}
			else
			{
				_updateContext._volume = context._volume * _volume;
			}
			_updateContext._volume *= _volumeOffset;
			_updateContext._volume *= _sideChainGain;
			_updateContext._volume *= _mixerVolume;
			_updateContext._volume *= _runtimeProperties._volume;
			_updateContext._volume *= _rtpProperties._volume;
			if (_updateContext._audioBus != null && _updateContext._audioBus._audioMixerGroup == null)
			{
				_updateContext._volume *= _updateContext._audioBus._volume;
			}
			if (_overrideFadeProperties || _parentComponent == null)
			{
				_updateContext._fadeParameter = _fadeParameter.Get(FabricTimer.Get());
			}
			else
			{
				_updateContext._fadeParameter = context._fadeParameter * _fadeParameter.Get(FabricTimer.Get());
			}
			if (!IsVirtualizationActive())
			{
				_updateContext._volume *= _updateContext._fadeParameter;
			}
			if (_initialiseParameters != null && _initialiseParameters._isMutliplier && _initialiseParameters._volume.IsDirty)
			{
				_updateContext._volume *= _initialiseParameters._volume.Value;
			}
			if (_audioSource.volume != _updateContext._volume)
			{
				_audioSource.volume = _updateContext._volume;
			}
			if (_overrideParentPitch || _parentComponent == null)
			{
				_updateContext._pitch = _pitch;
			}
			else
			{
				_updateContext._pitch = context._pitch * _pitch;
			}
			_updateContext._pitch *= _pitchOffset;
			_updateContext._pitch *= _mixerPitch;
			_updateContext._pitch *= _runtimeProperties._pitch;
			_updateContext._pitch *= _rtpProperties._pitch;
			if (_updateContext._audioBus != null && _updateContext._audioBus._audioMixerGroup == null)
			{
				_updateContext._pitch *= _updateContext._audioBus._pitch;
			}
			if (_initialiseParameters != null && _initialiseParameters._isMutliplier && _initialiseParameters._pitch.IsDirty)
			{
				_updateContext._pitch *= _initialiseParameters._pitch.Value;
			}
			if (_audioSource.pitch != _updateContext._pitch)
			{
				_audioSource.pitch = _updateContext._pitch;
			}
			if (_override2DProperties || _parentComponent == null)
			{
				_updateContext._pan2D = _pan2D;
			}
			else
			{
				_updateContext._pan2D = context._pan2D;
			}
			_updateContext._pan2D += _pan2DOffset;
			_updateContext._pan2D += _runtimeProperties._pan2D;
			if (_audioSource.panStereo != _updateContext._pan2D)
			{
				_audioSource.panStereo = _updateContext._pan2D;
			}
			if (_override3DProperties || _parentComponent == null)
			{
				if (_customCurves != null)
				{
					if (_audioSource.minDistance != _customCurves._minDistance)
					{
						_audioSource.minDistance = _customCurves._minDistance;
					}
					if (_audioSource.maxDistance != _customCurves._maxDistance)
					{
						_audioSource.maxDistance = _customCurves._maxDistance;
					}
					SetCustomCurves(_customCurves);
				}
				else
				{
					if (_audioSource.minDistance != _minDistance)
					{
						_audioSource.minDistance = _minDistance;
					}
					if (_audioSource.maxDistance != _maxDistance)
					{
						_audioSource.maxDistance = _maxDistance;
					}
					if (_audioSource.rolloffMode != _rolloffMode)
					{
						_audioSource.rolloffMode = _rolloffMode;
					}
				}
				if (_audioSource.priority != _priority)
				{
					_audioSource.priority = _priority;
				}
				if (_audioSource.spatialBlend != _panLevel)
				{
					_audioSource.spatialBlend = _panLevel;
				}
				if (_audioSource.spread != _spreadLevel)
				{
					_audioSource.spread = _spreadLevel;
				}
				if (_audioSource.dopplerLevel != _dopplerLevel)
				{
					_audioSource.dopplerLevel = _dopplerLevel;
				}
			}
			else
			{
				if (context._customCurves != null)
				{
					if (_audioSource.minDistance != context._customCurves._minDistance)
					{
						_audioSource.minDistance = context._customCurves._minDistance;
					}
					if (_audioSource.maxDistance != context._customCurves._maxDistance)
					{
						_audioSource.maxDistance = context._customCurves._maxDistance;
					}
					SetCustomCurves(context._customCurves);
				}
				else
				{
					if (_audioSource.minDistance != context._minDistance)
					{
						_audioSource.minDistance = context._minDistance;
					}
					if (_audioSource.maxDistance != context._maxDistance)
					{
						_audioSource.maxDistance = context._maxDistance;
					}
					if (_audioSource.rolloffMode != context._rolloffMode)
					{
						_audioSource.rolloffMode = context._rolloffMode;
					}
				}
				if (_audioSource.priority != context._priority)
				{
					_audioSource.priority = context._priority;
				}
				if (_audioSource.spatialBlend != context._panLevel)
				{
					_audioSource.spatialBlend = context._panLevel;
				}
				if (_audioSource.spread != context._spreadLevel)
				{
					_audioSource.spread = context._spreadLevel;
				}
				if (_audioSource.dopplerLevel != context._dopplerLevel)
				{
					_audioSource.dopplerLevel = context._dopplerLevel;
				}
			}
			if (_overrideBypassProperties || _parentComponent == null)
			{
				if (_audioSource.bypassEffects != _bypassEffects)
				{
					_audioSource.bypassEffects = _bypassEffects;
				}
				if (_audioSource.bypassListenerEffects != _bypassListenerEffects)
				{
					_audioSource.bypassListenerEffects = _bypassListenerEffects;
				}
				if (_audioSource.bypassReverbZones != _bypassReverbZones)
				{
					_audioSource.bypassReverbZones = _bypassReverbZones;
				}
			}
			else
			{
				if (_audioSource.bypassEffects != context._bypassEffects)
				{
					_audioSource.bypassEffects = context._bypassEffects;
				}
				if (_audioSource.bypassListenerEffects != context._bypassListenerEffects)
				{
					_audioSource.bypassListenerEffects = context._bypassListenerEffects;
				}
				if (_audioSource.bypassReverbZones != context._bypassReverbZones)
				{
					_audioSource.bypassReverbZones = context._bypassReverbZones;
				}
			}
			float num = context._reverbZoneMix * _updateContext._reverbZoneMix;
			if (AudioSource.reverbZoneMix != num)
			{
				_audioSource.reverbZoneMix = num;
			}
			if (_overrideSpatializeProperty || _parentComponent == null)
			{
				_updateContext._spatialize = _spatialize;
			}
			else
			{
				_updateContext._spatialize = context._spatialize;
			}
			if (AudioSource.spatialize != _updateContext._spatialize)
			{
				_audioSource.spatialize = _updateContext._spatialize;
			}
			if (_overrideAudioBus || _parentComponent == null)
			{
				_updateContext._audioBus = _audioBus;
				if (_audioBus != null && _audioBus._audioMixerGroup != null && AudioSource.outputAudioMixerGroup != _audioBus._audioMixerGroup)
				{
					_audioSource.outputAudioMixerGroup = _audioBus._audioMixerGroup;
				}
			}
			else
			{
				_updateContext._audioBus = context._audioBus;
				if (_updateContext._audioBus != null && _updateContext._audioBus._audioMixerGroup != null && AudioSource.outputAudioMixerGroup != _updateContext._audioBus._audioMixerGroup)
				{
					_audioSource.outputAudioMixerGroup = _updateContext._audioBus._audioMixerGroup;
				}
			}
			if (_updateContext._audioBus == null)
			{
				if (_overrideAudioMixerGroup || _parentComponent == null)
				{
					if (AudioSource.outputAudioMixerGroup != _audioMixerGroup)
					{
						_audioSource.outputAudioMixerGroup = _audioMixerGroup;
					}
				}
				else if (AudioSource.outputAudioMixerGroup != context._audioMixerGroup)
				{
					_audioSource.outputAudioMixerGroup = context._audioMixerGroup;
				}
			}
			UpdatePosition();
			return true;
		}

		private void SetCustomCurves(CustomCurves customCurves)
		{
			if (customCurves._enableCustomRolloff && _audioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff) != customCurves._customRolloff)
			{
				_audioSource.rolloffMode = AudioRolloffMode.Custom;
				_audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, customCurves._customRolloff);
			}
			if (customCurves._enableReverbZoneMix && _audioSource.GetCustomCurve(AudioSourceCurveType.ReverbZoneMix) != customCurves._reverbZoneMix)
			{
				_audioSource.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, customCurves._reverbZoneMix);
			}
			if (customCurves._enableSpatialBlend && _audioSource.GetCustomCurve(AudioSourceCurveType.SpatialBlend) != customCurves._spatialBlend)
			{
				_audioSource.SetCustomCurve(AudioSourceCurveType.SpatialBlend, customCurves._spatialBlend);
			}
			if (customCurves._enableSpread && _audioSource.GetCustomCurve(AudioSourceCurveType.Spread) != customCurves._spread)
			{
				_audioSource.SetCustomCurve(AudioSourceCurveType.Spread, customCurves._spread);
			}
		}

		private void UpdatePosition()
		{
			if (_audioSource == null)
			{
				return;
			}
			if (_componentInstance != null && _componentInstance._parentGameObject != null)
			{
				if (!(_audioSource.transform.position != _componentInstance._transform.position))
				{
					return;
				}
				Vector3 forceAxisVector = FabricManager.Instance._forceAxisVector;
				if (forceAxisVector.x != 0f || forceAxisVector.y != 0f || forceAxisVector.z != 0f)
				{
					float x = (forceAxisVector.x != 0f) ? forceAxisVector.x : _componentInstance._transform.position.x;
					float y = (forceAxisVector.y != 0f) ? forceAxisVector.y : _componentInstance._transform.position.y;
					float z = (forceAxisVector.z != 0f) ? forceAxisVector.z : _componentInstance._transform.position.z;
					Vector3 vector = new Vector3(x, y, z);
					if (vector != _audioSource.transform.position)
					{
						_audioSource.transform.position = vector;
					}
				}
				else if (_randomizePosition)
				{
					_audioSource.transform.position = _componentInstance._transform.position + _componentInstance._randomisePosition;
				}
				else
				{
					_audioSource.transform.position = _componentInstance._transform.position;
				}
				_cachedPosition = _audioSource.transform.position;
			}
			else
			{
				_audioSource.transform.position = _cachedPosition;
			}
		}

		private Vector3 RandomisePosition(float minVal, float maxVal)
		{
			return new Vector3(minVal, minVal, minVal) + UnityEngine.Random.insideUnitSphere * (maxVal - minVal);
		}

		public override EventStatus OnProcessEvent(Event zEvent, ComponentInstance zInstance)
		{
			EventStatus result = EventStatus.Not_Handled;
			EventAction eventAction = zEvent.EventAction;
			EventAction eventAction2 = eventAction;
			if (eventAction2 == EventAction.SetMarker)
			{
				string b = (string)zEvent._parameter;
				if (_markers != null && _markers.Count > 0)
				{
					for (int i = 0; i < _markers.Count; i++)
					{
						Marker marker = _markers[i];
						if (marker.name == b)
						{
							SetTime(marker.offsetTime);
						}
					}
				}
				result = EventStatus.Handled;
			}
			return result;
		}

		protected virtual void StopAudioComponent(bool notifyParent = true)
		{
			if (_audioSource == null)
			{
				_currentState = AudioComponentState.Stopped;
			}
			else
			{
				if (_currentState == AudioComponentState.Stopped)
				{
					return;
				}
				if (_dynamicAudioClipLoading)
				{
					AudioComponent audioComponent = base.ComponentHolder as AudioComponent;
					if ((bool)audioComponent)
					{
						audioComponent._audioClipHandle.DecRef((!_dynamicAsyncAudioClipLoading) ? true : false);
						if (audioComponent._audioClipHandle.IsAudioClipPathSet())
						{
							_audioClip = null;
							_audioSource.clip = null;
							if (_loopRegionAudioSources != null)
							{
								_loopRegionAudioSources[1].clip = null;
							}
						}
						if (audioComponent._audioClipHandle.RefCount == 0 && _dynamicAsyncAudioClipLoading)
						{
							_dynamicLoadAsyncState = DynamicLoadAsyncState.NotLoaded;
						}
					}
				}
				if (!_hasAudioSource)
				{
					RemoveDSPInstances();
					if (_loopRegionAudioSources != null && _loopRegionAudioSources.Length > 1)
					{
						FabricManager.Instance.AudioSourcePoolManager.Free(_loopRegionAudioSources[0]);
						FabricManager.Instance.AudioSourcePoolManager.Free(_loopRegionAudioSources[1]);
					}
					else
					{
						FabricManager.Instance.AudioSourcePoolManager.Free(_audioSource);
					}
					_audioSource = null;
					_audioSourceGameObject = null;
				}
				else if (_audioSourceGameObject != null)
				{
					Generic.SetGameObjectActive(_audioSourceGameObject, false);
					if (_loopRegionAudioSources != null && _loopRegionAudioSources.Length > 0 && _loopRegionAudioSources[1] != null)
					{
						Generic.SetGameObjectActive(_loopRegionAudioSources[1].gameObject, false);
					}
				}
				if (_updateContext._audioBus != null)
				{
					_updateContext._audioBus.DecrementAudioComponent();
				}
				if (HasValidEventNotifier())
				{
					NotifyEvent(EventNotificationType.OnAudioComponentStopped, this);
					NotifyEvent(EventNotificationType.OnFinished, this);
				}
				if (_triggerPlayingHandlerOnce && _currentState == AudioComponentState.Playing && notifyParent)
				{
					OnFinishPlaying(0.0);
					_triggerPlayingHandlerOnce = false;
				}
				_currentState = AudioComponentState.Stopped;
				_componentStatus = ComponentStatus.Stopped;
			}
		}

		public override void StopInternal(bool stopInstances, bool forceStop, float target, float curve, double scheduleEnd = 0.0)
		{
			if (_currentState == AudioComponentState.Stopped)
			{
				return;
			}
			if ((_syncWithMusicTime && _activeMusicTimeSettings != null) || scheduleEnd > 0.0)
			{
				if (scheduleEnd == 0.0)
				{
					scheduleEnd = _activeMusicTimeSettings.GetDelay();
				}
				if (_audioSource != null)
				{
					double dspTime = AudioSettings.dspTime;
					_audioSource.SetScheduledEndTime(AudioSettings.dspTime + scheduleEnd);
					_currentState = AudioComponentState.ScheduledToStop;
				}
				return;
			}
			if (forceStop && _updateContext._audioBus != null)
			{
				_updateContext._audioBus.DecrementAudioComponent();
			}
			base.StopInternal(stopInstances, forceStop, target, curve);
			if (_audioSource == null)
			{
				_currentState = AudioComponentState.Stopped;
				return;
			}
			if (forceStop)
			{
				if (_dynamicAudioClipLoading)
				{
					AudioComponent audioComponent = base.ComponentHolder as AudioComponent;
					if ((bool)audioComponent)
					{
						audioComponent._audioClipHandle.DecRef((!_dynamicAsyncAudioClipLoading) ? true : false);
						if (audioComponent._audioClipHandle.IsAudioClipPathSet())
						{
							_audioClip = null;
							_audioSource.clip = null;
							if (_loopRegionAudioSources != null)
							{
								_loopRegionAudioSources[1].clip = null;
							}
						}
						if (audioComponent._audioClipHandle.RefCount == 0 && _dynamicAsyncAudioClipLoading)
						{
							_dynamicLoadAsyncState = DynamicLoadAsyncState.NotLoaded;
						}
					}
				}
				if (!_hasAudioSource)
				{
					RemoveDSPInstances();
					FabricManager.Instance.AudioSourcePoolManager.Free(_audioSource, true);
					if (_loopRegionAudioSources != null && _loopRegionAudioSources.Length > 0 && _loopRegionAudioSources[1] != null)
					{
						FabricManager.Instance.AudioSourcePoolManager.Free(_loopRegionAudioSources[1]);
					}
					_audioSource = null;
				}
				else
				{
					_audioSource.volume = 0f;
					_audioSource.Stop();
					if (_loop && _loopRegionAudioSources != null && (_loopMarkersLoaded || _ignoreNativeLoop || _numLoops > 0))
					{
						_loopRegionAudioSources[1].Stop();
					}
					if (_audioSourceGameObject != null)
					{
						Generic.SetGameObjectActive(_audioSourceGameObject, false);
						if (_loopRegionAudioSources != null && _loopRegionAudioSources.Length > 0 && _loopRegionAudioSources[1] != null)
						{
							Generic.SetGameObjectActive(_loopRegionAudioSources[1].gameObject, false);
						}
					}
				}
				_currentState = AudioComponentState.Stopped;
				_isComponentActive = false;
			}
			else if (_currentState != AudioComponentState.WaitingToStop)
			{
				_currentState = AudioComponentState.WaitingToStop;
			}
			if (_componentInstances == null || !stopInstances)
			{
				return;
			}
			for (int i = 0; i < _componentInstances.Length; i++)
			{
				ComponentInstance componentInstance = _componentInstances[i];
				if (componentInstance._instance.IsInstance && componentInstance._instance.IsPlaying())
				{
					componentInstance._instance.Stop(stopInstances, forceStop);
				}
			}
		}

		public override void Pause(bool pause)
		{
			if (_componentInstances != null)
			{
				for (int i = 0; i < _componentInstances.Length; i++)
				{
					ComponentInstance componentInstance = _componentInstances[i];
					if (componentInstance._instance.IsInstance)
					{
						componentInstance._instance.Pause(pause);
					}
				}
			}
			if (_audioSource == null || AudioClip == null)
			{
				return;
			}
			if (_currentState == AudioComponentState.Paused && !pause)
			{
				_currentState = AudioComponentState.Playing;
				if (_audioSourceGameObject != null)
				{
					Generic.SetGameObjectActive(_audioSourceGameObject, true);
					if (_loopRegionAudioSources != null && _loopRegionAudioSources.Length > 0 && _loopRegionAudioSources[1] != null)
					{
						Generic.SetGameObjectActive(_loopRegionAudioSources[1].gameObject, true);
					}
				}
				_audioSource.time = Math.Min(_pauseAudioSouceTimeInSecs, AudioClip.length);
				_dspTimeTriggered += AudioSettings.dspTime - _pauseTimeInSec;
				_audioSource.Play();
				_triggerPlayingHandlerOnce = true;
			}
			else if (pause && _currentState != AudioComponentState.Stopped)
			{
				_currentState = AudioComponentState.Paused;
				_pauseAudioSouceTimeInSecs = _audioSource.time;
				_pauseTimeInSec = AudioSettings.dspTime;
				_audioSource.Pause();
			}
		}

		public override void SetTime(float time)
		{
			if (!(_audioSource == null) && !(AudioClip == null))
			{
				_audioSource.time = Mathf.Min(time, AudioClip.length - 0.01f);
			}
		}

		public override void AdvanceTime(float time, float length)
		{
			if (_audioSource == null || AudioClip == null)
			{
				return;
			}
			float num = time + length;
			float length2 = _audioSource.clip.length;
			int num2 = (int)(num / length2);
			if (num2 > 0)
			{
				if (!_loop)
				{
					StopAudioComponent();
					return;
				}
				for (int i = 0; i < num2; i++)
				{
					num -= length2;
				}
			}
			_audioSource.time = Mathf.Min(num, AudioClip.length - 0.01f);
		}

		public override void LoadAudio()
		{
			if (_audioClipHandle != null && _audioClip.loadState != AudioDataLoadState.Loaded)
			{
				if (_audioClipHandle.AudioClip == null)
				{
					_audioClipHandle.AudioClip = _audioClip;
				}
				_audioClipHandle.LoadAudioData();
			}
		}

		public override void UnloadAudio()
		{
			if (_audioClipHandle != null)
			{
				_audioClipHandle.UnloadAudioData();
			}
		}

		public void OnDrawGizmos()
		{
			if (_randomizePosition)
			{
				if (_componentInstance != null)
				{
					Gizmos.DrawWireSphere(_componentInstance._transform.position, _randomizeMinPosition);
					Gizmos.DrawWireSphere(_componentInstance._transform.position, _randomizeMaxPosition);
				}
				else
				{
					Gizmos.DrawWireSphere(base.transform.position, _randomizeMinPosition);
					Gizmos.DrawWireSphere(base.transform.position, _randomizeMaxPosition);
				}
			}
		}

		public virtual void SetAudioClip(AudioClip audioClip, GameObject parentGameObject)
		{
			for (int i = 0; i < _componentInstances.Length; i++)
			{
				ComponentInstance componentInstance = _componentInstances[i];
				AudioComponent audioComponent = componentInstance._instance as AudioComponent;
				if (audioComponent != null)
				{
					if (parentGameObject == null)
					{
						audioComponent._audioClip = audioClip;
						audioComponent._audioSource.clip = audioComponent._audioClip;
					}
					else if (parentGameObject == componentInstance._parentGameObject)
					{
						audioComponent._audioClip = audioClip;
						audioComponent._audioSource.clip = audioComponent._audioClip;
					}
				}
			}
		}
	}
}
