using Disney.Kelowna.Common.DataModel;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	public class GameObjectReferenceData : ScopedData
	{
		[SerializeField]
		private GameObject gameObjectReference;

		public GameObject GameObject
		{
			get
			{
				return gameObjectReference;
			}
			set
			{
				gameObjectReference = value;
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(GameObjectReferenceDataMonoBehaviour);
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
			gameObjectReference = null;
		}
	}
}
