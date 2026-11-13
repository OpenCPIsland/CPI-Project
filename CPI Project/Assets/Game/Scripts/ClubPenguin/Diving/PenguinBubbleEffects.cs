using ClubPenguin.World.Activities.Diving;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Diving
{
	public class PenguinBubbleEffects : MonoBehaviour
	{
		public ParticleSystem collisionBubbles;

		public ParticleSystem freeAirBubbles;

		public ParticleSystem airBubbleBurstBubbles;

		private EventDispatcher dispatcher;

		private void Start()
		{
			dispatcher = Service.Get<EventDispatcher>();
			if (dispatcher != null)
			{
				dispatcher.AddListener<DivingEvents.CollisionEffects>(showCollisionBubbles);
				dispatcher.AddListener<DivingEvents.FreeAirEffects>(showFreeAirBubbles);
				dispatcher.AddListener<DivingEvents.AirBubbleBurstEffects>(showAirBurstBubbles);
			}
		}

		private bool showCollisionBubbles(DivingEvents.CollisionEffects e)
		{
			if (e.Tag == "Player" && collisionBubbles != null)
			{
				collisionBubbles.Play();
			}
			return false;
		}

		private bool showAirBurstBubbles(DivingEvents.AirBubbleBurstEffects e)
		{
			if (e.Tag == "Player" && airBubbleBurstBubbles != null)
			{
				airBubbleBurstBubbles.Play();
			}
			return false;
		}

		private bool showFreeAirBubbles(DivingEvents.FreeAirEffects e)
		{
			if (e.Tag == "Player" && freeAirBubbles != null)
			{
				if (e.Enabled)
				{
					freeAirBubbles.Play();
				}
				else
				{
					freeAirBubbles.Stop();
				}
			}
			return false;
		}

		public void OnDestroy()
		{
			if (dispatcher != null)
			{
				dispatcher.RemoveListener<DivingEvents.CollisionEffects>(showCollisionBubbles);
				dispatcher.RemoveListener<DivingEvents.FreeAirEffects>(showFreeAirBubbles);
				dispatcher.RemoveListener<DivingEvents.AirBubbleBurstEffects>(showAirBurstBubbles);
			}
		}
	}
}
