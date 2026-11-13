using UnityEngine;

namespace ClubPenguin.Props
{
	[RequireComponent(typeof(Prop))]
	[DisallowMultipleComponent]
	public class PropUserLookAtXZOnUse : MonoBehaviour
	{
		private Prop prop;

		private void Awake()
		{
			prop = GetComponent<Prop>();
			prop.EUsed += onPropUsed;
		}

		private void OnDestroy()
		{
			prop.EUsed -= onPropUsed;
		}

		private void onPropUsed()
		{
			Vector3 onUseDestination = prop.OnUseDestination;
			onUseDestination.y = prop.PropUserRef.transform.position.y;
			prop.PropUserRef.transform.LookAt(onUseDestination);
		}
	}
}
