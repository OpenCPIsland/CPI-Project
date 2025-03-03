using System.Runtime.InteropServices;
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

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ActionSequenceCompleted
		{
		}
	}
}
