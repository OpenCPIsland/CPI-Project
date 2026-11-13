using Disney.Kelowna.Common.DataModel;
using System;

namespace ClubPenguin.Newsfeed
{
	[Serializable]
	public class NewPostData : BaseData
	{
		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(NewPostDataMonoBehaviour);
			}
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
