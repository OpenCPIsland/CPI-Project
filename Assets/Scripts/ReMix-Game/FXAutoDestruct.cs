using UnityEngine;

public class FXAutoDestruct : MonoBehaviour
{
	public bool AutoDestructWhenParticlesEnd = true;

	public bool AutoDestructAfterSeconds = false;

	public float DestroyAfterSeconds = 0f;

	private void Start()
	{
		if (AutoDestructAfterSeconds)
		{
			float t = Mathf.Clamp(DestroyAfterSeconds, 0f, float.MaxValue);
			Object.Destroy(base.gameObject, t);
		}
	}

	private void Update()
	{
		if (AutoDestructWhenParticlesEnd && !base.gameObject.GetComponent<ParticleSystem>().IsAlive(true))
		{
			Object.Destroy(base.gameObject);
		}
	}
}
