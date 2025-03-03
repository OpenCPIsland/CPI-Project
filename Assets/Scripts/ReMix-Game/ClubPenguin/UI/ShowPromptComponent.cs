using ClubPenguin.Analytics;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.Events;

namespace ClubPenguin.UI
{
	public class ShowPromptComponent : MonoBehaviour
	{
		[Header("Use Prompt Definition")]
		public PromptDefinitionKey UsePromptDefinitionKey;

		[Header("Or Prompt Details")]
		public PrefabContentKey PromptPrefabContentKey;

		public string TitleToken;

		public string BodyToken;

		public bool OkButton = true;

		public bool CancelButton;

		public bool YesButton;

		public bool NoButton;

		public bool CloseButton;

		public Sprite Image = null;

		public bool IsModal = true;

		public bool AutoClose = true;

		public bool IsTranslated = false;

		[Header("BI Logging")]
		public string SwrveLogTier1;

		public string SwrveLogTier2;

		[Header("Button Actions")]
		public UnityEvent OnOkPressed;

		public UnityEvent OnCancelPressed;

		public UnityEvent OnYesPressed;

		public UnityEvent OnNoPressed;

		public UnityEvent OnClosePressed;

		private bool isLoadingPrefab = false;

		public void ShowPrompt()
		{
			if (UsePromptDefinitionKey != null && !string.IsNullOrEmpty(UsePromptDefinitionKey.Id))
			{
				Service.Get<PromptManager>().ShowPrompt(UsePromptDefinitionKey.Id, onButtonPressed);
			}
			else if (!isLoadingPrefab)
			{
				isLoadingPrefab = true;
				DPrompt.ButtonFlags buttonFlags = DPrompt.ButtonFlags.None;
				if (OkButton)
				{
					buttonFlags |= DPrompt.ButtonFlags.OK;
				}
				if (CancelButton)
				{
					buttonFlags |= DPrompt.ButtonFlags.CANCEL;
				}
				if (YesButton)
				{
					buttonFlags |= DPrompt.ButtonFlags.YES;
				}
				if (NoButton)
				{
					buttonFlags |= DPrompt.ButtonFlags.NO;
				}
				if (CloseButton)
				{
					buttonFlags |= DPrompt.ButtonFlags.CLOSE;
				}
				if (!string.IsNullOrEmpty(SwrveLogTier1))
				{
					Service.Get<ICPSwrveService>().Action(SwrveLogTier1, SwrveLogTier2);
				}
				DPrompt data = new DPrompt(TitleToken, BodyToken, buttonFlags, Image, IsModal, AutoClose, IsTranslated);
				if (PromptPrefabContentKey != null)
				{
					Content.LoadAsync(delegate(string path, GameObject prefab)
					{
						onPromptLoaded(data, prefab);
					}, PromptPrefabContentKey);
					return;
				}
				isLoadingPrefab = false;
				Service.Get<PromptManager>().ShowPrompt(data, onButtonPressed);
			}
		}

		private void onPromptLoaded(DPrompt data, GameObject promptPrefab)
		{
			isLoadingPrefab = false;
			PromptController component = promptPrefab.GetComponent<PromptController>();
			Service.Get<PromptManager>().ShowPrompt(data, onButtonPressed, component);
		}

		private void onButtonPressed(DPrompt.ButtonFlags pressed)
		{
			switch (pressed)
			{
			case DPrompt.ButtonFlags.OK:
				OnOkPressed.Invoke();
				break;
			case DPrompt.ButtonFlags.CANCEL:
				OnCancelPressed.Invoke();
				break;
			case DPrompt.ButtonFlags.YES:
				OnYesPressed.Invoke();
				break;
			case DPrompt.ButtonFlags.NO:
				OnNoPressed.Invoke();
				break;
			case DPrompt.ButtonFlags.CLOSE:
				OnClosePressed.Invoke();
				break;
			}
		}
	}
}
