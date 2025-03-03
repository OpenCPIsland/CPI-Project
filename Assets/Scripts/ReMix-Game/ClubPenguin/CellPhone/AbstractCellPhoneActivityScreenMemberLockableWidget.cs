using ClubPenguin.Core;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.CellPhone
{
	public abstract class AbstractCellPhoneActivityScreenMemberLockableWidget : MonoBehaviour, ICellPhoneAcitivtyScreenWidget
	{
		public GameObject MemberLockedPanel;

		private CellPhoneActivityDefinition widgetData;

		private bool isMemberLocked;

		public void SetWidgetData(CellPhoneActivityDefinition widgetData)
		{
			this.widgetData = widgetData;
			isMemberLocked = (widgetData.IsMemberOnly && !getIsLocalPlayerMember());
			showMemberStatus();
			setWidgetData(widgetData);
		}

		public void OnGoButtonClicked()
		{
			if (!isMemberLocked)
			{
				onGoButtonClicked();
			}
			else
			{
				showMemberShipFlow();
			}
		}

		protected abstract void setWidgetData(CellPhoneActivityDefinition setWidgetData);

		protected abstract void onGoButtonClicked();

		private bool getIsLocalPlayerMember()
		{
			return Service.Get<CPDataEntityCollection>().IsLocalPlayerMember();
		}

		private void showMemberStatus()
		{
			if (MemberLockedPanel != null)
			{
				MemberLockedPanel.SetActive(isMemberLocked);
			}
		}

		protected void showMemberShipFlow()
		{
			Service.Get<GameStateController>().ShowAccountSystemMembership(widgetData.name);
		}
	}
}
