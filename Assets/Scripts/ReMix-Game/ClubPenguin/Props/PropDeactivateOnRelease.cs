using UnityEngine;

namespace ClubPenguin.Props
{
	[RequireComponent(typeof(Prop))]
	[DisallowMultipleComponent]
	public class PropDeactivateOnRelease : MonoBehaviour
	{
		private Prop prop;

		private void Awake()
		{
			prop = GetComponent<Prop>();
			prop.EActionEventReceived += onActionEventReceived;
		}

		private void OnDestroy()
		{
			prop.EActionEventReceived -= onActionEventReceived;
		}

		private void onActionEventReceived(string actionEvent)
		{
			if (actionEvent == "release")
			{
				prop.PropUserRef.RemoveProp();
				prop.gameObject.SetActive(false);
			}
		}
	}
}
