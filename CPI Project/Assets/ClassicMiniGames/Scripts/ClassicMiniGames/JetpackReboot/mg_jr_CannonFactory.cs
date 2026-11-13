using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_CannonFactory : mg_jr_ObstacleFactory
	{
		protected mg_jr_Sound m_shootSound;

		protected float m_shootStartDistance;

		public mg_jr_CannonFactory(mg_jr_ResourceList _cannon, mg_jr_Sound _shootSound, float _shootStartDistance)
			: base(_cannon)
		{
			m_shootSound = _shootSound;
			m_shootStartDistance = _shootStartDistance;
		}

		public override GameObject Create()
		{
			GameObject gameObject = base.Create();
			SpriteRenderer componentInChildren = gameObject.GetComponentInChildren<SpriteRenderer>();
			componentInChildren.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.OBSTACLE_1);
			mg_jr_Cannon component = gameObject.GetComponent<mg_jr_Cannon>();
			Assert.NotNull(component, "Cannon component is required for a cannon");
			component.ShootSound = m_shootSound;
			component.StartFiringDistance = m_shootStartDistance;
			return gameObject;
		}
	}
}
