using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_ObstacleFactory : mg_jr_GameObjectFactory
	{
		protected mg_jr_ResourceList m_resourceId;

		protected mg_jr_SpriteDrawingLayers.DrawingLayers m_defaultDrawingLayer;

		public mg_jr_ObstacleFactory(mg_jr_ResourceList _resource)
			: this(_resource, mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_0)
		{
		}

		public mg_jr_ObstacleFactory(mg_jr_ResourceList _resource, mg_jr_SpriteDrawingLayers.DrawingLayers _drawingLayer)
		{
			m_resourceId = _resource;
			m_defaultDrawingLayer = _drawingLayer;
		}

		public override GameObject Create()
		{
			GameObject gameObject = null;
			gameObject = base.Resources.GetInstancedResource(m_resourceId);
			Assert.NotNull(gameObject, "No gameobject created.");
			SpriteRenderer componentInChildren = gameObject.GetComponentInChildren<SpriteRenderer>();
			componentInChildren.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(m_defaultDrawingLayer);
			Collider2D component = gameObject.GetComponent<Collider2D>();
			Assert.NotNull(component, "Obstacles must have a collider on the root object");
			Assert.IsTrue(component.isTrigger, "Obstacles must be triggers");
			if (component.gameObject.GetComponent<mg_jr_Obstacle>() == null)
			{
				component.gameObject.AddComponent<mg_jr_Obstacle>();
			}
			mg_jr_DestroyWhenPastScreen[] componentsInChildren = gameObject.GetComponentsInChildren<mg_jr_DestroyWhenPastScreen>(true);
			Assert.IsFalse(componentsInChildren.Length > 1, "Should only be one mg_jr_DestroyWhenPastScreen on an obstacle");
			if (componentsInChildren.Length == 0)
			{
				gameObject.AddComponent<mg_jr_DestroyWhenPastScreen>();
			}
			return gameObject;
		}
	}
}
