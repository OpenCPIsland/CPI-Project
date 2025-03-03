using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Components/MusicComponent")]
	public class MusicComponent : Component
	{
		[SerializeField]
		[HideInInspector]
		public List<MusicTransition> _transitions = new List<MusicTransition>();

		[HideInInspector]
		[SerializeField]
		public Component _defaultComponent;

		[NonSerialized]
		[HideInInspector]
		private MusicTransition _activeTransition;

		[NonSerialized]
		[HideInInspector]
		public Component _toComponent;

		[NonSerialized]
		[HideInInspector]
		private Component _currentlyPlayingComponent;

		[NonSerialized]
		[HideInInspector]
		private MusicTransitionState _musicTransitionState;

		[HideInInspector]
		[SerializeField]
		public bool _syncToMusicOnFirstPlay = true;

		protected override void OnInitialise(bool inPreviewMode = false)
		{
			_musicTransitionState = MusicTransitionState.Idle;
			base.OnInitialise(inPreviewMode);
		}

		public override EventStatus OnProcessEvent(Event zEvent, ComponentInstance zInstance)
		{
			EventStatus result = EventStatus.Failed_Uknown;
			if (zEvent.EventAction == EventAction.SetSwitch)
			{
				List<ComponentInstance> list = FindInstances(zEvent.parentGameObject, false);
				if (list != null && list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						ComponentInstance componentInstance = list[i];
						((MusicComponent)componentInstance._instance).SetBestMatchTransition((string)zEvent._parameter);
						result = EventStatus.Handled;
					}
				}
				else
				{
					SetBestMatchTransition((string)zEvent._parameter);
					result = EventStatus.Handled;
				}
			}
			return result;
		}

		public override void PlayInternal(ComponentInstance zComponentInstance, float target, float curve, bool dontPlayComponents)
		{
			if (!CheckMIDI(zComponentInstance))
			{
				return;
			}
			base.PlayInternal(zComponentInstance, target, curve, true);
			if (!(_defaultComponent != null))
			{
				return;
			}
			if (_activeMusicTimeSettings != null)
			{
				if (_syncToMusicOnFirstPlay && !_musicTimeResetOnPlay)
				{
					_componentInstance._instance.SetPlayScheduledAdditive(_activeMusicTimeSettings.GetDelay(this), 0.0);
				}
				_activeMusicTimeSettings._onBeatDetected += OnBeat;
				_activeMusicTimeSettings._onBarDetected += OnBar;
			}
			_defaultComponent.PlayInternal(zComponentInstance, 0f, 0.5f);
			_currentlyPlayingComponent = _defaultComponent;
		}

		public override void StopInternal(bool stopInstances, bool forceStop, float target, float curve, double scheduleEnd = 0.0)
		{
			base.StopInternal(stopInstances, forceStop, target, curve, scheduleEnd);
			if (_activeMusicTimeSettings != null)
			{
				_activeMusicTimeSettings._onBeatDetected -= OnBeat;
				_activeMusicTimeSettings._onBarDetected -= OnBar;
			}
		}

		internal override void OnFinishPlaying(double time)
		{
			if (_activeTransition != null)
			{
				MusicTransition.MusicTransitionHolder activeTransitionComponentFromState = _activeTransition.GetActiveTransitionComponentFromState(_musicTransitionState);
				if (activeTransitionComponentFromState != null && activeTransitionComponentFromState._musicSyncType == MusicSyncType.OnEnd)
				{
					PlayNext(time);
				}
			}
		}

		internal override bool OnMarker(double time)
		{
			if (_activeTransition == null)
			{
				return false;
			}
			MusicTransition.MusicTransitionHolder activeTransitionComponentFromState = _activeTransition.GetActiveTransitionComponentFromState(_musicTransitionState);
			if (activeTransitionComponentFromState != null && activeTransitionComponentFromState._musicSyncType == MusicSyncType.OnMarker)
			{
				PlayNext(time);
				return true;
			}
			return false;
		}

		private void OnBeat(double time)
		{
			if (_activeTransition != null)
			{
				MusicTransition.MusicTransitionHolder activeTransitionComponentFromState = _activeTransition.GetActiveTransitionComponentFromState(_musicTransitionState);
				if (activeTransitionComponentFromState != null && activeTransitionComponentFromState._musicSyncType == MusicSyncType.OnBeat)
				{
					PlayNext(time);
				}
			}
		}

		private void OnBar(double time)
		{
			if (_activeTransition != null)
			{
				MusicTransition.MusicTransitionHolder activeTransitionComponentFromState = _activeTransition.GetActiveTransitionComponentFromState(_musicTransitionState);
				if (activeTransitionComponentFromState != null && activeTransitionComponentFromState._musicSyncType == MusicSyncType.OnBar)
				{
					PlayNext(time);
				}
			}
		}

		private MusicSyncType GetTransitionMusicSyncType(MusicTransition transition, MusicTransitionState state)
		{
			switch (state)
			{
			case MusicTransitionState.FromComponent:
				return transition._fromComponent._musicSyncType;
			case MusicTransitionState.Transition:
				return transition._transition._musicSyncType;
			case MusicTransitionState.ToComponent:
				return transition._toComponent._musicSyncType;
			default:
				return MusicSyncType.OnBar;
			}
		}

		private void PlayNext(double time)
		{
			Component component = null;
			MusicSyncType transitionMusicSyncType = GetTransitionMusicSyncType(_activeTransition, _musicTransitionState);
			if (_musicTransitionState == MusicTransitionState.FromComponent)
			{
				if (_activeTransition._transition._component != null)
				{
					component = _activeTransition._transition._component;
					_musicTransitionState = MusicTransitionState.Transition;
				}
				else
				{
					component = _activeTransition._toComponent._component;
					_musicTransitionState = MusicTransitionState.Idle;
				}
			}
			else if (_musicTransitionState == MusicTransitionState.Transition)
			{
				component = _activeTransition._toComponent._component;
				_musicTransitionState = MusicTransitionState.Idle;
			}
			else
			{
				if (!_toComponent)
				{
					return;
				}
				component = _toComponent;
				_toComponent = null;
			}
			if (_currentlyPlayingComponent != null && transitionMusicSyncType != MusicSyncType.OnMarker)
			{
				double scheduleEnd = time;
				if (_currentlyPlayingComponent.FadeOutTime > 0f)
				{
					scheduleEnd = 0.0;
				}
				_currentlyPlayingComponent.StopInternal(false, false, 0f, 0.5f, scheduleEnd);
			}
			_componentInstance._instance.SetPlayScheduled(time, 0.0);
			component.PlayInternal(_componentInstance, 0f, 0.5f);
			_currentlyPlayingComponent = component;
		}

		private void SetBestMatchTransition(string name)
		{
			if (_currentlyPlayingComponent != null)
			{
				for (int i = 0; i < _transitions.Count; i++)
				{
					MusicTransition musicTransition = _transitions[i];
					if (musicTransition._fromComponent._component.Name == _currentlyPlayingComponent.Name && musicTransition._toComponent._component.Name == name)
					{
						_activeTransition = musicTransition;
						_musicTransitionState = MusicTransitionState.FromComponent;
						return;
					}
				}
			}
			int num = 0;
			Component component;
			while (true)
			{
				if (num < _components.Count)
				{
					component = _components[num];
					if (component.name == name)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			_toComponent = component;
		}
	}
}
