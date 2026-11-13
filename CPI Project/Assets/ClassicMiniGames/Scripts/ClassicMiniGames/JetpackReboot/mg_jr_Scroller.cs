using MinigameFramework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_Scroller : MonoBehaviour
	{
		private mg_jr_ScrollingSpeed m_scrollingData;

		public mg_jr_SpriteDrawingLayers.DrawingLayers ScrollingLayer
		{
			get;
			set;
		}

		private void Awake()
		{
			ScrollingLayer = mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_0;
		}

		private void Start()
		{
			mg_JetpackReboot active = MinigameManager.GetActive<mg_JetpackReboot>();
			m_scrollingData = active.GameLogic.ScrollingSpeed;
			ScrollingLayer = mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_0;
		}

		private void Update()
		{
			if (!MinigameManager.IsPaused)
			{
				Vector3 vector = new Vector3((0f - Time.deltaTime) * m_scrollingData.CurrentSpeedFor(ScrollingLayer), 0f, 0f);
				base.transform.position += vector;
			}
		}
	}
}
