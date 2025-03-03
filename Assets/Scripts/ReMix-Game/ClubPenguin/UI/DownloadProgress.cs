using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(RectTransform))]
	public class DownloadProgress : MonoBehaviour
	{
		private LoadingController loadingController;

		private RectTransform rectransform;

		private void Awake()
		{
			loadingController = Service.Get<LoadingController>();
			rectransform = GetComponent<RectTransform>();
		}

		private void Update()
		{
			float? downloadProgress = loadingController.DownloadProgress;
			Vector3 localScale = rectransform.localScale;
			localScale.x = 0f;
			if (downloadProgress.HasValue)
			{
				localScale.x = downloadProgress.Value;
			}
			rectransform.localScale = localScale;
		}
	}
}
