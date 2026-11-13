using UnityEngine;

namespace ClubPenguin
{
	public static class ActionSequencerEvents
	{
		public struct ActionSequenceStarted
		{
			public GameObject actionGameObject;

			public ActionSequenceStarted(GameObject go)
			{
				actionGameObject = go;
			}
		}

		public struct ActionSequenceCompleted
		{
			public GameObject actionGameObject;
			public bool wasSuccessful;

			public ActionSequenceCompleted(GameObject go, bool wasSuccessful)
			{
				actionGameObject = go;
				this.wasSuccessful = wasSuccessful;
			}
		}
	}
}
