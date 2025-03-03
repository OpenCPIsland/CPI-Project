using ClubPenguin.Locomotion;
using UnityEngine;

namespace ClubPenguin.World.Activities.Diving
{
	public class DivingGameObserver : MonoBehaviour
	{
		public float InvokeDelay = 0.1f;

		public int NumberOfTriggers = 0;

		public bool IsUnderWater
		{
			get
			{
				return NumberOfTriggers > 0;
			}
			set
			{
				if (!value)
				{
					NumberOfTriggers = 0;
				}
			}
		}

		private void Start()
		{
			checkForDivingGameController();
		}

		private void checkForDivingGameController(SwimControllerData data = null)
		{
			if (!base.gameObject.GetComponent<DivingGameController>())
			{
				DivingGameController divingGameController = base.gameObject.AddComponent<DivingGameController>();
				if (data != null)
				{
					divingGameController.SetData(data);
				}
			}
		}

		public void DivingTriggerEnter(SwimControllerData data = null)
		{
			checkForDivingGameController(data);
			setUnderWater();
		}

		public void DivingTriggerExit()
		{
			NumberOfTriggers--;
			Invoke("RemoveDivingGameController", InvokeDelay);
		}

		private void setUnderWater()
		{
			NumberOfTriggers++;
			CancelInvoke("RemoveDivingGameController");
		}

		private void RemoveDivingGameController()
		{
			if (!IsUnderWater)
			{
				DivingGameController component = base.gameObject.GetComponent<DivingGameController>();
				component.RemoveLocalPlayerAirBubbleData();
				Object.Destroy(component);
				Object.Destroy(base.gameObject.GetComponent<DivingGameObserver>());
			}
		}
	}
}
