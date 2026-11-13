using ClubPenguin.Adventure;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Graphic))]
	public class QuestColorSelector : MonoBehaviour
	{
		public string[] MascotNames;

		public Color[] Colors;

		private Graphic graphic;

		public void Awake()
		{
			graphic = GetComponent<Graphic>();
		}

		public void Start()
		{
			SelectColor();
		}

		public void RefreshSprite()
		{
			SelectColor();
		}

		private void SelectColor()
		{
			Quest activeQuest = Service.Get<QuestService>().ActiveQuest;
			if (activeQuest != null)
			{
				int num = Array.IndexOf(MascotNames, activeQuest.Mascot.Definition.name);
				if (num >= 0 && num < Colors.Length)
				{
					graphic.color = Colors[num];
				}
			}
		}
	}
}
