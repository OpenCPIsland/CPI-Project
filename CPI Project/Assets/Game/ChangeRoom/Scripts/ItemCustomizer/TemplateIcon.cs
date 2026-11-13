using ClubPenguin.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class TemplateIcon : CustomizationButton
	{
		[SerializeField]
		private GameObject memberBadge;

		[SerializeField]
		private GameObject progressionBadge;

		[SerializeField]
		private GameObject mascotBadges;

		[SerializeField]
		private MaterialSelector materialSelector;

		private bool isEnabled = true;
		private bool isMemberLocked = false;

		private Button button;

		public TemplateDefinition TemplateData { get; private set; }
		public bool CanSelect { get; private set; }

		public Image itemIconn;

		public bool IsEnabled
		{
			get { return isEnabled; }
			set { isEnabled = value; }
		}

		protected override void Awake()
		{
			base.Awake();
			memberBadge.SetActive(false);
			progressionBadge.SetActive(false);
			mascotBadges.SetActive(false);
			button = GetComponent<Button>();
		}

		private void OnEnable()
		{
			if (button != null)
			{
				button.onClick.AddListener(onButtonClicked);
			}
		}

		private void OnDisable()
		{
			if (button != null)
			{
				button.onClick.RemoveListener(onButtonClicked);
			}
		}

		public void SetTemplateToUnlocked(TemplateDefinition templateData)
		{
			TemplateData = templateData;
			materialSelector.SelectMaterial(0);
			CanSelect = true;
			progressionBadge.SetActive(false);
			memberBadge.SetActive(false);
			mascotBadges.SetActive(false);
			isMemberLocked = false;
		}

		public void SetTemplateMemberLocked(TemplateDefinition templateData)
		{
			TemplateData = templateData;
			materialSelector.SelectMaterial(1);
			CanSelect = false;
			progressionBadge.SetActive(false);
			mascotBadges.SetActive(false);
			memberBadge.SetActive(true);
			isMemberLocked = true;
		}

		public void SetTemplateToLevelLocked(TemplateDefinition templateData, int level)
		{
			TemplateData = templateData;
			materialSelector.SelectMaterial(1);
			CanSelect = false;
			memberBadge.SetActive(false);
			mascotBadges.SetActive(false);
			progressionBadge.SetActive(true);
			progressionBadge.GetComponentInChildren<Text>().text = level.ToString();
			isMemberLocked = false;
		}

		public void SetTemplateToProgressionLocked(TemplateDefinition templateData, string mascotName)
		{
			TemplateData = templateData;
			materialSelector.SelectMaterial(1);
			CanSelect = false;
			progressionBadge.SetActive(false);
			memberBadge.SetActive(false);
			mascotBadges.SetActive(true);
			IList<Transform> children = mascotBadges.GetChildren();
			for (int i = 0; i < children.Count; i++)
			{
				children[i].gameObject.SetActive(children[i].name.Equals(mascotName));
			}
			isMemberLocked = false;
		}

		private void onButtonClicked()
		{
			TriggerClickAction();
		}

		private void TriggerClickAction()
		{

			if (isMemberLocked)
			{
				ClothingDesignerContext.EventBus.DispatchEvent(
					new ClothingDesignerUIEvents.ShowMemberFlow("blueprints")
				);
			}
			else
			{

				if(isMemberLocked)
				{
                    Debug.Log("TemplateIcon clicked. Member locked: " + isMemberLocked);
				}else if (mascotBadges.activeSelf)
				{
                    Debug.Log("TemplateIcon clicked. MascotBadges locked: " + isMemberLocked);
				}else if (progressionBadge.activeSelf)
				{
                    Debug.Log("TemplateIcon clicked. ProgressionBadge locked: " + isMemberLocked);
				}
				else
				{
                    // Dispatch the event so ClothingTemplateSelectionController can handle the template change
                    CustomizationContext.EventBus.DispatchEvent(
                        new CustomizerUIEvents.SelectTemplate(TemplateData, itemIconn.sprite) // Pass sprite if available
                    );
                }
			}
		}
	}
}