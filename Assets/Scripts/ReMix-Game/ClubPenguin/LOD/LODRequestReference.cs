using Disney.Kelowna.Common.DataModel;
using System;

namespace ClubPenguin.LOD
{
	[Serializable]
	public class LODRequestReference : ScopedData
	{
		public LODRequest Request;

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(LODRequestReferenceMonoBehaviour);
			}
		}

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Scene.ToString();
			}
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
