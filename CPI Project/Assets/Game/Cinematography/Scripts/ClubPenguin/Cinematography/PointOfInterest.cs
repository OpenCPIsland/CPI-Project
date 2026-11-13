using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	internal class PointOfInterest : MonoBehaviour
	{
		public float Weight = 100f;

		public bool IsActive = true;

		private Dictionary<Collider, PointOfInterestDirector> cachedDirectors = new Dictionary<Collider, PointOfInterestDirector>();

		public void OnTriggerStay(Collider col)
		{
			PointOfInterestDirector pointOfInterestDirectorForCollider = getPointOfInterestDirectorForCollider(col);
			if (pointOfInterestDirectorForCollider != null)
			{
				Vector3 vector = base.transform.position - col.transform.position;
				if (Vector3.Dot(rhs: col.transform.TransformDirection(Vector3.forward).normalized, lhs: vector.normalized) > 0f)
				{
					pointOfInterestDirectorForCollider.Activate(this);
				}
				else
				{
					pointOfInterestDirectorForCollider.Deactivate(this);
				}
			}
		}

		public void OnTriggerExit(Collider col)
		{
			PointOfInterestDirector pointOfInterestDirectorForCollider = getPointOfInterestDirectorForCollider(col);
			if (pointOfInterestDirectorForCollider != null)
			{
				pointOfInterestDirectorForCollider.Deactivate(this);
			}
		}

		private PointOfInterestDirector getPointOfInterestDirectorForCollider(Collider col)
		{
			PointOfInterestDirector pointOfInterestDirector;
			if (cachedDirectors.ContainsKey(col))
			{
				pointOfInterestDirector = cachedDirectors[col];
			}
			else
			{
				pointOfInterestDirector = col.GetComponent<PointOfInterestDirector>();
				if (pointOfInterestDirector != null)
				{
					cachedDirectors.Add(col, pointOfInterestDirector);
				}
			}
			return pointOfInterestDirector;
		}
	}
}
