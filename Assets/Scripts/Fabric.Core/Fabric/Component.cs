using Fabric.MIDI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;

namespace Fabric
{
	public abstract class Component : MonoBehaviour, IEventListener, IRTPPropertyListener
	{
		public enum NotifyParentType
		{
			ComponentHasFinished,
			AllComponentsHaveFinished
		}

		public class RuntimeProperties
		{
			public float _volume = 1f;

			public float _pitch = 1f;

			public float _pan2D;

			public float _panLevel = 1f;

			public float _spreadLevel;

			public float _dopplerLevel = 1f;

			public int _priority = 128;

			public void Reset(Component component)
			{
				_volume = component.Volume;
				_pitch = component.Pitch;
				_pan2D = component.Pan2D;
				_panLevel = component.PanLevel;
				_priority = component.Priority;
				_spreadLevel = component.SpreadLevel;
				_dopplerLevel = component.DopplerLevel;
			}

			public void Reset()
			{
				_volume = 1f;
				_pitch = 1f;
				_pan2D = 0f;
				_panLevel = 1f;
				_spreadLevel = 0f;
				_dopplerLevel = 1f;
				_priority = 128;
			}
		}

		[Serializable]
		public class MIDIProperties
		{
			[SerializeField]
			[HideInInspector]
			public bool _overrideParentNoteTracking;

			[HideInInspector]
			[SerializeField]
			public bool _noteTracking;

			[HideInInspector]
			[SerializeField]
			public int _keyRangeMin;

			[SerializeField]
			[HideInInspector]
			public int _keyRangeMax = 127;

			[HideInInspector]
			[SerializeField]
			public int _velocityRangeMin;

			[SerializeField]
			[HideInInspector]
			public int _velocityRangeMax = 127;

			[SerializeField]
			[HideInInspector]
			public int _channelRangeMin = 1;

			[SerializeField]
			[HideInInspector]
			public int _channelRangeMax = 16;

			[HideInInspector]
			[SerializeField]
			public int _rootNote = -36;

			[HideInInspector]
			[SerializeField]
			public int _transpose;

			[SerializeField]
			[HideInInspector]
			public int _velocity;
		}

		private class sortVirtualizationInstancesByDistance : IComparer<VirtualizationEventInstance>
		{
			public int Compare(VirtualizationEventInstance a, VirtualizationEventInstance b)
			{
				if (a == null || b == null)
				{
					return 0;
				}
				if (a._distanceFromListener > b._distanceFromListener)
				{
					return 1;
				}
				if (a._distanceFromListener < b._distanceFromListener)
				{
					return -1;
				}
				return 0;
			}
		}

		private class sortByNearestDistance : IComparer<ComponentInstance>
		{
			public int Compare(ComponentInstance a, ComponentInstance b)
			{
				if (a == null || b == null || a._parentGameObject == null || b._parentGameObject == null)
				{
					return 0;
				}
				float num = 0f;
				float num2 = 0f;
				if (a._parentGameObject == null || a._parentGameObject == null)
				{
					return 0;
				}
				if (FabricManager.Instance._audioListener != null)
				{
					num = Vector3.Distance(a._parentGameObject.transform.position, FabricManager.Instance._audioListener.transform.position);
					num2 = Vector3.Distance(b._parentGameObject.transform.position, FabricManager.Instance._audioListener.transform.position);
				}
				else if (Camera.main != null)
				{
					num = Vector3.Distance(a._parentGameObject.transform.position, Camera.main.transform.position);
					num2 = Vector3.Distance(b._parentGameObject.transform.position, Camera.main.transform.position);
				}
				if (num > num2)
				{
					return 1;
				}
				if (num < num2)
				{
					return -1;
				}
				return 0;
			}
		}

		private class sortByFurthestDistance : IComparer<ComponentInstance>
		{
			public int Compare(ComponentInstance a, ComponentInstance b)
			{
				if (a == null || b == null || a._parentGameObject == null || b._parentGameObject == null)
				{
					return 0;
				}
				float num = 0f;
				float num2 = 0f;
				if (FabricManager.Instance._audioListener != null)
				{
					num = Vector3.Distance(a._parentGameObject.transform.position, FabricManager.Instance._audioListener.transform.position);
					num2 = Vector3.Distance(b._parentGameObject.transform.position, FabricManager.Instance._audioListener.transform.position);
				}
				else if (Camera.main != null)
				{
					num = Vector3.Distance(a._parentGameObject.transform.position, Camera.main.transform.position);
					num2 = Vector3.Distance(b._parentGameObject.transform.position, Camera.main.transform.position);
				}
				if (num < num2)
				{
					return 1;
				}
				if (num > num2)
				{
					return -1;
				}
				return 0;
			}
		}

		public enum RTPPropertyEnum
		{
			Volume,
			Pitch,
			Pan2D,
			PanLevel,
			SpreadLevel,
			DopplerLevel,
			Priority,
			ReverbZoneMix,
			Custom
		}

		protected List<Component> _components = new List<Component>();

		protected Component[] _componentsArray = new Component[0];

		protected EventListener[] _eventListeners;

		[NonSerialized]
		[HideInInspector]
		public DSPComponent[] _dspComponents;

		[NonSerialized]
		[HideInInspector]
		public SideChain[] _sideChainComponents;

		[SerializeField]
		[HideInInspector]
		public int _numVirtualizationEvents = 100;

		[NonSerialized]
		private VirtualizationEventInstanceManager _virtualizationEventInstanceManager;

		[NonSerialized]
		public ComponentInstance[] _componentInstances;

		private GameObject _instanceHolder;

		protected Context _updateContext = new Context();

		protected InitialiseParameters _initialiseParameters;

		protected bool _isInstance;

		[NonSerialized]
		[HideInInspector]
		public bool _isComponentActive;

		private int _delaySamples;

		private double _playScheduled;

		private double _playScheduledDelay;

		private double _playScheduledTime;

		[NonSerialized]
		public double _scheduledDspTime;

		[HideInInspector]
		[SerializeField]
		protected int _maxInstances = 1;

		[HideInInspector]
		[SerializeField]
		protected ComponentStealingBehaviour _stealingBehaviour;

		[SerializeField]
		[HideInInspector]
		protected float _minimumPlaybackInterval;

		[NonSerialized]
		[HideInInspector]
		protected float _minimumPlaybackIntervalTimeStamp;

		[HideInInspector]
		[SerializeField]
		protected bool _overrideParentVolume;

		[HideInInspector]
		[SerializeField]
		protected float _volume = 1f;

		[HideInInspector]
		[SerializeField]
		protected float _volumeRandomization;

		[SerializeField]
		[HideInInspector]
		protected bool _mute;

		[HideInInspector]
		[SerializeField]
		protected bool _overrideParentPitch;

		[SerializeField]
		[HideInInspector]
		protected float _pitch = 1f;

		[SerializeField]
		[HideInInspector]
		protected float _pitchRandomization;

		[SerializeField]
		[HideInInspector]
		protected bool _override2DProperties;

		[SerializeField]
		[HideInInspector]
		protected float _pan2D;

		[HideInInspector]
		[SerializeField]
		protected float _pan2DRandomization;

		[SerializeField]
		[HideInInspector]
		protected bool _override3DProperties;

		[HideInInspector]
		[SerializeField]
		protected int _priority = 128;

		[SerializeField]
		[HideInInspector]
		protected float _panLevel = 1f;

		[HideInInspector]
		[SerializeField]
		protected float _spreadLevel;

		[HideInInspector]
		[SerializeField]
		protected bool _spatialize;

		[SerializeField]
		[HideInInspector]
		protected float _dopplerLevel = 1f;

		[SerializeField]
		[HideInInspector]
		protected float _maxDistance = 500f;

		[SerializeField]
		[HideInInspector]
		protected float _minDistance = 1f;

		[HideInInspector]
		[SerializeField]
		protected AudioRolloffMode _rolloffMode;

		[HideInInspector]
		[SerializeField]
		public ComponentCustomCurvesType _customCurvesType;

		[SerializeField]
		[HideInInspector]
		public string _customCurvesName;

		[NonSerialized]
		protected CustomCurves _customCurves;

		[HideInInspector]
		[SerializeField]
		protected bool _overrideFadeProperties;

		[HideInInspector]
		[SerializeField]
		protected float _fadeInTime;

		[SerializeField]
		[HideInInspector]
		protected float _fadeInCurve = 0.5f;

		[SerializeField]
		[HideInInspector]
		protected float _fadeOutTime;

		[SerializeField]
		[HideInInspector]
		public float _fadeOutCurve = 0.5f;

		[SerializeField]
		[HideInInspector]
		public float _reverbZoneMix = 1f;

		[SerializeField]
		[HideInInspector]
		protected bool _overrideBypassProperties;

		[SerializeField]
		[HideInInspector]
		protected bool _bypassEffects;

		[HideInInspector]
		[SerializeField]
		protected bool _bypassListenerEffects;

		[HideInInspector]
		[SerializeField]
		protected bool _bypassReverbZones;

		[SerializeField]
		[HideInInspector]
		public bool _overrideAudioMixerGroup;

		[HideInInspector]
		[SerializeField]
		public AudioMixerGroup _audioMixerGroup;

		[NonSerialized]
		[HideInInspector]
		private GameObject _globalParentGameObject;

		[SerializeField]
		[HideInInspector]
		public NotifyParentType _notifyParentComponent;

		[SerializeField]
		[HideInInspector]
		public string _audioBusName;

		[NonSerialized]
		protected AudioBus _audioBus;

		[SerializeField]
		[HideInInspector]
		public bool _overrideAudioBus;

		[SerializeField]
		[HideInInspector]
		public bool _overrideMusicTimeSettings;

		[NonSerialized]
		[HideInInspector]
		protected MusicTimeSittings _activeMusicTimeSettings;

		[SerializeField]
		[HideInInspector]
		public int _musicTimeSettingsIndex = -1;

		[SerializeField]
		[HideInInspector]
		public float _musicTempo = -1f;

		[SerializeField]
		[HideInInspector]
		public int _musicTimeSignatureLower;

		[SerializeField]
		[HideInInspector]
		public int _musicTimeSignatureUpper;

		[SerializeField]
		[HideInInspector]
		public bool _musicTimeResetOnPlay;

		[HideInInspector]
		[SerializeField]
		public MIDIProperties _midiProperties = new MIDIProperties();

		[SerializeField]
		[HideInInspector]
		protected bool _overrideSpatializeProperty;

		[HideInInspector]
		[SerializeField]
		public int _probability = 100;

		[NonSerialized]
		[HideInInspector]
		public InterpolatedParameter _fadeParameter = new InterpolatedParameter(1f);

		[NonSerialized]
		[HideInInspector]
		public ComponentStatus _componentStatus = ComponentStatus.Stopped;

		[SerializeField]
		[HideInInspector]
		protected bool _multipleInstancesPerGameObject = true;

		[HideInInspector]
		[SerializeField]
		public bool _componentVirtualization;

		[SerializeField]
		[HideInInspector]
		public bool _overrideVolumeThreshold;

		[HideInInspector]
		[SerializeField]
		public float _volumeThreshold = 1f;

		[NonSerialized]
		[HideInInspector]
		public List<VirtualizationEventInstance> _componentVirtualizationEvents = new List<VirtualizationEventInstance>();

		[SerializeField]
		[HideInInspector]
		public VirtualizationBehavior _virtualizationBehavior = VirtualizationBehavior.PlayFromStart;

		private bool _componentVirtualizationActive;

		protected ComponentInstance _componentInstance;

		protected Component _parentComponent;

		[NonSerialized]
		[HideInInspector]
		public CodeProfiler profiler = new CodeProfiler();

		protected string _name = "";

		protected bool _quitting;

		protected RuntimeProperties _runtimeProperties = new RuntimeProperties();

		protected RuntimeProperties _rtpProperties = new RuntimeProperties();

		protected float _volumeOffset = 1f;

		protected float _sideChainGain = 1f;

		protected float _mixerVolume = 1f;

		protected float _pitchOffset = 1f;

		protected float _mixerPitch = 1f;

		protected float _pan2DOffset;

		[HideInInspector]
		[SerializeField]
		public RTPManager _RTPManager;

		protected VolumeMeter _volumeMeter;

		private static List<ComponentInstance> findInstances = new List<ComponentInstance>();

		[SerializeField]
		[HideInInspector]
		public string Guid;

		[SerializeField]
		[HideInInspector]
		public Component _componentHolder;

		protected static bool _initializationInProgress = false;

		protected bool _updatePropertiesFlag;

		protected OnEventNotify OnEventNotifyInvoker;

		public string _onEventNotifyEventName;

		private MidiEvent _midiEvent;

		private IComparer<ComponentInstance> sortByFurthestDistanceComparer = new sortByFurthestDistance();

		private sortVirtualizationInstancesByDistance _sortVirtualizationInstancesByDistance = new sortVirtualizationInstancesByDistance();

