using ClubPenguin;
using ClubPenguin.IslandTargets;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IslandTarget : MonoBehaviour
{
	public enum TargetState
	{
		Disabled,
		Regenerating,
		Ready,
		Active,
		Damaged,
		PendingDestroy,
		Destroyed,
		TimedOut
	}

	public const float FULL_DAMAGE = 1f;

	private const float TIME_OUT_WAIT_TIME = 2f;

	public bool DamagedBySwords = false;

	public bool DamagedBySnowballs = true;

	public ParticleSystem DestroyedParticles = null;

	public Slider DamageSlider = null;

	public IslandTargetDamageHandler DamageHandler = null;

	public GameObject CheckMarkObject;

	public GameObject AimAssistObject;

	public GameObject HitBoxObject;

	public ParticleSystem HitEffectParticleSystem;

	[Range(0.5f, 6f)]
	[Tooltip("The amount of time to allow the client to wait for the local snowball count to match the server count when a target is destroyed on the server")]
	public float SecondsDestroyWindow = 0.5f;

	[Tooltip("Max time delay for target to animate into view.")]
	[Range(0f, 2f)]
	public float MaxSecondsWaitAnimatedIntoView = 0.5f;

	[Range(0f, 2f)]
	[Tooltip("Min time delay for target to animate into view.")]
	public float MinSecondsWaitAnimatedIntoView = 0f;

	[Range(0f, 4f)]
	[Tooltip("Wait time to delay enable colliders while animating into view")]
	public float DelayTimeEnableColliders = 2f;

	[Header("Animation Trigger Names")]
	public string TriggerStart = "Start";

	public string TriggerTimeout = "TimeOut";

	public float TimeoutAnimTime = 0f;

	public string TriggertHit = "";

	public string TriggerDefeat = "";

	public float DefeatAnimTime = 0f;

	public string TriggerHeadFlashInit = "";

	public string TriggerHeadFlashStop = "";

	[HideInInspector]
	public int DamageCapacity = 0;

	[HideInInspector]
	public int ServerDamageCount = 0;

	[HideInInspector]
	public float DamagePercent;

	private int localDamageCount = 0;

	private float pendingDestroyStartTime = 0f;

	private float timedOutStartTime = 0f;

	private TargetState targetState = TargetState.Disabled;

	private IslandTargetPlaygroundController playground;

	private EventDispatcher dispatcher;

	private Animator animator;

	private bool wasDefeated;

	public void Awake()
	{
		animator = GetComponentInParent<Animator>();
		EnableColliders(false);
	}

	public void OnEnable()
	{
		targetState = TargetState.Ready;
	}

	public void OnDisable()
	{
	}

	private void RemoveListeners()
	{
		dispatcher.RemoveListener<IslandTargetsEvents.TargetGameTimeOut>(onGameTimeOut);
		dispatcher.RemoveListener<IslandTargetsEvents.GameRoundEnded>(onGameRoundEnded);
	}

	public void Initialize(int startingDamageCount, int hitCapacity, IslandTargetPlaygroundController owner)
	{
		if (dispatcher == null)
		{
			dispatcher = owner.EventDispatcher;
		}
		dispatcher.AddListener<IslandTargetsEvents.TargetGameTimeOut>(onGameTimeOut);
		dispatcher.AddListener<IslandTargetsEvents.GameRoundEnded>(onGameRoundEnded);
		wasDefeated = false;
		ResetTarget();
		if (DamageHandler != null)
		{
			DamageHandler.Reset();
		}
		localDamageCount = startingDamageCount;
		DamagePercent = (float)startingDamageCount / (float)DamageCapacity;
		if (startingDamageCount >= hitCapacity)
		{
			if (DamageHandler != null)
			{
				DamageHandler.SetLightRotationAndState(1f);
			}
			DestroyTarget();
		}
		else if (startingDamageCount > 0)
		{
			if (DamageHandler != null)
			{
				DamagePercent = (float)startingDamageCount / (float)hitCapacity;
				DamageHandler.SetLightRotationAndState(DamagePercent);
			}
			ChangeState(TargetState.Damaged);
		}
		else
		{
			ChangeState(TargetState.Ready);
		}
		CoroutineRunner.Start(AnimateIntoView(), this, "TargetAnimateIn");
	}

	public void OnValidate()
	{
	}

	private void Update()
	{
		ProcessTargetState();
	}

	private bool CanBeDamaged()
	{
		switch (targetState)
		{
		case TargetState.Active:
		case TargetState.Damaged:
		case TargetState.PendingDestroy:
			return true;
		default:
			return false;
		}
	}

	public void CollisionFromHitBox(Collision collision)
	{
		if (!DamagedBySnowballs)
		{
			return;
		}
		StartHitEffect();
		if (CanBeDamaged() && collision.collider.CompareTag("Snowball") && targetState != 0)
		{
			SetDamage();
			SnowballController component = collision.gameObject.GetComponent<SnowballController>();
			if (component != null && component.OwnerId != 0)
			{
				dispatcher.DispatchEvent(new IslandTargetsEvents.LocalPlayerHitTargetEvent(this));
			}
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		if (CanBeDamaged() && DamagedBySwords && targetState != 0 && col.CompareTag("SwordCollider"))
		{
			SetDamage();
		}
	}

	private void SetDamage()
	{
		if (!((float)DamageCapacity > 0f))
		{
			return;
		}
		localDamageCount++;
		if (localDamageCount > DamageCapacity)
		{
			localDamageCount = DamageCapacity;
		}
		ChangeState(TargetState.Damaged);
		if (ServerDamageCount >= DamageCapacity)
		{
			DestroyTarget();
			return;
		}
		DamagePercent = (float)localDamageCount / (float)DamageCapacity;
		if (ServerDamageCount > localDamageCount)
		{
			DamagePercent = (float)ServerDamageCount / (float)DamageCapacity;
			localDamageCount = ServerDamageCount;
		}
		if (DamageHandler != null && localDamageCount < DamageCapacity)
		{
			DamageHandler.SetLightRotationAndState(DamagePercent);
		}
	}

	private void StartHitEffect()
	{
		if (HitEffectParticleSystem != null)
		{
			HitEffectParticleSystem.Emit(1);
		}
		if (animator != null && !string.IsNullOrEmpty(TriggertHit))
		{
			animator.SetTrigger(TriggertHit);
		}
	}

	public void UpdateDamageSlider()
	{
		if (DamageSlider != null)
		{
			DamageSlider.value = 1 - ServerDamageCount / DamageCapacity;
		}
	}

	public void ChangeState(TargetState newState)
	{
		TargetState targetState = this.targetState;
		if (targetState == TargetState.PendingDestroy)
		{
			if (newState != TargetState.Damaged)
			{
				this.targetState = newState;
			}
		}
		else
		{
			this.targetState = newState;
		}
	}

	private void ProcessTargetState()
	{
		switch (targetState)
		{
		case TargetState.Regenerating:
			break;
		case TargetState.Destroyed:
			break;
		case TargetState.Disabled:
			DisableTarget();
			break;
		case TargetState.Ready:
			ChangeState(TargetState.Active);
			break;
		case TargetState.Active:
			ShowDamageSlider(false);
			break;
		case TargetState.Damaged:
			ShowDamageSlider(true);
			break;
		case TargetState.TimedOut:
			if (Time.time >= timedOutStartTime + 2f)
			{
				TimeOutDestroy();
			}
			break;
		case TargetState.PendingDestroy:
			if (Time.time >= pendingDestroyStartTime + SecondsDestroyWindow)
			{
				DestroyTarget();
			}
			break;
		}
	}

	public void CueDestroyTargetByServer()
	{
		if (localDamageCount >= DamageCapacity)
		{
			DestroyTarget();
			return;
		}
		pendingDestroyStartTime = Time.time;
		ChangeState(TargetState.PendingDestroy);
	}

	public void DestroyTarget()
	{
		if (targetState == TargetState.Destroyed)
		{
			return;
		}
		DamagePercent = 1f;
		if (DamageHandler != null)
		{
			DamageHandler.SetLightRotationAndState(1f);
		}
		if (DestroyedParticles != null)
		{
			ParticleSystem particleSystem = Object.Instantiate(DestroyedParticles, base.transform.position, Quaternion.identity);
			particleSystem.transform.SetParent(base.transform);
			particleSystem.transform.localPosition = Vector3.zero;
			particleSystem.Play();
		}
		if (animator != null && !string.IsNullOrEmpty(TriggerDefeat))
		{
			animator.SetTrigger(TriggerDefeat);
			wasDefeated = true;
			if (!string.IsNullOrEmpty(TriggerHeadFlashStop))
			{
				animator.SetTrigger(TriggerHeadFlashStop);
			}
		}
		ShowDamageSlider(false);
		EnableColliders(false);
		ChangeState(TargetState.Destroyed);
		CoroutineRunner.Start(DestroyTargetHide(DefeatAnimTime), this, "TargetAnimateOut");
	}

	private IEnumerator DestroyTargetHide(float waitTime = 0f)
	{
		yield return new WaitForSeconds(waitTime);
		GetComponent<MeshRenderer>().enabled = false;
		CheckMarkObject.SetActive(true);
	}

	public void TimeOutDestroy()
	{
		GetComponent<MeshRenderer>().enabled = false;
		ShowDamageSlider(false);
		EnableColliders(false);
		EnableGameObjects(false);
		ChangeState(TargetState.Destroyed);
	}

	private void EnableGameObjects(bool enable = true, bool applyToSiblings = false)
	{
		GameObject gameObject = base.transform.parent.gameObject;
		GameObject gameObject2 = gameObject.transform.parent.gameObject;
		gameObject.SetActive(enable);
		gameObject2.SetActive(enable);
		if (applyToSiblings)
		{
			foreach (Transform child in gameObject.GetChildren())
			{
				child.gameObject.SetActive(enable);
			}
		}
	}

	public void DisableTarget()
	{
		ShowDamageSlider(false);
	}

	public void ResetTarget()
	{
		EnableGameObjects(true, true);
		GetComponent<MeshRenderer>().enabled = true;
		if (DamageHandler != null)
		{
			DamageHandler.SetLightRotationAndState(0f);
		}
		DamagePercent = 0f;
		GetComponent<MeshRenderer>().enabled = true;
		CheckMarkObject.SetActive(false);
	}

	private void EnableColliders(bool enable)
	{
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		Collider[] array = componentsInChildren;
		foreach (Collider collider in array)
		{
			collider.enabled = enable;
		}
		AimAssistObject.GetComponent<Collider>().enabled = enable;
		HitBoxObject.GetComponent<Collider>().enabled = enable;
	}

	private void ShowDamageSlider(bool enable)
	{
		if (DamageSlider != null)
		{
			DamageSlider.gameObject.SetActive(enable);
		}
	}

	public void ShakeTarget(Vector3 strength)
	{
	}

	private IEnumerator AnimateIntoView()
	{
		float waitTime = Random.Range(MinSecondsWaitAnimatedIntoView, MaxSecondsWaitAnimatedIntoView);
		yield return new WaitForSeconds(waitTime);
		if (animator != null)
		{
			animator.SetTrigger(TriggerStart);
			if (!string.IsNullOrEmpty(TriggerHeadFlashInit))
			{
				animator.ResetTrigger(TriggerHeadFlashInit);
				animator.SetTrigger(TriggerHeadFlashInit);
			}
			if (!string.IsNullOrEmpty(TriggerHeadFlashStop))
			{
				animator.ResetTrigger(TriggerHeadFlashStop);
			}
		}
		yield return EnableCollidersDelayed();
	}

	private IEnumerator EnableCollidersDelayed()
	{
		yield return new WaitForSeconds(DelayTimeEnableColliders);
		EnableColliders(true);
	}

	private IEnumerator AnimateOutOfView()
	{
		if (!base.gameObject.IsDestroyed())
		{
			EnableColliders(false);
			float waitTime = Random.Range(MinSecondsWaitAnimatedIntoView, MaxSecondsWaitAnimatedIntoView);
			yield return AnimateOutOfViewDelayed(waitTime);
		}
	}

	private IEnumerator AnimateOutOfViewDelayed(float waitTime)
	{
		if (base.gameObject.IsDestroyed())
		{
			yield break;
		}
		EnableColliders(false);
		yield return new WaitForSeconds(waitTime);
		if (animator != null && !string.IsNullOrEmpty(TriggerDefeat) && !wasDefeated)
		{
			animator.SetTrigger(TriggerTimeout);
			if (!string.IsNullOrEmpty(TriggerHeadFlashStop))
			{
				animator.SetTrigger(TriggerHeadFlashStop);
			}
		}
	}

	private bool onGameTimeOut(IslandTargetsEvents.TargetGameTimeOut evt)
	{
		RemoveListeners();
		CoroutineRunner.Start(AnimateOutOfView(), this, "TargetAnimateOut");
		return false;
	}

	private bool onGameRoundEnded(IslandTargetsEvents.GameRoundEnded evt)
	{
		RemoveListeners();
		if (localDamageCount < DamageCapacity)
		{
			CoroutineRunner.Start(AnimateOutOfViewDelayed(2f), this, "TargetAnimateOutDelayed");
		}
		else
		{
			CoroutineRunner.Start(AnimateOutOfView(), this, "TargetAnimateOut");
		}
		return false;
	}
}
