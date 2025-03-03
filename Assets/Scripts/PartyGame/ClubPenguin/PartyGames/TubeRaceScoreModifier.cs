using UnityEngine;

namespace ClubPenguin.PartyGames
{
	public class TubeRaceScoreModifier : MonoBehaviour
	{
		public int ScoreDelta;

		public int ModifierId;

		private GameObject modifierPrefab;

		private void OnDrawGizmos()
		{
			if (ScoreDelta > 0)
			{
				Gizmos.color = Color.green;
			}
			else
			{
				Gizmos.color = Color.red;
			}
			Gizmos.DrawSphere(base.transform.position, 0.5f);
		}

		public void Activate(GameObject positiveModifierPrefab, GameObject negativeModifierPrefab)
		{
			Deactivate();
			if (ScoreDelta > 0)
			{
				modifierPrefab = Object.Instantiate(positiveModifierPrefab, base.transform);
			}
			else if (ScoreDelta < 0)
			{
				modifierPrefab = Object.Instantiate(negativeModifierPrefab, base.transform);
			}
		}

		public void Deactivate()
		{
			if (modifierPrefab != null)
			{
				Object.Destroy(modifierPrefab);
			}
		}
	}
}
