using System;
using UnityEngine;

namespace Fabric.TimelineComponent
{
	[AddComponentMenu("")]
	public class TimelineRegion : Component
	{
		[SerializeField]
		[HideInInspector]
		public RandomComponent _component;

		[HideInInspector]
		[SerializeField]
		public Envelope _volumeEnvelope = new Envelope();

		[SerializeField]
		[HideInInspector]
		public float _x;

		[HideInInspector]
		[SerializeField]
		public bool _loop;

		[HideInInspector]
		[SerializeField]
		public float _width;

		[SerializeField]
		[HideInInspector]
		public bool _autopitchenabled;

		[SerializeField]
		[HideInInspector]
		public float _autopitchreference;

		[SerializeField]
		[HideInInspector]
		public RegionStartMode _startMode;

		[HideInInspector]
		[SerializeField]
		public RegionLoopMode _loopMode = RegionLoopMode.Cutoff;

		[NonSerialized]
		[HideInInspector]
		public bool _regionIsActive;

		[HideInInspector]
		[SerializeField]
		public CurveTypes _fadeInType = CurveTypes.Linear;

		[SerializeField]
		[HideInInspector]
		public CurveTypes _fadeOutType = CurveTypes.Linear;

		[HideInInspector]
		[SerializeField]
		public float _regionVolume = 1f;

		private GameObject randomComponent;

		protected override void OnPreInitialise(bool inPreviewMode = false)
		{
			if ((bool)_component)
			{
				RandomComponent componentInChildren = base.gameObject.GetComponentInChildren<RandomComponent>();
				if (componentInChildren != null)
				{
					randomComponent = componentInChildren.gameObject;
				}
				if (randomComponent == null)
				{
					randomComponent = UnityEngine.Object.Instantiate(_component.gameObject, _component.gameObject.transform.position, _component.gameObject.transform.rotation);
					randomComponent.transform.parent = base.transform;
				}
				SetLoop(_loop, true);
			}
		}

		protected override void OnInitialise(bool inPreviewMode = false)
		{
			ResetVolumeEnvelope();
		}

		public void SetLoop(bool loop, bool forceUpdate = false)
		{
			if (loop != _loop || forceUpdate)
			{
				AudioComponent[] componentsInChildren = base.gameObject.GetComponentsInChildren<AudioComponent>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].Loop = loop;
				}
				_loop = loop;
			}
		}

		public int CompareTo(object obj)
		{
			if (obj is TimelineRegion)
			{
				return base.name.CompareTo((obj as TimelineRegion).name);
			}
			throw new ArgumentException("Object is not a TimelineRegion");
		}

		public float GetAutopitchReference(TimelineParameter parameter)
		{
			float num = parameter._max - parameter._min;
			return _autopitchreference * num;
		}

		public void ResetVolumeEnvelope()
		{
			if (_volumeEnvelope._points == null)
			{
				_volumeEnvelope._points = new Point[4];
				_volumeEnvelope._points[0] = Point.Alloc(_x, 0f, _fadeInType);
				_volumeEnvelope._points[1] = Point.Alloc(_x, 1f, _fadeInType);
				_volumeEnvelope._points[2] = Point.Alloc(_x + _width, 1f, _fadeOutType);
				_volumeEnvelope._points[3] = Point.Alloc(_x + _width, 0f, _fadeOutType);
			}
			_volumeEnvelope._points[0]._x = _x;
			_volumeEnvelope._points[1]._x = _x;
			_volumeEnvelope._points[2]._x = _x + _width;
			_volumeEnvelope._points[3]._x = _x + _width;
			_regionIsActive = false;
		}

		public override bool IsPlaying()
		{
			if (_regionIsActive)
			{
				return true;
			}
			return false;
		}

		public override void PlayInternal(ComponentInstance zComponentInstance, float target, float curve, bool dontPlayComponents = false)
		{
			base.PlayInternal(zComponentInstance, target, curve, dontPlayComponents);
			_regionIsActive = true;
		}

		public override void StopInternal(bool stopInstances, bool forceStop, float target, float curve, double scheduleEnd = 0.0)
		{
			base.StopInternal(stopInstances, forceStop, target, curve, scheduleEnd);
			_regionIsActive = false;
		}
	}
}
