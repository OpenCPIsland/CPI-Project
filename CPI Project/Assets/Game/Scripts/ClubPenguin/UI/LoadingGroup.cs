using UnityEngine;

namespace ClubPenguin.UI
{
	public class LoadingGroup : MonoBehaviour
	{
		private void Start()
		{
			CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
			if (canvasGroup == null)
			{
				canvasGroup = base.gameObject.AddComponent<CanvasGroup>();
			}
			canvasGroup.alpha = 0f;
		}

		public void OnLoadingComplete()
		{
			base.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
		}
	}
}
