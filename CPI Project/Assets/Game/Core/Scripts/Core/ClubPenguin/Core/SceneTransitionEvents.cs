using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin.Core
{
	public class SceneTransitionEvents
	{
		public struct TransitionStart
		{
			public readonly string SceneName;

			public TransitionStart(string sceneName)
			{
				SceneName = sceneName;
			}
		}

		public struct TransitionComplete
		{
			public readonly string SceneName;

			public TransitionComplete(string sceneName)
			{
				SceneName = sceneName;
			}
		}

		public struct SetIsTransitioningFlag
		{
			public bool IsTransitioning;

			public SetIsTransitioningFlag(bool isTransitioning)
			{
				IsTransitioning = isTransitioning;
			}
		}

		public struct LayoutGameObjectsLoaded
		{
			public readonly Transform Container;

			public readonly SceneLayoutData Layout;

			public LayoutGameObjectsLoaded(Transform container, SceneLayoutData layout)
			{
				Container = container;
				Layout = layout;
			}
		}

		public struct MusicTrackPrefabLoaded
		{
			public readonly GameObject MusicTrackPrefab;

			public MusicTrackPrefabLoaded(GameObject musicTrackPrefab)
			{
				MusicTrackPrefab = musicTrackPrefab;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SceneSwapLoadStarted
		{
		}
	}
}
