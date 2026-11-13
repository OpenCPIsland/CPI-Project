using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	internal class IglooCreateButton : MonoBehaviour
	{
		public GameObject LockedOverlay;

		public GameObject MemberLockedOverlay;

		public GameObject ProgressionLockedOverlay;

		public Transform GetSlotContainer
		{
			get
			{
				return base.transform.parent;
			}
		}

		public void SetState(IglooPropertiesCard.IglooCardState state)
		{
			switch (state)
			{
			case IglooPropertiesCard.IglooCardState.MemberLocked:
				LockedOverlay.SetActive(true);
				MemberLockedOverlay.SetActive(true);
				break;
			case IglooPropertiesCard.IglooCardState.ProgressionLocked:
				LockedOverlay.SetActive(true);
				ProgressionLockedOverlay.SetActive(true);
				break;
			default:
				LockedOverlay.SetActive(false);
				MemberLockedOverlay.SetActive(false);
				ProgressionLockedOverlay.SetActive(false);
				break;
			}
		}

		public void SetLockedLevel(int level)
		{
			Text componentInChildren = ProgressionLockedOverlay.GetComponentInChildren<Text>();
			if (componentInChildren != null)
			{
				componentInChildren.text = level.ToString();
			}
		}
	}
}
