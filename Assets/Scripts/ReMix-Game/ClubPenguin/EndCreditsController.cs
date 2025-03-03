using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class EndCreditsController : MonoBehaviour
	{
		public string[] StoryTokens;

		public SpriteSelector StorySpriteSelector;

		public Text StoryText;

		public GameObject StoryContent;

		public GameObject CreditContent;

		public GameObject PenguinImage;

		public ContentScroller ContentScroller;

		public Animator FadeToBlackAnimator;

		public string FadeToBlackStateName;

		public float FadeToBlackNormalizedPosition;

		private int index;

		private Localizer localizer;

		private void Awake()
		{
			if (StoryTokens.Length != StorySpriteSelector.Sprites.Length)
			{
				Log.LogErrorFormatted(this, "There are {0} story tokens, and {1} story sprites. These need to be equal.", StoryTokens.Length, StorySpriteSelector.Sprites.Length);
			}
			localizer = Service.Get<Localizer>();
			StoryContent.SetActive(false);
			CreditContent.SetActive(false);
			PenguinImage.SetActive(false);
			index = 0;
			applyIndex();
		}

		public void OnAnimationComplete()
		{
			index++;
			applyIndex();
		}

		public void OnCreditContentIntroComplete()
		{
			ContentScroller.enabled = true;
			ContentScroller.PositionUpdated += contentScrollerPositionUpdated;
			ContentScroller.ScrollComplete += onCreditsComplete;
		}

		private void contentScrollerPositionUpdated(float normalizedPosition)
		{
			if (normalizedPosition >= FadeToBlackNormalizedPosition)
			{
				ContentScroller.PositionUpdated -= contentScrollerPositionUpdated;
				BeginFadeToBlack();
			}
		}

		private void onCreditsComplete()
		{
			ReturnToZone();
		}

		public void ReturnToZone()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add(SceneTransitionService.SceneArgs.LoadingScreenOverride.ToString(), "LoadingScreenPrefabs/BlackLoadScreen");
			Service.Get<GameStateController>().ReturnToZoneScene(dictionary);
		}

		public void OnFadeInComplete()
		{
			StoryContent.SetActive(true);
		}

		public void BeginFadeToBlack()
		{
			FadeToBlackAnimator.Play(FadeToBlackStateName);
		}

		private void applyIndex()
		{
			if (index < StorySpriteSelector.Sprites.Length && index < StoryTokens.Length)
			{
				StorySpriteSelector.Select(index);
				string text = localizer.GetTokenTranslation(StoryTokens[index]);
				if (index == StoryTokens.Length - 1)
				{
					text = string.Format(text, getDisplayName());
					PenguinImage.SetActive(true);
				}
				StoryText.text = text;
			}
			else
			{
				StoryContent.SetActive(false);
				CreditContent.SetActive(true);
			}
		}

		private string getDisplayName()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DisplayNameData component;
			if (cPDataEntityCollection != null && cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				return component.DisplayName;
			}
			Log.LogError(this, "Could not get display name for local player");
			return null;
		}
	}
}
