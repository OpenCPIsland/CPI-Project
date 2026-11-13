using ClubPenguin.Adventure;
using ClubPenguin.Breadcrumbs;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class QuestLogMascotItem : MonoBehaviour
	{
		public MascotDefinition MascotDefinition;

		public FeatureLabelBreadcrumb FeatureLabel;

		private Mascot mascot;

		private void Start()
		{
			if (MascotDefinition != null)
			{
				mascot = Service.Get<MascotService>().GetMascot(MascotDefinition.name);
			}
		}

		public void LoadMascotData(Mascot mascot)
		{
			this.mascot = mascot;
			if (FeatureLabel != null)
			{
				FeatureLabel.TypeId = mascot.Name;
				FeatureLabel.SetBreadcrumbVisibility();
			}
		}

		public void OnClick()
		{
			ShowQuestAdventures();
		}

		private void ShowQuestAdventures()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new QuestScreenEvents.ShowQuestLogAdventures(mascot.Definition.name));
		}
	}
}
