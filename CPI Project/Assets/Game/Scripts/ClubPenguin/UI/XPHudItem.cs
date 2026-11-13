using Fabric;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class XPHudItem : MonoBehaviour
	{
		private const string LOOP_SOUND_EVENT = "SFX/UI/XP/CollectLoop";

		public Action IntroAnimationCompleteAction;

		public Action OutroAnimationCompleteAction;

		public Image LevelProgressImage;

		public Animator XPAnimator;

		public void OnIntroAnimationComplete()
		{
			if (IntroAnimationCompleteAction != null)
			{
				IntroAnimationCompleteAction();
			}
		}

		public void OnOutroAnimationComplete()
		{
			if (OutroAnimationCompleteAction != null)
			{
				OutroAnimationCompleteAction();
			}
		}

		public void EnableParticleSystems()
		{
			ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				ParticleSystem.EmissionModule emission = componentsInChildren[i].emission;
				emission.enabled = true;
			}
			startLoopAudio();
		}

		public void DisableParticleSystems()
		{
			ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				ParticleSystem.EmissionModule emission = componentsInChildren[i].emission;
				emission.enabled = false;
			}
			stopLoopAudio();
		}

		private void OnDestroy()
		{
			stopLoopAudio();
		}

		private void startLoopAudio()
		{
			EventManager.Instance.PostEvent("SFX/UI/XP/CollectLoop", EventAction.PlaySound, this);
		}

		private void stopLoopAudio()
		{
			EventManager.Instance.PostEvent("SFX/UI/XP/CollectLoop", EventAction.StopSound, this);
		}
	}
}
