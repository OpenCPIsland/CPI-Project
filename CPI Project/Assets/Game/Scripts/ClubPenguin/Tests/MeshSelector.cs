using UnityEngine;

namespace ClubPenguin.Tests
{
	[RequireComponent(typeof(MeshFilter))]
	public class MeshSelector : MonoBehaviour
	{
		public Mesh[] Meshes;

		public void SelectMesh(int index)
		{
			GetComponent<MeshFilter>().mesh = Meshes[index];
		}
	}
}
