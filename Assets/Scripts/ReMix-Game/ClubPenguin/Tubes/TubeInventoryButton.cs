using ClubPenguin.Analytics;
using ClubPenguin.Breadcrumbs;
using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Tubes
{
	public class TubeInventoryButton : MonoBehaviour
	{
		public Image Icon;

		public Text Title;

		public Button SelectButton;

		public NotificationBreadcrumb breadcrumb;

		public TutorialBreadcrumb TutorialBreadcrumb;

		public PersistentBreadcrumbTypeDefinitionKey BreadcrumbType;

		public GameObject SelectedPanel;

		private TubeDefinition definition;

		private TubeData tubeData;

		public void OnEnable()
		{
			SelectButton.onClick.AddListener(onClicked);
		}

		public void SetTubeDefinition(TubeDefinition def)
		{
			definition = def;
			setDisplayText();
			setupBreadcrumbs();
			loadIcon();
			getTubeData();
			onSelectedTubeChanged(tubeData.SelectedTubeId);
		}

		private void onClicked()
		{
			removeBreadcrumbs();
			selectTube();
			equipTube();
			closeTray();
			logTubeSelectedBI();
		}

		private void getTubeData()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			TubeData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				tubeData = component;
				component.OnTubeSelected += onSelectedTubeChanged;
			}
		}

		private void selectTube()
		{
			Service.Get<INetworkServicesManager>().PlayerStateService.SelectTube(definition.Id);
			tubeData.SelectedTubeId = definition.Id;
		}

		private void equipTube()
		{
			if (!LocomotionHelper.IsCurrentControllerOfType<SlideController>(SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject))
			{
				Service.Get<EventDispatcher>().DispatchEvent(new InputEvents.SwitchChangeEvent(InputEvents.Switches.Tube, true));
			}
		}

		private void onSelectedTubeChanged(int tubeId)
		{
			SelectedPanel.SetActive(definition.Id == tubeId);
		}

		private void setDisplayText()
		{
			Title.text = Service.Get<Localizer>().GetTokenTranslation(definition.DisplayNameToken);
		}

		private void setupBreadcrumbs()
		{
			breadcrumb.SetBreadcrumbId(BreadcrumbType, definition.Id.ToString());
			TutorialBreadcrumb.SetBreadcrumbId(string.Format("Tube_{0}", definition.Id));
		}

		private void removeBreadcrumbs()
		{
			Service.Get<NotificationBreadcrumbController>().RemovePersistentBreadcrumb(BreadcrumbType, definition.Id.ToString());
			Service.Get<TutorialBreadcrumbController>().RemoveBreadcrumb(TutorialBreadcrumb.BreadcrumbId);
			Service.Get<NotificationBreadcrumbController>().ResetBreadcrumbs(breadcrumb.BreadcrumbId);
		}

		private void loadIcon()
		{
			Content.LoadAsync(onIconLoaded, definition.IconContentKey);
		}

		private void onIconLoaded(string path, Sprite sprite)
		{
			Icon.sprite = sprite;
			Icon.enabled = true;
		}

		private void closeTray()
		{
			StateMachineContext component = SceneRefs.UiTrayRoot.GetComponent<StateMachineContext>();
			component.SendEvent(new ExternalEvent("Root", "mainnav_locomotion"));
		}

		private void logTubeSelectedBI()
		{
			Service.Get<ICPSwrveService>().Action("tube_activate", definition.name);
		}

		private void OnDisable()
		{
			removeListeners();
		}

		private void OnDestroy()
		{
			removeListeners();
		}

		private void removeListeners()
		{
			if (tubeData != null)
			{
				tubeData.OnTubeSelected -= onSelectedTubeChanged;
			}
			if (SelectButton != null)
			{
				SelectButton.onClick.RemoveListener(onClicked);
			}
		}

		public int getDefinitionId()
		{
			if ((bool)definition)
			{
				return definition.Id;
			}
			return -1;
		}
	}
}
