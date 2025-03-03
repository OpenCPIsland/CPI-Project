using ClubPenguin.Adventure;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class QuestGameObjectSelector : MonoBehaviour
	{
		public string[] MascotNames;

		public GameObject[] GameObjects;

		public GameObject CurrentSelectedObject
		{
			get;
			private set;
		}

		public void Awake()
		{
			for (int i = 0; i < GameObjects.Length; i++)
			{
				GameObjects[i].SetActive(false);
			}
			SelectGameObject();
		}

		private void SelectGameObject()
		{
			if (MascotNames == null || GameObjects == null)
			{
				Log.LogError(this, "Misconfigured quest gameobject selector. Null properties found");
			}
			else
			{
				if (base.gameObject.IsDestroyed())
				{
					return;
				}
				Quest activeQuest = Service.Get<QuestService>().ActiveQuest;
				if (activeQuest != null)
				{
					int num = Array.IndexOf(MascotNames, activeQuest.Mascot.Definition.name);
					if (num >= 0 && num < GameObjects.Length)
					{
						CurrentSelectedObject = GameObjects[num];
						CurrentSelectedObject.SetActive(true);
					}
				}
			}
		}
	}
}
