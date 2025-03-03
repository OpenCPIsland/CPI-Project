using MinigameFramework;
using UnityEngine;

namespace IceFishing
{
	public class mg_if_Puffle : mg_if_GameObject
	{
		private GameObject m_text;

		protected override void Awake()
		{
			base.Awake();
			m_text = base.transform.Find("mg_if_Text").gameObject;
			m_text.GetComponent<Renderer>().sortingOrder = 23;
		}

		public override void Spawn()
		{
			base.Spawn();
			MinigameManager.GetActive().PlaySFX("mg_if_sfx_PuffleSwimBubbles");
			Vector2 vector = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
			TweenPosition.Begin(base.gameObject, 8f, base.gameObject.transform.position + new Vector3(vector.x * 2f, 0f, 0f));
		}

		public void SetText(string p_text)
		{
			m_text.GetComponent<TextMesh>().text = p_text;
		}

		public void OnAnimationFinished()
		{
			MinigameManager.GetActive().StopSFX("mg_if_sfx_PuffleSwimBubbles");
			Despawn();
		}
	}
}
