using ClubPenguin.Analytics;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class SkyCafeTrigger : MonoBehaviour
	{
		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				triggerSkyCafe();
			}
		}

		private void triggerSkyCafe()
		{
			Service.Get<ICPSwrveService>().Action("trampoline", "cloud_cafe");
		}
	}
}
