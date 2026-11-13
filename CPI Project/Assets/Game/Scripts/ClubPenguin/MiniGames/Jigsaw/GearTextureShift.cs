using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.MiniGames.Jigsaw
{
	[RequireComponent(typeof(ResourceCleaner))]
	public class GearTextureShift : MonoBehaviour
	{
		public int GearTeeth = 0;

		public int FirstGearTeeth = 0;

		public int SequenceNumber = 0;

		private Renderer rend;

		private GearPuzzleCompletionController gearController;

		private float offset = 0f;

		private void Start()
		{
			gearController = base.gameObject.GetComponentInParent<GearPuzzleCompletionController>();
			rend = GetComponent<Renderer>();
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
			offset += num * num3 * num2 * Time.deltaTime * 1.0001f;
			if (offset > 1f)
			{
				offset -= 1f;
			}
			rend.material.SetTextureOffset("_MainTex", new Vector2(offset, 0f));
		}
	}
}
