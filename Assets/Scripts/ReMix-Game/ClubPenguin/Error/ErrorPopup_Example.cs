using UnityEngine;

namespace ClubPenguin.Error
{
	public class ErrorPopup_Example : MonoBehaviour
	{
		public RectTransform errorRectTransform;

		public RectTransform parentRectTransform;

		public ErrorDirection errorDirection;

		public Vector2 errorPosition;

		public void ShowError()
		{
			new ShowErrorOnTransformCommand("100", errorDirection, errorRectTransform, parentRectTransform).Execute();
		}

		public void ShowErrorAtPosition()
		{
			new ShowErrorOnPositionCommand("100", errorPosition, parentRectTransform).Execute();
		}
	}
}
