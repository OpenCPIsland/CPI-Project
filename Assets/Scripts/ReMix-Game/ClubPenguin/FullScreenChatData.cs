using Disney.Kelowna.Common.DataModel;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	public class FullScreenChatData : ScopedData
	{
		[SerializeField]
		private int messageCount;

		public int MessageCount
		{
			get
			{
				return messageCount;
			}
			set
			{
				messageCount = value;
			}
		}

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.LocalPlayerZone.ToString();
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(FullScreenChatDataMonoBehaviour);
			}
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
