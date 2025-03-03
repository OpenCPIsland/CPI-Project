using MinigameFramework;
using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class mg_jr_StretchToFillScreen : MonoBehaviour
	{
		private void Awake()
		{
			mg_JetpackReboot active = MinigameManager.GetActive<mg_JetpackReboot>();
			SpriteRenderer component = GetComponent<SpriteRenderer>();
			Assert.NotNull(component, "spriterenderer required by mg_jr_StretchToFillScreen");
			float x = component.sprite.bounds.size.x;
			float y = component.sprite.bounds.size.y;
			float num = active.MainCamera.orthographicSize * 2f;
			float num2 = num * active.MainCamera.aspect;
			base.transform.localScale = new Vector3(num2 / x, num / y, 1f);
			base.transform.position = new Vector3(active.VisibleWorldBounds.min.x, active.VisibleWorldBounds.max.y, 0f);
		}
	}
}
