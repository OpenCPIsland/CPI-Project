using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	public class InteractionRequestData
	{
		private class SortInteractionRequestProximityAscendingHelper : IComparer
		{
			public int Compare(object x, object y)
			{
				InteractionRequestData interactionRequestData = x as InteractionRequestData;
				InteractionRequestData interactionRequestData2 = y as InteractionRequestData;
				if (interactionRequestData == null || interactionRequestData2 == null)
				{
					throw new ArgumentException("Wrong type to compare interaction request");
				}
				float num = Vector3.Distance(interactionRequestData.TargetPosition, interactionRequestData.RequestingObjectPosition);
				float num2 = Vector3.Distance(interactionRequestData2.TargetPosition, interactionRequestData2.RequestingObjectPosition);
				if (num > num2)
				{
					return 1;
				}
				if (num < num2)
				{
					return -1;
				}
				return 0;
			}
		}

		private class SortInteractionRequestPriorityAscendingHelper : IComparer
		{
			public int Compare(object x, object y)
			{
				InteractionRequestData interactionRequestData = x as InteractionRequestData;
				InteractionRequestData interactionRequestData2 = y as InteractionRequestData;
				if (interactionRequestData == null || interactionRequestData2 == null)
				{
					throw new ArgumentException("Wrong type to compare interaction request");
				}
				if (interactionRequestData.Priority > interactionRequestData2.Priority)
				{
					return 1;
				}
				if (interactionRequestData.Priority < interactionRequestData2.Priority)
				{
					return -1;
				}
				return 0;
			}
		}

		private class SortInteractionRequestProximityThenPriorityAscendingHelper : IComparer
		{
			public int Compare(object x, object y)
			{
				SortInteractionRequestProximityAscendingHelper sortInteractionRequestProximityAscendingHelper = new SortInteractionRequestProximityAscendingHelper();
				int num = sortInteractionRequestProximityAscendingHelper.Compare(x, y);
				if (num == 0)
				{
					num = new SortInteractionRequestPriorityAscendingHelper().Compare(x, y);
				}
				return num;
			}
		}

		public readonly Vector3 TargetPosition;

		public readonly Vector3 RequestingObjectPosition;

		public readonly int Priority;

		public InteractionRequestData(Vector3 targetPosition, Vector3 requestingObjectPosition, int priority)
		{
			TargetPosition = targetPosition;
			RequestingObjectPosition = requestingObjectPosition;
			Priority = priority;
		}

		public static IComparer SortInteractionRequestProximityAscending()
		{
			return new SortInteractionRequestProximityAscendingHelper();
		}

		public static IComparer SortInteractionRequestPriorityAscending()
		{
			return new SortInteractionRequestPriorityAscendingHelper();
		}

		public static IComparer SortInteractionRequestProximityThenPriorityAscending()
		{
			return new SortInteractionRequestProximityThenPriorityAscendingHelper();
		}
	}
}
