using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Choreography
{
	public class SimpleMoveAction : ScriptableAction
	{
		public float Duration;

		public Vector3 Position;

		public override IEnumerator Execute(ScriptableActionPlayer player)
		{
			float elapsed = 0f;
			Vector3 startPos = player.transform.position;
			for (; elapsed < Duration; elapsed += Time.deltaTime)
			{
				float t = elapsed / Duration;
				player.transform.position = Vector3.Lerp(startPos, Position, t);
				yield return null;
			}
			player.transform.position = Position;
		}
	}
}
