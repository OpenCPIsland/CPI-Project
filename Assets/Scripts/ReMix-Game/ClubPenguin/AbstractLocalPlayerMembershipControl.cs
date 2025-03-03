using ClubPenguin.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public abstract class AbstractLocalPlayerMembershipControl : MonoBehaviour
	{
		private DataEventListener dataEventListener;

		private MembershipData membershipData;

		private bool isMember = false;

		protected abstract void membershipSet(bool isMember);

		protected void Start()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			dataEventListener = cPDataEntityCollection.When<MembershipData>(cPDataEntityCollection.LocalPlayerHandle, onMembershipDataAdded);
		}

		protected void OnDestroy()
		{
			dataEventListener.StopListening();
			if (membershipData != null)
			{
				membershipData.MembershipDataUpdated -= onMembershipDataChanged;
			}
		}

		private void onMembershipDataAdded(MembershipData membershipData)
		{
			this.membershipData = membershipData;
			onMembershipDataChanged(membershipData);
			membershipData.MembershipDataUpdated += onMembershipDataChanged;
		}

		private void onMembershipDataChanged(MembershipData obj)
		{
			if (isMember != obj.IsMember)
			{
				isMember = obj.IsMember;
				membershipSet(isMember);
			}
		}
	}
}
