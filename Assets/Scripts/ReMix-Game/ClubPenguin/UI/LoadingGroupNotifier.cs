using UnityEngine;

namespace ClubPenguin.UI
{
	public class LoadingGroupNotifier : MonoBehaviour
	{
		private void Start()
		{
			LoadingGroup componentInParent = GetComponentInParent<LoadingGroup>();
			if (componentInParent != null)
			{
				componentInParent.OnLoadingComplete();
			}
		}
	}
}
