using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Components/SequenceComponent")]
	public class SequenceComponent : Component
	{
		[NonSerialized]
		[HideInInspector]
		public Component _currentlyPlayingComponent;

		[HideInInspector]
		[SerializeField]
		public Component[] _playlist;

		[HideInInspector]
		[SerializeField]
		public bool[] _playlistPlayToEnd;

		private int _prevPlayingComponentIndex;

		private bool _onlyStopOnce;

		private bool _advanceEventTriggered;

		[SerializeField]
		[HideInInspector]
		public bool _resetOnFirstPlay;

		[HideInInspector]
		[SerializeField]
		public bool _syncToMusicOnFirstPlay = true;

		[HideInInspector]
		[SerializeField]
		public float _transitionOffset;

		[SerializeField]
		[HideInInspector]
		public float _transitionOffsetRandomization;

		[SerializeField]
		[HideInInspector]
		public SequenceComponentType _sequenceType;

		[HideInInspector]
		[SerializeField]
		public SequenceComponentPlayMode _sequencePlayMode;

		[SerializeField]
		[HideInInspector]
		public SequenceComponentAdvanceMode _sequenceAdvanceMode = SequenceComponentAdvanceMode.OnAdvanceSequenceEventAction;

		[SerializeField]
		[HideInInspector]
		public bool _syncAdvanceBetweenInstances;

		[NonSerialized]
		[HideInInspector]
		private bool _stopInProgress;

		[HideInInspector]
		public int _playingComponentIndex
		{
			get;
			set;
		}

		protected override void OnInitialise(bool inPreviewMode = false)
		{
			_playingComponentIndex = 0;
			_prevPlayingComponentIndex = 0;
			base.OnInitialise(inPreviewMode);
		}

		public bool IsMusicSyncEnabled()
		{
			if (_sequenceType != SequenceComponentType.PlayOnAdvance || _sequenceAdvanceMode != SequenceComponentAdvanceMode.OnMusicSync)
			{
				return false;
			}
			return true;
		}

		public override void PlayInternal(ComponentInstance zComponentInstance, float target, float curve, bool dontPlayComponents)
		{
			if (CheckMIDI(zComponentInstance))
			{
				if (!IsPlaying() && _resetOnFirstPlay)
				{
					_playingComponentIndex = 0;
					_prevPlayingComponentIndex = 0;
				}
				if (!_syncAdvanceBetweenInstances && _sequenceAdvanceMode != 0)
				{
					_playingComponentIndex = 0;
				}
				_onlyStopOnce = true;
				_advanceEventTriggered = false;
				_stopInProgress = false;
				base.PlayInternal(zComponentInstance, _fadeInTime, _fadeInCurve, true);
				double playScheduledDelay = (double)_transitionOffset + base._random.NextDouble() * (double)_transitionOffsetRandomization;
				double playScheduled = 0.0;
				if (_activeMusicTimeSettings != null && IsMusicSyncEnabled() && _syncToMusicOnFirstPlay && !_musicTimeResetOnPlay)
				{
					playScheduled = _activeMusicTimeSettings.GetDelay(this);
				}
				_componentInstance._instance.SetPlayScheduledAdditive(playScheduled, playScheduledDelay);
				PlayNextEntry();
			}
		}

		public override void StopInternal(bool stopInstances, bool forceStop, float target, float curve, double scheduleEnd = 0.0)
		{
			ResetSequence();
			_stopInProgress = true;
			base.StopInternal(stopInstances, forceStop, target, curve, scheduleEnd);
		}

		private bool PlayNextEntry(double offset = 0.0, double scheduledStop = 0.0)
		{
			if (_playlist != null && _playlist.Length > 0)
			{
				int num = _playlist.Length - 1;
				if (_playingComponentIndex > num)
				{
					if (_sequencePlayMode != SequenceComponentPlayMode.Loop)
					{
						return false;
					}
					_playingComponentIndex = 0;
					if (HasValidEventNotifier())
					{
						NotifyEvent(EventNotificationType.OnSequenceEnd, this);
					}
				}
				if (_currentlyPlayingComponent != null && _sequenceType == SequenceComponentType.PlayOnAdvance)
				{
					_currentlyPlayingComponent.StopInternal(false, false, 0f, 0.5f, scheduledStop);
				}
				_prevPlayingComponentIndex = _playingComponentIndex;
				_currentlyPlayingComponent = _playlist[_playingComponentIndex++];
				if (HasValidEventNotifier())
				{
					NotifyEvent(EventNotificationType.OnSequenceNextEntry, _currentlyPlayingComponent);
				}
			}
			if (_currentlyPlayingComponent != null && _componentInstance != null)
			{
				_currentlyPlayingComponent.PlayInternal(_componentInstance, 0f, 0.5f);
				if (_componentStatus == ComponentStatus.Stopping && _onlyStopOnce && _sequenceType != SequenceComponentType.PlayOnAdvance)
				{
					StopInternal(false, false, _fadeParameter.GetTimeRemaining(FabricTimer.Get()), _fadeOutCurve);
					_onlyStopOnce = false;
				}
			}
			return true;
		}

		public override EventStatus OnProcessEvent(Event zEvent, ComponentInstance zInstance)
		{
			EventStatus result = EventStatus.Failed_Uknown;
			if (zEvent.EventAction == EventAction.AdvanceSequence && _sequenceType == SequenceComponentType.PlayOnAdvance)
			{
				List<ComponentInstance> list = null;
				if (_syncAdvanceBetweenInstances)
				{
					list = new List<ComponentInstance>(_componentInstances.Length);
					for (int i = 0; i < _componentInstances.Length; i++)
					{
						list.Add(_componentInstances[i]);
					}
				}
				else
				{
					list = FindInstances(zEvent.parentGameObject, false);
				}
				if (list != null && list.Count > 0)
				{
					for (int j = 0; j < list.Count; j++)
					{
						ComponentInstance componentInstance = list[j];
						if (!((SequenceComponent)componentInstance._instance)._stopInProgress)
						{
							if ((_playlistPlayToEnd[_prevPlayingComponentIndex] || IsMusicSyncEnabled()) && _currentlyPlayingComponent != null && _currentlyPlayingComponent.IsPlaying())
							{
								((SequenceComponent)componentInstance._instance)._advanceEventTriggered = true;
								continue;
							}
							((SequenceComponent)componentInstance._instance).PlayNextEntry();
							result = EventStatus.Handled;
							if (HasValidEventNotifier())
							{
								NotifyEvent(EventNotificationType.OnSequenceAdvance, _currentlyPlayingComponent);
							}
						}
						else
						{
							result = EventStatus.Not_Handled_Stopped;
						}
					}
				}
				else
				{
					PlayNextEntry();
					result = EventStatus.Handled;
				}
			}
			else if (zEvent.EventAction == EventAction.ResetSequence)
			{
				List<ComponentInstance> list2 = FindInstances(zEvent.parentGameObject, false);
				if (list2 != null && list2.Count > 0)
				{
					for (int k = 0; k < list2.Count; k++)
					{
						ComponentInstance componentInstance2 = list2[k];
						((SequenceComponent)componentInstance2._instance).ResetSequence();
						result = EventStatus.Handled;
					}
				}
				else
				{
					ResetSequence();
					result = EventStatus.Handled;
				}
			}
			return result;
		}

		private void ResetSequence()
		{
			if (_sequenceType != SequenceComponentType.PlayOnAdvance || _sequenceAdvanceMode != 0)
			{
				_playingComponentIndex = 0;
				_prevPlayingComponentIndex = 0;
			}
			_stopInProgress = false;
			Reset();
		}

		internal override bool OnMarker(double time)
		{
			return AdvanceSequence(time);
		}

		internal override void OnFinishPlaying(double time)
		{
			AdvanceSequence(time);
		}

		public override bool UpdateInternal(ref Context context)
		{
			if (IsMusicSyncEnabled() && _activeMusicTimeSettings != null)
			{
				double offset = 0.0;
				if (_activeMusicTimeSettings.CheckIfNextEventIsWithinRange(ref offset))
				{
					AdvanceSequence(offset);
				}
			}
			return base.UpdateInternal(ref context);
		}

		private bool AdvanceSequence(double time)
		{
			if ((IsPlaying() && _sequenceType == SequenceComponentType.PlayContinuous) || _advanceEventTriggered)
			{
				double num = (double)_transitionOffset + base._random.NextDouble() * (double)_transitionOffsetRandomization;
				_componentInstance._instance.SetPlayScheduled(time + num, num);
				if (!PlayNextEntry(num, time))
				{
					base.OnFinishPlaying(time);
					return false;
				}
				_advanceEventTriggered = false;
				return true;
			}
			base.OnFinishPlaying(time);
			return false;
		}
	}
}
