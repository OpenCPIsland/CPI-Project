using UnityEngine;

namespace ClubPenguin
{
	public class BackgroundButtonsController : MonoBehaviour
	{
		public GameObject[] EnableButtons;

		public GameObject[] DisableButtons;

		private void Start()
		{
			GameObject[] enableButtons = EnableButtons;
			foreach (GameObject gameObject in enableButtons)
			{
				gameObject.SetActive(true);
			}
			enableButtons = DisableButtons;
			foreach (GameObject gameObject in enableButtons)
			{
				gameObject.SetActive(false);
			}
		}
	}
}
