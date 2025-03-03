using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_FxFactory : mg_jr_GameObjectFactory
	{
		protected mg_jr_ResourceList m_resourceId;

		protected mg_jr_SpriteDrawingLayers.DrawingLayers m_defaultDrawingLayer;

		public mg_jr_FxFactory(mg_jr_ResourceList _resource)
		{
			m_resourceId = _resource;
			m_defaultDrawingLayer = mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_0;
		}

		public mg_jr_FxFactory(mg_jr_ResourceList _resource, mg_jr_SpriteDrawingLayers.DrawingLayers _drawingLayer)
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
			return gameObject;
		}
	}
}