		protected System.Random _random
		{
			get
			{
				return Generic._random;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public List<Component> Components
		{
			get
			{
				return _components;
			}
		}

		public Component ComponentHolder
		{
			get
			{
				return _componentHolder;
			}
			set
			{
				_componentHolder = value;
			}
		}

		public bool IsInstance
		{
			get
			{
				return _isInstance;
			}
		}

		public bool IsTopNode
		{
			get
			{
				if (base.transform.parent == null)
				{
					return false;
				}
				if (!(base.transform.parent.GetComponent<FabricManager>() == null))
				{
					return true;
				}
				return false;
			}
		}

		public GameObject ParentGameObject
		{
			get
			{
				if (_componentInstance == null)
				{
					return null;
				}
				return _componentInstance._parentGameObject;
			}
		}

		public bool HasEventListener
		{
			get
			{
				if (!(base.gameObject.GetComponent<EventListener>() != null))
				{
					return IsInEventEditor;
				}
				return true;
			}
		}

		public bool IsInEventEditor
		{
			get
			{
				GroupComponent groupComponent = IsExternalGroupComponent();
				if (groupComponent == null && EventManager.Instance != null)
				{
					return EventManager.Instance._eventEditor.IsComponentPresent(this);
				}
				if (groupComponent._eventEditor != null)
				{
					return groupComponent._eventEditor.IsComponentPresent(this);
				}
				return false;
			}
		}

		public bool HasVolumeMeter
		{
			get
			{
				if (!(_volumeMeter != null))
				{
					return false;
				}
				return true;
			}
		}

		public VolumeMeter VolumeMeter
		{
			get
			{
				return _volumeMeter;
			}
		}

		public Context UpdateContext
		{
			get
			{
				return _updateContext;
			}
		}

		public int MaxInstances
		{
			get
			{
				return _maxInstances;
			}
			set
			{
				_maxInstances = value;
			}
		}

		public float MinimumPlaybackInterval
		{
			get
			{
				return _minimumPlaybackInterval;
			}
			set
			{
				_minimumPlaybackInterval = value;
			}
		}

		public ComponentStealingBehaviour StealingBehaviour
		{
			get
			{
				return _stealingBehaviour;
			}
			set
			{
				_stealingBehaviour = value;
			}
		}

		public bool OverrideParentVolume
		{
			get
			{
				return _overrideParentVolume;
			}
			set
			{
				_overrideParentVolume = value;
			}
		}

		public float Volume
		{
			get
			{
				return _volume;
			}
			set
			{
				_volume = value;
			}
		}

		public float VolumeOffset
		{
			get
			{
				return _volumeOffset;
			}
		}

		public float VolumeRandomization
		{
			get
			{
				return _volumeRandomization;
			}
			set
			{
				_volumeRandomization = value;
			}
		}

		public float MixerVolume
		{
			get
			{
				return _mixerVolume;
			}
			set
			{
				_mixerVolume = value;
			}
		}

		public float MixerPitch
		{
			get
			{
				return _mixerPitch;
			}
			set
			{
				_mixerPitch = value;
			}
		}

		public bool Mute
		{
			get
			{
				return _mute;
			}
			set
			{
				_mute = value;
			}
		}

		public float SideChainGain
		{
			set
			{
				_sideChainGain = value;
			}
		}

		public bool OverrideParentPitch
		{
			get
			{
				return _overrideParentPitch;
			}
			set
			{
				_overrideParentPitch = value;
			}
		}

		public float Pitch
		{
			get
			{
				return _pitch;
			}
			set
			{
				_pitch = value;
			}
		}

		public float PitchOffset
		{
			get
			{
				return _pitchOffset;
			}
		}

		public float PitchRandomization
		{
			get
			{
				return _pitchRandomization;
			}
			set
			{
				_pitchRandomization = value;
			}
		}

		public bool Override2DProperties
		{
			get
			{
				return _override2DProperties;
			}
			set
			{
				_override2DProperties = value;
			}
		}

		public float Pan2D
		{
			get
			{
				return _pan2D;
			}
			set
			{
				_pan2D = value;
			}
		}

		public float Pan2dOffset
		{
			get
			{
				return _pan2DOffset;
			}
		}

		public float Pan2DRandomization
		{
			get
			{
				return _pan2DRandomization;
			}
			set
			{
				_pan2DRandomization = value;
			}
		}

		public bool Override3DProperties
		{
			get
			{
				return _override3DProperties;
			}
			set
			{
				_override3DProperties = value;
			}
		}

		public int Priority
		{
			get
			{
				return _priority;
			}
			set
			{
				_priority = value;
			}
		}

		public float PanLevel
		{
			get
			{
				return _panLevel;
			}
			set
			{
				_panLevel = value;
			}
		}

		public float SpreadLevel
		{
			get
			{
				return _spreadLevel;
			}
			set
			{
				_spreadLevel = value;
			}
		}

		public float DopplerLevel
		{
			get
			{
				return _dopplerLevel;
			}
			set
			{
				_dopplerLevel = value;
			}
		}

		public float MaxDistance
		{
			get
			{
				return _maxDistance;
			}
			set
			{
				_maxDistance = value;
			}
		}

		public float MinDistance
		{
			get
			{
				return _minDistance;
			}
			set
			{
				_minDistance = value;
			}
		}

		public AudioRolloffMode RolloffMode
		{
			get
			{
				return _rolloffMode;
			}
			set
			{
				_rolloffMode = value;
			}
		}

		public bool OverrideFadeProperties
		{
			get
			{
				return _overrideFadeProperties;
			}
			set
			{
				_overrideFadeProperties = value;
			}
		}

		public float FadeInTime
		{
			get
			{
				return _fadeInTime;
			}
			set
			{
				_fadeInTime = value;
			}
		}

		public float FadeInCurve
		{
			get
			{
				return _fadeInCurve;
			}
			set
			{
				_fadeInCurve = value;
			}
		}

		public float FadeOutTime
		{
			get
			{
				return _fadeOutTime;
			}
			set
			{
				_fadeOutTime = value;
			}
		}

		public float FadeOutCurve
		{
			get
			{
				return _fadeOutCurve;
			}
			set
			{
				_fadeOutCurve = value;
			}
		}

		public bool OverrideBypassProperties
		{
			get
			{
				return _overrideBypassProperties;
			}
			set
			{
				_overrideBypassProperties = value;
			}
		}

		public bool OverrideSpatializeProperties
		{
			get
			{
				return _overrideSpatializeProperty;
			}
			set
			{
				_overrideSpatializeProperty = value;
			}
		}

		public bool BypassEffects
		{
			get
			{
				return _bypassEffects;
			}
			set
			{
				_bypassEffects = value;
			}
		}

		public bool BypassListenerEffects
		{
			get
			{
				return _bypassListenerEffects;
			}
			set
			{
				_bypassListenerEffects = value;
			}
		}

		public bool BypassReverbZones
		{
			get
			{
				return _bypassReverbZones;
			}
			set
			{
				_bypassReverbZones = value;
			}
		}

		public int DelaySamples
		{
			get
			{
				return _delaySamples;
			}
			set
			{
				_delaySamples = value;
			}
		}

		public double PlayScheduled
		{
			get
			{
				return _playScheduled;
			}
			set
			{
				_playScheduled = value;
			}
		}

		public double PlayScheduledDelay
		{
			get
			{
				return _playScheduledDelay;
			}
			set
			{
				_playScheduledDelay = value;
			}
		}

		public Component ParentComponent
		{
			get
			{
				return _parentComponent;
			}
			set
			{
				_parentComponent = value;
			}
		}

		public RTPManager RTPManager
		{
			get
			{
				if (_RTPManager == null)
				{
					_RTPManager = new RTPManager();
				}
				if (_RTPManager._reference != null && _RTPManager._reference != this)
				{
					return _RTPManager._reference.RTPManager;
				}
				return _RTPManager;
			}
		}

		public CustomCurves CustomCurves
		{
			get
			{
				if (_customCurves == null)
				{
					return FabricManager.Instance._customCurvesManager.GetCustomCurvesByName(_customCurvesName);
				}
				return _customCurves;
			}
			set
			{
				_customCurves = value;
			}
		}

		bool IEventListener.IsDestroyed
		{
			get
			{
				return this == null;
			}
		}

		public event OnEventNotify _onEventNotify
		{
			add
			{
				OnEventNotifyInvoker = null;
				OnEventNotifyInvoker = (OnEventNotify)Delegate.Combine(OnEventNotifyInvoker, value);
			}
			remove
			{
				OnEventNotifyInvoker = (OnEventNotify)Delegate.Remove(OnEventNotifyInvoker, value);
			}
		}

		public void UpdateComponentsArray()
		{
			_componentsArray = _components.ToArray();
		}

		public GroupComponent IsExternalGroupComponent()
		{
			if (base.transform.parent != null)
			{
				Component component = base.transform.parent.GetComponent<Component>();
				if (component != null)
				{
					return component.IsExternalGroupComponent();
				}
			}
			GroupComponent groupComponent = this as GroupComponent;
			if ((bool)groupComponent && !groupComponent.IsFabricHierarchyPresent())
			{
				return groupComponent;
			}
			return null;
		}

		internal virtual void OnFinishPlaying(double time)
		{
			if (ParentComponent != null)
			{
				ParentComponent.OnFinishPlaying(time);
			}
		}

		internal virtual bool OnMarker(double time)
		{
			if (ParentComponent != null)
			{
				return ParentComponent.OnMarker(time);
			}
			return true;
		}

		protected bool HasValidEventNotifier()
		{
			if (_componentInstance != null && _componentInstance._instance != null)
			{
				return _componentInstance._instance.OnEventNotifyInvoker != null;
			}
			return false;
		}

		protected void NotifyEvent(EventNotificationType type, object data)
		{
			_componentInstance._instance.OnEventNotifyInvoker(type, _onEventNotifyEventName, data, _componentInstance._parentGameObject);
		}

		public virtual void SetMusicTimeSettings(MusicTimeSittings musicTimeSettings, MusicSyncType musicSyncType)
		{
			if (_overrideMusicTimeSettings || _parentComponent == null)
			{
				musicTimeSettings = _activeMusicTimeSettings;
			}
			for (int i = 0; i < _componentsArray.Length; i++)
			{
				_componentsArray[i].SetMusicTimeSettings(musicTimeSettings, musicSyncType);
			}
		}

		public Component[] GetChildComponents()
		{
			List<Component> list = new List<Component>();
			int childCount = base.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Component component = base.transform.GetChild(i).GetComponent<Component>();
				if (component != null)
				{
					list.Add(component);
				}
			}
			return list.ToArray();
		}

		protected virtual void ComponentDestroyed(Component component)
		{
		}

		internal void SetPlayScheduled(double playScheduled, double playScheduledDelay)
		{
			_playScheduled = playScheduled;
			_playScheduledDelay = playScheduledDelay;
			_scheduledDspTime = AudioSettings.dspTime;
		}

		internal void SetPlayScheduledAdditive(double playScheduled, double playScheduledDelay)
		{
			_playScheduled += playScheduled;
			_playScheduledDelay += playScheduledDelay;
			_scheduledDspTime = AudioSettings.dspTime;
		}

		internal void SetPlayScheduledTime(double time)
		{
			_playScheduledTime = time;
		}

		internal void ResetPlayScheduled()
		{
			_playScheduled = 0.0;
			_playScheduledDelay = 0.0;
			_scheduledDspTime = 0.0;
			_playScheduledTime = 0.0;
		}

		internal void GetPlayScheduleTime(ref double playScheduledTime)
		{
			playScheduledTime = _playScheduledTime;
		}

		internal void GetPlayScheduled(ref double playScheduled, ref double playScheduledDelay)
		{
			double num = AudioSettings.dspTime - _scheduledDspTime;
			playScheduled = _playScheduled - num;
			playScheduled = _playScheduled;
			playScheduledDelay = _playScheduledDelay;
		}

		internal void SetMIDIEvent(MidiEvent midiEvent)
		{
			_midiEvent = midiEvent;
		}

		internal MidiEvent GetMIDIEvent(ComponentInstance componentInstance)
		{
			if (_midiEvent != null)
			{
				return _midiEvent;
			}
			if (componentInstance != null)
			{
				return componentInstance._instance._midiEvent;
			}
			return null;
		}

		internal bool CheckMIDI(ComponentInstance componentInstance)
		{
			MidiEvent mIDIEvent = GetMIDIEvent(componentInstance);
			int transpose = _midiProperties._transpose;
			int velocity = _midiProperties._velocity;
			if (mIDIEvent != null && _midiProperties != null && mIDIEvent.midiChannelEvent == MidiHelper.MidiChannelEvent.Note_On)
			{
				if (mIDIEvent.parameter1 < transpose + _midiProperties._keyRangeMin || mIDIEvent.parameter1 > transpose + _midiProperties._keyRangeMax)
				{
					return false;
				}
				if (mIDIEvent.parameter2 < velocity + _midiProperties._velocityRangeMin || mIDIEvent.parameter2 > velocity + _midiProperties._velocityRangeMax)
				{
					return false;
				}
			}
			return true;
		}

		internal void ApplyMIDI(ComponentInstance componentInstance)
		{
			MidiEvent mIDIEvent = GetMIDIEvent(componentInstance);
			if (mIDIEvent != null && mIDIEvent.midiChannelEvent == MidiHelper.MidiChannelEvent.Note_On)
			{
				float pitch = 1f;
				if (_midiProperties._overrideParentNoteTracking && _midiProperties._noteTracking)
				{
					pitch = Mathf.Pow(2f, ((float)(int)mIDIEvent.parameter1 + (float)_midiProperties._rootNote + (float)_midiProperties._transpose) / 12f);
				}
				SetPitch(pitch);
				float volume = (float)(mIDIEvent.parameter2 + _midiProperties._velocity) / 127f;
				SetVolume(volume);
			}
		}

		private void OnApplicationQuit()
		{
			if (_RTPManager != null)
			{
				_RTPManager.Reset();
			}
			_quitting = true;
		}

		private void CollectChildrenComponents(Component parentComponent, bool isComponentInstance)
		{
			_components.Clear();
			int childCount = base.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Component component = base.transform.GetChild(i).GetComponent<Component>();
				if (component != null)
				{
					component.OnPreInitialise();
					component.Initialise(parentComponent, isComponentInstance);
					component.OnPostInitialise();
					AddComponent(component);
				}
			}
			_componentsArray = _components.ToArray();
		}

