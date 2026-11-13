using UnityEngine;

namespace ClubPenguin.UI
{
	public class UITrayRoot : MonoBehaviour
	{
		private void Awake()
		{
			SceneRefs.SetUiTrayRoot(base.gameObject);
		}
	}
}
