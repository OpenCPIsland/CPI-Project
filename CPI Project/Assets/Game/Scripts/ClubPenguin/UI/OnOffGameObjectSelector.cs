using UnityEngine;

namespace ClubPenguin.UI
{
	public class OnOffGameObjectSelector : MonoBehaviour
	{
		public GameObject[] OffObjects;

		public GameObject[] OnObjects;

		private bool isOn;

		public bool IsOn
		{
			get
			{
				return isOn;
			}
			set
			{
				isOn = value;
				for (int i = 0; i < OffObjects.Length; i++)
				{
					OffObjects[i].SetActive(!isOn);
				}
				for (int i = 0; i < OnObjects.Length; i++)
				{
					OnObjects[i].SetActive(isOn);
				}
			}
		}
	}
}
