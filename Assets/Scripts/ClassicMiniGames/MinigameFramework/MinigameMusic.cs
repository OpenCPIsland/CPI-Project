using UnityEngine;

namespace MinigameFramework
{
	public class MinigameMusic : MonoBehaviour
	{
		public AudioSource MusicTrack;

		private void Start()
		{
			Minigame active = MinigameManager.GetActive();
			if (active != null)
			{
				active.RegisterMusic(MusicTrack);
			}
		}
	}
}
