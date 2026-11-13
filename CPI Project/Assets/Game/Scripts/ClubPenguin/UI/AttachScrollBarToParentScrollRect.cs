using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Scrollbar))]
	public class AttachScrollBarToParentScrollRect : MonoBehaviour
	{
		private void Start()
		{
			attachToScrollRectInParent();
		}

		private void attachToScrollRectInParent()
		{
			ScrollRect componentInParent = GetComponentInParent<ScrollRect>();
			if (componentInParent != null)
			{
				if (componentInParent.vertical)
				{
					componentInParent.verticalScrollbar = GetComponent<Scrollbar>();
				}
				else if (componentInParent.horizontal)
				{
					componentInParent.horizontalScrollbar = GetComponent<Scrollbar>();
				}
			}
		}
	}
}
