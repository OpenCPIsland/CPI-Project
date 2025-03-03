using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Text))]
	public class TextMinSizeUpdater : AbstractMinSizeUpdater
	{
		protected override ILayoutElement getTargetLayoutElement()
		{
			return GetComponent<Text>();
		}
	}
}
