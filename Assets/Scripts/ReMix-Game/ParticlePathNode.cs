using ClubPenguin;
using UnityEngine;

public class ParticlePathNode : MonoBehaviour
{
	private GameObject particles;

	public void CreateParticles(GameObject particleEffectPrefab, Transform nextNode)
	{
		if (nextNode == null)
		{
			return;
		}
		Vector3 position = base.transform.position;
		Vector3 position2 = nextNode.position;
		float num = Vector3.Distance(position, position2);
		particles = Object.Instantiate(particleEffectPrefab);
		particles.transform.SetParent(base.transform, false);
		base.transform.rotation = Quaternion.LookRotation(position2 - position);
		ParticleSystem[] componentsInChildren = particles.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			if (particleSystem.transform.localRotation.eulerAngles.x == 0f)
			{
				particleSystem.SetStartLifeTimeConstant(num / particleSystem.main.startSpeed.constant * 1.1f);
				continue;
			}
			particleSystem.transform.localScale = new Vector3(particleSystem.transform.localScale.x, num * 0.37f, particleSystem.transform.localScale.z);
			particleSystem.transform.localPosition = new Vector3(particleSystem.transform.localPosition.x, particleSystem.transform.localPosition.y, num * 0.185f);
		}
	}
}
