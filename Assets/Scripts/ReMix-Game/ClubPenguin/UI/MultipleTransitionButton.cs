using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class MultipleTransitionButton : Button
	{
		protected override void DoStateTransition(SelectionState state, bool instant)
		{
			base.DoStateTransition(state, instant);
			Sprite overrideSprite = null;
			switch (state)
			{
			case SelectionState.Normal:
				overrideSprite = null;
				break;
			case SelectionState.Disabled:
				overrideSprite = base.spriteState.disabledSprite;
				break;
			case SelectionState.Highlighted:
				overrideSprite = base.spriteState.highlightedSprite;
				break;
			case SelectionState.Pressed:
				overrideSprite = base.spriteState.pressedSprite;
				break;
			}
			base.image.overrideSprite = overrideSprite;
		}
	}
}
