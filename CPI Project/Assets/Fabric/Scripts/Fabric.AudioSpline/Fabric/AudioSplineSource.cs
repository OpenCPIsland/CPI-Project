using UnityEngine;

namespace Fabric
{
	public class AudioSplineSource : MonoBehaviour
	{
		public EventTrigger _eventParameterTrigger;

		public void UpdateWithNormaliseTime(float t)
		{
			if (_eventParameterTrigger != null && _eventParameterTrigger._eventAction == EventAction.SetParameter)
			{
				EventManager.Instance.SetParameter(_eventParameterTrigger._eventName, _eventParameterTrigger._eventParameterName, t, base.gameObject);
			}
		}
	}
}
