using UnityEngine;

namespace ClubPenguin
{
	[ExecuteInEditMode]
	public class PenguinViewportSimulator : MonoBehaviour
	{
		private void Update()
		{
			ClampZCoord();
		}

		private void OnDrawGizmosSelected()
		{
			ClampZCoord();
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(base.transform.position, new Vector3(5.5f, 5.3f, 0f));
			Gizmos.DrawWireCube(base.transform.position, new Vector3(7.4f, 5.3f, 0f));
			Gizmos.color = Color.grey;
			Gizmos.DrawWireSphere(base.transform.position, 0.5f);
		}

		private void ClampZCoord()
		{
			Vector3 position = base.gameObject.transform.position;
			position.z = 0f;
			base.gameObject.transform.position = position;
		}
	}
}
