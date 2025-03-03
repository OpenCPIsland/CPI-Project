using MinigameFramework;
using UnityEngine;

namespace IceFishing
{
	public class mg_if_TitleLogic : MonoBehaviour
	{
		public void PlayGame()
		{
			MinigameManager.GetActive<mg_IceFishing>().ShowGame();
		}
	}
}
