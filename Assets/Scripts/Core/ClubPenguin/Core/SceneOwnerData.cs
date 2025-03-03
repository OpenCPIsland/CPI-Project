using Disney.Kelowna.Common.DataModel;
using System;

namespace ClubPenguin.Core
{
	[Serializable]
	public class SceneOwnerData : ScopedData
	{
		public string Name;

		public bool IsOwner;

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Zone.ToString();
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(SceneOwnerDataMonoBehaviour);
			}
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
