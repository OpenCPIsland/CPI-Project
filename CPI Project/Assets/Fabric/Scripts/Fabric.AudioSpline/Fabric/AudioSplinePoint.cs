using UnityEngine;

namespace Fabric
{
	public class AudioSplinePoint : MonoBehaviour
	{
		public EventTrigger _onEnterEventTrigger;

		public EventTrigger _onExitEventTrigger;

		public float _radius = 0.25f;

		private bool _entered;

		public bool HasEventTrigger()
		{
			if (!(_onEnterEventTrigger != null) && !(_onExitEventTrigger != null))
			{
				return false;
			}
			return true;
		}

		public bool IsEntered()
		{
			return _entered;
		}

		public void OnEnter()
		{
			if ((bool)_onEnterEventTrigger)
			{
				_onEnterEventTrigger.PostEvent();
			}
			_entered = true;
		}

		public void OnExit()
		{
			if ((bool)_onExitEventTrigger)
			{
				_onExitEventTrigger.PostEvent();
			}
			_entered = false;
		}
	}
}
