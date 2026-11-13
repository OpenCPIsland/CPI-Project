using UnityEngine;

namespace ClubPenguin
{
	public class DestroyOnStart : MonoBehaviour
	{
		[Tooltip("Keep object alive when debugging in editor?")]
		[Header("Attached GameObject will be duestroyed when game starts")]
		public bool DontDestroyInEditor = false;

		public void Awake()
		{
			Object.Destroy(base.gameObject);
		}
	}
}
