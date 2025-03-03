using UnityEngine;

namespace Fabric
{
	public class VirtualizationEventInstance
	{
		public Event _event;

		public Component _rootComponent;

		public ComponentInstance _componentInstance;

		public bool _isPlaying;

		public double _dspTime;

		public float _time;

		public float _distanceFromListener = float.MaxValue;

		public VirtualizationEventInstance(Component component)
		{
			_rootComponent = component;
		}

		public bool UpdateDistanceFromListener()
		{
			if (_rootComponent == null || _event.parentGameObject == null)
			{
				return false;
			}
			if (FabricManager.Instance._audioListener != null)
			{
				_distanceFromListener = Vector3.Distance(_event.parentGameObject.transform.position, FabricManager.Instance._audioListener.transform.position);
			}
			else
			{
				if (!(Camera.main != null))
				{
					return false;
				}
				_distanceFromListener = Vector3.Distance(_event.parentGameObject.transform.position, Camera.main.transform.position);
			}
			return _distanceFromListener < _rootComponent.MaxDistance;
		}
	}
}
