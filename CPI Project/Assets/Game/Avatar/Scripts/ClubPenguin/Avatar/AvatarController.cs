using UnityEngine;

namespace ClubPenguin.Avatar
{
	[DisallowMultipleComponent]
	public class AvatarController : MonoBehaviour
	{
		public AvatarModel Model;

		public virtual void Awake()
		{
			if (Model == null)
			{
				Model = GetComponent<AvatarModel>();
			}
		}
	}
}
