using UnityEngine;

namespace ClubPenguin.UI
{
	public class UIChatRoot : MonoBehaviour
	{
		private void Awake()
		{
			SceneRefs.SetUiChatRoot(base.gameObject);
		}
	}
}
