using UnityEngine;

namespace ClubPenguin.WorldEditor.Optimization
{
	public class VisibilityRegion : MonoBehaviour
	{
		private const float STEP_SIZE = 1.5f;

		[Range(1f, 360f)]
		public float DirectionalFovDegs = 45f;

		[HideInInspector]
		public bool DrawGizmos = true;

		private float lastFovDot;

		private VisibilityDirection visibilityDirection;

		public Bounds Bounds
		{
			get
			{
				return new Bounds(base.transform.position, base.transform.localScale);
			}
		}

		public Vector3 WorldSpaceDirection
		{
			get
			{
				return GetVisibilityDirection().TransformRef.forward;
			}
		}

		private void Awake()
		{
			Object.Destroy(base.gameObject);
		}

		public VisibilityDirection GetVisibilityDirection()
		{
			if (visibilityDirection == null)
			{
				visibilityDirection = GetComponentInChildren<VisibilityDirection>();
				if (visibilityDirection == null)
				{
					GameObject gameObject = new GameObject();
					gameObject.name = "Visibility Direction";
					gameObject.transform.SetParent(base.transform, false);
					visibilityDirection = gameObject.AddComponent<VisibilityDirection>();
				}
			}
			return visibilityDirection;
		}

		private void OnDrawGizmos()
		{
			if (DrawGizmos)
			{
				drawDirecitonGizmos();
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireCube(Bounds.center, base.transform.localScale);
			}
		}

		private void drawDirecitonGizmos()
		{
			VisibilityDirection visibilityDirection = GetVisibilityDirection();
			visibilityDirection.TransformRef.localPosition = default(Vector3);
			Vector3 forward = visibilityDirection.TransformRef.forward;
			float num = Bounds.center.x - Bounds.extents.x;
			float num2 = Bounds.center.x + Bounds.extents.x;
			float num3 = Bounds.center.y - Bounds.extents.y;
			float num4 = Bounds.center.y + Bounds.extents.y;
			float num5 = Bounds.center.z - Bounds.extents.z;
			float num6 = Bounds.center.z + Bounds.extents.z;
			Vector3 vector = default(Vector3);
			for (float num7 = num5; num7 < num6; num7 += 1.5f)
			{
				vector.z = num7;
				for (float num8 = num3; num8 < num4; num8 += 1.5f)
				{
					vector.y = num8;
					for (float num9 = num; num9 < num2; num9 += 1.5f)
					{
						vector.x = num9;
						Gizmos.color = Color.gray;
						Gizmos.DrawLine(vector, vector + forward * 0.8f);
						Gizmos.color = Color.green;
						Gizmos.DrawLine(vector + forward * 0.8f, vector + forward);
					}
				}
			}
		}
	}
}
