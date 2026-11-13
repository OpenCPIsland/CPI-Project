using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class ScriptableActionPause : ScriptableAction
	{
		private static ScriptableAction instance;

		public static ScriptableAction Instance
		{
			get
			{
				if (instance == null)
				{
					instance = ScriptableObject.CreateInstance<ScriptableActionPause>();
				}
				return instance;
			}
		}

		[RuntimeInitializeOnLoadMethod]
		private static void initialize()
		{
			Instance.name = "Pause";
		}

		public override IEnumerator Execute(ScriptableActionPlayer player)
		{
			while (!player.ActionIsFinished)
			{
				yield return null;
			}
		}
	}
}
