using ClubPenguin.World.Activities.Diving;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Diving
{
	public class FreeAirZone : MonoBehaviour
	{
		public List<GameObject> LightsOff;

		public List<GameObject> LightsOn;

		private EventDispatcher dispatcher;

		private Collider colliderRef;

		private float interval = 10f;

		public float Interval
		{
			get
			{
				return interval;
			}
			set
			{
				interval = value;
				CancelInvoke();
				startHeartBeat();
			}
		}

		private void Start()
		{
			colliderRef = GetComponent<Collider>();
			dispatcher = Service.Get<EventDispatcher>();
		}

		private void OnEnable()
		{
			startHeartBeat();
		}

		private void startHeartBeat()
		{
			InvokeRepeating("pulseCollider", Interval, Interval);
		}

		private void pulseCollider()
		{
			colliderRef.enabled = false;
			Invoke("renableCollider", 0.1f);
		}

		private void renableCollider()
		{
			colliderRef.enabled = true;
		}

		private void OnDisable()
		{
			CancelInvoke();
		}

		private void OnTriggerEnter(Collider collider)
		{
			getDispatcher();
			if (dispatcher != null && collider.gameObject.CompareTag("Player"))
			{
				dispatcher.DispatchEvent(new DivingEvents.FreeAirEffects(true, "Player"));
			}
		}

		private void OnTriggerExit(Collider collider)
		{
			getDispatcher();
			if (dispatcher != null && collider.gameObject.CompareTag("Player"))
			{
				dispatcher.DispatchEvent(new DivingEvents.FreeAirEffects(false, "Player"));
			}
		}

		private void getDispatcher()
		{
			if (dispatcher == null)
			{
				dispatcher = Service.Get<EventDispatcher>();
			}
		}
	}
}
