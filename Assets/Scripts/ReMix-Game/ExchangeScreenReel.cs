using UnityEngine;
using UnityEngine.UI;

public class ExchangeScreenReel : MonoBehaviour
{
	private enum ExchangeReelState
	{
		Idle,
		WaitingToStart,
		Spinning,
		WaitingToStop
	}

	public Text NumberText;

	private Animator animator;

	private bool stopSpin = false;

	private ExchangeReelState currentState;

	private int currentDisplayNumber = 0;

	private float delayAmount = 0f;

	private float delayTimer = 0f;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		NumberText.text = "0";
	}

	private void Update()
	{
		if (currentState != ExchangeReelState.WaitingToStart && currentState != ExchangeReelState.WaitingToStop)
		{
			return;
		}
		delayTimer += Time.deltaTime;
		if (delayTimer > delayAmount)
		{
			if (currentState == ExchangeReelState.WaitingToStart)
			{
				StartSpinAnimation();
			}
			else
			{
				StopSpinAnimation();
			}
		}
	}

	public void StartSpin(float delay = 0f)
	{
		if (delay == 0f)
		{
			StartSpinAnimation();
			return;
		}
		delayTimer = 0f;
		delayAmount = delay;
		currentState = ExchangeReelState.WaitingToStart;
	}

	public void StopSpin()
	{
		delayTimer = 0f;
		stopSpin = true;
		currentState = ExchangeReelState.WaitingToStop;
	}

	private void StartSpinAnimation()
	{
		animator.SetTrigger("StartSpin");
		currentState = ExchangeReelState.Spinning;
	}

	private void StopSpinAnimation()
	{
		animator.SetTrigger("StopSpin");
		currentState = ExchangeReelState.Idle;
	}

	public void OnReelSpin()
	{
		if (stopSpin)
		{
			StopSpinAnimation();
			return;
		}
		currentDisplayNumber++;
		if (currentDisplayNumber > 9)
		{
			currentDisplayNumber = 0;
		}
		NumberText.text = currentDisplayNumber.ToString();
	}

	public void SetNumber(int count)
	{
		NumberText.text = count.ToString();
	}
}
