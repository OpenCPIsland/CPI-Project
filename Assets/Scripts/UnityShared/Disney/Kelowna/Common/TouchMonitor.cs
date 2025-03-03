using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class TouchMonitor : MonoBehaviour
	{
		[Tooltip("The magnitude of movement that consitutes a swipe gesture")]
		[Range(1f, 100f)]
		public float SwipeMagnitude = 10f;

		public bool EnableSampling = true;

		private bool isTouching = false;

		public Vector2 DeltaPosition
		{
			get;
			private set;
		}

		public float Magnitude
		{
			get
			{
				return DeltaPosition.magnitude;
			}
		}

		public bool IsSwiping
		{
			get
			{
				return Magnitude > SwipeMagnitude;
			}
		}

		private void Update()
		{
			if (EnableSampling)
			{
				if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
				{
					DeltaPosition = Input.GetTouch(0).deltaPosition;
					isTouching = true;
				}
				else if (isTouching)
				{
					DeltaPosition = Vector2.zero;
					isTouching = false;
				}
			}
		}
	}
}
