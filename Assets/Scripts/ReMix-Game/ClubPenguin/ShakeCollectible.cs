using UnityEngine;

namespace ClubPenguin
{
	public class ShakeCollectible : MonoBehaviour
	{
		public float ScaleAmount = 1.5f;

		public iTween.EaseType EaseType = iTween.EaseType.easeOutElastic;

		public float TweenTime = 1.5f;

		public GameObject ParticlesActivated;

		public Vector3 ParticlesOffset = Vector3.zero;

		public Color spawnPointColor = Color.red;

		private bool isShaking = false;

		private Vector3 originalScale;

		private void Awake()
		{
			originalScale = base.gameObject.transform.localScale;
		}

		public void OnActionGraphActivation()
		{
			if (!isShaking)
			{
				isShaking = true;
				animateShake();
				if (ParticlesActivated != null)
				{
					Object.Instantiate(ParticlesActivated, base.gameObject.transform.position + ParticlesOffset, ParticlesActivated.transform.rotation);
				}
			}
		}

		private void animateShake()
		{
			Vector3 localScale = base.gameObject.transform.localScale;
			localScale.x *= ScaleAmount;
			localScale.y *= ScaleAmount;
			localScale.z *= ScaleAmount;
			iTween.ScaleFrom(base.gameObject, iTween.Hash("scale", localScale, "easeType", EaseType, "time", TweenTime, "oncomplete", "onScaleFromComplete", "oncompletetarget", base.gameObject));
		}

		private void onScaleFromComplete()
		{
			isShaking = false;
			base.gameObject.transform.localScale = originalScale;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = spawnPointColor;
			Vector3 vector = base.gameObject.transform.position + ParticlesOffset;
			Gizmos.DrawSphere(vector, 0.05f);
			Gizmos.DrawLine(vector + new Vector3(-1f, 0f, 0f), vector + new Vector3(1f, 0f, 0f));
			Gizmos.DrawLine(vector + new Vector3(0f, -1f, 0f), vector + new Vector3(0f, 1f, 0f));
			Gizmos.DrawLine(vector + new Vector3(0f, 0f, -1f), vector + new Vector3(0f, 0f, 1f));
		}
	}
}