		public void MoveToComponent(Component targetComponent)
		{
			if (_parentComponent != null)
			{
				_parentComponent.RemoveComponent(this);
			}
			else
			{
				FabricManager.Instance.RemoveComponent(this);
			}
			_parentComponent = targetComponent;
			for (int i = 1; i < _maxInstances; i++)
			{
				_componentInstances[i]._instance._parentComponent = targetComponent;
			}
			targetComponent.AddComponent(this);
		}

		public void AddComponent(Component component)
		{
			_components.Add(component);
			_componentsArray = _components.ToArray();
		}

		public void RemoveComponent(Component component)
		{
			_components.Remove(component);
			_componentsArray = _components.ToArray();
		}

		public virtual void SetComponentActive(bool isActive)
		{
			if (_fadeParameter.GetTarget() == 0f && isActive)
			{
				_fadeParameter.SetTarget(FabricTimer.Get(), 1f, _fadeInTime, _fadeInCurve);
			}
			_isComponentActive = isActive;
			if (_parentComponent != null)
			{
				_parentComponent.SetComponentActive(isActive);
			}
		}

		protected void UpdateParentProperties()
		{
			if (_parentComponent != null)
			{
				_parentComponent.UpdateParentProperties();
				UpdateProperties(_parentComponent.UpdateContext);
			}
			else
			{
				UpdateProperties(_updateContext);
			}
		}

		public void BuildComponentEventPathName(ref string path, int depth)
		{
			depth++;
			if (_parentComponent != null && _parentComponent != this)
			{
				_parentComponent.BuildComponentEventPathName(ref path, depth);
			}
			else
			{
				Component component = base.transform.parent.GetComponent<Component>();
				if (component != null)
				{
					component.BuildComponentEventPathName(ref path, depth);
				}
			}
			depth--;
			path += base.name;
			if (depth > 0)
			{
				path += "/";
			}
		}

		public void BakeComponentInstances()
		{
			Component[] childComponents = GetChildComponents();
			for (int i = 0; i < childComponents.Length; i++)
			{
				childComponents[i].BakeComponentInstances();
			}
			EventListener component = base.gameObject.GetComponent<EventListener>();
			if (_maxInstances > 1 && (bool)component)
			{
				GameObject gameObject = new GameObject("Instances");
				if (!GetFabricManager.Instance()._showAllInstances)
				{
					gameObject.hideFlags = HideFlags.HideAndDontSave;
				}
				else
				{
					gameObject.hideFlags = HideFlags.DontSave;
				}
				for (int j = 1; j < _maxInstances; j++)
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate(base.gameObject, base.gameObject.transform.position, base.gameObject.transform.rotation);
					gameObject2.name = gameObject2.name.Replace("(Clone)", "_" + j);
					gameObject2.transform.parent = gameObject.transform;
				}
				gameObject.transform.parent = base.gameObject.transform;
			}
		}

		public void CleanBakedComponentInstances()
		{
			Component[] childComponents = GetChildComponents();
			for (int i = 0; i < childComponents.Length; i++)
			{
				childComponents[i].CleanBakedComponentInstances();
			}
			for (int j = 0; j < base.transform.childCount; j++)
			{
				GameObject gameObject = base.transform.GetChild(j).gameObject;
				if (gameObject.name == "Instances")
				{
					UnityEngine.Object.DestroyImmediate(gameObject.gameObject);
				}
			}
		}

		public void Initialise(Component parentComponent, bool isComponentInstance)
		{
			_parentComponent = parentComponent;
			if (!isComponentInstance)
			{
				if (FabricManager.Instance._createEventListeners)
				{
					EventListener eventListener = base.gameObject.AddComponent<EventListener>();
					if (eventListener != null)
					{
						string path = "";
						int depth = 0;
						if (!FabricManager.Instance._useFullPathForEventListeners)
						{
							path = base.gameObject.name;
						}
						else
						{
							BuildComponentEventPathName(ref path, depth);
						}
						EventManager.Instance._eventList.Add(path);
						eventListener._eventName = path;
					}
				}
				_eventListeners = base.gameObject.GetComponents<EventListener>();
				CollectChildrenComponents(this, isComponentInstance);
				if ((_eventListeners != null && _eventListeners.Length > 0) || IsInEventEditor)
				{
					if (_maxInstances == 0)
					{
						_maxInstances = 1;
					}
					if (_componentVirtualization)
					{
						_virtualizationEventInstanceManager = new VirtualizationEventInstanceManager();
						_virtualizationEventInstanceManager.Initialise(_numVirtualizationEvents, this);
					}
					_componentInstances = new ComponentInstance[_maxInstances];
					ComponentInstance componentInstance = new ComponentInstance();
					componentInstance._componentGameObject = base.gameObject;
					componentInstance._componentInstanceHolder = this;
					componentInstance._instance = this;
					componentInstance._instance.ComponentHolder = this;
					componentInstance._parentGameObject = null;
					componentInstance._instance._dspComponents = base.gameObject.GetComponentsInChildren<DSPComponent>();
					_componentInstances[0] = componentInstance;
					for (int i = 0; i < _eventListeners.Length; i++)
					{
						EventListener eventListener2 = _eventListeners[i];
						eventListener2._eventID = EventManager.GetIDFromEventName(eventListener2._eventName);
						EventManager.Instance.RegisterListener(this, eventListener2._eventName);
					}
					if (_maxInstances > 1)
					{
						if (!GetFabricManager.Instance()._bakeComponentInstances)
						{
							if (!Application.isPlaying)
							{
								int childCount = base.transform.childCount;
								for (int j = 0; j < childCount; j++)
								{
									GameObject gameObject = base.transform.GetChild(j).gameObject;
									if (gameObject.name == "Instances")
									{
										UnityEngine.Object.DestroyImmediate(gameObject);
										break;
									}
								}
							}
							_instanceHolder = new GameObject("Instances");
						}
						else
						{
							int childCount2 = base.transform.childCount;
							for (int k = 0; k < childCount2; k++)
							{
								GameObject gameObject2 = base.transform.GetChild(k).gameObject;
								if (gameObject2.name == "Instances")
								{
									_instanceHolder = gameObject2;
									break;
								}
							}
						}
						if (!GetFabricManager.Instance()._showAllInstances)
						{
							_instanceHolder.hideFlags = HideFlags.HideAndDontSave;
						}
						else
						{
							_instanceHolder.hideFlags = HideFlags.DontSave;
						}
						for (int l = 1; l < _maxInstances; l++)
						{
							ComponentInstance componentInstance2 = new ComponentInstance();
							_initializationInProgress = true;
							if (GetFabricManager.Instance()._bakeComponentInstances && _instanceHolder != null)
							{
								componentInstance2._componentGameObject = _instanceHolder.transform.GetChild(l - 1).gameObject;
							}
							else
							{
								componentInstance2._componentGameObject = UnityEngine.Object.Instantiate(base.gameObject, base.gameObject.transform.position, base.gameObject.transform.rotation);
							}
							_initializationInProgress = false;
							componentInstance2._componentInstanceHolder = this;
							componentInstance2._instance = componentInstance2._componentGameObject.GetComponent<Component>();
							componentInstance2._instance.ComponentHolder = this;
							componentInstance2._instance._isInstance = true;
							componentInstance2._instance._parentComponent = _parentComponent;
							componentInstance2._instance.name = componentInstance2._instance.name.Replace("(Clone)", "_" + l);
							componentInstance2._componentGameObject.GetComponent<Component>()._maxInstances = 0;
							componentInstance2._componentGameObject.GetComponent<Component>().CollectChildrenComponents(componentInstance2._instance, true);
							componentInstance2._parentGameObject = null;
							if (!GetFabricManager.Instance()._bakeComponentInstances)
							{
								componentInstance2._componentGameObject.transform.parent = _instanceHolder.transform;
							}
							_componentInstances[l] = componentInstance2;
							componentInstance2._instance.OnInitialise();
							componentInstance2._instance.Reset();
						}
						if (!GetFabricManager.Instance()._bakeComponentInstances)
						{
							_instanceHolder.transform.parent = base.gameObject.transform;
						}
					}
				}
				else
				{
					_eventListeners = base.gameObject.GetComponents<EventListener>();
					for (int m = 0; m < _eventListeners.Length; m++)
					{
						EventListener eventListener3 = _eventListeners[m];
						eventListener3._eventID = EventManager.GetIDFromEventName(eventListener3._eventName);
						EventManager.Instance.RegisterListener(this, eventListener3._eventName);
						UnityEngine.Object.Destroy(base.gameObject.GetComponent<EventListener>());
					}
					base.gameObject.GetComponent<Component>()._maxInstances = 1;
					_componentInstances = new ComponentInstance[1];
					ComponentInstance componentInstance3 = new ComponentInstance();
					componentInstance3._componentGameObject = base.gameObject;
					componentInstance3._componentInstanceHolder = this;
					componentInstance3._instance = this;
					componentInstance3._instance.ComponentHolder = this;
					componentInstance3._parentGameObject = null;
					_componentInstances[0] = componentInstance3;
				}
			}
			else
			{
				CollectChildrenComponents(this, isComponentInstance);
			}
			Reset();
			OnInitialise();
			_volumeMeter = base.gameObject.GetComponent<VolumeMeter>();
			if (_volumeMeter != null)
			{
				_volumeMeter.OnInitialise();
			}
			if (GetFabricManager.Instance()._bakeComponentInstances)
			{
				ApplyPropertiesToInstances();
			}
			InitialiseMusicTimingSettings();
			_isComponentActive = false;
		}

		public virtual void Destroy()
		{
			for (int i = 0; i < _componentsArray.Length; i++)
			{
				_componentsArray[i].Destroy();
			}
			UnregisterEventListeners();
			if (_virtualizationEventInstanceManager != null)
			{
				_virtualizationEventInstanceManager.Shutdown();
			}
		}

		protected void InitialiseAudioBus()
		{
			if (_audioBusName != null)
			{
				_audioBus = FabricManager.Instance._audioBusManager.FindAudioBusByName(_audioBusName);
			}
		}

		protected void InitialiseCustomCurves()
		{
			if (_customCurvesName != null && _customCurvesType == ComponentCustomCurvesType.Global)
			{
				_customCurves = FabricManager.Instance._customCurvesManager.GetCustomCurvesByName(_customCurvesName);
			}
			else
			{
				_customCurves = null;
			}
		}

		protected void InitialiseDSPWrappers()
		{
			_dspComponents = base.gameObject.GetComponents<DSPComponent>();
			if (_componentInstances != null)
			{
				for (int i = 0; i < _componentInstances.Length; i++)
				{
					for (int j = 0; j < _componentInstances[i]._instance._dspComponents.Length; j++)
					{
						_componentInstances[i]._instance._dspComponents[j].OnInitialise();
					}
				}
			}
			else if (!HasEventListener)
			{
				for (int k = 0; k < _dspComponents.Length; k++)
				{
					_dspComponents[k].OnInitialise();
				}
			}
			else
			{
				for (int l = 0; l < _dspComponents.Length; l++)
				{
					_dspComponents[l].OnInitialise(false);
				}
			}
			_sideChainComponents = base.gameObject.GetComponents<SideChain>();
			if (_RTPManager != null)
			{
				_RTPManager.Init(this);
			}
		}

