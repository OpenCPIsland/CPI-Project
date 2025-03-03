using ClubPenguin.Analytics;
using ClubPenguin.Avatar;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common.SEDFSM;
using Disney.MobileNetwork;
using System;
using System.Reflection;
using UnityEngine;

namespace ClubPenguin
{
	public class CreatePopupWizardContentController : CreatePopupContentController
	{
		[Header("Progress Bar")]
		[SerializeField]
		private AvatarRenderTextureComponent[] shardAvatarRenderers;

		[SerializeField]
		private GameObject[] shardProgressBarPanels;

		[SerializeField]
		private Animator[] shardProgressBarAnimators;

		[Header("Transition State Handlers")]
		[SerializeField]
		private GameObject[] shardTransitionStateHandlers;

		[Header("InitialActiveForms")]
		[SerializeField]
		private GameObject[] shardInitialActiveFormContainers;

		[SerializeField]
		private CreateWizardProgressTextInfo[] shardInitialProgressText;

		[SerializeField]
		[Header("Swrve Resource ID")]
		private string swrveResourceUniqueID;

		protected override void Start()
		{
			shardSetup();
			base.Start();
		}

		public void UpdateProgressBar(string progressBarTrigger, string avatarAnimation)
		{
			if (shardProgressBarAnimators[shard] != null && !string.IsNullOrEmpty(progressBarTrigger))
			{
				shardProgressBarAnimators[shard].SetTrigger(progressBarTrigger);
			}
			if (shardAvatarRenderers[shard] != null && !string.IsNullOrEmpty(avatarAnimation))
			{
				AvatarAnimationFrame animationFrame = new AvatarAnimationFrame(avatarAnimation, 0f);
				shardAvatarRenderers[shard].PlayAnimation(animationFrame);
			}
		}

		protected override void startInputValidation()
		{
			if (!legalCheckBoxes.IsValidationInProgress && !legalCheckBoxes.IsValidationComplete)
			{
				legalCheckBoxes.ValidateLegalCheckBoxes();
			}
		}

		protected override bool isValidationComplete()
		{
			return legalCheckBoxes.IsValidationComplete;
		}

		protected override bool checkForValidationErrors()
		{
			return legalCheckBoxes.HasError;
		}

		protected override void resetValidation()
		{
			legalCheckBoxes.IsValidationComplete = false;
		}

		protected override void showUsernameError(string errorMessage)
		{
			base.showUsernameError(errorMessage);
			GetComponent<StateMachine>().SendEvent("error_username");
		}

		protected override void showPasswordError(string errorMessage)
		{
			base.showPasswordError(errorMessage);
			GetComponent<StateMachine>().SendEvent("error_password");
		}

		protected override void showParentEmailError(string errorMessage)
		{
			base.showParentEmailError(errorMessage);
			GetComponent<StateMachine>().SendEvent("error_parentemail");
		}

		protected override void showFirstNameError(string errorMessage)
		{
			base.showFirstNameError(errorMessage);
			GetComponent<StateMachine>().SendEvent("error_firstname");
		}

		private void shardSetup()
		{
			shard = Service.Get<ICPSwrveService>().ResourceManager.GetResourceAttribute(swrveResourceUniqueID, "Shard", -1);
			if (shard != -1)
			{
				Service.Get<ICPSwrveService>().TestImpression(swrveResourceUniqueID, shard.ToString(), true);
			}
			if (shard == -1)
			{
				shard = 0;
			}
			if (shardProgressBarPanels != null && shardProgressBarPanels.Length > 0 && shardProgressBarPanels[shard] != null)
			{
				shardProgressBarPanels[shard].SetActive(true);
				for (int i = 0; i < shardProgressBarPanels.Length; i++)
				{
					if (!shardProgressBarPanels[i].Equals(shardProgressBarPanels[shard]))
					{
						shardProgressBarPanels[i].SetActive(false);
					}
				}
			}
			if (shardInitialActiveFormContainers != null && shardInitialActiveFormContainers.Length > 0 && shardInitialActiveFormContainers[shard] != null)
			{
				shardInitialActiveFormContainers[shard].SetActive(true);
				Animator component = shardInitialActiveFormContainers[shard].GetComponent<Animator>();
				if (component != null && component.name == "UsernamePasswordContainer")
				{
					component.SetTrigger("Step01");
				}
			}
			if (shardInitialProgressText != null && shardInitialProgressText.Length > 0 && shardInitialProgressText[shard].TextField != null)
			{
				Localizer localizer = Service.Get<Localizer>();
				shardInitialProgressText[shard].TextField.text = localizer.GetTokenTranslation(shardInitialProgressText[shard].Token);
			}
			if (shardTransitionStateHandlers != null && shardTransitionStateHandlers.Length > 0 && shardTransitionStateHandlers[shard] != null)
			{
				CreateWizardTransitionStateHandler[] components = shardTransitionStateHandlers[shard].GetComponents<CreateWizardTransitionStateHandler>();
				foreach (CreateWizardTransitionStateHandler createWizardTransitionStateHandler in components)
				{
					CreateWizardTransitionStateHandler createWizardTransitionStateHandler2 = base.gameObject.AddComponent<CreateWizardTransitionStateHandler>();
					Type type = createWizardTransitionStateHandler.GetType();
					FieldInfo[] fields = type.GetFields();
					FieldInfo[] array = fields;
					foreach (FieldInfo fieldInfo in array)
					{
						fieldInfo.SetValue(createWizardTransitionStateHandler2, fieldInfo.GetValue(createWizardTransitionStateHandler));
					}
					createWizardTransitionStateHandler2.enabled = true;
				}
				for (int i = 0; i < shardTransitionStateHandlers.Length; i++)
				{
					shardTransitionStateHandlers[i].SetActive(false);
				}
			}
			if (shardAvatarRenderers != null && shardAvatarRenderers.Length > 0 && shardAvatarRenderers[shard] != null)
			{
				shardAvatarRenderers[shard].RenderAvatar(new DCustomEquipment[0]);
				for (int i = 0; i < shardAvatarRenderers.Length; i++)
				{
					if (!shardAvatarRenderers[i].Equals(shardAvatarRenderers[shard]))
					{
						shardAvatarRenderers[i].enabled = false;
					}
				}
			}
			if (shardProgressBarAnimators == null || shardProgressBarAnimators.Length <= 0 || !(shardProgressBarAnimators[shard] != null))
			{
				return;
			}
			for (int i = 0; i < shardProgressBarAnimators.Length; i++)
			{
				if (!shardProgressBarAnimators[i].Equals(shardProgressBarAnimators[shard]))
				{
					shardProgressBarAnimators[i].enabled = false;
				}
			}
		}
	}
}
