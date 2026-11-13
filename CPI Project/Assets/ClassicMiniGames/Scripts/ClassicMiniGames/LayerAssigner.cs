using UnityEngine;

public class LayerAssigner : MonoBehaviour
{
	public int Layer;

	private void Awake()
	{
		Renderer component = base.gameObject.GetComponent<Renderer>();
		if (component != null)
		{
			component.sortingOrder = Layer;
		}
	}
}
