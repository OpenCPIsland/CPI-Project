using System;
using UnityEngine;

public class BeachBallTarget : MonoBehaviour
{
	[Serializable]
	public class TargetDefinition
	{
		public BeachBallTargetController.TargetType type;

		public BeachBallTargetController.TargetFace face;

		public int pointValue;

		public int blockerLevel;
	}

	public delegate void HitTarget(TargetDefinition definition, BeachBall ball, BeachBallTarget target);

	private Vector3 originPosition;

	public GameObject[] targetFaces;

	public ScorePopUp scorePopUp;

	public ParticleSystem shockwaveEffect;

	public ParticleSystem hitEffect;

	public float ExplosiveForce = 50f;

	public float ExplsionRadius = 5f;

	public TargetDefinition definition;

	public Vector3 OriginPosition
	{
		get
		{
			return originPosition;
		}
	}

	public ScorePopUp ScorePopUp
	{
		get
		{
			return scorePopUp;
		}
	}

	public static event HitTarget OnTargetHit;

	private void Awake()
	{
		originPosition = base.transform.localPosition;
	}

	public void SetDefintion(BeachBallTargetController.TargetDefinition def)
	{
		definition.type = def.type;
		definition.face = def.face;
		definition.pointValue = def.pointValue;
		definition.blockerLevel = def.blockerLevel;
		showTargetFace();
	}

	private void showTargetFace()
	{
		GameObject[] array = targetFaces;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		switch (definition.face)
		{
		case BeachBallTargetController.TargetFace.Low:
			if (targetFaces[0] != null)
			{
				targetFaces[0].SetActive(true);
			}
			break;
		case BeachBallTargetController.TargetFace.Medium:
			if (targetFaces[1] != null)
			{
				targetFaces[1].SetActive(true);
			}
			break;
		case BeachBallTargetController.TargetFace.High:
			if (targetFaces[2] != null)
			{
				targetFaces[2].SetActive(true);
			}
			break;
		case BeachBallTargetController.TargetFace.Negative:
			if (targetFaces[3] != null)
			{
				targetFaces[3].SetActive(true);
			}
			break;
		case BeachBallTargetController.TargetFace.Shield:
			if (targetFaces[4] != null)
			{
				targetFaces[4].SetActive(true);
			}
			break;
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		BeachBall component = other.gameObject.GetComponent<BeachBall>();
		BeachBallTarget.OnTargetHit(definition, component, this);
		if (!(component != null))
		{
			return;
		}
		switch (definition.type)
		{
		case BeachBallTargetController.TargetType.Positive:
			component.EnableCollider(false);
			onPositiveHit();
			break;
		case BeachBallTargetController.TargetType.Negative:
			component.EnableCollider(false);
			onNegativeHit();
			break;
		case BeachBallTargetController.TargetType.Shielded:
			if (definition.blockerLevel > 0)
			{
				onShieldHit();
			}
			break;
		}
	}

	private void onPositiveHit()
	{
		if (hitEffect != null)
		{
			hitEffect.Play();
		}
	}

	private void onNegativeHit()
	{
		Vector3 position = base.transform.position;
		Collider[] array = Physics.OverlapSphere(position, ExplsionRadius);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (collider.GetComponent<BeachBall>() != null)
			{
				Rigidbody component = collider.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.AddExplosionForce(ExplosiveForce, position, ExplsionRadius, 2f);
				}
			}
		}
		if (shockwaveEffect != null)
		{
			shockwaveEffect.Play();
		}
	}

	private void onShieldHit()
	{
		definition.blockerLevel--;
		if (definition.blockerLevel == 0)
		{
			definition.type = BeachBallTargetController.TargetType.Positive;
			definition.face = BeachBallTargetController.TargetFace.High;
			showTargetFace();
		}
	}

	public void EnableColliders(bool enable)
	{
		GetComponent<Collider>().enabled = enable;
	}
}
