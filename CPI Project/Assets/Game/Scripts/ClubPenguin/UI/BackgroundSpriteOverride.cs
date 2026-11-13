using Disney.LaunchPadFramework;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class BackgroundSpriteOverride : MonoBehaviour
	{
		public Sprite[] OverrideSprites;

		private TrayInputButton trayInputButton;

		private Sprite[] originalSprites;

		private bool isOverridden;

		private void Start()
		{
			trayInputButton = GetComponentInParent<TrayInputButton>();
			if (trayInputButton != null)
			{
				if (trayInputButton.IsBackgroundVisible)
				{
					Sprite[] sprites = trayInputButton.BackgroundSprite.Sprites;
					originalSprites = new Sprite[OverrideSprites.Length];
					for (int i = 0; i < OverrideSprites.Length; i++)
					{
						originalSprites[i] = sprites[i];
						if (OverrideSprites[i] != null)
						{
							sprites[i] = OverrideSprites[i];
							isOverridden = true;
						}
					}
					int currentViewState = (int)trayInputButton.CurrentViewState;
					trayInputButton.BackgroundSprite.SelectSprite(currentViewState);
				}
				else
				{
					Log.LogError(this, "Background sprite is not visible for this button");
				}
			}
			else
			{
				Log.LogError(this, "Could not find TrayInputButton in parent");
			}
		}

		private void OnDestroy()
		{
			if (trayInputButton != null && isOverridden)
			{
				for (int i = 0; i < OverrideSprites.Length; i++)
				{
					trayInputButton.BackgroundSprite.Sprites[i] = originalSprites[i];
				}
				int currentViewState = (int)trayInputButton.CurrentViewState;
				trayInputButton.BackgroundSprite.SelectSprite(currentViewState);
			}
		}
	}
}
