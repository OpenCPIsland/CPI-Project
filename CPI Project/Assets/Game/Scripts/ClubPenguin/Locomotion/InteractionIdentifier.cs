using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class InteractionIdentifier : MonoBehaviour
	{
		public string ID;

		public void OnValidate()
		{
		}

		public override string ToString()
		{
			return ID;
		}
	}
}
