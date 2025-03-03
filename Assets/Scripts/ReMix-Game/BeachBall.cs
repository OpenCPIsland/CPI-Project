using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;

public class BeachBall : MonoBehaviour
{
	private const float DISTANCE_THRESHOLD = 6f;

	private Vector3 originPosition;

	private bool outOfBounds;

	private Rigidbody rb;

	public ParticleSystem BurstEffect;

	public ParticleSystem outOfBoundsEffect;

	public Renderer[] RenderersToHide;

	private void Awake()
	{
		setOriginPosition();
		getReferences();
	}

	private void OnEnable()
	{
		BeachBallTarget.OnTargetHit += onHitTarget;
	}

	private void OnDisable()
	{
		BeachBallTarget.OnTargetHit -= onHitTarget;
	}

	private void Update()
	{
		if (!outOfBounds)
		{
			checkForReset();
		}
	}

	private void getReferences()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void onHitTarget(BeachBallTarget.TargetDefinition definition, BeachBall ball, BeachBallTarget target)
	{
		if (ball == this)
		{
			switch (definition.type)
			{
			case BeachBallTargetController.TargetType.Shielded:
				break;
			case BeachBallTargetController.TargetType.Positive:
				resetPosition(true);
				break;
			case BeachBallTargetController.TargetType.Negative:
				resetPosition(false);
				break;
			}
		}
	}

	private void checkForReset()
	{
		Vector3 zero = Vector3.zero;
		zero.y = base.transform.localPosition.y;
		if (Vector3.Distance(base.transform.localPosition, zero) > 6f)
		{
			outOfBounds = true;
			resetPosition(false);
		}
	}

	private void resetPosition(bool hitTarget)
	{
		if (rb != null)
		{
			rb.velocity = Vector3.zero;
		}
		CoroutineRunner.Start(resetBeachBall(hitTarget), this, "ResetOutOfBoundsBeachBall");
	}

	private IEnumerator resetBeachBall(bool hitGood)
	{
		showRenderers(false);
		if (hitGood)
		{
			if (BurstEffect != null)
			{
				BurstEffect.transform.position = base.transform.position;
				BurstEffect.Play();
			}
		}
		else if (outOfBoundsEffect != null)
		{
			outOfBoundsEffect.Play();
		}
		yield return new WaitForSeconds(BurstEffect.main.duration);
		if (!base.gameObject.IsDestroyed())
		{
			base.transform.position = originPosition;
			EnableCollider(true);
			showRenderers(true);
			outOfBounds = false;
		}
	}

	public void EnableCollider(bool enable)
	{
		GetComponent<Collider>().enabled = enable;
	}

	private void showRenderers(bool hide)
	{
		Renderer[] renderersToHide = RenderersToHide;
		foreach (Renderer renderer in renderersToHide)
		{
			renderer.enabled = hide;
		}
	}

	private void setOriginPosition()
	{
		originPosition = base.transform.position;
	}
}