		protected void UpdateDSPComponents()
		{
			if (_dspComponents != null)
			{
				for (int i = 0; i < _dspComponents.Length; i++)
				{
					_dspComponents[i].UpdateParameters();
				}
			}
			_sideChainGain = 1f;
			if (_sideChainComponents != null)
			{
				for (int j = 0; j < _sideChainComponents.Length; j++)
				{
					_sideChainGain *= _sideChainComponents[j]._sideChainGain;
				}
			}
		}

		protected virtual void OnInitialise(bool inPreviewMode = false)
		{
			_volumeOffset = 1f;
			_pitchOffset = 1f;
			_pan2DOffset = 0f;
			_updateContext._panLevel = _panLevel;
			_updateContext._spreadLevel = _spreadLevel;
			_updateContext._dopplerLevel = _dopplerLevel;
			_updateContext._minDistance = _minDistance;
			_updateContext._maxDistance = _maxDistance;
			_updateContext._rolloffMode = _rolloffMode;
			_updateContext._pan2D = _pan2D;
			_updateContext._priority = _priority;
			_updateContext._reverbZoneMix = _reverbZoneMix;
			_updateContext._bypassEffects = _bypassEffects;
			_updateContext._bypassListenerEffects = _bypassListenerEffects;
			_updateContext._bypassReverbZones = _bypassReverbZones;
			_updateContext._audioMixerGroup = _audioMixerGroup;
			InitialiseAudioBus();
			InitialiseCustomCurves();
			InitialiseDSPWrappers();
			_name = base.name;
		}

		protected virtual void OnPreInitialise(bool inPreviewMode = false)
		{
		}

		protected virtual void OnPostInitialise()
		{
		}

		protected T CopyComponent<T>(T original, T destination) where T : Component
		{
			Type type = original.GetType();
			FieldInfo[] fields = type.GetFields();
			FieldInfo[] array = fields;
			foreach (FieldInfo fieldInfo in array)
			{
				if (fieldInfo.IsDefined(typeof(SerializeField), false))
				{
					fieldInfo.SetValue(destination, fieldInfo.GetValue(original));
				}
			}
			return destination;
		}

