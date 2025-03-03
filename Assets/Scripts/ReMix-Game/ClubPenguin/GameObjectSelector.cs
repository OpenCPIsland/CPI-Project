using Disney.LaunchPadFramework;
using UnityEngine;

namespace ClubPenguin
{
	public class GameObjectSelector : MonoBehaviour
	{
		public GameObject[] GameObjects;

		public int DefaultIndex = 0;

		private int currentSelectedIndex;

		public GameObject SelectedObject
		{
			get
			{
				return GameObjects[currentSelectedIndex];
			}
		}

		private void Awake()
		{
			SelectGameObject(DefaultIndex);
		}

		public void SelectGameObject(int index)
		{
			if (GameObjects != null && index < GameObjects.Length)
			{
				for (int i = 0; i < GameObjects.Length; i++)
				{
					if (i != index)
					{
						GameObjects[i].SetActive(false);
					}
					else
					{
						GameObjects[i].SetActive(true);
					}
				}
				currentSelectedIndex = index;
			}
			else
			{
				Log.LogErrorFormatted(this, "GameObjects array was null on game object {0}", base.name);
			}
		}
	}
}
