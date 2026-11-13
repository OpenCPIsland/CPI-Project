using ClubPenguin.World.Activities.Diving;
using UnityEngine;

namespace ClubPenguin.Benchmarking
{
	public class InvincibleDiver : MonoBehaviour
	{
		public void Start()
		{
		}

		public void Update()
		{
			DivingGameController component = base.gameObject.GetComponent<DivingGameController>();
			if (component != null)
			{
				component.DegradeRate = 0f;
			}
		}
	}
}
