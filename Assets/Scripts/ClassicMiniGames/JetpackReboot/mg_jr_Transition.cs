using MinigameFramework;
using NUnit.Framework;
using System.Collections;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_Transition : MonoBehaviour
	{
		public delegate void OnTransitionCompleted();

		private const float WHITE_FLASH_INITIAL_ALPHA = 0.5882353f;

		private const float WHITE_FLASH_DURATION = 0.1f;

		private const float FLYING_DURATION = 2f;

		private SpriteRenderer m_whiteSplash;

		private mg_jr_SpeedLineScreenFx m_speedLineFX;

		private void Awake()
		{
			base.gameObject.SetActive(false);
			SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>(true);
			foreach (SpriteRenderer spriteRenderer in componentsInChildren)
			{
				if (spriteRenderer.gameObject.name.Equals("mg_jr_TransitionWhiteOverlay"))
				{
					m_whiteSplash = spriteRenderer;
					m_whiteSplash.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.MAX);
				}
				else if (spriteRenderer.gameObject.name.Equals("mg_jr_TransitionBackground"))
				{
					spriteRenderer.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.TRANSITION_BG);
				}
			}
			m_speedLineFX = GetComponentsInChildren<mg_jr_SpeedLineScreenFx>(true)[0];
			Assert.NotNull(m_whiteSplash, "White splash renderer not found");
		}

		public void Transition(mg_jr_Penguin _penguin, OnTransitionCompleted _completionCallback)
		{
			base.gameObject.SetActive(true);
			StartCoroutine(DoTransition(_penguin, _completionCallback));
		}

		private IEnumerator DoTransition(mg_jr_Penguin _penguin, OnTransitionCompleted _completionCallback)
		{
			Assert.NotNull(_penguin, "Penguin must be provided");
			Minigame miniGame = MinigameManager.GetActive();
			miniGame.PlaySFX(mg_jr_Sound.UI_TURBO_MODE_START.ClipName());
			m_speedLineFX.StartLines(mg_jr_SpeedLineScreenFx.LineStartMode.RANDOM_POSITION);
			_penguin.StartTransition();
			miniGame.PlaySFX(mg_jr_Sound.PLAYER_EXPLODE.ClipName());
			yield return StartCoroutine(FlashWhite(0.1f));
			yield return new WaitForSeconds(2f);
			yield return StartCoroutine(FlashWhite(0.1f));
			_penguin.EndTransition();
			m_speedLineFX.StopLinesImmediately();
			miniGame.PlaySFX(mg_jr_Sound.UI_TURBO_MODE_END.ClipName());
			base.gameObject.SetActive(false);
			if (_completionCallback != null)
			{
				_completionCallback();
			}
		}

		private IEnumerator FlashWhite(float _duration)
		{
			float elapsedTimeSinceFlashStart = 0f;
			m_whiteSplash.gameObject.SetActive(true);
			while (elapsedTimeSinceFlashStart < _duration)
			{
				Color currentSplashColor = m_whiteSplash.color;
				float t = Mathf.InverseLerp(0f, 0.1f, elapsedTimeSinceFlashStart);
				currentSplashColor.a = Mathf.Lerp(0.5882353f, 0f, t);
				m_whiteSplash.color = currentSplashColor;
				yield return 0;
				if (!MinigameManager.IsPaused)
				{
					elapsedTimeSinceFlashStart += Time.deltaTime;
				}
			}
			m_whiteSplash.gameObject.SetActive(false);
		}
	}
}
