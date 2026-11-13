using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	internal class PointOfInterestDirector : MonoBehaviour
	{
		private List<PointOfInterest> activePointsOfInterest = new List<PointOfInterest>();

		public bool HasActivePointOfInterest
		{
			get
			{
				for (int i = 0; i < activePointsOfInterest.Count; i++)
				{
					if (activePointsOfInterest[i].IsActive)
					{
						return true;
					}
				}
				return false;
			}
		}

		public void Activate(PointOfInterest poi)
		{
			if (!activePointsOfInterest.Contains(poi))
			{
				activePointsOfInterest.Add(poi);
			}
		}

		public void Deactivate(PointOfInterest poi)
		{
			activePointsOfInterest.Remove(poi);
		}

		public Vector3 GetWeightedPointOfInterest()
		{
			Vector3 result = Vector3.zero;
			if (activePointsOfInterest.Count == 1)
			{
				if (activePointsOfInterest[0].IsActive)
				{
					result = activePointsOfInterest[0].transform.position;
				}
			}
			else
			{
				float num = 0f;
				for (int i = 0; i < activePointsOfInterest.Count; i++)
				{
					if (activePointsOfInterest[i].IsActive)
					{
						result += activePointsOfInterest[i].transform.position * activePointsOfInterest[i].Weight;
						num += activePointsOfInterest[i].Weight;
					}
				}
				result /= num;
			}
			return result;
		}

		public void OnDrawGizmos()
		{
			if (activePointsOfInterest.Count > 1)
			{
				Vector3 weightedPointOfInterest = GetWeightedPointOfInterest();
				Gizmos.color = Color.cyan;
				for (int i = 0; i < activePointsOfInterest.Count; i++)
				{
					if (activePointsOfInterest[i].IsActive)
					{
						Gizmos.DrawIcon(activePointsOfInterest[i].transform.position, "Cinematography/PointOfInterest.png");
						Gizmos.DrawLine(activePointsOfInterest[i].transform.position, weightedPointOfInterest);
					}
					else
					{
						Gizmos.DrawIcon(activePointsOfInterest[i].transform.position, "Cinematography/PointOfInterestInactive.png");
					}
				}
				Gizmos.DrawSphere(weightedPointOfInterest, 0.2f);
			}
			else if (activePointsOfInterest.Count == 1)
			{
				if (activePointsOfInterest[0].IsActive)
				{
					Gizmos.DrawIcon(activePointsOfInterest[0].transform.position, "Cinematography/PointOfInterest.png");
				}
				else
				{
					Gizmos.DrawIcon(activePointsOfInterest[0].transform.position, "Cinematography/PointOfInterestInactive.png");
				}
			}
		}
	}
}
