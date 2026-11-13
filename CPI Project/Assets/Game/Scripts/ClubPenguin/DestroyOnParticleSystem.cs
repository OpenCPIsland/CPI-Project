using UnityEngine;

namespace ClubPenguin
{
	public class DestroyOnParticleSystem : MonoBehaviour
	{
		private void OnEnable()
		{
			float num = -1f;
			ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].main.duration + componentsInChildren[i].main.startLifetime.constant > num)
				{
					num = componentsInChildren[i].main.duration + componentsInChildren[i].main.startLifetime.constant;
				}
			}
			if (num != -1f)
			{
				Object.Destroy(base.gameObject, Mathf.Clamp(num, 0f, float.PositiveInfinity));
			}
		}
	}
}
