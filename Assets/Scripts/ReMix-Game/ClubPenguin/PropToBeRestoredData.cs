using Disney.Kelowna.Common.DataModel;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	public class PropToBeRestoredData : ScopedData
	{
		[SerializeField]
		private string propId;

		public string PropId
		{
			get
			{
				return propId;
			}
			set
			{
				propId = value;
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(PropToBeRestoredDataMonoBehaviour);
			}
		}

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Session.ToString();
			}
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
