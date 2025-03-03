using Disney.Kelowna.Common.DataModel;
using System;
using System.Collections;

namespace ClubPenguin.Tutorial
{
	[Serializable]
	public class TutorialData : ScopedData
	{
		private BitArray data = new BitArray(0);

		public BitArray Data
		{
			get
			{
				return data;
			}
			set
			{
				data = value;
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(TutorialDataMonoBehaviour);
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
