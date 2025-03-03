using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Choreography
{
	public class WaitScriptableAction : ScriptableAction
	{
		public float Seconds;

		public override IEnumerator Execute(ScriptableActionPlayer player)
		{
			yield return new WaitForSeconds(Seconds);
		}
	}
}
