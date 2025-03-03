using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class SplashScreenController : MonoBehaviour
	{
		public Text MessageText;

		public float ShowTime = 3f;

		private float showTimer = 0f;

		private bool isShowing = true;

		private bool resourcesUnloaded = false;

		public void Start()
		{
			resourcesUnloaded = false;
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(SplashScreenEvents.SplashScreenClosed));
		}

		public void Update()
		{
			if (isShowing)
			{
				showTimer += Time.deltaTime;
				if (resourcesUnloaded && showTimer > ShowTime)
				{
					Hide();
				}
			}
		}

		public void SetMessage(string message)
		{
			MessageText.text = message;
		}

		private void Show()
		{
			GetComponent<Animator>().SetTrigger("Intro");
			isShowing = true;
		}

		private void Hide()
		{
			GetComponent<Animator>().SetTrigger("Outro");
			isShowing = false;
		}

		private void AnimationComplete(string position)
		{
			if (isShowing && position == "END")
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(SplashScreenEvents.SplashScreenOpened));
				StartCoroutine(unloadResourceAssets());
			}
			else if (!isShowing && position == "START")
			{
				Object.Destroy(base.gameObject);
			}
		}

		private IEnumerator unloadResourceAssets()
		{
			yield return Resources.UnloadUnusedAssets();
			resourcesUnloaded = true;
		}
	}
}
