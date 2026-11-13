using UnityEngine;

public class TriggerActivatedParticles : MonoBehaviour
{
	public bool EnableOnEnter = true;

	public bool DisableOnExit = true;

	public ParticleSystem ParticlesToToggle;

	private GameObject MainCamera;

	private ParticleSystem airParticles;

	private void Start()
	{
		MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		airParticles = MainCamera.GetComponentInChildren<ParticleSystem>();
		if (ParticlesToToggle.isPlaying)
		{
			ParticlesToToggle.Stop();
		}
	}

	private void OnTriggerEnter(Collider penguin)
	{
		if (penguin.CompareTag("Player") && EnableOnEnter)
		{
			ParticlesToToggle.Play();
			airParticles.Stop();
		}
	}

	private void OnTriggerExit(Collider penguin)
	{
		if (penguin.CompareTag("Player") && DisableOnExit)
		{
			ParticlesToToggle.Stop();
			airParticles.Play();
		}
	}
}
