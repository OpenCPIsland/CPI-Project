using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	public class MembershipData : BaseData
	{
		[SerializeField]
		private bool isMember;

		[SerializeField]
		private MembershipType membershipType;

		private long membershipExpireDate;

		public bool IsMember
		{
			get
			{
				return isMember;
			}
			set
			{
				bool flag = isMember;
				isMember = value;
				if (value != flag)
				{
					dispatchUpdatedAction();
				}
			}
		}

		public MembershipType MembershipType
		{
			get
			{
				return membershipType;
			}
			set
			{
				MembershipType membershipType = this.membershipType;
				this.membershipType = value;
				if (value != membershipType)
				{
					dispatchUpdatedAction();
				}
			}
		}

		public long MembershipExpireDate
		{
			get
			{
				return membershipExpireDate;
			}
			set
			{
				membershipExpireDate = value;
			}
		}

		public bool MembershipTrialAvailable
		{
			get;
			set;
		}

		public bool HasHadMembership
		{
			get
			{
				return MembershipExpireDate > 0;
			}
		}

		public DateTime MembershipExpireDateTime
		{
			get
			{
				return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(MembershipExpireDate).ToLocalTime();
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(MembershipDataMonoBehaviour);
			}
		}

		public event Action<MembershipData> MembershipDataUpdated;

		private void dispatchUpdatedAction()
		{
			if (this.MembershipDataUpdated != null)
			{
				this.MembershipDataUpdated.InvokeSafe(this);
			}
		}

		protected override void notifyWillBeDestroyed()
		{
			this.MembershipDataUpdated = null;
		}

		public override string ToString()
		{
			return string.Format(arg1: new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(MembershipExpireDate).ToLocalTime().ToShortDateString(), format: "membershipData: \n \t IsMember {0}, \n \t MembershipExpireDate: {1}, {2}", arg0: IsMember, arg2: MembershipExpireDate);
		}
	}
}
