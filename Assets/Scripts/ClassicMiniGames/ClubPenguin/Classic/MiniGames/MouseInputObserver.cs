using System;
using UnityEngine;

namespace ClubPenguin.Classic.MiniGames
{
	public class MouseInputObserver : MonoBehaviour
	{
		public string HorizontalAxisName = "Mouse X";

		public string VerticalAxisName = "Mouse Y";

		public float horizontalSpeed = 2f;

		public float verticalSpeed = 2f;

		public event Action<Vector3, Vector2> MouseMovedEvent;

		public event Action PrimaryMouseButtonDownEvent;

		private void Update()
		{
			float num = horizontalSpeed * Input.GetAxis(HorizontalAxisName) * Time.deltaTime;
			float num2 = verticalSpeed * Input.GetAxis(VerticalAxisName) * Time.deltaTime;
			if (this.MouseMovedEvent != null && num != 0f && num2 != 0f)
			{
				this.MouseMovedEvent(Input.mousePosition, new Vector2(num, num2));
			}
			if (Input.GetMouseButtonDown(0) && this.PrimaryMouseButtonDownEvent != null)
			{
				this.PrimaryMouseButtonDownEvent();
			}
		}

		private void OnDestroy()
		{
			this.MouseMovedEvent = null;
			this.PrimaryMouseButtonDownEvent = null;
		}
	}
}