		public void ApplyPropertiesToInstances()
		{
			if (_componentInstances == null)
			{
				return;
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			for (int i = 0; i < _componentInstances.Length; i++)
			{
				Component instance = _componentInstances[i]._instance;
				if (!instance.IsInstance)
				{
					foreach (Serialization.IField item in Serialization.EnumerateFields(instance))
					{
						dictionary[item.FieldName] = item.GetValue();
					}
				}
				else
				{
					foreach (Serialization.IField item2 in Serialization.EnumerateFields(instance))
					{
						item2.SetValue(dictionary[item2.FieldName]);
					}
				}
			}
		}

		private ComponentInstance StealOldest(GameObject parentGameObject)
		{
			int num = -1;
			float num2 = 0f;
			for (int i = 0; i < _maxInstances; i++)
			{
				ComponentInstance componentInstance = _componentInstances[i];
				float num3 = Mathf.Abs(componentInstance._triggerTime - FabricTimer.Get());
				if (componentInstance != null && num3 >= num2)
				{
					num2 = num3;
					num = i;
				}
			}
			if (num < 0)
			{
				return null;
			}
			ComponentInstance componentInstance2 = _componentInstances[num];
			if (componentInstance2 != null)
			{
				Component instance = componentInstance2._instance;
				if (instance != null)
				{
					componentInstance2._instance.Stop(false, true);
				}
			}
			return componentInstance2;
		}

		private ComponentInstance StealNewest(GameObject parentGameObject)
		{
			int num = -1;
			float num2 = float.MaxValue;
			for (int i = 0; i < _maxInstances; i++)
			{
				ComponentInstance componentInstance = _componentInstances[i];
				float num3 = Mathf.Abs(componentInstance._triggerTime - FabricTimer.Get());
				if (componentInstance != null && num3 < num2)
				{
					num2 = num3;
					num = i;
				}
			}
			if (num < 0)
			{
				return null;
			}
			ComponentInstance componentInstance2 = _componentInstances[num];
			if (componentInstance2 != null)
			{
				Component instance = componentInstance2._instance;
				if (instance != null)
				{
					instance.Stop(false, true);
				}
			}
			return componentInstance2;
		}

		private ComponentInstance StealFurthest(GameObject parentGameObject)
		{
			ComponentInstance componentInstance = null;
			List<ComponentInstance> list = FindInstances(parentGameObject, false);
			if (list.Count > 0)
			{
				list.Sort(sortByFurthestDistanceComparer);
				componentInstance = list[0];
			}
			else
			{
				for (int i = 0; i < _componentInstances.Length; i++)
				{
					list.Add(_componentInstances[i]);
				}
				list.Sort(sortByFurthestDistanceComparer);
				componentInstance = list[0];
			}
			if (componentInstance != null)
			{
				Component instance = componentInstance._instance;
				if (instance != null)
				{
					instance.Stop(false, true);
				}
			}
			return componentInstance;
		}

		internal ComponentInstance CreateInstance(GameObject parentGameObject, bool isRegisteringGameObject = false)
		{
			ComponentInstance componentInstance = null;
			if (!_multipleInstancesPerGameObject)
			{
				componentInstance = FindInstance(parentGameObject);
			}
			if (componentInstance == null)
			{
				if (GetNumActiveComponentInstances(isRegisteringGameObject) < _maxInstances)
				{
					componentInstance = GetFreeComponentInstance(parentGameObject, isRegisteringGameObject);
				}
				else
				{
					switch (_stealingBehaviour)
					{
					case ComponentStealingBehaviour.None:
						if (HasValidEventNotifier())
						{
							NotifyEvent(EventNotificationType.OnStolenNone, this);
						}
						break;
					case ComponentStealingBehaviour.Oldest:
						componentInstance = StealOldest(parentGameObject);
						if (HasValidEventNotifier())
						{
							NotifyEvent(EventNotificationType.OnStolenOldest, this);
						}
						break;
					case ComponentStealingBehaviour.Newest:
						componentInstance = StealNewest(parentGameObject);
						if (HasValidEventNotifier())
						{
							NotifyEvent(EventNotificationType.OnStolenNewest, this);
						}
						break;
					case ComponentStealingBehaviour.Farthest:
						componentInstance = StealFurthest(parentGameObject);
						if (HasValidEventNotifier())
						{
							NotifyEvent(EventNotificationType.OnStolenFarthest, this);
						}
						break;
					}
					if (componentInstance == null)
					{
						return null;
					}
					componentInstance._instance.Reset();
				}
				componentInstance._triggerTime = FabricTimer.Get();
				componentInstance._parentGameObject = parentGameObject;
				componentInstance._transform = parentGameObject.transform;
			}
			return componentInstance;
		}

		public void Play(GameObject gameObject)
		{
			ComponentInstance componentInstance = CreateInstance(gameObject);
			if (componentInstance != null)
			{
				Play(componentInstance);
			}
		}

		public void Play(double playScheduled, GameObject gameObject = null)
		{
			ComponentInstance componentInstance = CreateInstance(gameObject);
			if (componentInstance != null)
			{
				componentInstance._instance.SetPlayScheduled(playScheduled, 0.0);
				Play(componentInstance, false);
				if (HasValidEventNotifier())
				{
					NotifyEvent(EventNotificationType.OnPlay, this);
				}
			}
		}

		internal virtual void Play(ComponentInstance zComponentInstance, bool resetPlayScheduled = true)
		{
			if (resetPlayScheduled)
			{
				zComponentInstance._instance.ResetPlayScheduled();
			}
			PlayInternal(zComponentInstance, _fadeInTime, _fadeInCurve);
			if (HasValidEventNotifier())
			{
				NotifyEvent(EventNotificationType.OnPlay, this);
			}
		}

		public virtual void PlayInternal(ComponentInstance zComponentInstance, float target, float curve, bool dontPlayComponents = false)
		{
			ApplyMIDI(zComponentInstance);
			ApplyRandomization();
			float num;
			float curve2;
			if (_overrideFadeProperties || _parentComponent == null)
			{
				num = _fadeInTime;
				curve2 = _fadeInCurve;
			}
			else
			{
				num = target + _fadeInTime;
				curve2 = curve;
			}
			_componentInstance = zComponentInstance;
			if (_fadeParameter.HasReachedTarget())
			{
				_fadeParameter.Reset(0f);
			}
			_fadeParameter.SetTarget(FabricTimer.Get(), 1f, num, curve2);
			if (_RTPManager != null)
			{
				_RTPManager.Reset();
			}
			SetComponentActive(true);
			if (!dontPlayComponents)
			{
				for (int i = 0; i < _componentsArray.Length; i++)
				{
					_componentsArray[i].PlayInternal(zComponentInstance, num, curve2);
				}
			}
			if (_musicTimeResetOnPlay && _activeMusicTimeSettings != null)
			{
				_activeMusicTimeSettings.SetNextBeatBarEvents();
			}
			_componentStatus = ComponentStatus.Playing;
		}

		public void Stop()
		{
			Stop(false, false, false, 0f);
		}

		public void Stop(bool stopInstances)
		{
			Stop(stopInstances, false, false, 0f);
		}

		public void Stop(bool stopInstances, bool forceStop)
		{
			Stop(stopInstances, forceStop, false, 0f);
		}

		public void Stop(bool stopInstances, bool forceStop, bool ignoreFade)
		{
			Stop(stopInstances, forceStop, ignoreFade, 0f);
		}

		public virtual void Stop(bool stopInstances, bool forceStop, bool ignoreFade, float fadeTime)
		{
			StopInternal(stopInstances, forceStop, _fadeOutTime, _fadeOutCurve);
		}

		public virtual void StopInternal(bool stopInstances, bool forceStop, float target, float curve, double scheduleEnd = 0.0)
		{
			Reset();
			float num;
			float curve2;
			if (_overrideFadeProperties || _parentComponent == null)
			{
				num = _fadeOutTime;
				curve2 = _fadeOutCurve;
			}
			else
			{
				num = target + _fadeOutTime;
				curve2 = curve;
			}
			if (scheduleEnd == 0.0)
			{
				_fadeParameter.SetTarget(FabricTimer.Get(), 0f, num, curve2);
			}
			for (int i = 0; i < _componentsArray.Length; i++)
			{
				_componentsArray[i].StopInternal(stopInstances, forceStop, num, curve2, scheduleEnd);
			}
			if (_componentInstances != null && stopInstances)
			{
				for (int j = 0; j < _componentInstances.Length; j++)
				{
					ComponentInstance componentInstance = _componentInstances[j];
					if (componentInstance._instance._isInstance)
					{
						componentInstance._instance.StopInternal(stopInstances, forceStop, num, curve2, scheduleEnd);
					}
				}
			}
			_initialiseParameters = null;
			if (_componentStatus != ComponentStatus.Stopped)
			{
				_componentStatus = ComponentStatus.Stopping;
			}
		}

		public virtual void Pause(bool pause)
		{
			for (int i = 0; i < _componentsArray.Length; i++)
			{
				_componentsArray[i].Pause(pause);
			}
			if (_componentInstances == null)
			{
				return;
			}
			for (int j = 0; j < _componentInstances.Length; j++)
			{
				ComponentInstance componentInstance = _componentInstances[j];
				if (componentInstance._instance._isInstance)
				{
					componentInstance._instance.Pause(pause);
				}
			}
		}

		public virtual void SetTime(float time)
		{
			for (int i = 0; i < _componentsArray.Length; i++)
			{
				_componentsArray[i].SetTime(time);
			}
			if (_componentInstances == null)
			{
				return;
			}
			for (int j = 0; j < _componentInstances.Length; j++)
			{
				ComponentInstance componentInstance = _componentInstances[j];
				if (componentInstance._instance._isInstance)
				{
					componentInstance._instance.SetTime(time);
				}
			}
		}

		public virtual void AdvanceTime(float time, float length)
		{
			for (int i = 0; i < _componentsArray.Length; i++)
			{
				_componentsArray[i].AdvanceTime(time, length);
			}
			if (_componentInstances == null)
			{
				return;
			}
			for (int j = 0; j < _componentInstances.Length; j++)
			{
				ComponentInstance componentInstance = _componentInstances[j];
				if (componentInstance._instance._isInstance)
				{
					componentInstance._instance.AdvanceTime(time, length);
				}
			}
		}

		public void SetVolume(float volume)
		{
			_runtimeProperties._volume = volume;
			if (_componentInstances == null)
			{
				return;
			}
			for (int i = 0; i < _componentInstances.Length; i++)
			{
				ComponentInstance componentInstance = _componentInstances[i];
				if (componentInstance._instance._isInstance)
				{
					componentInstance._instance.SetVolume(volume);
				}
			}
		}

		public void SetPitch(float pitch)
		{
			_runtimeProperties._pitch = pitch;
			if (_componentInstances == null)
			{
				return;
			}
			for (int i = 0; i < _componentInstances.Length; i++)
			{
				ComponentInstance componentInstance = _componentInstances[i];
				if (componentInstance._instance._isInstance)
				{
					componentInstance._instance.SetPitch(pitch);
				}
			}
		}

		public void SetPan(float pan)
		{
			_runtimeProperties._pan2D = pan;
			if (_componentInstances == null)
			{
				return;
			}
			for (int i = 0; i < _componentInstances.Length; i++)
			{
				ComponentInstance componentInstance = _componentInstances[i];
				if (componentInstance._instance._isInstance)
				{
					componentInstance._instance.SetPan(pan);
				}
			}
		}

		public virtual void SetProperty(string name, object value)
		{
			if (name.Contains("volume"))
			{
				_volume = (float)value;
			}
			else if (name.Contains("pitch"))
			{
				_pitch = (float)value;
			}
		}

		public void FadeIn(float targetMS, float curve)
		{
			ApplyFadeIn(targetMS, curve);
		}

		private void ApplyFadeIn(float target, float curve, bool onlyApplyFade = false)
		{
			float num;
			float curve2;
			if (_overrideFadeProperties || _parentComponent == null)
			{
				num = _fadeInTime;
				curve2 = _fadeInCurve;
			}
			else
			{
				num = target + _fadeInTime;
				curve2 = curve;
			}
			_fadeParameter.SetTarget(FabricTimer.Get(), 1f, num, curve2);
			if (onlyApplyFade)
			{
				return;
			}
			if (_componentInstances != null)
			{
				for (int i = 0; i < _componentInstances.Length; i++)
				{
					ComponentInstance componentInstance = _componentInstances[i];
					if (componentInstance._instance._isInstance)
					{
						componentInstance._instance.ApplyFadeIn(num, curve2);
					}
				}
			}
			for (int j = 0; j < _componentsArray.Length; j++)
			{
				_componentsArray[j].ApplyFadeIn(num, curve2);
			}
		}

		public void FadeOut(float targetMS, float curve)
		{
			ApplyFadeOut(targetMS, curve);
		}

		private void ApplyFadeOut(float target, float curve, bool onlyApplyFade = false)
		{
			float num;
			float curve2;
			if (_overrideFadeProperties || _parentComponent == null)
			{
				num = _fadeOutTime;
				curve2 = _fadeOutCurve;
			}
			else
			{
				num = target + _fadeOutTime;
				curve2 = curve;
			}
			_fadeParameter.SetTarget(FabricTimer.Get(), 0f, num, curve2);
			if (onlyApplyFade)
			{
				return;
			}
			if (_componentInstances != null)
			{
				for (int i = 0; i < _componentInstances.Length; i++)
				{
					ComponentInstance componentInstance = _componentInstances[i];
					if (componentInstance._instance._isInstance)
					{
						componentInstance._instance.ApplyFadeOut(num, curve2);
					}
				}
			}
			for (int j = 0; j < _componentsArray.Length; j++)
			{
				_componentsArray[j].ApplyFadeOut(num, curve2);
			}
		}

		private void ApplyRandomization()
		{
			if (_pitchRandomization > 0f)
			{
				int num = (int)(12f * Mathf.Log(_pitchRandomization + 1f, 2f));
				int num2 = _random.Next(-num, num);
				float f = Mathf.Pow(2f, 0.0833333358f);
				_pitchOffset = Mathf.Pow(f, num2);
			}
			if (_volumeRandomization > 0f)
			{
				float num3 = AudioTools.LinearToDB(1f - _volumeRandomization);
				float db = (float)(_random.NextDouble() * (double)num3);
				_volumeOffset = AudioTools.DBToLinear(db);
			}
			if ((_override2DProperties || _parentComponent == null) && _pan2DRandomization != 0f)
			{
				_pan2DOffset = (float)(_random.NextDouble() * 2.0) + (0f - _pan2DRandomization);
			}
		}

		protected virtual void Reset()
		{
			_updateContext.Reset();
			_runtimeProperties.Reset();
			_rtpProperties.Reset();
			_playScheduled = 0.0;
			_playScheduledDelay = 0.0;
			_scheduledDspTime = 0.0;
			_updatePropertiesFlag = false;
			_componentStatus = ComponentStatus.Stopped;
		}

		private void AddVirtualizationEventInstance(VirtualizationEventInstance eventInstance)
		{
			_componentVirtualizationEvents.Add(eventInstance);
			if (!_componentVirtualizationActive)
			{
				_componentVirtualizationActive = true;
			}
		}

		private void RemoveVirtualizationEventInstance(VirtualizationEventInstance eventInstance)
		{
			_componentVirtualizationEvents.Remove(eventInstance);
			_virtualizationEventInstanceManager.Free(eventInstance);
			if (_componentVirtualizationEvents.Count == 0 && _componentVirtualizationActive)
			{
				_componentVirtualizationActive = false;
			}
		}

		EventStatus IEventListener.Process(Event zEvent)
		{
			if (_componentVirtualization && IsLooped())
			{
				if (zEvent.EventAction == EventAction.PlaySound)
				{
					VirtualizationEventInstance virtualizationEventInstance = _virtualizationEventInstanceManager.Alloc(zEvent);
					if (virtualizationEventInstance != null)
					{
						AddVirtualizationEventInstance(virtualizationEventInstance);
						SetComponentActive(true);
						return EventStatus.Handled_Virtualized;
					}
					return EventStatus.Failed_NotEnoughVirtualEvents;
				}
				if (zEvent.EventAction == EventAction.StopSound)
				{
					for (int i = 0; i < _componentVirtualizationEvents.Count; i++)
					{
						VirtualizationEventInstance virtualizationEventInstance2 = _componentVirtualizationEvents[i];
						if (virtualizationEventInstance2._event.parentGameObject == zEvent.parentGameObject)
						{
							RemoveVirtualizationEventInstance(virtualizationEventInstance2);
						}
					}
				}
				else if (zEvent.EventAction == EventAction.StopAll)
				{
					for (int j = 0; j < _componentVirtualizationEvents.Count; j++)
					{
						_virtualizationEventInstanceManager.Free(_componentVirtualizationEvents[j]);
					}
					_componentVirtualizationEvents.Clear();
					_componentVirtualizationActive = false;
				}
			}
			ComponentInstance newComponentInstance = null;
			return ProcessEvent(zEvent, ref newComponentInstance);
		}

		private bool CanPlay(ref EventStatus status)
		{
			if (!_componentVirtualization && _overrideVolumeThreshold)
			{
				UpdateParentProperties();
				if (_updateContext._volume <= _volumeThreshold)
				{
					status = EventStatus.Not_Handled_VolumeThreshold;
					return false;
				}
			}
			return true;
		}

		private bool CheckProbability()
		{
			if (_probability < 100)
			{
				System.Random random = Generic._random;
				int num = (int)(random.NextDouble() * 100.0);
				if (num > _probability)
				{
					return true;
				}
			}
			return false;
		}

		private EventStatus ProcessEvent(Event zEvent, ref ComponentInstance newComponentInstance)
		{
			EventStatus status = EventStatus.Failed_Uknown;
			if (_globalParentGameObject != null)
			{
				zEvent.parentGameObject = _globalParentGameObject;
			}
			else if (zEvent.parentGameObject == null)
			{
				zEvent.parentGameObject = FabricManager.Instance.gameObject;
			}
			GameObject parentGameObject = zEvent.parentGameObject;
			List<ComponentInstance> list = null;
			switch (zEvent.EventAction)
			{
			case EventAction.PlaySound:
			case EventAction.PlayScheduled:
			{
				if (_minimumPlaybackInterval > 0f && FabricTimer.Get() < _minimumPlaybackIntervalTimeStamp)
				{
					if (zEvent._onEventNotify != null)
					{
						_onEventNotify += zEvent._onEventNotify;
						OnEventNotifyInvoker(EventNotificationType.OnPlayNotHandled, zEvent._eventName, null, null);
						_onEventNotify -= zEvent._onEventNotify;
					}
					return EventStatus.Not_Handled_MinimumPlaybackInterval;
				}
				if (CheckProbability())
				{
					if (zEvent._onEventNotify != null)
					{
						_onEventNotify += zEvent._onEventNotify;
						OnEventNotifyInvoker(EventNotificationType.OnPlayNotHandled, zEvent._eventName, null, null);
						_onEventNotify -= zEvent._onEventNotify;
					}
					return EventStatus.Not_Handled_MinimumPlaybackInterval;
				}
				ComponentInstance componentInstance18 = CreateInstance(parentGameObject);
				if (componentInstance18 == null)
				{
					break;
				}
				Component instance = componentInstance18._instance;
				instance.SetInitialiseParameters(zEvent._initialiseParameters);
				componentInstance18._instance.UpdateParentProperties();
				instance._onEventNotifyEventName = "";
				if (zEvent._onEventNotify != null)
				{
					instance._onEventNotify += zEvent._onEventNotify;
					instance._onEventNotifyEventName = zEvent._eventName;
				}
				if (!instance.CanPlay(ref status))
				{
					if (HasValidEventNotifier())
					{
						NotifyEvent(EventNotificationType.OnPlayNotHandled, this);
					}
					return status;
				}
				if (zEvent.EventAction == EventAction.PlayScheduled)
				{
					componentInstance18._instance.SetPlayScheduled((double)zEvent._parameter, 0.0);
				}
				else
				{
					componentInstance18._instance.ResetPlayScheduled();
				}
				instance.Play(componentInstance18, false);
				if (instance.IsPlaying())
				{
					status = EventStatus.Handled;
					_minimumPlaybackIntervalTimeStamp = FabricTimer.Get() + _minimumPlaybackInterval;
				}
				else if (HasValidEventNotifier())
				{
					NotifyEvent(EventNotificationType.OnPlayNotHandled, this);
				}
				newComponentInstance = componentInstance18;
				break;
			}
			case EventAction.StopSound:
				list = FindInstances(parentGameObject, false);
				if (list != null && list.Count > 0)
				{
					for (int num11 = 0; num11 < list.Count; num11++)
					{
						ComponentInstance componentInstance15 = list[num11];
						componentInstance15._instance.Stop();
						status = EventStatus.Handled;
					}
				}
				break;
			case EventAction.StopScheduled:
				list = FindInstances(parentGameObject, false);
				if (list != null && list.Count > 0)
				{
					for (int num13 = 0; num13 < list.Count; num13++)
					{
						ComponentInstance componentInstance17 = list[num13];
						double scheduleEnd = (double)zEvent._parameter;
						componentInstance17._instance.StopInternal(false, false, _fadeOutTime, _fadeOutCurve, scheduleEnd);
						status = EventStatus.Handled;
					}
				}
				break;
			case EventAction.StopAll:
				Stop(true, false);
				break;
			case EventAction.PauseSound:
				list = FindInstances(parentGameObject, false);
				if (list != null && list.Count > 0)
				{
					for (int num6 = 0; num6 < list.Count; num6++)
					{
						ComponentInstance componentInstance10 = list[num6];
						componentInstance10._instance.Pause(true);
						status = EventStatus.Handled;
					}
				}
				else
				{
					Pause(true);
					status = EventStatus.Handled;
				}
				break;
			case EventAction.UnpauseSound:
				list = FindInstances(parentGameObject, false);
				if (list != null && list.Count > 0)
				{
					for (int j = 0; j < list.Count; j++)
					{
						ComponentInstance componentInstance2 = list[j];
						componentInstance2._instance.Pause(false);
						status = EventStatus.Handled;
					}
				}
				else
				{
					Pause(false);
					status = EventStatus.Handled;
				}
				break;
			case EventAction.SetVolume:
				list = FindInstances(parentGameObject, false);
				if (list != null && list.Count > 0)
				{
					for (int num7 = 0; num7 < list.Count; num7++)
					{
						ComponentInstance componentInstance11 = list[num7];
						componentInstance11._instance._runtimeProperties._volume = (float)zEvent._parameter;
						status = EventStatus.Handled;
					}
				}
				else
				{
					SetVolume((float)zEvent._parameter);
					status = EventStatus.Handled;
				}
				break;
			case EventAction.SetPitch:
				list = FindInstances(parentGameObject, false);
				if (list != null && list.Count > 0)
				{
					for (int m = 0; m < list.Count; m++)
					{
						ComponentInstance componentInstance5 = list[m];
						componentInstance5._instance._runtimeProperties._pitch = (float)zEvent._parameter;
						status = EventStatus.Handled;
					}
				}
				else
				{
					SetPitch((float)zEvent._parameter);
					status = EventStatus.Handled;
				}
				break;
			case EventAction.SetPan:
				list = FindInstances(parentGameObject, false);
				if (list != null && list.Count > 0)
				{
					for (int k = 0; k < list.Count; k++)
					{
						ComponentInstance componentInstance3 = list[k];
						componentInstance3._instance._runtimeProperties._pan2D = (float)zEvent._parameter;
						status = EventStatus.Handled;
					}
				}
				else
				{
					SetPan((float)zEvent._parameter);
					status = EventStatus.Handled;
				}
				break;
			case EventAction.SetTime:
				list = FindInstances(parentGameObject, false);
				if (list != null && list.Count > 0)
				{
					for (int num9 = 0; num9 < list.Count; num9++)
					{
						ComponentInstance componentInstance13 = list[num9];
						componentInstance13._instance.SetTime((float)zEvent._parameter);
						status = EventStatus.Handled;
					}
				}
				else
				{
					SetTime((float)zEvent._parameter);
					status = EventStatus.Handled;
				}
				break;
			case EventAction.SetFadeIn:
				list = FindInstances(parentGameObject, false);
				if (list != null && list.Count > 0)
				{
					for (int n = 0; n < list.Count; n++)
					{
						ComponentInstance componentInstance6 = list[n];
						componentInstance6._instance.ApplyFadeIn(_fadeInTime, _fadeInCurve, true);
						status = EventStatus.Handled;
					}
				}
				else
				{
					ApplyFadeIn(_fadeInTime, _fadeInCurve);
					status = EventStatus.Handled;
				}
				break;
			case EventAction.SetFadeOut:
				list = FindInstances(parentGameObject, false);
				if (list != null && list.Count > 0)
				{
					for (int num12 = 0; num12 < list.Count; num12++)
					{
						ComponentInstance componentInstance16 = list[num12];
						componentInstance16._instance.ApplyFadeOut(_fadeOutTime, _fadeOutCurve, true);
						status = EventStatus.Handled;
					}
				}
				else
				{
					ApplyFadeOut(_fadeOutTime, _fadeOutCurve);
					status = EventStatus.Handled;
				}
				break;
			case EventAction.SetSwitch:
				list = FindInstances(parentGameObject);
				break;
			case EventAction.SetParameter:
				list = FindInstances(parentGameObject, false);
				if (list != null && list.Count > 0)
				{
					for (int num5 = 0; num5 < list.Count; num5++)
					{
						ComponentInstance componentInstance9 = list[num5];
						if (componentInstance9._instance._RTPManager != null)
						{
							status = componentInstance9._instance._RTPManager.SetParameter(zEvent);
						}
					}
				}
				else if (_RTPManager != null)
				{
					status = _RTPManager.SetParameter(zEvent);
				}
				break;
			case EventAction.SetMarker:
				list = FindInstances(parentGameObject, false);
				if (list != null && list.Count > 0)
				{
					for (int num14 = 0; num14 < list.Count; num14++)
					{
						ComponentInstance componentInstance19 = list[num14];
						if (componentInstance19._instance._RTPManager != null)
						{
							status = componentInstance19._instance._RTPManager.SetMarker(zEvent);
						}
					}
				}
				else if (_RTPManager != null)
				{
					status = _RTPManager.SetMarker(zEvent);
				}
				break;
			case EventAction.KeyOffMarker:
				list = FindInstances(parentGameObject, false);
				if (list != null && list.Count > 0)
				{
					for (int num10 = 0; num10 < list.Count; num10++)
					{
						ComponentInstance componentInstance14 = list[num10];
						if (componentInstance14._instance._RTPManager != null)
						{
							status = componentInstance14._instance._RTPManager.KeyOffMarker(zEvent);
						}
					}
				}
				else if (_RTPManager != null)
				{
					status = _RTPManager.KeyOffMarker(zEvent);
				}
				break;
			case EventAction.SetDSPParameter:
			{
				list = FindInstances(parentGameObject, false);
				DSPParameterData dSPParameterData = (DSPParameterData)zEvent._parameter;
				if (list != null && list.Count > 0)
				{
					for (int num = 0; num < list.Count; num++)
					{
						ComponentInstance componentInstance7 = list[num];
						for (int num2 = 0; num2 < componentInstance7._instance._dspComponents.Length; num2++)
						{
							if (dSPParameterData._dspType == componentInstance7._instance._dspComponents[num2].Type)
							{
								componentInstance7._instance._dspComponents[num2].SetParameterValue(dSPParameterData._parameter, dSPParameterData._value, dSPParameterData._time, dSPParameterData._curve);
								status = EventStatus.Handled;
							}
						}
					}
					break;
				}
				for (int num3 = 0; num3 < _dspComponents.Length; num3++)
				{
					if (dSPParameterData._dspType == _dspComponents[num3].Type)
					{
						_dspComponents[num3].SetParameterValue(dSPParameterData._parameter, dSPParameterData._value, dSPParameterData._time, dSPParameterData._curve);
						status = EventStatus.Handled;
					}
				}
				break;
			}
			case EventAction.RegisterGameObject:
				list = FindInstances(parentGameObject, false);
				if (list.Count == 0)
				{
					RegisterGameObject(parentGameObject);
				}
				status = EventStatus.Handled;
				break;
			case EventAction.UnregisterGameObject:
				list = FindInstances(parentGameObject, false);
				if (list.Count > 0)
				{
					UnregisterGameObject(parentGameObject);
				}
				status = EventStatus.Handled;
				break;
			case EventAction.SetGameObject:
				_globalParentGameObject = parentGameObject;
				status = EventStatus.Handled;
				break;
			case EventAction.UnsetGameObject:
				_globalParentGameObject = null;
				status = EventStatus.Handled;
				break;
			case EventAction.AddPreset:
				if (GetDynamicMixer.Instance() != null)
				{
					GetDynamicMixer.Instance().AddPreset((string)zEvent._parameter);
					status = EventStatus.Handled;
				}
				break;
			case EventAction.RemovePreset:
				if (GetDynamicMixer.Instance() != null)
				{
					GetDynamicMixer.Instance().RemovePreset((string)zEvent._parameter);
					status = EventStatus.Handled;
				}
				break;
			case EventAction.ResetDynamicMixer:
				if (GetDynamicMixer.Instance() != null)
				{
					GetDynamicMixer.Instance().Reset();
					status = EventStatus.Handled;
				}
				break;
			case EventAction.SwitchPreset:
				if (GetDynamicMixer.Instance() != null)
				{
					GetDynamicMixer.Instance().SwitchPreset((string)zEvent._parameter);
					status = EventStatus.Handled;
				}
				break;
			case EventAction.LoadAudio:
				list = FindInstances(parentGameObject, false);
				if (list.Count == 0)
				{
					RegisterGameObject(parentGameObject);
				}
				list = FindInstances(parentGameObject, false);
				if (list != null && list.Count > 0)
				{
					for (int num8 = 0; num8 < list.Count; num8++)
					{
						ComponentInstance componentInstance12 = list[num8];
						componentInstance12._instance.LoadAudio();
					}
				}
				break;
			case EventAction.UnloadAudio:
				list = FindInstances(parentGameObject, false);
				if (list != null && list.Count > 0)
				{
					for (int num4 = 0; num4 < list.Count; num4++)
					{
						ComponentInstance componentInstance8 = list[num4];
						componentInstance8._instance.UnloadAudio();
					}
				}
				break;
			case EventAction.SetVolumeProperty:
				list = FindInstances(parentGameObject, false);
				if (list != null && list.Count > 0)
				{
					for (int l = 0; l < list.Count; l++)
					{
						ComponentInstance componentInstance4 = list[l];
						componentInstance4._instance.SetProperty("volume", zEvent._parameter);
						status = EventStatus.Handled;
					}
				}
				else
				{
					SetProperty("volume", zEvent._parameter);
					status = EventStatus.Handled;
				}
				break;
			case EventAction.SetPitchProperty:
				list = FindInstances(parentGameObject, false);
				if (list != null && list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						ComponentInstance componentInstance = list[i];
						componentInstance._instance.SetProperty("pitch", zEvent._parameter);
						status = EventStatus.Handled;
					}
				}
				else
				{
					SetProperty("pitch", zEvent._parameter);
					status = EventStatus.Handled;
				}
				break;
			}
			if (list == null)
			{
				if (status == EventStatus.Failed_Uknown)
				{
					return OnProcessEvent(zEvent, _componentInstances[0]);
				}
				return status;
			}
			bool flag = true;
			for (int num15 = 0; num15 < list.Count; num15++)
			{
				EventStatus eventStatus = OnProcessEvent(zEvent, list[num15]);
				if (eventStatus >= EventStatus.Failed_Uknown)
				{
					flag = flag;
				}
			}
			if (!flag)
			{
				status = EventStatus.Failed_Uknown;
			}
			return EventStatus.Handled;
		}

