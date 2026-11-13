using System.Collections;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/UI Tween Scale")]
	public class UI_TweenScale : MonoBehaviour
	{
		public AnimationCurve animCurve;

		[Tooltip("Animation speed multiplier")]
		public float speed = 1f;

		[Tooltip("If true animation will loop, for best effect set animation curve to loop on start and end point")]
		public bool isLoop = false;

		[Tooltip("If true animation will start automatically, otherwise you need to call Play() method to start the animation")]
		public bool playAtAwake = false;

		[Space(10f)]
		[Header("Non uniform scale")]
		[Tooltip("If true component will scale by the same amount in X and Y axis, otherwise use animCurve for X scale and animCurveY for Y scale")]
		public bool isUniform = true;

		public AnimationCurve animCurveY;

		private Vector3 initScale;

		private Transform myTransform;

		private Vector3 newScale = Vector3.one;

		private void Awake()
		{
			myTransform = GetComponent<Transform>();
			initScale = myTransform.localScale;
			if (playAtAwake)
			{
				Play();
			}
		}

		public void Play()
		{
			StartCoroutine("Tween");
		}

		private IEnumerator Tween()
		{
			myTransform.localScale = initScale;
			float t = 0f;
			float maxT = animCurve.keys[animCurve.length - 1].time;
			while (t < maxT || isLoop)
			{
				t += speed * Time.deltaTime;
				if (!isUniform)
				{
					newScale.x = 1f * animCurve.Evaluate(t);
					newScale.y = 1f * animCurveY.Evaluate(t);
					myTransform.localScale = newScale;
				}
				else
				{
					myTransform.localScale = Vector3.one * animCurve.Evaluate(t);
				}
				yield return null;
			}
		}

		public void ResetTween()
		{
			StopCoroutine("Tween");
			myTransform.localScale = initScale;
		}
	}
}
