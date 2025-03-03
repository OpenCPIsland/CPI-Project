using ClubPenguin.Adventure;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class ToggleForMascot : MonoBehaviour
	{
		public MascotDefinitionKey MascotKey;

		private MascotDefinition mascotDefinition;

		private void Start()
		{
			mascotDefinition = Service.Get<MascotService>().GetMascot(MascotKey.Id).Definition;
			DRewardPopup popupData = GetComponentInParent<RewardPopupController>().PopupData;
			if (!string.IsNullOrEmpty(popupData.MascotName) && mascotDefinition != null)
			{
				toggleForMascot(popupData.MascotName);
			}
		}

		private void toggleForMascot(string mascotName)
		{
			Mascot mascot = Service.Get<MascotService>().GetMascot(mascotName);
			if (mascot != null)
			{
				if (mascotDefinition.name == mascot.Definition.name)
				{
					base.gameObject.SetActive(true);
				}
				else
				{
					base.gameObject.SetActive(false);
				}
			}
		}
	}
}
