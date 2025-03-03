using UnityEngine;

namespace ClubPenguin.UI
{
	public interface IDisneyStoreController
	{
		DisneyStoreTrayAnimator GetTrayAnimator();

		void OnCloseClicked();

		void ShowLoadingModal();

		void HideLoadingModal();

		void onLoadingModalLoadComplete(string Path, GameObject loadingModalPrefab);
	}
}
