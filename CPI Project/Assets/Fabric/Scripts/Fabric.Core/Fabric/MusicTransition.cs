using System;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class MusicTransition
	{
		[Serializable]
		public class MusicTransitionHolder
		{
			[SerializeField]
			public Component _component;

			[SerializeField]
			public MusicSyncType _musicSyncType;
		}

		[SerializeField]
		[HideInInspector]
		public MusicTransitionHolder _fromComponent = new MusicTransitionHolder();

		[HideInInspector]
		[SerializeField]
		public MusicTransitionHolder _transition = new MusicTransitionHolder();

		[SerializeField]
		[HideInInspector]
		public MusicTransitionHolder _toComponent = new MusicTransitionHolder();

		public MusicTransitionHolder GetActiveTransitionComponentFromState(MusicTransitionState state)
		{
			switch (state)
			{
			case MusicTransitionState.FromComponent:
				return _fromComponent;
			case MusicTransitionState.ToComponent:
				return _toComponent;
			case MusicTransitionState.Transition:
				return _transition;
			default:
				return null;
			}
		}
	}
}
