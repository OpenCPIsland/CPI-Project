using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class RadialProgressBar : MonoBehaviour
	{
		public Image ProgressImage;

		public float TimePerUpdate;

		public float ProgressPerUpdate;

		private float currentProgress = 0f;

		private float targetProgress;

		private bool isPaused;

		public void SetProgress(float progress)
		{
			currentProgress = progress;
			ProgressImage.fillAmount = currentProgress;
		}

		public void AnimateProgress(float progress)
		{
			targetProgress = progress;
			CoroutineRunner.Start(updateProgress(), this, "updateProgress");
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}

		private IEnumerator updateProgress()
		{
			while (currentProgress < targetProgress)
			{
				if (!isPaused)
				{
					currentProgress += ProgressPerUpdate;
					ProgressImage.fillAmount = currentProgress;
				}
				yield return new WaitForSeconds(TimePerUpdate);
			}
		}

		public void Pause()
		{
			isPaused = true;
		}

		public void UnPause()
		{
			isPaused = false;
		}
	}
}
