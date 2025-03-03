using UnityEngine;

namespace ClubPenguin.MiniGames.Jigsaw
{
	public class GearPuzzleCompletion : MonoBehaviour
	{
		public int GearTeeth = 0;

		public int FirstGearTeeth = 0;

		public int SequenceNumber = 0;

		private GearPuzzleCompletionController gearController;

		private void Start()
		{
			gearController = base.gameObject.GetComponentInParent<GearPuzzleCompletionController>();
		}

		private void Update()
		{
			float num = (float)FirstGearTeeth / (float)GearTeeth;
			float num2 = 1f;
			if (SequenceNumber % 2 == 0)
			{
				num2 = -1f;
			}
			float num3 = 30f;
			if (gearController != null)
			{
				num3 = gearController.Speed;
			}
			base.gameObject.transform.Rotate(0f, 0f, num * num3 * num2 * Time.deltaTime);
		}
	}
}
