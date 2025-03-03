using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(LayoutGroup))]
	public class MinSizeUpdater : AbstractMinSizeUpdater
	{
		protected override ILayoutElement getTargetLayoutElement()
		{
			return GetComponent<LayoutGroup>();
		}
	}
}
