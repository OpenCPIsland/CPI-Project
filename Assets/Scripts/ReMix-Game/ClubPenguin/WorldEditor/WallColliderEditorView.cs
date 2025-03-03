using UnityEngine;

namespace ClubPenguin.WorldEditor
{
	[DisallowMultipleComponent]
	public class WallColliderEditorView : MonoBehaviour
	{
		[HideInInspector]
		public Renderer Renderer;

		private void Awake()
		{
			Object.Destroy(this);
		}
	}
}
