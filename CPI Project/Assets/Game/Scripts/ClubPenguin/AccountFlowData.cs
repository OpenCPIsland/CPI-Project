using Disney.Kelowna.Common.DataModel;
using System;
using System.Collections.Generic;

namespace ClubPenguin
{
	[Serializable]
	public class AccountFlowData : ScopedData
	{
		public bool LoginViaMembership;

		public bool LoginViaRestore;

		public bool SkipMembership;

		public bool SkipWelcome;

		public List<string> PreValidatedDisplayNames;

		public List<string> PreValidatedUserNames;

		public AccountFlowType FlowType;

		public string Referrer;

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Session.ToString();
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(AccountFlowDataMonoBehaviour);
			}
		}

		public void Initialize()
		{
			LoginViaMembership = false;
			LoginViaRestore = false;
			SkipMembership = false;
			SkipWelcome = false;
			PreValidatedDisplayNames = new List<string>();
			PreValidatedUserNames = new List<string>();
			FlowType = AccountFlowType.none;
			Referrer = "";
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
