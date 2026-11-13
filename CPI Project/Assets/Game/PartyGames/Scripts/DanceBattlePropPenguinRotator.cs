using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class DanceBattlePropPenguinRotator : MonoBehaviour
	{
		public float PenguinYAxisRotation = -51.77f;

		private void Start()
		{
			AvatarDataHandle componentInParent = GetComponentInParent<AvatarDataHandle>();
			if (componentInParent != null)
			{
				Vector3 eulerAngles = componentInParent.gameObject.GetComponent<Transform>().rotation.eulerAngles;
				eulerAngles.y = PenguinYAxisRotation;
				componentInParent.gameObject.GetComponent<Transform>().rotation = Quaternion.Euler(eulerAngles);
			}
		}
	}
}
