using MinigameFramework;
using UnityEngine;

namespace JetpackReboot
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class mg_jr_TintApplicator : MonoBehaviour
	{
		private void Start()
		{
			SpriteRenderer component = GetComponent<SpriteRenderer>();
			component.color = MinigameManager.Instance.GetPenguinColor();
			component.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.TINT);
		}
	}
}
