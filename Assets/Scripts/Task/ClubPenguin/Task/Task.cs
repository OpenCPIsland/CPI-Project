using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin.Task
{
	public class Task : ITask
	{
		public readonly TaskDefinition Definition;

		public int Counter
		{
			get;
			private set;
		}

		public int Goal
		{
			get
			{
				if ((bool)Definition)
				{
					return Definition.Threshold;
				}
				return -1;
			}
		}

		public bool IsComplete
		{
			get
			{
				bool result = false;
				switch (Definition.Comparison)
				{
				case TaskDefinition.TaskComparison.Equals:
					result = (Counter == Definition.Threshold);
					break;
				case TaskDefinition.TaskComparison.LessThan:
					result = (Counter < Definition.Threshold);
					break;
				case TaskDefinition.TaskComparison.GreaterThan:
					result = (Counter > Definition.Threshold);
					break;
				}
				return result;
			}
		}

		public bool IsRewardClaimed
		{
			get;
			set;
		}

		public string Id
		{
			get
			{
				return Definition.name;
			}
		}

		public bool IsTaskAvailable(bool isMember, int playerProgressionLevel, int mascotQuestLevel)
		{
			return !IsComplete && !IsMemberLocked(isMember) && !IsProgressionLocked(playerProgressionLevel) && !IsQuestLocked(mascotQuestLevel);
		}

		public bool IsMemberLocked(bool isMember)
		{
			return Definition.IsMemberOnly && !isMember;
		}

		public bool IsProgressionLocked(int playerProgressionLevel)
		{
			return Definition.LevelType == LockType.PenguinLevel && Definition.LevelRequired > playerProgressionLevel;
		}

		public bool IsQuestLocked(int mascotQuestLevel)
		{
			return Definition.LevelType == LockType.QuestLevel && Definition.LevelRequired > mascotQuestLevel;
		}

		public Task(TaskDefinition definition)
		{
			Definition = definition;
		}

		public void Increment()
		{
			if (Definition.CounterMax == 0 || Counter < Definition.CounterMax)
			{
				SetCounter(Counter + 1);
			}
		}

		public void SetCounter(int count)
		{
			bool isComplete = IsComplete;
			Counter = count;
			Service.Get<EventDispatcher>().DispatchEvent(new TaskEvents.TaskUpdated(this));
			if (!isComplete && IsComplete)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new TaskEvents.TaskCompleted(this));
			}
		}
	}
}
