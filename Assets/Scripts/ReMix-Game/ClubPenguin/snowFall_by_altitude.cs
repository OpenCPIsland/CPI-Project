using UnityEngine;

namespace ClubPenguin
{
	public class snowFall_by_altitude : MonoBehaviour
	{
		[Range(0f, 100f)]
		public float SnowStartHeight;

		[Range(0f, 10f)]
		public float SnowFallMultiplier;

		[Range(0f, 100f)]
		public int MaxSnowFall;

		public GameObject CameraToFollow;

		[Range(0f, 100f)]
		public float FollowSpeed;

		[Range(0f, 100f)]
		public float SnowYOffset;

		public float SnowDepthOffset;

		public float SnowSideOffset;

		[Range(0f, 100f)]
		public float SnowDownBuffer;

		private ParticleSystem snowPS;

		private Vector3 oldCamPos;

		private void Awake()
		{
			snowPS = GetComponent<ParticleSystem>();
			oldCamPos = CameraToFollow.transform.position;
		}

		private void Update()
		{
			Vector3 position = CameraToFollow.transform.position;
			Vector3 position2 = snowPS.transform.position;
			Vector3 vector = position2;
			vector.z = position.z + SnowSideOffset;
			vector.y = position.y + SnowYOffset;
			if (position2.y > SnowStartHeight)
			{
				snowPS.Play();
			}
			if (position.y != oldCamPos.y)
			{
				float num = position2.y - position.y;
				if (num > SnowDownBuffer)
				{
					vector = Vector3.Lerp(position2, vector, FollowSpeed * Time.deltaTime);
				}
			}
			vector.x = position.x + SnowDepthOffset;
			base.transform.position = vector;
			if (position2.y >= SnowStartHeight && snowPS.GetEmissionRate() <= (float)MaxSnowFall)
			{
				snowPS.SetEmissionRate((vector.y - SnowStartHeight) * SnowFallMultiplier);
			}
			if (position2.y < SnowStartHeight)
			{
				snowPS.SetEmissionRate(0f);
			}
			if (position2.y > 40f)
			{
				SnowDepthOffset = 1f;
				snowPS.SetEmissionRate(MaxSnowFall);
			}
			if (position.y <= SnowStartHeight - SnowYOffset)
			{
				snowPS.Stop(true);
			}
			oldCamPos = position;
		}
	}
}
