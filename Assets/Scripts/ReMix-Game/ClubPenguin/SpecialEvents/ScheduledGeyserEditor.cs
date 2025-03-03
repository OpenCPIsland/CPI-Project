using ClubPenguin.Core;
using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.SpecialEvents
{
	public class ScheduledGeyserEditor : MonoBehaviour
	{
		public float GeyserDiameter = 1f;

		public float GeyserHeight = 3f;

		public float MagnetDiameter = 1.2f;

		public Color GeyserColor = Color.green;

		public Color MagnetColor = Color.white;

		private void OnDrawGizmosSelected()
		{
			Vector3? vector = PhysicsUtil.IntersectionPointWithLayer(base.gameObject, LayerConstants.GetPlayerLayerCollisionMask(), 20f, Vector3.down);
			if (vector.HasValue)
			{
				Gizmos.color = GeyserColor;
				Gizmos.DrawLine(base.gameObject.transform.position, vector.Value);
				Gizmos.DrawSphere(vector.Value, 0.05f);
				Gizmos.DrawWireSphere(vector.Value, GeyserDiameter / 2f);
				Gizmos.color = MagnetColor;
				Gizmos.DrawSphere(vector.Value + new Vector3(0f, GeyserHeight, 0f), 0.05f);
				Gizmos.DrawWireSphere(vector.Value + new Vector3(0f, GeyserHeight, 0f), MagnetDiameter / 2f);
			}
		}
	}
}
