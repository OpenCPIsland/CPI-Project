using UnityEngine;

public class ParticlePath : MonoBehaviour
{
	public GameObject ParticleEffectPrefab;

	private ParticlePathNode[] nodes;

	private void Awake()
	{
		nodes = GetComponentsInChildren<ParticlePathNode>();
	}

	private void Start()
	{
		InitNodes();
	}

	private void OnDrawGizmos()
	{
		ParticlePathNode[] componentsInChildren = GetComponentsInChildren<ParticlePathNode>();
		if (componentsInChildren.Length != 0)
		{
			renameAllNodes();
		}
	}

	private void renameAllNodes()
	{
		ParticlePathNode[] componentsInChildren = GetComponentsInChildren<ParticlePathNode>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			string text = "Node " + i;
			if (!componentsInChildren[i].name.Equals(text))
			{
				componentsInChildren[i].name = text;
			}
		}
	}

	private void InitNodes()
	{
		for (int i = 0; i < nodes.Length - 1; i++)
		{
			nodes[i].CreateParticles(ParticleEffectPrefab, nodes[i + 1].transform);
		}
	}
}
