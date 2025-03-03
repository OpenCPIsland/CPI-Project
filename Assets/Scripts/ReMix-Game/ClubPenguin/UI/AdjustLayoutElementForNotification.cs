using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(LayoutElement))]
	public class AdjustLayoutElementForNotification : AbstractAdjustForNotification
	{
		private LayoutElement layoutElement;

		protected override void start()
		{
			layoutElement = GetComponent<LayoutElement>();
		}

		protected override void doMoveUp(float height)
		{
			layoutElement.minHeight = 0f;
		}

		protected override void doMoveDown(float height)
		{
			layoutElement.minHeight = height;
		}
	}
}
