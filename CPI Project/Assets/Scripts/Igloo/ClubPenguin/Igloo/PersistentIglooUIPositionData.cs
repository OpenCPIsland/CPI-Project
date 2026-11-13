using Disney.Kelowna.Common.DataModel;
using System;
using UnityEngine;

namespace ClubPenguin.Igloo
{
	[Serializable]
	public class PersistentIglooUIPositionData : ScopedData
	{
		public string ScreenName;

		public Vector2 Position;

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Scene.ToString();
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(PersistentIglooUIPositionDataMonoBehaviour);
			}
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
