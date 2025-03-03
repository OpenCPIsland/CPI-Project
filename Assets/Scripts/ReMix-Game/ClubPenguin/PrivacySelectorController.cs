using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Igloo;
using ClubPenguin.Net.Domain.Igloo;
using ClubPenguin.UI;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class PrivacySelectorController : MonoBehaviour
	{
		private CPDataEntityCollection dataEntityCollection;

		private SavedIgloosMetaData savedIgloosMetaData;

		public Button OnlyMeButton;

		private GameObject OnlyMeButtonSelected;

		public Button PublicButton;

		private GameObject PublicButtonSelected;

		private Animator uiAnimator;

		private void Awake()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			if (!dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out savedIgloosMetaData))
			{
				dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<SavedIgloosMetaData>>(onSavedIgloosMetaDataAdded);
			}
			OnlyMeButtonSelected = getSelectedStateGameObject(OnlyMeButton);
			PublicButtonSelected = getSelectedStateGameObject(PublicButton);
			uiAnimator = GetComponent<Animator>();
			updateButtonSelection();
		}

		private GameObject getSelectedStateGameObject(Button button)
		{
			if (button != null)
			{
				for (int i = 0; i < button.transform.childCount; i++)
				{
					GameObject gameObject = button.transform.GetChild(i).gameObject;
					if (gameObject.name.Equals("SelectedState"))
					{
						return gameObject;
					}
				}
			}
			return null;
		}

		public void Animation_Event_Outro_End()
		{
			Object.Destroy(base.gameObject);
		}

		private void OnDestroy()
		{
			if (savedIgloosMetaData == null)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<SavedIgloosMetaData>>(onSavedIgloosMetaDataAdded);
			}
		}

		private bool onSavedIgloosMetaDataAdded(DataEntityEvents.ComponentAddedEvent<SavedIgloosMetaData> evt)
		{
			if (evt.Handle == dataEntityCollection.LocalPlayerHandle)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<SavedIgloosMetaData>>(onSavedIgloosMetaDataAdded);
				savedIgloosMetaData = evt.Component;
				updateButtonSelection();
			}
			return false;
		}

		public void OnPublicButtonClicked()
		{
			setPublishStatus(IglooVisibility.PUBLIC);
			updateButtonSelection();
			Service.Get<ICPSwrveService>().Action("igloo", "visibility_settings", "public");
			uiAnimator.SetBool("Outro", true);
		}

		public void OnPrivateButtonClicked()
		{
			if (savedIgloosMetaData.IglooVisibility != 0)
			{
				ShowConfirmChangePrivacyPrompt();
			}
		}

		private void ShowConfirmChangePrivacyPrompt()
		{
			PromptDefinition promptDefinition = Service.Get<PromptManager>().GetPromptDefinition("IglooConfirmChangePrivacyPrompt");
			PromptLoaderCMD promptLoaderCMD = new PromptLoaderCMD(this, promptDefinition, onIglooConfirmChangePrivacyPromptLoaded);
			promptLoaderCMD.Execute();
		}

		private void onIglooConfirmChangePrivacyPromptLoaded(PromptLoaderCMD promptLoader)
		{
			Service.Get<PromptManager>().ShowPrompt(promptLoader.PromptData, onIglooConfirmChangePrivacyPromptButtonClicked, promptLoader.Prefab);
		}

		private void onIglooConfirmChangePrivacyPromptButtonClicked(DPrompt.ButtonFlags flags)
		{
			if (flags == DPrompt.ButtonFlags.YES)
			{
				confirmChangeToPrivate();
			}
			else
			{
				uiAnimator.SetBool("Outro", true);
			}
		}

		private void confirmChangeToPrivate()
		{
			setPublishStatus(IglooVisibility.PRIVATE);
			updateButtonSelection();
			Service.Get<ICPSwrveService>().Action("igloo", "visibility_settings", "private");
			uiAnimator.SetBool("Outro", true);
		}

		private void updateButtonSelection()
		{
			if (savedIgloosMetaData != null && !(OnlyMeButtonSelected == null) && !(PublicButtonSelected == null))
			{
				IglooVisibility iglooVisibility = savedIgloosMetaData.IglooVisibility;
				OnlyMeButtonSelected.SetActive(iglooVisibility == IglooVisibility.PRIVATE);
				PublicButtonSelected.SetActive(iglooVisibility == IglooVisibility.PUBLIC);
			}
		}

		private void setPublishStatus(IglooVisibility status)
		{
			savedIgloosMetaData.IglooVisibility = status;
			savedIgloosMetaData.IsDirty = true;
		}
	}
}
