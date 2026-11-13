using Disney.LaunchPadFramework;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	[RequireComponent(typeof(Renderer))]
	public class MaterialLoader : MonoBehaviour
	{
		public MaterialContentKey Material;

		private Renderer rendererComponent;

		private void Awake()
		{
			rendererComponent = GetComponent<Renderer>();
			Content.LoadAsync(onMaterialLoaded, Material);
		}

		private void onMaterialLoaded(string key, Material material)
		{
			if (material != null)
			{
				rendererComponent.material = material;
			}
			else
			{
				Log.LogError(this, "Loaded a null material for MaterialContentKey: " + Material.Key);
			}
		}
	}
}