		private void RegisterGameObject(GameObject eventGameObject)
		{
			ComponentInstance componentInstance = CreateInstance(eventGameObject, true);
			if (componentInstance != null)
			{
				componentInstance._parentGameObject = eventGameObject;
				componentInstance._instance._componentInstance = componentInstance;
			}
		}

		private void UnregisterGameObject(GameObject eventGameObject)
		{
			ComponentInstance componentInstance = FindInstance(eventGameObject);
			if (componentInstance != null)
			{
				componentInstance._parentGameObject = null;
				componentInstance._instance._componentInstance = null;
				componentInstance._instance.Reset();
			}
		}

		bool IEventListener.IsActive(GameObject parentGameObject)
		{
			if (parentGameObject == null)
			{
				return IsComponentActive();
			}
			return IsPlaying(parentGameObject);
		}

		bool IEventListener.GetEventInfo(GameObject parentGameObject, ref EventInfo eventInfo)
		{
			if (parentGameObject == null)
			{
				eventInfo._numOfActiveInstances += GetNumActiveComponentInstances();
				eventInfo._numOfVirtualInstances += GetNumVirtualEventInstances();
				return true;
			}
			List<ComponentInstance> list = FindInstances(parentGameObject, false);
			if (list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					ComponentInstance componentInstance = list[i];
					if (componentInstance._instance.IsComponentActive())
					{
						eventInfo._numOfActiveInstances++;
					}
				}
			}
			return true;
		}

		bool IEventListener.GetEventListeners(string eventName, List<EventListener> listeners)
		{
			listeners.Clear();
			if (_eventListeners != null && _eventListeners.Length > 0)
			{
				for (int i = 0; i < _eventListeners.Length; i++)
				{
					EventListener eventListener = _eventListeners[i];
					if ((bool)eventListener && eventListener._eventName == eventName)
					{
						listeners.Add(eventListener);
					}
				}
			}
			if (listeners.Count > 0)
			{
				return true;
			}
			return false;
		}

		bool IEventListener.GetEventListeners(int eventID, List<EventListener> listeners)
		{
			listeners.Clear();
			if (_eventListeners != null && _eventListeners.Length > 0)
			{
				for (int i = 0; i < _eventListeners.Length; i++)
				{
					EventListener eventListener = _eventListeners[i];
					if ((bool)eventListener && eventListener._eventID == eventID)
					{
						listeners.Add(eventListener);
					}
				}
			}
			if (listeners.Count > 0)
			{
				return true;
			}
			return false;
		}

		public void UnregisterEventListeners()
		{
			if (EventManager.IsInitialised() && _eventListeners != null)
			{
				for (int i = 0; i < _eventListeners.Length; i++)
				{
					EventManager.Instance.UnregisterListener(this, _eventListeners[i]._eventName);
				}
			}
		}

		public virtual EventStatus OnProcessEvent(Event zEvent, ComponentInstance zInstance)
		{
			return EventStatus.Not_Handled;
		}

		protected void ApplyInitialiseParameters(ref Context context)
		{
			if (_initialiseParameters != null && _initialiseParameters._isMutliplier)
			{
				if (_initialiseParameters._volume.IsDirty)
				{
					context._volume *= _initialiseParameters._volume.Value;
				}
				if (_initialiseParameters._pitch.IsDirty)
				{
					context._pitch *= _initialiseParameters._pitch.Value;
				}
				if (_initialiseParameters._panLevel.IsDirty)
				{
					context._panLevel *= _initialiseParameters._panLevel.Value;
				}
				if (_initialiseParameters._pan2D.IsDirty)
				{
					context._pan2D *= _initialiseParameters._pan2D.Value;
				}
				if (_initialiseParameters._spreadLevel.IsDirty)
				{
					context._spreadLevel *= _initialiseParameters._spreadLevel.Value;
				}
				if (_initialiseParameters._dopplerLevel.IsDirty)
				{
					context._dopplerLevel *= _initialiseParameters._dopplerLevel.Value;
				}
				if (_initialiseParameters._minDistance.IsDirty)
				{
					context._minDistance *= _initialiseParameters._minDistance.Value;
				}
				if (_initialiseParameters._maxDistance.IsDirty)
				{
					context._maxDistance *= _initialiseParameters._maxDistance.Value;
				}
			}
		}

		private void SetInitialiseParameters(InitialiseParameters initialiseParameters)
		{
			_initialiseParameters = initialiseParameters;
			if (_initialiseParameters != null && !_initialiseParameters._isMutliplier)
			{
				if (initialiseParameters._volume.IsDirty)
				{
					_runtimeProperties._volume = initialiseParameters._volume.Value;
				}
				if (initialiseParameters._volumeRandomization.IsDirty)
				{
					_volumeRandomization = initialiseParameters._volumeRandomization.Value;
				}
				if (initialiseParameters._fadeInTime.IsDirty)
				{
					_fadeInTime = initialiseParameters._fadeInTime.Value;
				}
				if (initialiseParameters._fadeInTime.IsDirty)
				{
					_fadeInTime = initialiseParameters._fadeInTime.Value;
				}
				if (initialiseParameters._fadeOutTime.IsDirty)
				{
					_fadeOutTime = initialiseParameters._fadeOutTime.Value;
				}
				if (initialiseParameters._pitch.IsDirty)
				{
					_runtimeProperties._pitch = initialiseParameters._pitch.Value;
				}
				if (initialiseParameters._pitchRandomization.IsDirty)
				{
					_pitchRandomization = initialiseParameters._pitchRandomization.Value;
				}
				if (initialiseParameters._panLevel.IsDirty)
				{
					_panLevel = initialiseParameters._panLevel.Value;
				}
				if (initialiseParameters._pan2D.IsDirty)
				{
					_runtimeProperties._pan2D = initialiseParameters._pan2D.Value;
				}
				if (initialiseParameters._spreadLevel.IsDirty)
				{
					_spreadLevel = initialiseParameters._spreadLevel.Value;
				}
				if (initialiseParameters._dopplerLevel.IsDirty)
				{
					_dopplerLevel = initialiseParameters._dopplerLevel.Value;
				}
				if (initialiseParameters._minDistance.IsDirty)
				{
					_minDistance = initialiseParameters._minDistance.Value;
				}
				if (initialiseParameters._maxDistance.IsDirty)
				{
					_maxDistance = initialiseParameters._maxDistance.Value;
				}
				if (initialiseParameters._rolloffMode.IsDirty)
				{
					_rolloffMode = initialiseParameters._rolloffMode.Value;
				}
				if (initialiseParameters._delaySamples.IsDirty)
				{
					_delaySamples = initialiseParameters._delaySamples.Value;
				}
			}
		}

		public virtual bool IsPlaying()
		{
			_fadeParameter.Get(FabricTimer.Get());
			if (_fadeParameter.HasReachedTarget() && _componentStatus == ComponentStatus.Stopped)
			{
				return false;
			}
			return _isComponentActive;
		}

		public bool IsVirtualizationActive()
		{
			if (!_componentVirtualization || !_componentVirtualizationActive)
			{
				return false;
			}
			return true;
		}

		public virtual bool IsComponentActive()
		{
			_fadeParameter.Get(FabricTimer.Get());
			if ((!_fadeParameter.HasReachedTarget() && _componentStatus != ComponentStatus.Stopped) || IsVirtualizationActive())
			{
				return true;
			}
			return _isComponentActive;
		}

		public bool IsPlaying(GameObject parentGameObject)
		{
			List<ComponentInstance> list = FindInstances(parentGameObject, false);
			if (list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					ComponentInstance componentInstance = list[i];
					if (componentInstance._instance.IsPlaying())
					{
						return true;
					}
				}
			}
			return false;
		}

		public virtual bool IsOneShot()
		{
			for (int i = 0; i < _componentsArray.Length; i++)
			{
				Component component = _componentsArray[i];
				if (component.IsOneShot())
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool IsLooped()
		{
			for (int i = 0; i < _componentsArray.Length; i++)
			{
				Component component = _componentsArray[i];
				if (component.IsLooped())
				{
					return true;
				}
			}
			return false;
		}

		public virtual int GetTimeSamples()
		{
			int num = -1;
			for (int i = 0; i < _componentsArray.Length; i++)
			{
				Component component = _componentsArray[i];
				if (component.IsPlaying())
				{
					int timeSamples = component.GetTimeSamples();
					if (timeSamples > num)
					{
						num = timeSamples;
					}
				}
			}
			return num;
		}

		public virtual double GetTime()
		{
			double num = 0.0;
			for (int i = 0; i < _componentsArray.Length; i++)
			{
				Component component = _componentsArray[i];
				double time = component.GetTime();
				if (time > num)
				{
					num = time;
				}
			}
			return num;
		}

		public virtual double GetCurrentTime(bool returnFirstInstance = true)
		{
			double num = 0.0;
			for (int i = 0; i < _componentsArray.Length; i++)
			{
				Component component = _componentsArray[i];
				if (component.IsComponentActive())
				{
					if (returnFirstInstance)
					{
						return component.GetCurrentTime();
					}
					double currentTime = component.GetCurrentTime();
					if (currentTime > num)
					{
						num = currentTime;
					}
				}
			}
			return num;
		}

		public virtual int GetSampleRate()
		{
			int result = -1;
			for (int i = 0; i < _componentsArray.Length; i++)
			{
				Component component = _componentsArray[i];
				if (component.IsPlaying())
				{
					result = component.GetSampleRate();
				}
			}
			return result;
		}

		public virtual float GetLength()
		{
			float result = 0f;
			for (int i = 0; i < _componentsArray.Length; i++)
			{
				Component component = _componentsArray[i];
				if (component.IsPlaying())
				{
					result = component.GetLength();
				}
			}
			return result;
		}

		public virtual bool HasReachedEnd()
		{
			for (int i = 0; i < _componentsArray.Length; i++)
			{
				Component component = _componentsArray[i];
				if (component.HasReachedEnd())
				{
					return true;
				}
			}
			return false;
		}

		public virtual void LoadAudio()
		{
			for (int i = 0; i < _componentsArray.Length; i++)
			{
				Component component = _componentsArray[i];
				component.LoadAudio();
			}
		}

		public virtual void UnloadAudio()
		{
			for (int i = 0; i < _componentsArray.Length; i++)
			{
				Component component = _componentsArray[i];
				component.UnloadAudio();
			}
		}

		public void CopyPropertiesFrom(Component sourceComponent)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (Serialization.IField item in Serialization.EnumerateFields(sourceComponent))
			{
				dictionary[item.FieldName] = item.GetValue();
			}
			foreach (Serialization.IField item2 in Serialization.EnumerateFields(this))
			{
				if (item2 != null && dictionary.ContainsKey(item2.FieldName))
				{
					item2.SetValue(dictionary[item2.FieldName]);
				}
			}
		}

		public int GetNumActiveComponentInstances(bool checkRegisteredGameObject = false)
		{
			int num = 0;
			if (_componentInstances != null)
			{
				for (int i = 0; i < _componentInstances.Length; i++)
				{
					ComponentInstance componentInstance = _componentInstances[i];
					if (componentInstance != null && (componentInstance._instance.IsPlaying() || (checkRegisteredGameObject && componentInstance._parentGameObject != null)))
					{
						num++;
					}
				}
			}
			return num;
		}

		public int GetNumVirtualEventInstances()
		{
			return _componentVirtualizationEvents.Count;
		}

		private ComponentInstance GetFreeComponentInstance(GameObject gameObject, bool checkRegisteredGameObject)
		{
			if (_componentInstances != null)
			{
				for (int i = 0; i < _componentInstances.Length; i++)
				{
					ComponentInstance componentInstance = _componentInstances[i];
					if ((!checkRegisteredGameObject || !(componentInstance._parentGameObject != null)) && !componentInstance._instance.IsPlaying())
					{
						return componentInstance;
					}
				}
			}
			return null;
		}

		protected ComponentInstance FindInstance(GameObject parentGameObject)
		{
			if (_componentInstances != null)
			{
				for (int i = 0; i < _componentInstances.Length; i++)
				{
					ComponentInstance componentInstance = _componentInstances[i];
					if (componentInstance._parentGameObject == parentGameObject)
					{
						return componentInstance;
					}
				}
			}
			return null;
		}

		public List<ComponentInstance> FindInstances(GameObject parentGameObject, bool createIfNoneFound = true)
		{
			findInstances.Clear();
			if (_componentInstances != null)
			{
				for (int i = 0; i < _componentInstances.Length; i++)
				{
					ComponentInstance componentInstance = _componentInstances[i];
					if (componentInstance._parentGameObject == parentGameObject)
					{
						findInstances.Add(componentInstance);
					}
				}
			}
			if (findInstances.Count == 0 && _componentInstance != null && _componentInstance._parentGameObject == parentGameObject)
			{
				findInstances.Add(_componentInstances[0]);
			}
			if (findInstances.Count == 0 && createIfNoneFound)
			{
				ComponentInstance componentInstance2 = CreateInstance(parentGameObject);
				if (componentInstance2 == null)
				{
					DebugLog.Print("Component failed to create instances, probably due to stealing behaviour", DebugLevel.Error);
				}
				else
				{
					findInstances.Add(componentInstance2);
				}
			}
			return findInstances;
		}

		protected virtual void InitialiseMusicTimingSettings()
		{
			if (_musicTimeSettingsIndex == 0 && _musicTempo > 0f)
			{
				_activeMusicTimeSettings = new MusicTimeSittings();
				_activeMusicTimeSettings._bpm = _musicTempo;
				_activeMusicTimeSettings._timeSignatureLower = _musicTimeSignatureLower;
				_activeMusicTimeSettings._timeSignatureUpper = _musicTimeSignatureUpper;
				_activeMusicTimeSettings.Init();
				FabricManager.Instance.RegisterComponentMusicTimeSettings(this, _activeMusicTimeSettings);
			}
			else if (_musicTimeSettingsIndex >= 1)
			{
				_activeMusicTimeSettings = FabricManager.Instance.GetMusicSettingByIndex(_musicTimeSettingsIndex);
			}
		}

		public void ResizeMaxInstance(int maxInstances)
		{
			_maxInstances = maxInstances;
			if (_maxInstances > 1)
			{
				Initialise(ParentComponent, false);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(_instanceHolder.gameObject);
			}
		}

		private EventStatus UpdateVirtualizationEventInstance2(VirtualizationEventInstance eventInstance, Context context)
		{
			float num = 0f;
			if (FabricManager.IsInitialised() && FabricManager.Instance._audioListener != null)
			{
				num = Vector3.Distance(eventInstance._event.parentGameObject.transform.position, FabricManager.Instance._audioListener.transform.position);
			}
			else
			{
				if (!(Camera.main != null))
				{
					return EventStatus.Not_Handled;
				}
				num = Vector3.Distance(eventInstance._event.parentGameObject.transform.position, Camera.main.transform.position);
			}
			float maxDistance = _maxDistance;
			if (context != null)
			{
				maxDistance = context._maxDistance;
			}
			bool flag = ((double)context._volume > ((!_overrideVolumeThreshold) ? FabricManager.Instance._volumeThreshold : ((double)_volumeThreshold))) ? true : false;
			if (num < maxDistance && flag && !eventInstance._isPlaying)
			{
				eventInstance._event.EventAction = EventAction.PlaySound;
				ProcessEvent(eventInstance._event, ref eventInstance._componentInstance);
				if (eventInstance._componentInstance != null && _virtualizationBehavior == VirtualizationBehavior.PlayFromElapsedTime && eventInstance._dspTime > 0.0)
				{
					float length = (float)(AudioSettings.dspTime - eventInstance._dspTime);
					eventInstance._componentInstance._instance.AdvanceTime(eventInstance._time, length);
				}
				else if (eventInstance._componentInstance != null && _virtualizationBehavior == VirtualizationBehavior.Resume && eventInstance._time > 0f)
				{
					eventInstance._componentInstance._instance.SetTime(eventInstance._time);
				}
				eventInstance._isPlaying = true;
			}
			else if ((num > maxDistance || !flag) && eventInstance._isPlaying)
			{
				if (eventInstance._componentInstance != null && _virtualizationBehavior == VirtualizationBehavior.PlayFromElapsedTime)
				{
					eventInstance._dspTime = AudioSettings.dspTime;
					eventInstance._time = (float)eventInstance._componentInstance._instance.GetCurrentTime();
				}
				else if (eventInstance._componentInstance != null && _virtualizationBehavior == VirtualizationBehavior.Resume)
				{
					eventInstance._time = (float)eventInstance._componentInstance._instance.GetCurrentTime();
				}
				eventInstance._event.EventAction = EventAction.StopSound;
				ProcessEvent(eventInstance._event, ref eventInstance._componentInstance);
				eventInstance._isPlaying = false;
			}
			return EventStatus.Handled_Virtualized;
		}

		protected void UpdateVirtualization2(Context context)
		{
			if (!_componentVirtualization)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < _componentVirtualizationEvents.Count; i++)
			{
				flag |= _componentVirtualizationEvents[i].UpdateDistanceFromListener();
			}
			if (flag)
			{
				_componentVirtualizationEvents.Sort(_sortVirtualizationInstancesByDistance);
			}
			for (int j = 0; j < _componentVirtualizationEvents.Count; j++)
			{
				VirtualizationEventInstance virtualizationEventInstance = _componentVirtualizationEvents[j];
				if (virtualizationEventInstance == null)
				{
					continue;
				}
				if (virtualizationEventInstance._event != null && virtualizationEventInstance._event.parentGameObject != null)
				{
					if (j < _maxInstances)
					{
						UpdateVirtualizationEventInstance2(virtualizationEventInstance, context);
					}
					else if (virtualizationEventInstance._isPlaying)
					{
						virtualizationEventInstance._event.EventAction = EventAction.StopSound;
						ProcessEvent(virtualizationEventInstance._event, ref virtualizationEventInstance._componentInstance);
						virtualizationEventInstance._isPlaying = false;
					}
				}
				else
				{
					if (virtualizationEventInstance._isPlaying)
					{
						virtualizationEventInstance._event.EventAction = EventAction.StopSound;
						ProcessEvent(virtualizationEventInstance._event, ref virtualizationEventInstance._componentInstance);
					}
					RemoveVirtualizationEventInstance(virtualizationEventInstance);
				}
			}
		}

		public virtual bool UpdateInternal(ref Context context)
		{
			profiler.Begin();
			bool isComponentActive = _isComponentActive;
			_isComponentActive = false;
			if (_componentInstances != null)
			{
				for (int i = 0; i < _componentInstances.Length; i++)
				{
					ComponentInstance componentInstance = _componentInstances[i];
					if (componentInstance._instance._isInstance && componentInstance._instance.IsComponentActive())
					{
						_isComponentActive = true;
						componentInstance._instance.UpdateInternal(ref context);
					}
				}
			}
			UpdateProperties(context);
			_updateContext._depth = context._depth + 1;
			for (int j = 0; j < _componentsArray.Length; j++)
			{
				Component component = _componentsArray[j];
				if (component.IsComponentActive())
				{
					_isComponentActive = true;
					component.UpdateInternal(ref _updateContext);
				}
			}
			if (IsVirtualizationActive())
			{
				_isComponentActive = true;
			}
			if (!_isComponentActive && isComponentActive)
			{
				_componentStatus = ComponentStatus.Stopped;
				_updateContext.Reset();
				if (HasValidEventNotifier())
				{
					NotifyEvent(EventNotificationType.OnFinished, this);
				}
			}
			else
			{
				UpdateVirtualization2(_updateContext);
			}
			_updatePropertiesFlag = false;
			isComponentActive = _isComponentActive;
			profiler.End();
			return _isComponentActive;
		}

		internal virtual bool UpdateProperties(Context context)
		{
			if (_updatePropertiesFlag)
			{
				return false;
			}
			if (_RTPManager != null)
			{
				_RTPManager.Update(this);
			}
			UpdateDSPComponents();
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
			if (_mute)
			{
				_updateContext._volume = 0f;
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
			if (_parentComponent == null)
			{
				_updateContext._priority = _priority;
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
			if (_override3DProperties || _parentComponent == null)
			{
				_updateContext._panLevel = _panLevel;
				_updateContext._spreadLevel = _spreadLevel;
				_updateContext._dopplerLevel = _dopplerLevel;
				_updateContext._minDistance = _minDistance;
				_updateContext._maxDistance = _maxDistance;
				_updateContext._rolloffMode = _rolloffMode;
				_updateContext._customCurves = _customCurves;
			}
			else
			{
				_updateContext._panLevel = context._panLevel;
				_updateContext._spreadLevel = context._spreadLevel;
				_updateContext._dopplerLevel = context._dopplerLevel;
				_updateContext._minDistance = context._minDistance;
				_updateContext._maxDistance = context._maxDistance;
				_updateContext._rolloffMode = context._rolloffMode;
				_updateContext._customCurves = context._customCurves;
			}
			if (_overrideBypassProperties || _parentComponent == null)
			{
				_updateContext._bypassEffects = _bypassEffects;
				_updateContext._bypassListenerEffects = _bypassListenerEffects;
				_updateContext._bypassReverbZones = _bypassReverbZones;
			}
			else
			{
				_updateContext._bypassEffects = context._bypassEffects;
				_updateContext._bypassListenerEffects = context._bypassListenerEffects;
				_updateContext._bypassReverbZones = context._bypassReverbZones;
			}
			_updateContext._reverbZoneMix = context._reverbZoneMix * _reverbZoneMix;
			if (_overrideAudioMixerGroup || _parentComponent == null)
			{
				_updateContext._audioMixerGroup = _audioMixerGroup;
			}
			else
			{
				_updateContext._audioMixerGroup = context._audioMixerGroup;
			}
			if (_overrideSpatializeProperty || _parentComponent == null)
			{
				_updateContext._spatialize = _spatialize;
			}
			else
			{
				_updateContext._spatialize = context._spatialize;
			}
			if (_overrideAudioBus || _parentComponent == null)
			{
				_updateContext._audioBus = _audioBus;
				if (_audioBus != null && _audioBus._audioMixerGroup != null)
				{
					_audioMixerGroup = _audioBus._audioMixerGroup;
				}
			}
			else
			{
				_updateContext._audioBus = context._audioBus;
			}
			ApplyInitialiseParameters(ref _updateContext);
			_updatePropertiesFlag = true;
			return true;
		}

		List<RTPProperty> IRTPPropertyListener.CollectProperties()
		{
			return CollectProperties();
		}

		protected List<RTPProperty> CollectProperties()
		{
			List<RTPProperty> list = new List<RTPProperty>();
			list.Add(new RTPProperty(0, RTPPropertyEnum.Volume.ToString(), 0f, 1f));
			list.Add(new RTPProperty(1, RTPPropertyEnum.Pitch.ToString(), 0f, 3f));
			list.Add(new RTPProperty(2, RTPPropertyEnum.Pan2D.ToString(), -1f, 1f));
			list.Add(new RTPProperty(3, RTPPropertyEnum.PanLevel.ToString(), 0f, 1f));
			list.Add(new RTPProperty(4, RTPPropertyEnum.SpreadLevel.ToString(), 0f, 360f));
			list.Add(new RTPProperty(5, RTPPropertyEnum.DopplerLevel.ToString(), 0f, 5f));
			list.Add(new RTPProperty(6, RTPPropertyEnum.Priority.ToString(), 0f, 255f));
			list.Add(new RTPProperty(7, RTPPropertyEnum.ReverbZoneMix.ToString(), 0f, 1f));
			int num = list.Count;
			DSPComponent[] componentsInChildren = base.gameObject.GetComponentsInChildren<DSPComponent>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (!componentsInChildren[i].IsInitialized)
				{
					componentsInChildren[i].OnInitialise(false);
				}
				for (int j = 0; j < componentsInChildren[i].GetNumberOfParameters(); j++)
				{
					DSPParameter parameterByIndex = componentsInChildren[i].GetParameterByIndex(j);
					string name = (componentsInChildren[i].Type != DSPType.External) ? (componentsInChildren[i].GetTypeByName() + "/" + componentsInChildren[i].GetParameterNameByIndex(j)) : (componentsInChildren[i].GetTypeByName() + "/" + componentsInChildren[i].GetParameterNameByIndex(j));
					list.Add(new RTPProperty(num, name, parameterByIndex.Min, parameterByIndex.Max));
					num++;
				}
			}
			SideChain[] components = base.gameObject.GetComponents<SideChain>();
			for (int k = 0; k < components.Length; k++)
			{
				list.Add(new RTPProperty(num, "SideChain/Gain", 0f, 1f));
				num++;
			}
			return list;
		}

		bool IRTPPropertyListener.UpdateProperty(RTPProperty property, float value, RTPPropertyType type)
		{
			return UpdateProperty(property, value, type);
		}

		protected bool UpdateProperty(RTPProperty property, float value, RTPPropertyType type)
		{
			bool flag = false;
			if (property._componentName == "Component")
			{
				switch (property._property)
				{
				case 0:
					if (type == RTPPropertyType.Set)
					{
						Volume = value;
					}
					else
					{
						_rtpProperties._volume = RTPParameterToProperty.SetValueByType(Volume, value, type);
					}
					flag = true;
					break;
				case 1:
					if (type == RTPPropertyType.Set)
					{
						Pitch = value;
					}
					else
					{
						_rtpProperties._pitch = RTPParameterToProperty.SetValueByType(Pitch, value, type);
					}
					flag = true;
					break;
				case 2:
					if (type == RTPPropertyType.Set)
					{
						Pan2D = value;
					}
					else
					{
						_rtpProperties._pan2D = RTPParameterToProperty.SetValueByType(Pan2D, value, type);
					}
					flag = true;
					break;
				case 3:
					if (type == RTPPropertyType.Set)
					{
						PanLevel = value;
					}
					else
					{
						_rtpProperties._panLevel = RTPParameterToProperty.SetValueByType(PanLevel, value, type);
					}
					flag = true;
					break;
				case 4:
					if (type == RTPPropertyType.Set)
					{
						SpreadLevel = value;
					}
					else
					{
						_rtpProperties._spreadLevel = RTPParameterToProperty.SetValueByType(SpreadLevel, value, type);
					}
					flag = true;
					break;
				case 5:
					if (type == RTPPropertyType.Set)
					{
						DopplerLevel = value;
					}
					else
					{
						_rtpProperties._dopplerLevel = RTPParameterToProperty.SetValueByType(DopplerLevel, value, type);
					}
					flag = true;
					break;
				case 6:
					if (type == RTPPropertyType.Set)
					{
						Priority = (int)value;
					}
					else
					{
						_rtpProperties._priority = (int)RTPParameterToProperty.SetValueByType(Priority, value, type);
					}
					flag = true;
					break;
				case 7:
					_reverbZoneMix = value;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				for (int i = 0; i < _dspComponents.Length; i++)
				{
					DSPComponent dSPComponent = _dspComponents[i];
					if (dSPComponent.GetTypeByName() == property._componentName)
					{
						flag |= _dspComponents[i].SetParameterValue(property._propertyName, value, 0f, 0.5f);
					}
					if (flag)
					{
						break;
					}
				}
			}
			if (!flag && _sideChainComponents != null && property._componentName == "SideChain")
			{
				for (int j = 0; j < _sideChainComponents.Length; j++)
				{
					SideChain sideChain = _sideChainComponents[j];
					if ((bool)sideChain)
					{
						sideChain.gain = value;
						flag = true;
					}
				}
			}
			return flag;
		}

		public void PreviewPlay(Camera sceneViewCamera)
		{
			FabricManager.CleanUpPreviewGameObjects();
			GameObject gameObject = null;
			AudioListener audioListener = (AudioListener)UnityEngine.Object.FindObjectOfType(typeof(AudioListener));
			if (audioListener == null && sceneViewCamera != null)
			{
				audioListener = sceneViewCamera.GetComponent<AudioListener>();
				if (audioListener == null)
				{
					audioListener = sceneViewCamera.gameObject.AddComponent<AudioListener>();
				}
			}
			if (audioListener != null)
			{
				gameObject = audioListener.gameObject;
			}
			if (gameObject == null)
			{
				if (Camera.main != null)
				{
					gameObject = Camera.main.gameObject;
				}
				if (gameObject == null && GetFabricManager.Instance() != null)
				{
					gameObject = GetFabricManager.Instance().gameObject;
				}
			}
			if (_parentComponent == null)
			{
				InitialisePreview(null);
			}
			ComponentInstance componentInstance = _componentInstances[0];
			if (componentInstance != null)
			{
				componentInstance._triggerTime = FabricTimer.Get();
				componentInstance._parentGameObject = gameObject;
				componentInstance._transform = gameObject.transform;
				Play(componentInstance);
			}
		}

		public void InitialisePreview(Component parentComponent)
		{
			_parentComponent = parentComponent;
			_eventListeners = base.gameObject.GetComponents<EventListener>();
			if (_eventListeners != null && _eventListeners.Length > 0)
			{
				_componentInstances = new ComponentInstance[1];
				ComponentInstance componentInstance = new ComponentInstance();
				componentInstance._componentGameObject = base.gameObject;
				componentInstance._componentInstanceHolder = this;
				componentInstance._instance = this;
				componentInstance._parentGameObject = null;
				componentInstance._instance._dspComponents = base.gameObject.GetComponentsInChildren<DSPComponent>();
				_componentInstances[0] = componentInstance;
				for (int i = 0; i < _eventListeners.Length; i++)
				{
					EventListener eventListener = _eventListeners[i];
					EventManager.Instance.RegisterListener(this, eventListener._eventName);
				}
			}
			CollectChildrenComponentPreview(this);
			Reset();
			OnInitialise(true);
			_volumeMeter = base.gameObject.GetComponent<VolumeMeter>();
			if (_volumeMeter != null)
			{
				_volumeMeter.OnInitialise();
			}
			InitialiseMusicTimingSettings();
		}

		private void CollectChildrenComponentPreview(Component parentComponent)
		{
			_components.Clear();
			int childCount = base.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Component component = base.transform.GetChild(i).GetComponent<Component>();
				if (component != null)
				{
					component.OnPreInitialise(true);
					component.InitialisePreview(parentComponent);
					component.OnPostInitialise();
					AddComponent(component);
				}
			}
			_componentsArray = _components.ToArray();
		}

		public virtual void PreviewUpdate()
		{
			if (_volumeMeter != null)
			{
				_volumeMeter.Update();
			}
		}
	}
}
