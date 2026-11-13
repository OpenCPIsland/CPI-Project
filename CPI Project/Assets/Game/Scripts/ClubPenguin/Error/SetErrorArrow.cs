using UnityEngine;

namespace ClubPenguin.Error
{
	public class SetErrorArrow : MonoBehaviour
	{
		public GameObject leftArrow;

		public GameObject upArrow;

		public GameObject rightArrow;

		public GameObject downArrow;

		private void OnEnable()
		{
			leftArrow.SetActive(false);
			upArrow.SetActive(false);
			rightArrow.SetActive(false);
			downArrow.SetActive(false);
		}

		public void SetArrowByDirection(ErrorDirection errorPosition)
		{
			switch (errorPosition)
			{
			case ErrorDirection.DOWN:
				upArrow.SetActive(true);
				break;
			case ErrorDirection.LEFT:
				rightArrow.SetActive(true);
				break;
			case ErrorDirection.RIGHT:
				leftArrow.SetActive(true);
				break;
			case ErrorDirection.UP:
				downArrow.SetActive(true);
				break;
			}
		}
	}
}
