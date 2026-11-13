using UnityEngine;

namespace ClubPenguin.UI
{
	public class SetCanvasSortOrderOnEnable : MonoBehaviour
	{
		public int SortOrder = -5;

		public void OnEnable()
		{
			Canvas componentInParent = GetComponentInParent<Canvas>();
			if (componentInParent != null)
			{
				componentInParent.sortingOrder = SortOrder;
			}
		}
	}
}
