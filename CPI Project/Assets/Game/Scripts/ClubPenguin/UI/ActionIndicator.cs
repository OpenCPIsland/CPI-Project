using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class ActionIndicator : MonoBehaviour
	{
		public float TargetScale = 0.08f;

		public float MinScale = 0.5f;

		public float MaxScale = 1.4f;

		private Transform transformReference;

		private Transform cameraTransform;

		private Vector3 initialScale;

		private Renderer indicatorRenderer;

		private EventChannel eventChannel;

		public void Start()
		{
			transformReference = base.transform;
			cameraTransform = Camera.main.transform;
			indicatorRenderer = GetComponent<Renderer>();
			initialScale = base.transform.localScale;
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<CinematicSpeechEvents.ShowSpeechEvent>(onShowSpeech);
			eventChannel.AddListener<CinematicSpeechEvents.SpeechCompleteEvent>(onSpeechComplete);
		}

		public void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
		}

		public void Update()
		{
			rotateToCamera();
			adjustScale();
		}

		private bool onShowSpeech(CinematicSpeechEvents.ShowSpeechEvent evt)
		{
			indicatorRenderer.enabled = false;
			return false;
		}

		private bool onSpeechComplete(CinematicSpeechEvents.SpeechCompleteEvent evt)
		{
			indicatorRenderer.enabled = true;
			return false;
		}

		private void rotateToCamera()
		{
			transformReference.LookAt(transformReference.position + cameraTransform.rotation * Vector3.back, cameraTransform.rotation * Vector3.up);
		}

		private void adjustScale()
		{
			float distanceToPoint = new Plane(cameraTransform.forward, cameraTransform.position).GetDistanceToPoint(transformReference.position);
			transformReference.localScale = initialScale * distanceToPoint * TargetScale;
			float num = Mathf.Clamp(transformReference.localScale.x, MinScale, MaxScale);
			transformReference.localScale = new Vector3(num, num, num);
		}
	}
}
