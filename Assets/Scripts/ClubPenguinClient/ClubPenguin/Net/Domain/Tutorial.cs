using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct Tutorial
	{
		public int tutorialID;

		public bool isComplete;

		public Tutorial(int tutorialID, bool isComplete)
		{
			this.tutorialID = tutorialID;
			this.isComplete = isComplete;
		}

		public override string ToString()
		{
			return string.Format("[Tutorial] ID: {0} IsComplete: {1}", tutorialID, isComplete);
		}
	}
}
