using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using System;
using UnityEngine;

namespace ClubPenguin.Igloo
{
	[Serializable]
	public class SceneStateData : ScopedData
	{
		public enum SceneState
		{
			Play,
			Edit,
			Preview,
			StructurePlacement,
			Create
		}

		[SerializeField]
		private SceneState state;

		public SceneState State
		{
			get
			{
				return state;
			}
			set
			{
				this.OnStateChanged.InvokeSafe(value);
				state = value;
			}
		}

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
				return typeof(SceneStateDataMonoBehaviour);
			}
		}

		public event Action<SceneState> OnStateChanged;

		protected override void notifyWillBeDestroyed()
		{
			this.OnStateChanged = null;
		}
	}
}
