using UnityEngine;

namespace ClubPenguin
{
	public class BackButtonDisabler : MonoBehaviour
	{
		private void OnEnable()
		{
			BackButtonStateHandler componentInParent = GetComponentInParent<BackButtonStateHandler>();
			if (!(componentInParent == null) && !componentInParent.CanGoBack())
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
