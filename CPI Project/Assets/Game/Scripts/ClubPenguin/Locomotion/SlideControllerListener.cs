using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class SlideControllerListener : MonoBehaviour
	{
		public SlideController SlideController
		{
			get;
			set;
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (SlideController != null)
			{
				SlideController.OnCollisionEnter(collision);
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (SlideController != null && SlideController.gameObject != null)
			{
				PenguinInteraction component = SlideController.gameObject.GetComponent<PenguinInteraction>();
				if (component != null)
				{
					component.OnTriggerEnter(collider);
				}
			}
		}
	}
}
