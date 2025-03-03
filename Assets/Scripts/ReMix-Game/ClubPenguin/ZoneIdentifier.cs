using UnityEngine;

namespace ClubPenguin
{
	public class ZoneIdentifier : MonoBehaviour
	{
		public ZoneDefinition Zone;

		private void Awake()
		{
			if (Zone == null)
			{
				throw new MissingReferenceException("Zone definition is null");
			}
		}
	}
}
