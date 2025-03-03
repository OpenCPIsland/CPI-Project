using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using System;
using UnityEngine;

namespace ClubPenguin.Choreography
{
	public class InteractionDefinition : ScriptableObject
	{
		[Serializable]
		public struct Track
		{
			[Serializable]
			public struct Step
			{
				public int SyncPointIndex;

				public ScriptableAction Action;
			}

			public string Name;

			public Step[] Steps;
		}

		[Serializable]
		public struct SyncPoint
		{
			public string Name;

			public int WaitMask;

			public SharedLogic Logic;
		}

		public Track[] Tracks;

		public SyncPoint[] SyncPoints;

		public void Begin(params GameObject[] actors)
		{
			if (Tracks.Length == actors.Length)
			{
				bool flag = false;
				for (int i = 0; i < actors.Length; i++)
				{
					if (actors[i].GetComponent<ScriptableActionPlayer>() != null)
					{
						Log.LogError(this, string.Concat("Actor ", actors[i], " is already busy with actions, so interaction cannot start"));
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					new Actor.InteractionState(this, actors);
				}
			}
			else
			{
				Log.LogError(this, "Interaction cannot start without the correct number of actors. Tracks = " + Tracks.Length + ", Actors = " + actors.Length);
			}
		}
	}
}
