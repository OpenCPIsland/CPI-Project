using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Choreography
{
	public class TestScriptableAction : ScriptableAction
	{
		public int Count;

		public override IEnumerator Execute(ScriptableActionPlayer player)
		{
			player.AddFinalizer(delegate
			{
				Debug.Log("Finalizing action " + base.name + " on player " + player.gameObject);
			});
			for (int i = 0; i < Count; i++)
			{
				yield return null;
			}
		}
	}
}
