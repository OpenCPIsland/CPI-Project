using ClubPenguin.Analytics;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	[RequireComponent(typeof(Animator))]
	public class RotationWidget : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEventSystemHandler
	{
		private const string IDLE_ANIM = "Idle";

		private const string PRESS_ANIM = "Pressed";

		[SerializeField]
		private RectTransform rotationTarget;

		[SerializeField]
		private RectTransform rotationCenter;

		[SerializeField]
		private float rotationSpeed = 7.5f;

		private Vector3 _originalPosition = Vector3.zero;

		private Quaternion _originalRotation = Quaternion.identity;

		private float _value;

		private RectTransform rectTransform;

		private Vector2 prevPositionInLocal;

		private Animator animator;

		private EventChannel eventChannel;

		private bool _hasInput = false;

		private bool _interactable = true;

		public GameObject disabledCover = null;

		public float Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				ApplyRotation();
			}
		}

		private void Awake()
		{
			rectTransform = (base.transform as RectTransform);
			animator = GetComponent<Animator>();
			_originalPosition = rotationTarget.localPosition;
			_originalRotation = rotationTarget.localRotation;
			if (rotationCenter == null)
			{
				rotationCenter = rotationTarget;
			}
			eventChannel = new EventChannel(CustomizationContext.EventBus);
			eventChannel.AddListener<CustomizerWidgetEvents.RotationWdigetSetIsInteractable>(onSetInteractable);
			eventChannel.AddListener<CustomizerWidgetEvents.RotationWidgetSetValue>(onSetValue);
		}

		private bool onSetInteractable(CustomizerWidgetEvents.RotationWdigetSetIsInteractable evt)
		{
			bool isInteractable = evt.IsInteractable;
			bool flag = true;
			if (!isInteractable && _hasInput)
			{
				flag = false;
			}
			if (flag)
			{
				_interactable = isInteractable;
				if (disabledCover != null)
				{
					disabledCover.SetActive(!_interactable);
				}
			}
			return false;
		}

		private bool onSetValue(CustomizerWidgetEvents.RotationWidgetSetValue evt)
		{
			Value = evt.Value;
			return false;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (_interactable)
			{
				prevPositionInLocal = rectTransform.worldToLocalMatrix.MultiplyPoint(eventData.position);
				animator.ResetTrigger("Idle");
				animator.SetTrigger("Pressed");
				_hasInput = true;
				CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputStateChange(true, true));
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (_interactable)
			{
				Vector2 vector = rectTransform.worldToLocalMatrix.MultiplyPoint(eventData.position);
				float num = Vector2.Angle(vector, prevPositionInLocal);
				float num2 = Mathf.Sign(Vector3.Cross(prevPositionInLocal, vector).z);
				float num3 = num * num2;
				prevPositionInLocal = vector;
				_value += num3 * rotationSpeed * ((float)Math.PI / 180f);
				CustomizationContext.EventBus.DispatchEvent(new CustomizerWidgetEvents.RotationWidgetRotated(_value));
				ApplyRotation();
			}
		}

		private void ApplyRotation()
		{
			rotationTarget.localPosition = _originalPosition;
			rotationTarget.localRotation = _originalRotation;
			rotationTarget.RotateAround(rotationCenter.position, Vector3.forward, _value * 57.29578f);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (_interactable)
			{
				animator.ResetTrigger("Pressed");
				animator.SetTrigger("Idle");
				Service.Get<ICPSwrveService>().Action("game.template_adjust", "rotate");
				_hasInput = false;
				CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.InputStateChange(false, true));
			}
		}

		private void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
		}
	}
}
