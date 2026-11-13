using UnityEngine;

namespace ClubPenguin.WorldEditor
{
	public class DestroyRendererAndFilterOnAwake : MonoBehaviour
	{
		private void Awake()
		{
			Object.Destroy(GetComponent<Renderer>());
			Object.Destroy(GetComponent<MeshFilter>());
			Object.Destroy(this);
		}
	}
}
