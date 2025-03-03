using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_ConveyorObject : MonoBehaviour
	{
		private static string GO_LEFT = "left";

		private static string GO_RIGHT = "right";

		private mg_pt_ConveyorAsset m_left;

		private mg_pt_ConveyorAsset m_right;

		private float m_assetDistance;

		private float m_currentOffset;

		public Transform PizzaSpawn
		{
			get
			{
				return base.transform.Find("pizza_spawn");
			}
		}

		protected void Awake()
		{
			m_currentOffset = 0f;
		}

		protected void Start()
		{
			m_left = InitConveyorAsset(base.transform.Find(GO_LEFT));
			m_right = InitConveyorAsset(base.transform.Find(GO_RIGHT));
			m_assetDistance = m_right.StartPos.x - m_left.StartPos.x;
		}

		private mg_pt_ConveyorAsset InitConveyorAsset(Transform p_asset)
		{
			mg_pt_ConveyorAsset result = default(mg_pt_ConveyorAsset);
			result.Renderer = p_asset.GetComponent<SpriteRenderer>();
			result.StartPos = p_asset.localPosition;
			return result;
		}

		public void Reposition(float p_deltaTime, float p_conveyorSpeed)
		{
			m_currentOffset += p_deltaTime * p_conveyorSpeed;
			m_currentOffset %= m_assetDistance;
			UpdatePosition(m_left);
			UpdatePosition(m_right);
		}

		private void UpdatePosition(mg_pt_ConveyorAsset p_asset)
		{
			Vector2 startPos = p_asset.StartPos;
			startPos.x += m_currentOffset;
			p_asset.Renderer.transform.localPosition = startPos;
		}
	}
}
