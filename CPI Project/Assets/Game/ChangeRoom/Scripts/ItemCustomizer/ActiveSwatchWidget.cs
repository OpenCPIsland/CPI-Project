using ClubPenguin.UI;
using Disney.Kelowna.Common;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	[RequireComponent(typeof(ActiveSwatchWidgetDisabler))]
	public class ActiveSwatchWidget : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		private const float TOGGLE_DELAY = 0.5f;

		public RawImage fabricIconUI = null;

		public RawImage decalIconUI = null;

		public GameObject fabricBG;

		public GameObject decalBG;

		public GameObject disabledCover = null;

		public Transform[] iconScaleHandles = null;

		private Vector3[] _originalIconScales = null;

		public CanvasGroup[] iconLabelGroups = null;

		public float rotationStrengthDegrees = 180f;

		private readonly int INDEX_FABRIC = 0;

		private readonly int INDEX_DECAL = 1;

		public SimpleSpringInterper scaleInterper = null;

		public float minScale = 0.33f;

		public Transform moveHandle = null;

		private Vector3 _originalLocalPosition = Vector3.zero;

		public SimpleSpringInterper moveInterper = null;

		public Vector2 moveDirection = Vector2.up;

		public float moveStrength = 10f;

		public Transform[] bounceHandles = null;

		public Graphic[] bounceGraphicsFabric = null;

		public Graphic[] bounceGraphicsDecal = null;

		private Vector3[] _originalBouncePositions = null;

		private Color[] _originalBounceFabricColors = null;

		private Color[] _originalBounceDecalColors = null;

		private float _bounceT = -1f;

		public float bounceDuration = 1f;

		public float bounceStrength = 1f;

		public AnimationCurve bounceCurve = null;

		public Color bounceColor = Color.white;

		private bool _isVisible = true;

		private bool _isFabric = false;

		private bool _interactable = true;

		public bool doAnimate = true;

		private EventChannel eventChannel;

		private ActiveSwatchWidgetDisabler widgetDisabler;

		private void Awake()
		{
			_originalLocalPosition = moveHandle.localPosition;
			_originalIconScales = new Vector3[iconScaleHandles.Length];
			for (int i = 0; i < _originalIconScales.Length; i++)
			{
				_originalIconScales[i] = iconScaleHandles[i].localScale;
			}
			_originalBouncePositions = new Vector3[bounceHandles.Length];
			for (int i = 0; i < _originalBouncePositions.Length; i++)
			{
				_originalBouncePositions[i] = bounceHandles[i].localPosition;
			}
			_originalBounceFabricColors = new Color[bounceGraphicsFabric.Length];
			for (int i = 0; i < _originalBounceFabricColors.Length; i++)
			{
				_originalBounceFabricColors[i] = bounceGraphicsFabric[i].color;
			}
			_originalBounceDecalColors = new Color[bounceGraphicsDecal.Length];
			for (int i = 0; i < _originalBounceDecalColors.Length; i++)
			{
				_originalBounceDecalColors[i] = bounceGraphicsDecal[i].color;
			}
			SimpleSpringInterper simpleSpringInterper = moveInterper;
			simpleSpringInterper.OnSpringValueChanged = (SimpleSpringInterper.SpringValueChangedHandler)Delegate.Combine(simpleSpringInterper.OnSpringValueChanged, new SimpleSpringInterper.SpringValueChangedHandler(OnMoveValueChanged));
			SimpleSpringInterper simpleSpringInterper2 = scaleInterper;
			simpleSpringInterper2.OnSpringValueChanged = (SimpleSpringInterper.SpringValueChangedHandler)Delegate.Combine(simpleSpringInterper2.OnSpringValueChanged, new SimpleSpringInterper.SpringValueChangedHandler(OnScaleValueChanged));
			scaleInterper.SetSpringGoal(INDEX_FABRIC, true);
			setVisible(false, true);
			setIsFabric(true);
			setupListeners();
			widgetDisabler = GetComponent<ActiveSwatchWidgetDisabler>();
		}

		private void setupListeners()
		{
			eventChannel = new EventChannel(CustomizationContext.EventBus);
			eventChannel.AddListener<CustomizerActiveSwatchEvents.SetIsFabric>(onSetIsFabric);
			eventChannel.AddListener<CustomizerActiveSwatchEvents.SetIsVisible>(onSetVisible);
			eventChannel.AddListener<CustomizerActiveSwatchEvents.SetSwatch>(onSetSwatch);
			eventChannel.AddListener<CustomizerActiveSwatchEvents.SetIsInteractable>(onSetInteractable);
		}

		private void Update()
		{
			if (_isVisible && doAnimate)
			{
				_bounceT += Time.deltaTime;
				while (_bounceT >= bounceDuration)
				{
					_bounceT -= bounceDuration;
				}
				float num = Mathf.Clamp01(_bounceT / bounceDuration);
				if (bounceCurve != null && bounceCurve.length > 0)
				{
					num = bounceCurve.Evaluate(num);
				}
				Vector3 b = Vector3.up * num * bounceStrength;
				for (int i = 0; i < bounceHandles.Length; i++)
				{
					bounceHandles[i].localPosition = _originalBouncePositions[i] + b;
				}
				float t = num;
				float t2 = 0f;
				if (!_isFabric)
				{
					t = 0f;
					t2 = num;
				}
				for (int i = 0; i < bounceGraphicsFabric.Length; i++)
				{
					Color color = Color.Lerp(_originalBounceFabricColors[i], bounceColor, t);
					bounceGraphicsFabric[i].color = color;
				}
				for (int i = 0; i < bounceGraphicsDecal.Length; i++)
				{
					Color color = Color.Lerp(_originalBounceDecalColors[i], bounceColor, t2);
					bounceGraphicsDecal[i].color = color;
				}
			}
		}

		private bool onSetVisible(CustomizerActiveSwatchEvents.SetIsVisible evt)
		{
			setVisible(evt.IsVisible);
			return false;
		}

		private void setVisible(bool isVisible, bool instant = false)
		{
			base.gameObject.SetActive(isVisible);
			_isVisible = isVisible;
			float springGoal = 0f;
			if (!_isVisible)
			{
				springGoal = 1f;
			}
			moveInterper.SetSpringGoal(springGoal, instant);
		}

		private bool onSetIsFabric(CustomizerActiveSwatchEvents.SetIsFabric evt)
		{
			setIsFabric(evt.IsFabric);
			return false;
		}

		private void setIsFabric(bool isFabric)
		{
			_isFabric = isFabric;
			fabricBG.SetActive(isFabric);
			decalBG.SetActive(!isFabric);
			int num = INDEX_FABRIC;
			if (!_isFabric)
			{
				num = INDEX_DECAL;
			}
			scaleInterper.SetSpringGoal(num);
		}

		private bool onSetSwatch(CustomizerActiveSwatchEvents.SetSwatch evt)
		{
			fabricIconUI.texture = evt.Fabric;
			decalIconUI.texture = evt.Decal;
			decalIconUI.enabled = (evt.Decal != null);
			return false;
		}

		private void OnMoveValueChanged(float interp)
		{
			Vector3 b = moveDirection * moveStrength * interp;
			moveHandle.localPosition = _originalLocalPosition + b;
		}

		private void OnScaleValueChanged(float interp)
		{
			for (int i = 0; i < iconScaleHandles.Length; i++)
			{
				float t = Mathf.Abs(interp - (float)i);
				float d = Mathf.Lerp(1f, minScale, t);
				iconScaleHandles[i].localScale = _originalIconScales[i] * d;
				float alpha = Mathf.Lerp(1f, 0f, t);
				iconLabelGroups[i].alpha = alpha;
			}
		}

		private bool onSetInteractable(CustomizerActiveSwatchEvents.SetIsInteractable evt)
		{
			bool isInteractable = evt.IsInteractable;
			SetInteractable(isInteractable);
			return false;
		}

		public void SetInteractable(bool isInteractable)
		{
			if (widgetDisabler != null && !widgetDisabler.IsEnabled && isInteractable)
			{
				return;
			}
			_interactable = isInteractable;
			disabledCover.SetActive(!_interactable);
			Color color = Color.white;
			Color color2 = Color.white;
			if (!_interactable)
			{
				if (_isFabric)
				{
					color2 = Color.grey;
				}
				else
				{
					color = Color.grey;
				}
			}
			fabricIconUI.color = color;
			decalIconUI.color = color2;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (_isVisible && _interactable)
			{
				CustomizationContext.EventBus.DispatchEvent(default(CustomizerActiveSwatchEvents.ToggleActiveSwatch));
				SetInteractable(false);
				CoroutineRunner.Start(toggleDelay(), this, "");
			}
		}

		private IEnumerator toggleDelay()
		{
			yield return new WaitForSeconds(0.5f);
			SetInteractable(true);
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			eventChannel.RemoveAllListeners();
		}
	}
}
