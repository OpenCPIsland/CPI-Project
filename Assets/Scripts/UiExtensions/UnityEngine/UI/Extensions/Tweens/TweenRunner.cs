using System.Collections;

namespace UnityEngine.UI.Extensions.Tweens
{
	internal class TweenRunner<T> where T : struct, ITweenValue
	{
		protected MonoBehaviour m_CoroutineContainer;

		protected IEnumerator m_Tween;

		private static IEnumerator Start(T tweenInfo)
		{
			if (tweenInfo.ValidTarget())
			{
				float elapsedTime = 0f;
				while (elapsedTime < tweenInfo.duration)
				{
					elapsedTime += ((!tweenInfo.ignoreTimeScale) ? Time.deltaTime : Time.unscaledDeltaTime);
					float percentage = Mathf.Clamp01(elapsedTime / tweenInfo.duration);
					tweenInfo.TweenValue(percentage);
					yield return null;
				}
				tweenInfo.TweenValue(1f);
				tweenInfo.Finished();
			}
		}

		public void Init(MonoBehaviour coroutineContainer)
		{
			m_CoroutineContainer = coroutineContainer;
		}

		public void StartTween(T info)
		{
			if (m_CoroutineContainer == null)
			{
				Debug.LogWarning("Coroutine container not configured... did you forget to call Init?");
				return;
			}
			if (m_Tween != null)
			{
				m_CoroutineContainer.StopCoroutine(m_Tween);
				m_Tween = null;
			}
			if (!m_CoroutineContainer.gameObject.activeInHierarchy)
			{
				info.TweenValue(1f);
				return;
			}
			m_Tween = Start(info);
			m_CoroutineContainer.StartCoroutine(m_Tween);
		}
	}
}
