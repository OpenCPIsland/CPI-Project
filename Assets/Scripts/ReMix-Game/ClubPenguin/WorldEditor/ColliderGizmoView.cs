using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.WorldEditor
{
	[RequireComponent(typeof(Collider))]
	public class ColliderGizmoView : MonoBehaviour
	{
		public enum ColliderColorType
		{
			None,
			ColliderWall,
			InteractiveZone,
			InteractiveObject,
			QuickChatZone,
			Collectible,
			TextTrigger,
			Mascots,
			CameraZone,
			LocomotionTrigger,
			DailyChallenge,
			QuestTriggers
		}

		public ColliderColorType colliderColorType = ColliderColorType.None;

		private float fillValue = 0.1f;

		private List<Color> colorList = new List<Color>();

		private Collider myCollider = null;

		private bool hideFills = false;

		private bool hideOutlines = false;

		private bool selectedGizmo = false;

		public float FillValue
		{
			set
			{
				fillValue = value;
			}
		}

		public bool HideFills
		{
			get
			{
				return hideFills;
			}
			set
			{
				hideFills = value;
			}
		}

		public bool HideOutlines
		{
			get
			{
				return hideOutlines;
			}
			set
			{
				hideOutlines = value;
			}
		}

		public void OnDrawGizmosSelected()
		{
			selectedGizmo = true;
		}

		private void OnDrawGizmos()
		{
			if (myCollider == null)
			{
				GetCollider();
			}
			if (colorList.Count == 0)
			{
				SetColliderColors();
			}
			if (fillValue == 0f)
			{
				return;
			}
			Color color = Gizmos.color;
			Matrix4x4 matrix = Gizmos.matrix;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Color color2 = colorList[0];
			if ((int)colliderColorType < colorList.Count)
			{
				color2 = colorList[(int)colliderColorType];
			}
			color2.a = fillValue;
			Gizmos.color = color2;
			if (!hideFills)
			{
				if (myCollider is BoxCollider)
				{
					Gizmos.DrawCube(((BoxCollider)myCollider).center, ((BoxCollider)myCollider).size);
				}
				else if (myCollider is SphereCollider)
				{
					Gizmos.DrawSphere(((SphereCollider)myCollider).center, ((SphereCollider)myCollider).radius);
				}
				else if (myCollider is MeshCollider)
				{
					Gizmos.DrawMesh(((MeshCollider)myCollider).sharedMesh);
				}
			}
			if (selectedGizmo)
			{
				Gizmos.color = new Color(0f, 0f, 0f, 1f);
			}
			if (!hideOutlines)
			{
				if (myCollider is BoxCollider)
				{
					Gizmos.DrawWireCube(((BoxCollider)myCollider).center, ((BoxCollider)myCollider).size);
				}
				else if (myCollider is SphereCollider)
				{
					Gizmos.DrawWireSphere(((SphereCollider)myCollider).center, ((SphereCollider)myCollider).radius);
				}
				else if (myCollider is MeshCollider)
				{
					Gizmos.DrawWireMesh(((MeshCollider)myCollider).sharedMesh);
				}
			}
			Gizmos.color = color;
			Gizmos.matrix = matrix;
			selectedGizmo = false;
		}

		private void SetColliderColors()
		{
			colorList.Add(Color.black);
			colorList.Add(Color.blue);
			colorList.Add(Color.yellow);
			colorList.Add(Color.green);
			colorList.Add(Color.magenta);
			colorList.Add(Color.cyan);
			colorList.Add(Color.red);
			colorList.Add(Color.grey);
			colorList.Add(Color.white);
			colorList.Add(new Color32(byte.MaxValue, 100, byte.MaxValue, byte.MaxValue));
			colorList.Add(new Color32(128, 64, byte.MaxValue, byte.MaxValue));
			colorList.Add(new Color32(233, 155, 12, byte.MaxValue));
		}

		private void GetCollider()
		{
			myCollider = GetComponent<Collider>();
		}
	}
}
