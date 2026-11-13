using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Dropdown))]
	public class DropdownSpriteSelector : MonoBehaviour
	{
		private const int SELECTOR_CLOSED_INDEX = 0;

		private const int SELECTOR_OPEN_INDEX = 1;

		public SpriteSelector SpriteSelector;

		private int initialChildCount;

		public void Awake()
		{
			initialChildCount = base.transform.childCount;
		}

		public void OnTransformChildrenChanged()
		{
			if (base.transform.childCount > initialChildCount)
			{
				SpriteSelector.SelectSprite(1);
			}
			else
			{
				SpriteSelector.SelectSprite(0);
			}
		}
	}
}
