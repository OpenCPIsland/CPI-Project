using UnityEngine;

namespace Fabric
{
	[ExecuteInEditMode]
	[AddComponentMenu("Fabric/Events/Listener")]
	public class EventListener : MonoBehaviour
	{
		[SerializeField]
		[HideInInspector]
		public string _eventName = "_UnSet_";

		[HideInInspector]
		[SerializeField]
		public int _eventID;

		[SerializeField]
		[HideInInspector]
		public bool _overrideEventTriggerAction;

		[SerializeField]
		[HideInInspector]
		public OverrideParameters _overrideParameters;

		private void OnDestroy()
		{
			if (FabricManager._componentPreviewerUpdateIsActive)
			{
				Component component = base.gameObject.GetComponent<Component>();
				if (component != null)
				{
					component.UnregisterEventListeners();
				}
			}
		}
	}
}
