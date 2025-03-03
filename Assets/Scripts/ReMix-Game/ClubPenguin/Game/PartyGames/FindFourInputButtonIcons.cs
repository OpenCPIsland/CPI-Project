using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class FindFourInputButtonIcons : MonoBehaviour
	{
		public Sprite[] RedSprites;

		public Sprite[] BlueSprites;

		private SpriteSelector spriteSelector;

		private void Start()
		{
			Service.Get<EventDispatcher>().AddListener<FindFourEvents.ColorChanged>(onColorChanged);
			spriteSelector = base.transform.parent.parent.Find("Icon").GetComponentInChildren<SpriteSelector>();
			if (spriteSelector == null)
			{
				Log.LogErrorFormatted(this, "Could not find SpriteSelector for button {0}", base.name);
			}
		}

		private bool onColorChanged(FindFourEvents.ColorChanged evt)
		{
			if (spriteSelector != null)
			{
				switch (evt.Color)
				{
				case FindFour.FindFourTokenColor.RED:
					spriteSelector.Sprites = RedSprites;
					break;
				case FindFour.FindFourTokenColor.BLUE:
					spriteSelector.Sprites = BlueSprites;
					break;
				}
			}
			return false;
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<FindFourEvents.ColorChanged>(onColorChanged);
		}
	}
}
