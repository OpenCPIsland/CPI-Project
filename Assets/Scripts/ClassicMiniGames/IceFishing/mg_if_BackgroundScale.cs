using MinigameFramework;
using UnityEngine;

namespace IceFishing
{
	public class mg_if_BackgroundScale : MonoBehaviour
	{
		protected void Awake()
		{
			GameObject gameObject = base.transform.Find("mg_if_Background").gameObject;
			Vector3 localScale = gameObject.transform.localScale;
			MinigameSpriteHelper.FitSpriteToScreen(MinigameManager.GetActive().MainCamera, gameObject, false);
			base.transform.localScale = gameObject.transform.localScale;
			gameObject.transform.localScale = localScale;
		}
	}
}
