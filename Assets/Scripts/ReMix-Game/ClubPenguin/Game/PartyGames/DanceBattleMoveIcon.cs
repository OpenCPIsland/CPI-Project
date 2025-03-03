using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Game.PartyGames
{
	public class DanceBattleMoveIcon : MonoBehaviour
	{
		public enum MoveIconState
		{
			BlankRed,
			BlankBlue,
			Icon1,
			Icon2,
			Icon3
		}

		private Image image;

		private TintSelector tintSelector;

		private SpriteSelector spriteSelector;

		private void Awake()
		{
		}

		public void SetState(MoveIconState state)
		{
			getComponents();
			spriteSelector.SelectSprite((int)state);
			switch (state)
			{
			case MoveIconState.BlankRed:
				tintSelector.SelectColor(0);
				break;
			case MoveIconState.BlankBlue:
				tintSelector.SelectColor(1);
				break;
			default:
				image.color = Color.white;
				break;
			}
		}

		private void getComponents()
		{
			if (image == null)
			{
				image = GetComponent<Image>();
			}
			if (spriteSelector == null)
			{
				spriteSelector = GetComponent<SpriteSelector>();
			}
			if (tintSelector == null)
			{
				tintSelector = GetComponent<TintSelector>();
			}
		}
	}
}
