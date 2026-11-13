using ClubPenguin.Locomotion;
using Fabric;
using UnityEngine;

namespace ClubPenguin.Diving
{
	public class FreeAirSFX : MonoBehaviour
	{
		public SwimControllerData MasterData;

		private SwimControllerData mutableData;

		private void Start()
		{
			mutableData = Object.Instantiate(MasterData);
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (collider.gameObject.CompareTag("Player"))
			{
				playAudioEvent(mutableData.AirRechargeAudioEvent);
			}
		}

		private void playAudioEvent(string audioEvent, EventAction fabricEvent = EventAction.PlaySound)
		{
			if (!string.IsNullOrEmpty(audioEvent))
			{
				EventManager.Instance.PostEvent(audioEvent, fabricEvent, base.gameObject);
			}
		}
	}
}
