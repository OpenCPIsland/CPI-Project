using ClubPenguin.Compete;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Participation
{
	[DisallowMultipleComponent]
	public class ParticipationObserver : MonoBehaviour
	{
		public struct ObservationData
		{
			public readonly ParticipationController Controller;

			public readonly Competitor<GameObject> Competitor;

			public bool IsValid;

			public ObservationData(ParticipationController controller, Competitor<GameObject> competitor)
			{
				Controller = controller;
				Competitor = competitor;
				IsValid = true;
			}
		}

		public Dictionary<GameObject, ObservationData> Observed = new Dictionary<GameObject, ObservationData>();

		private List<ObservationData> ObserversToStop = new List<ObservationData>();

		private bool isBeingDestroyed;

		public event Action<Competitor<GameObject>, GameObject, ParticipationObserver> ObserverExit;

		public event Action<GameObject, ParticipationObserver> ObserverDisable;

		private void OnDisable()
		{
			if (this.ObserverDisable != null)
			{
				this.ObserverDisable(base.gameObject, this);
			}
		}

		public bool IsObserving(GameObject go)
		{
			return Observed.ContainsKey(go);
		}

		public void StartObserving(GameObject go, ParticipationController data, Competitor<GameObject> competitor)
		{
			if (!Observed.ContainsKey(go))
			{
				Observed[go] = new ObservationData(data, competitor);
			}
		}

		public void StopObserving(GameObject go)
		{
			if (!isBeingDestroyed && Observed != null && go != null)
			{
				Observed.Remove(go);
			}
		}

		private void Update()
		{
			ObserversToStop.Clear();
			foreach (KeyValuePair<GameObject, ObservationData> item in Observed)
			{
				if (!item.Value.IsValid)
				{
					ObserversToStop.Add(Observed[item.Key]);
				}
			}
			for (int i = 0; i < ObserversToStop.Count; i++)
			{
				ObserversToStop[i].Controller.StopObserving(ObserversToStop[i].Competitor);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if ((other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("RemotePlayer") || other.gameObject.CompareTag("DummyPlayer")) && Observed.ContainsKey(other.gameObject))
			{
				ObservationData observationData = Observed[other.gameObject];
				observationData.IsValid = true;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (isBeingDestroyed || base.gameObject.IsDestroyed() || !(other != null) || other.gameObject.IsDestroyed() || !Observed.ContainsKey(other.gameObject) || (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("RemotePlayer") && !other.gameObject.CompareTag("DummyPlayer")))
			{
				return;
			}
			ObservationData observationData = Observed[other.gameObject];
			if (observationData.Competitor != null && !observationData.Competitor.Value.IsDestroyed())
			{
				observationData.IsValid = false;
				if (this.ObserverExit != null)
				{
					this.ObserverExit(observationData.Competitor, other.gameObject, this);
				}
			}
		}

		private void OnDestroy()
		{
			isBeingDestroyed = true;
			this.ObserverExit = null;
			this.ObserverDisable = null;
		}
	}
}
