using ClubPenguin.Adventure;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Image))]
	public class QuestSpriteSelector : MonoBehaviour
	{
		public string[] MascotNames;

		public Sprite[] Sprites;

		private Image image;

		public void Awake()
		{
			image = GetComponent<Image>();
		}

		public void Start()
		{
			SelectSprite();
		}

		public void RefreshSprite()
		{
			SelectSprite();
		}

		private void SelectSprite()
		{
			if (MascotNames == null || Sprites == null)
			{
				Log.LogError(this, "Misconfigured quest sprite selector. Null properties found");
			}
			else
			{
				if (base.gameObject.IsDestroyed())
				{
					return;
				}
				Quest activeQuest = Service.Get<QuestService>().ActiveQuest;
				if (activeQuest == null)
				{
					return;
				}
				int num = Array.IndexOf(MascotNames, activeQuest.Mascot.Definition.name);
				if (num >= 0 && num < Sprites.Length)
				{
					Sprite sprite = Sprites[num];
					if (sprite != null)
					{
						image.sprite = sprite;
					}
					else
					{
						image.enabled = false;
					}
				}
			}
		}
	}
}
