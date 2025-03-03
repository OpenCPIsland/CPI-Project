using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.Choreography
{
	public class Actor : ScriptableActionPlayer
	{
		public class InteractionState
		{
			public readonly InteractionDefinition Definition;

			public readonly Actor[] Actors;

			private int[] syncPointsWaiting;

			internal bool isSyncPointReady(int syncPointIndex)
			{
				return syncPointsWaiting[syncPointIndex] == Definition.SyncPoints[syncPointIndex].WaitMask;
			}

			internal void reachedSyncPoint(int syncPointIndex, int trackIndex)
			{
				syncPointsWaiting[syncPointIndex] |= 1 << trackIndex;
				if (!isSyncPointReady(syncPointIndex))
				{
					return;
				}
				InteractionDefinition.SyncPoint syncPoint = Definition.SyncPoints[syncPointIndex];
				bool flag = true;
				if (syncPoint.Logic != null)
				{
					flag = syncPoint.Logic.Execute(this);
				}
				if (flag)
				{
					for (int i = 0; i < Actors.Length; i++)
					{
						Actor actor = Actors[i];
						int num = 1 << i;
						if ((syncPointsWaiting[syncPointIndex] & num) == num)
						{
							actor.ActionIsFinished = true;
						}
					}
				}
				else
				{
					Stop();
				}
			}

			public void Stop()
			{
				for (int i = 0; i < Actors.Length; i++)
				{
					Actors[i].stop();
				}
			}

			public InteractionState(InteractionDefinition definition, GameObject[] actorsGO)
			{
				int num = definition.Tracks.Length;
				Definition = definition;
				Actors = new Actor[num];
				syncPointsWaiting = new int[Definition.SyncPoints.Length];
				for (int i = 0; i < num; i++)
				{
					InteractionDefinition.Track track = definition.Tracks[i];
					Actor actor = actorsGO[i].AddComponent<Actor>();
					Actors[i] = actor;
					actor.begin(this, track, i);
				}
			}
		}

		private InteractionState interaction;

		private int currentStepIndex;

		public InteractionDefinition.Track Track
		{
			get;
			private set;
		}

		public int TrackIndex
		{
			get;
			private set;
		}

		protected override void onActionDone()
		{
			if (Track.Steps[currentStepIndex].Action == Action)
			{
				currentStepIndex++;
			}
			base.NextAction = getNextAction();
		}

		private void begin(InteractionState interactionState, InteractionDefinition.Track track, int trackIndex)
		{
			interaction = interactionState;
			Track = track;
			TrackIndex = trackIndex;
			Action = getNextAction();
			base.enabled = true;
		}

		private void stop()
		{
			base.AbortAction = true;
			base.ActionIsFinished = true;
			interaction = null;
		}

		private ScriptableAction getNextAction()
		{
			ScriptableAction result = null;
			if (interaction != null && currentStepIndex < Track.Steps.Length)
			{
				InteractionDefinition.Track.Step step = Track.Steps[currentStepIndex];
				if (step.SyncPointIndex >= 0)
				{
					interaction.reachedSyncPoint(step.SyncPointIndex, TrackIndex);
					result = ((!interaction.isSyncPointReady(step.SyncPointIndex)) ? ScriptableActionPause.Instance : step.Action);
				}
				else
				{
					result = step.Action;
				}
			}
			return result;
		}
	}
}
