using Disney.Kelowna.Common.DataModel;
using System;

namespace ClubPenguin
{
	[Serializable]
	public class LegacyProfileData : BaseData
	{
		public string Username
		{
			get;
			set;
		}

		public bool IsMember
		{
			get;
			set;
		}

		public long CreatedDate
		{
			get;
			set;
		}

		public long MigratedDate
		{
			get;
			set;
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(LegacyProfileDataDataMonoBehaviour);
			}
		}

		public override string ToString()
		{
			return string.Format("legacyProfileData: \n \t username: {0}, \n \t isMember: {1}, \n \t createdDate: {2},  \n \t migratedDate: {3}", Username.ToString(), IsMember.ToString(), CreatedDate.ToString(), MigratedDate.ToString());
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
