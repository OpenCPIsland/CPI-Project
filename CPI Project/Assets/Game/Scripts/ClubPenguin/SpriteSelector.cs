using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(Image))]
	public class SpriteSelector : Selector
	{
		public Sprite[] Sprites;

		private Image _image;

		private Image image
		{
			get
			{
				if (_image == null)
				{
					_image = GetComponent<Image>();
				}
				return _image;
			}
		}

		public override void Select(int index)
		{
			SelectSprite(index);
		}

		public void SelectSprite(int index)
		{
			if (Sprites != null)
			{
				if (index < Sprites.Length)
				{
					image.enabled = true;
					if (Sprites[index] != null)
					{
						image.sprite = Sprites[index];
					}
					else
					{
						Log.LogErrorFormatted(this, "Sprite at index {0} was null game object {1}", index, base.name);
					}
				}
				else
				{
					Log.LogErrorFormatted(this, "Sprites index out of bounds on game object {0}", base.name);
				}
			}
			else
			{
				Log.LogErrorFormatted(this, "Sprites array was null on game object {0}", base.name);
			}
		}
	}
}
