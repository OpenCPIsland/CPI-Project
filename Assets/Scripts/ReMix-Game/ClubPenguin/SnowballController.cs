using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(ResourceCleaner))]
	public class SnowballController : MonoBehaviour
	{
		private ParticleSystem trailFX;

		private Rigidbody rbody;

		private TrailRenderer tRenderer;

		private MeshRenderer mRenderer;

		private float unspawnDelay;

		private bool readyForUnspawn;

		public long OwnerId
		{
			get;
			private set;
		}

		private void Awake()
		{
			trailFX = GetComponent<ParticleSystem>();
			rbody = GetComponent<Rigidbody>();
			tRenderer = GetComponent<TrailRenderer>();
			mRenderer = GetComponent<MeshRenderer>();
			readyForUnspawn = false;
		}

		public void Update()
		{
			if (!readyForUnspawn)
			{
				return;
			}
			unspawnDelay -= Time.deltaTime;
			if (unspawnDelay < 0f)
			{
				if (tRenderer != null)
				{
					tRenderer.enabled = false;
				}
				readyForUnspawn = false;
				SnowballManager.Instance.UnspawnSnowball(this);
			}
		}

		public void OnAttached()
		{
			rbody.isKinematic = true;
			rbody.detectCollisions = false;
			mRenderer.enabled = true;
			readyForUnspawn = false;
			OwnerId = 0L;
			if (tRenderer != null)
			{
				tRenderer.Clear();
				tRenderer.enabled = false;
			}
		}

		public void OnDetached(long snowballOwner, ref Vector3 velocity, float chargeTime, float trailAlpha)
		{
			if (trailFX != null)
			{
				trailFX.Play();
			}
			unspawnDelay = -1f;
			if (tRenderer != null)
			{
				tRenderer.Clear();
				Color color = tRenderer.material.color;
				color.a = trailAlpha;
				tRenderer.material.color = color;
				tRenderer.enabled = true;
			}
			rbody.isKinematic = false;
			rbody.detectCollisions = true;
			rbody.AddForce(velocity, ForceMode.VelocityChange);
			OwnerId = snowballOwner;
		}

		private void OnCollisionEnter(Collision collision)
		{
			SnowballManager.Instance.OnSnowballCollision(collision);
			rbody.isKinematic = true;
			rbody.detectCollisions = false;
			mRenderer.enabled = false;
			if (trailFX != null)
			{
				trailFX.Stop();
			}
			readyForUnspawn = true;
			if (tRenderer != null)
			{
				unspawnDelay = tRenderer.time;
			}
			else
			{
				unspawnDelay = 0f;
			}
		}
	}
}
