using CameraExtensionMethods;
using MinigameFramework;
using System.Collections.Generic;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_Level : MonoBehaviour
	{
		public mg_jr_LevelDefinition LevelDefinition
		{
			get;
			set;
		}

		public bool IsSafelyOffLeftOfScreen()
		{
			bool flag = false;
			float num = base.transform.position.x + LevelDefinition.Size.x;
			return num < Camera.main.LeftEdgeInWorld() - 1f;
		}

		public bool IsOnScreen()
		{
			Vector3 b = new Vector3(LevelDefinition.Size.x / 2f, (0f - LevelDefinition.Size.y) / 2f, 1f);
			Vector3 center = base.transform.position + b;
			Vector3 size = new Vector3(LevelDefinition.Size.x, LevelDefinition.Size.y, 1f);
			Bounds bounds = new Bounds(center, size);
			Bounds visibleWorldBounds = MinigameManager.GetActive<mg_JetpackReboot>().VisibleWorldBounds;
			return bounds.Intersects(visibleWorldBounds);
		}

		public Vector3 TopRightCornerInWorld()
		{
			return base.transform.position + new Vector3(LevelDefinition.Size.x, 0f, 0f);
		}

		public void DestroyLevel()
		{
			mg_jr_Resources resources = MinigameManager.GetActive<mg_JetpackReboot>().Resources;
			List<Transform> list = new List<Transform>();
			foreach (Transform item in base.transform)
			{
				list.Add(item);
			}
			foreach (Transform item2 in list)
			{
				resources.ReturnPooledResource(item2.gameObject);
			}
			Object.Destroy(base.gameObject);
		}
	}
}
