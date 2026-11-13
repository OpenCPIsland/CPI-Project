using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_SplatObject : MonoBehaviour
	{
		public void Initialize(Vector2 p_position, float p_scale, Color p_color)
		{
			GetComponentInChildren<SpriteRenderer>().color = p_color;
			base.transform.position = p_position;
			Vector2 v = base.transform.localScale;
			v.x = p_scale;
			v.y = p_scale;
			base.transform.localScale = v;
		}
	}
}
