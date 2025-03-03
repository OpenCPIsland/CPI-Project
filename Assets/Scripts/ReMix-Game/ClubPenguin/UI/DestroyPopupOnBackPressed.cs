using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class DestroyPopupOnBackPressed : MonoBehaviour
	{
		private void Start()
		{
			Service.Get<BackButtonController>().Add(onBackButtonClicked);
		}

		private void OnDestroy()
		{
			Service.Get<BackButtonController>().Remove(onBackButtonClicked);
		}

		private void onBackButtonClicked()
		{
			Object.Destroy(base.gameObject);
		}
	}
}
