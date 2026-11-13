using UnityEngine;

namespace ClubPenguin.SceneManipulation
{
	public class SceneLayoutIdComponent : MonoBehaviour
	{
		[SerializeField]
		protected long layoutId;

		public long LayoutId
		{
			get
			{
				return layoutId;
			}
		}
	}
}
