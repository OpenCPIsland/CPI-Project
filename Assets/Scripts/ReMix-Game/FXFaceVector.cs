using UnityEngine;

public class FXFaceVector : MonoBehaviour
{
	public float DistancePerFrameForFX = 0.35f;

	private GameObject player;

	private Vector3 CurrentPos;

	private Vector3 OldPos;

	private ParticleSystem ParticleSys;

	private bool InitializedOK = true;

	private void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		if (player == null)
		{
			InitializedOK = false;
		}
		ParticleSys = base.gameObject.GetComponent<ParticleSystem>();
		if (ParticleSys == null)
		{
			InitializedOK = false;
		}
		OldPos = player.transform.position;
	}

	private void Update()
	{
		if (InitializedOK)
		{
			CurrentPos = player.transform.position;
			float magnitude = (CurrentPos - OldPos).magnitude;
			if (magnitude >= DistancePerFrameForFX && ParticleSys.isStopped)
			{
				ParticleSys.Play();
			}
			else if (magnitude < DistancePerFrameForFX && ParticleSys.isPlaying)
			{
				ParticleSys.Stop();
			}
			base.gameObject.transform.forward = -(CurrentPos - OldPos);
			OldPos = CurrentPos;
		}
	}
}
