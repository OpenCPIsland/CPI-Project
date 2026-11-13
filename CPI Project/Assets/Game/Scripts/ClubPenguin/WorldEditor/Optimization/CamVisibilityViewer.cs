using UnityEngine;

namespace ClubPenguin.WorldEditor.Optimization
{
	[RequireComponent(typeof(CamVisibilityIterator))]
	public class CamVisibilityViewer : MonoBehaviour
	{
		public Color Color = Color.blue;

		public bool DrawOriginal = true;

		public Color DerivedColor = Color.yellow;

		public bool DrawDerived = true;

		public float CubeSize = 0.15f;

		public bool DrawUpVectors = false;

		public bool DrawRightVectors = false;

		public void OnDrawGizmos()
		{
			CamVisibilityIterator component = GetComponent<CamVisibilityIterator>();
			CameraVisData camVisData = component.CamVisData;
			if (!(camVisData != null) || camVisData.Positions == null)
			{
				return;
			}
			Vector3 size = new Vector3(CubeSize, CubeSize, CubeSize);
			for (int i = 0; i < camVisData.Positions.Length; i++)
			{
				float d = 0.5f;
				float d2 = 0.5f;
				float d3 = 0.5f;
				if (camVisData.IsDerived[i])
				{
					if (!DrawDerived)
					{
						continue;
					}
					Gizmos.color = DerivedColor;
					d = 0.25f;
					d2 = 0.25f;
					d3 = 0.25f;
				}
				else
				{
					if (!DrawOriginal)
					{
						continue;
					}
					Gizmos.color = Color;
				}
				Gizmos.DrawCube(camVisData.Positions[i], size);
				Gizmos.DrawLine(camVisData.Positions[i], camVisData.Positions[i] + camVisData.ForwardVectors[i] * d);
				if (DrawRightVectors)
				{
					Gizmos.color = Color.red;
					Gizmos.DrawLine(camVisData.Positions[i], camVisData.Positions[i] + camVisData.RightVectors[i] * d2);
				}
				if (DrawUpVectors)
				{
					Gizmos.color = Color.green;
					Gizmos.DrawLine(camVisData.Positions[i], camVisData.Positions[i] + camVisData.UpVectors[i] * d3);
				}
			}
		}
	}
}
