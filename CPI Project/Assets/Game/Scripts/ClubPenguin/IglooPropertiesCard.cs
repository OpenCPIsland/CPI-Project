using ClubPenguin.Analytics;
using ClubPenguin.Igloo;
using ClubPenguin.SceneManipulation;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Globalization;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	internal class IglooPropertiesCard : SceneLayoutIdComponent
	{
		public enum IglooCardState
		{
			Active,
			InActive,
			MemberLocked,
			ProgressionLocked
		}

		[Header("Common ")]
		public TintSelector[] BackgroundTintSelectors;

		public Image IglooPreviewImage;

		public Text CreatedDate;

		public Text LastEditedDate;

		[Tooltip("Max number of characters before wrapping text to another line")]
		public int LastEditedDateMaxChars = 25;

		[Header("Inactive")]
		public GameObject DeleteIglooButton;

		[Header("Active Toggle")]
		public GameObject ActiveStatusOpen;

		public GameObject ActiveStatusClosed;

		public GameObject ActiveStateOutline;

		[Header("Lock Overlays")]
		public GameObject LockedOverlay;

		public GameObject MemberLockedOverlay;

		public GameObject ProgressionLockedOverlay;

		private SavedIglooMetaData savedIglooMetaData;

		private ManageIglooPopupController manageIgloos;

		public void Init(ManageIglooPopupController manageIglooPopupController, LotDefinition lotDefinition, SavedIglooMetaData iglooMetaData, IglooCardState state)
		{
			manageIgloos = manageIglooPopupController;
			savedIglooMetaData = iglooMetaData;
			layoutId = iglooMetaData.LayoutId;
			CreatedDate.text = formatCreatedDate(iglooMetaData.CreatedDate);
			LastEditedDate.text = formatLastEditedDate(iglooMetaData.LastModifiedDate);
			Content.LoadAsync(onImageLoadComplete, lotDefinition.PreviewImageLarge);
			SetCardState(state);
		}

		private string formatCreatedDate(long unixDate)
		{
			DateTime dt = DateTimeUtils.DateTimeFromUnixTime(unixDate);
			return dt.GetLocalizedDate();
		}

		private string formatLastEditedDate(long unixDate)
		{
			DateTime dt = DateTimeUtils.DateTimeFromUnixTime(unixDate);
			string text = Service.Get<Localizer>().GetTokenTranslation("GlobalUI.Prompts.DateAtTime");
			string text2 = "";
			try
			{
				text2 = string.Format(text, dt.GetLocalizedMonth(), dt.Year, dt.ToShortTimeString());
				if (text2.Length > LastEditedDateMaxChars)
				{
					text = text.Replace("{0} {1}", "{0} {1}\n");
					text2 = string.Format(text, dt.GetLocalizedMonth(), dt.Year, dt.ToShortTimeString());
				}
			}
			catch (Exception)
			{
				Log.LogErrorFormatted(this, "An error occurred trying to format the date token {0}.", text);
			}
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			return text2;
		}

		private void onImageLoadComplete(string path, Texture2D texture)
		{
			IglooPreviewImage.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero);
		}

		public void SetCardState(IglooCardState state)
		{
			switch (state)
			{
			case IglooCardState.MemberLocked:
				LockedOverlay.SetActive(true);
				MemberLockedOverlay.SetActive(true);
				return;
			case IglooCardState.ProgressionLocked:
				LockedOverlay.SetActive(true);
				ProgressionLockedOverlay.SetActive(true);
				return;
			}
			LockedOverlay.SetActive(false);
			MemberLockedOverlay.SetActive(false);
			ProgressionLockedOverlay.SetActive(false);
			ActiveStatusOpen.SetActive(state == IglooCardState.Active);
			ActiveStatusClosed.SetActive(state == IglooCardState.InActive);
			ActiveStateOutline.SetActive(state == IglooCardState.Active);
			DeleteIglooButton.SetActive(state == IglooCardState.InActive);
			if (state < IglooCardState.MemberLocked)
			{
				for (int i = 0; i < BackgroundTintSelectors.Length; i++)
				{
					BackgroundTintSelectors[i].SelectColor((int)state);
				}
			}
		}

		public void OnActiveToggleButton()
		{
			manageIgloos.SetActiveIgloo(savedIglooMetaData.LayoutId);
			Service.Get<ICPSwrveService>().Action("igloo", "active");
		}

		public void OnDestroyIgloo()
		{
			showIglooConfirmDeletePrompt();
		}

		private void showIglooConfirmDeletePrompt()
		{
			PromptDefinition promptDefinition = Service.Get<PromptManager>().GetPromptDefinition("IglooConfirmDeletePrompt");
			PromptLoaderCMD promptLoaderCMD = new PromptLoaderCMD(this, promptDefinition, onIglooConfirmDeletePromptLoaded);
			promptLoaderCMD.Execute();
		}

		private void onIglooConfirmDeletePromptLoaded(PromptLoaderCMD promptLoader)
		{
			Service.Get<PromptManager>().ShowPrompt(promptLoader.PromptData, onIglooConfirmDeletePromptButtonClicked, promptLoader.Prefab);
		}

		private void onIglooConfirmDeletePromptButtonClicked(DPrompt.ButtonFlags flags)
		{
			if (flags == DPrompt.ButtonFlags.YES)
			{
				manageIgloos.DeleteIglooLayout(savedIglooMetaData.LayoutId);
			}
		}
	}
}
