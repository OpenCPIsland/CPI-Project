using ClubPenguin.Net;
using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class SnowballLauncher : MonoBehaviour
	{
		public float Speed = 10f;

		public bool SynchronizeOnNetwork = false;

		private static GameObjectPool snowballPool;

		private void OnEnable()
		{
			if (snowballPool == null)
			{
				snowballPool = GameObject.Find("Pool[Snowball]").GetComponent<GameObjectPool>();
			}
		}

		public void Launch()
		{
			Vector3 a = base.transform.TransformDirection(Vector3.forward);
			Vector3 vector = a * Speed;
			Vector3 position = base.transform.position;
			Create(position, vector);
			if (SynchronizeOnNetwork)
			{
				LocomotionActionEvent action = default(LocomotionActionEvent);
				action.Type = LocomotionAction.LaunchThrow;
				action.Position = base.transform.parent.position;
				action.Velocity = vector;
				Service.Get<INetworkServicesManager>().PlayerActionService.LocomotionAction(action);
			}
		}

		public void Create(Vector3 position, Vector3 velocity)
		{
			GameObject gameObject = snowballPool.Spawn();
			if (gameObject != null)
			{
				gameObject.transform.position = position;
				gameObject.GetComponent<Rigidbody>().velocity = velocity;
			}
		}
	}
}
