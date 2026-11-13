using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_UIWarningSignFactory : mg_jr_GameObjectFactory
	{
		public override GameObject Create()
		{
			GameObject gameObject = null;
			gameObject = base.Resources.GetInstancedResource(mg_jr_ResourceList.WARNING_SIGN);
			Assert.NotNull(gameObject, "No gameobject created.");
			SpriteRenderer componentInChildren = gameObject.GetComponentInChildren<SpriteRenderer>();
			componentInChildren.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.FX_OVERLAY_0);
			return gameObject;
		}
	}
}
