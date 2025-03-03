using CameraExtensionMethods;
using MinigameFramework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_TurboAnnounce : MonoBehaviour
	{
		public delegate void OnAnnounceComplete();

		private OnAnnounceComplete m_callback;

		private Animator m_animator;

		private void Awake()
		{
			Transform transform = base.transform.Find("mg_jr_BackgroundEffect");
			SpriteRenderer component = transform.GetComponent<SpriteRenderer>();
			component.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.FX_OVERLAY_0);
			m_animator = GetComponent<Animator>();
			Transform transform2 = base.transform.Find("mg_jr_Text");
			SpriteRenderer component2 = transform2.GetComponent<SpriteRenderer>();
			component2.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.FX_OVERLAY_1);
			Vector3 position = new Vector3(Camera.main.LeftEdgeInWorld(), base.transform.position.y, 0f);
			base.transform.position = position;
			float num = Camera.main.RightEdgeInWorld() - Camera.main.LeftEdgeInWorld();
			float x = component.bounds.size.x;
			float x2 = num / x;
			base.transform.localScale = new Vector3(x2, 1f, 1f);
			component.sprite = null;
		}

		public void Announce(OnAnnounceComplete _completionCallback)
		{
			m_callback = _completionCallback;
			m_animator.SetTrigger("Announce");
		}

		private void OnAnimationComplete()
		{
			if (m_callback != null)
			{
				m_callback();
				m_callback = null;
			}
			MinigameManager.GetActive<mg_JetpackReboot>().Resources.ReturnPooledResource(base.gameObject);
		}
	}
}
