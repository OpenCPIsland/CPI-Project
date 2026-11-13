using ClubPenguin.Adventure;
using System.Runtime.InteropServices;

namespace ClubPenguin.UI
{
	public class QuestScreenEvents
	{
		public struct ShowQuestDetails
		{
			public readonly Quest Quest;

			public ShowQuestDetails(Quest questData)
			{
				Quest = questData;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowQuestLog
		{
		}

		public struct ShowQuestLogAdventures
		{
			public readonly string MascotID;

			public ShowQuestLogAdventures(string mascotID)
			{
				MascotID = mascotID;
			}
		}
	}
}
