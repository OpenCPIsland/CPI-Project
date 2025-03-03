using UnityEngine;

namespace ClubPenguin.MiniGames.Jigsaw
{
	public class JigsawForeground : MonoBehaviour
	{
		public GameObject[] ActivateList;

		public GameObject[] DeactivateList;

		public void OnPieceLocked()
		{
			int num = ActivateList.Length;
			for (int i = 0; i < num; i++)
			{
				ActivateList[i].SetActive(true);
			}
			num = DeactivateList.Length;
			for (int i = 0; i < num; i++)
			{
				DeactivateList[i].SetActive(false);
			}
		}
	}
}
