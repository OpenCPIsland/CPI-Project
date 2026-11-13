using MinigameFramework;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_BackgroundScale : MonoBehaviour
	{
		protected void Awake()
		{
			GameObject gameObject = base.transform.Find("mg_ss_Background").gameObject;
			Vector3 localScale = gameObject.transform.localScale;
			MinigameSpriteHelper.FitSpriteToScreen(MinigameManager.GetActive().MainCamera, gameObject, false);
			base.transform.localScale = gameObject.transform.localScale;
			gameObject.transform.localScale = localScale;
		}
	}
}
