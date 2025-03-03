using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class SnowballPowerMeter : MonoBehaviour
	{
		private enum PowerMeterState
		{
			Off,
			PreCharge,
			Charging,
			MaxPower,
			Fading
		}

		private const string MAX_POWER_ANIM_BOOL = "MaxPower";

		public Image PowerMeterImage;

		public CanvasGroup PowerMeterCanvasGroup;

		public RectTransform LastThrowIcon;

		public float FadeTime;

		public float ShowDelay = 0.2f;

		public float MaxMeterFill = 0.49f;

		public float MaxLastThrowIconAngle = 80f;

		public ColorUtils.ColorAtPercent[] MeterColors;

		public bool TransitionBetweenColors;

		private float timer;

		private float maxChargeTime;

		private float lastThrowPosition;

		private PowerMeterState currentState;

		private Animator animator;

		private void Start()
		{
			animator = GetComponent<Animator>();
			Service.Get<EventDispatcher>().AddListener<HudEvents.StartSnowballPowerMeter>(onPowerMeterStarted);
			Service.Get<EventDispatcher>().AddListener<HudEvents.StopSnowballPowerMeter>(onPowerMeterStopped);
			setState(PowerMeterState.Off);
			LastThrowIcon.gameObject.SetActive(false);
		}

		private void Update()
		{
			if (currentState == PowerMeterState.PreCharge)
			{
				timer += Time.deltaTime;
				if (timer > ShowDelay)
				{
					setState(PowerMeterState.Charging);
				}
			}
			else if (currentState == PowerMeterState.Charging)
			{
				timer += Time.deltaTime;
				if (timer < maxChargeTime)
				{
					float num = timer / maxChargeTime;
					PowerMeterImage.fillAmount = Mathf.Lerp(0f, MaxMeterFill, num);
					PowerMeterImage.color = ColorUtils.GetColorAtPercent(MeterColors, num, TransitionBetweenColors);
				}
				else
				{
					setState(PowerMeterState.MaxPower);
				}
			}
			else if (currentState == PowerMeterState.Fading)
			{
				timer += Time.deltaTime;
				if (timer < FadeTime)
				{
					PowerMeterCanvasGroup.alpha = 1f - timer / FadeTime;
				}
				else
				{
					setState(PowerMeterState.Off);
				}
			}
		}

		private bool onPowerMeterStarted(HudEvents.StartSnowballPowerMeter evt)
		{
			maxChargeTime = evt.MaxChargeTime;
			setState(PowerMeterState.PreCharge);
			return false;
		}

		private bool onPowerMeterStopped(HudEvents.StopSnowballPowerMeter evt)
		{
			if (!evt.WasCancelled && currentState == PowerMeterState.Charging)
			{
				lastThrowPosition = Mathf.Min(timer / maxChargeTime, 1f);
				if (lastThrowPosition > 0f)
				{
					LastThrowIcon.gameObject.SetActive(true);
					LastThrowIcon.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(MaxLastThrowIconAngle, 0f - MaxLastThrowIconAngle, lastThrowPosition));
				}
				else
				{
					LastThrowIcon.gameObject.SetActive(false);
				}
				setState(PowerMeterState.Fading);
			}
			else
			{
				lastThrowPosition = 0f;
				LastThrowIcon.gameObject.SetActive(false);
				setState(PowerMeterState.Off);
			}
			timer = 0f;
			return false;
		}

		private void setState(PowerMeterState newState)
		{
			switch (newState)
			{
			case PowerMeterState.Off:
				base.gameObject.SetActive(false);
				animator.SetBool("MaxPower", false);
				break;
			case PowerMeterState.PreCharge:
				timer = 0f;
				PowerMeterCanvasGroup.alpha = 0f;
				PowerMeterImage.fillAmount = 0f;
				base.gameObject.SetActive(true);
				break;
			case PowerMeterState.Charging:
				PowerMeterCanvasGroup.alpha = 1f;
				PowerMeterImage.fillAmount = Mathf.Lerp(0f, MaxMeterFill, timer / maxChargeTime);
				break;
			case PowerMeterState.MaxPower:
				animator.SetBool("MaxPower", true);
				PowerMeterImage.fillAmount = 1f;
				PowerMeterImage.color = MeterColors[MeterColors.Length - 1].Color;
				break;
			case PowerMeterState.Fading:
				animator.SetBool("MaxPower", false);
				break;
			}
			currentState = newState;
		}
	}
}
