using System.Collections;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.FSM
{
	public class FadeTransition : Transition
	{
		[SerializeField]
		public float fadeOutSeconds = 0.5f;

		[SerializeField]
		public float fadeInSeconds = 0.5f;

		[SerializeField]
		public float waitSeconds = 0.5f;

		[SerializeField]
		public Color fadeColor = Color.black;

		[SerializeField]
		public Texture2D texture = null;

		private float mTimeElapsed = 0f;

		private Color mCurrentFadeColor = Color.clear;

		private Texture2D mRenderTexture = null;

		private StateChangeArgs mStateChangeDetails = null;

		public override void Perform(StateChangeArgs stateChangeDetails)
		{
			mStateChangeDetails = stateChangeDetails;
			CreateTexture();
			mStateChangeDetails.StartState.RaisePreExitEvent(mStateChangeDetails);
			StartCoroutine(FadeOutScreen());
		}

		public override void Reset()
		{
			mStateChangeDetails = null;
			mTimeElapsed = 0f;
			base.EventDispatcher.ClearAll();
		}

		public void OnGUI()
		{
			GUI.depth = -1000;
			GUI.color = mCurrentFadeColor;
			Rect position = new Rect(0f, 0f, Screen.width, Screen.height);
			if (mRenderTexture != null)
			{
				GUI.DrawTexture(position, mRenderTexture);
			}
			GUI.color = Color.white;
		}

		private Color CalculateFadeColor(Color startColor, Color endColor, float timeInterval)
		{
			Color result = startColor;
			result.r = Mathf.SmoothStep(result.r, endColor.r, timeInterval);
			result.g = Mathf.SmoothStep(result.g, endColor.g, timeInterval);
			result.b = Mathf.SmoothStep(result.b, endColor.b, timeInterval);
			result.a = Mathf.SmoothStep(result.a, endColor.a, timeInterval);
			return result;
		}

		private IEnumerator FadeOutScreen()
		{
			mTimeElapsed = 0f;
			while (mTimeElapsed < fadeOutSeconds)
			{
				mCurrentFadeColor = CalculateFadeColor(Color.clear, Color.white, mTimeElapsed / fadeOutSeconds);
				mTimeElapsed += Time.deltaTime;
				yield return null;
			}
			mCurrentFadeColor = Color.white;
			mStateChangeDetails.StartState.RaiseExitEvent(mStateChangeDetails);
			mStateChangeDetails.StartState.RaisePostExitEvent(mStateChangeDetails);
			yield return new WaitForSeconds(waitSeconds);
			StartCoroutine(FadeInScreen());
		}

		private IEnumerator FadeInScreen()
		{
			mStateChangeDetails.EndState.RaisePreEnterEvent(mStateChangeDetails);
			mTimeElapsed = 0f;
			while (mTimeElapsed < fadeInSeconds)
			{
				mCurrentFadeColor = CalculateFadeColor(Color.white, Color.clear, mTimeElapsed / fadeInSeconds);
				mTimeElapsed += Time.deltaTime;
				yield return null;
			}
			DestroyTexture();
			mStateChangeDetails.EndState.RaiseEnterEvent(mStateChangeDetails);
			mStateChangeDetails.EndState.RaisePostEnterEvent(mStateChangeDetails);
			RaiseTransitionCompletedEvent();
		}

		private void CreateTexture()
		{
			if (texture == null)
			{
				mRenderTexture = new Texture2D(1, 1);
				mRenderTexture.SetPixels(new Color[1]
				{
					fadeColor
				});
				mRenderTexture.Apply();
			}
			else
			{
				mRenderTexture = texture;
			}
		}

		private void DestroyTexture()
		{
			if (mRenderTexture != texture)
			{
				Object.Destroy(mRenderTexture);
			}
			mRenderTexture = null;
		}
	}
}
