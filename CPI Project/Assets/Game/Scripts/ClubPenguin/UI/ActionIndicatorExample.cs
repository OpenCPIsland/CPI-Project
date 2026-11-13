using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class ActionIndicatorExample : MonoBehaviour
	{
		private const float CAMERA_MAX_X = 5f;

		private const float CAMERA_MIN_X = -5f;

		private const float CAMERA_SPEED = 1f;

		public Transform CameraTransform;

		public Transform Target1;

		public Transform Target2;

		private Vector3 cameraDelta;

		private IEnumerator Start()
		{
			cameraDelta = new Vector3(1f, 0f, 0f);
			yield return new WaitForSeconds(1f);
			SetTargets();
		}

		private void Update()
		{
			MoveCamera();
		}

		private void MoveCamera()
		{
			CameraTransform.position += cameraDelta * Time.deltaTime;
			if (CameraTransform.position.x >= 5f && cameraDelta.x > 0f)
			{
				cameraDelta = new Vector3(-1f, 0f, 0f);
			}
			else if (CameraTransform.position.x <= -5f && cameraDelta.x < 0f)
			{
				cameraDelta = new Vector3(1f, 0f, 0f);
			}
		}

		private void SetTargets()
		{
			if (Target1 != null)
			{
				DActionIndicator dActionIndicator = new DActionIndicator();
				dActionIndicator.IndicatorContentKey = "Prefabs/ActionIndicators/ActionIndicatorArrow";
				dActionIndicator.TargetTransform = Target1;
				dActionIndicator.TargetOffset = new Vector3(0f, 1f, 0f);
				Service.Get<EventDispatcher>().DispatchEvent(new ActionIndicatorEvents.AddActionIndicator(dActionIndicator));
			}
			if (Target2 != null)
			{
				DActionIndicator dActionIndicator2 = new DActionIndicator();
				dActionIndicator2.IndicatorContentKey = "Prefabs/ActionIndicators/ActionIndicatorArrow";
				dActionIndicator2.TargetTransform = Target2;
				dActionIndicator2.TargetOffset = new Vector3(0f, 1f, 0f);
				Service.Get<EventDispatcher>().DispatchEvent(new ActionIndicatorEvents.AddActionIndicator(dActionIndicator2));
			}
		}
	}
}
