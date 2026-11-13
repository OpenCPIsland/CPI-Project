using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Renderer))]
	public class SortingOrderOverride : MonoBehaviour
	{
		public int OrderInLayer = 0;

		public string SortingLayerName = "";

		public void Awake()
		{
			Renderer component = GetComponent<Renderer>();
			if (!string.IsNullOrEmpty(SortingLayerName))
			{
				int num = SortingLayer.NameToID(SortingLayerName);
				if (num != 0)
				{
					component.sortingLayerID = num;
				}
			}
			component.sortingOrder = OrderInLayer;
		}
	}
}
