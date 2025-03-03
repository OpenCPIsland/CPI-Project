using UnityEngine;

namespace ClubPenguin.WorldEditor.Optimization
{
	[RequireComponent(typeof(GameObjectData), typeof(Renderer))]
	[DisallowMultipleComponent]
	public class TextureAtlasData : MonoBehaviour
	{
		[Header("Should not be edited by user")]
		public Material OriginalMaterial;

		private Renderer render;

		private Texture2D diffuseTexture;

		public Renderer Renderer
		{
			get
			{
				if (render == null)
				{
					render = GetComponent<Renderer>();
				}
				return render;
			}
		}

		public Texture2D DiffuseTexture
		{
			get
			{
				if (diffuseTexture == null)
				{
					diffuseTexture = (Texture2D)Renderer.sharedMaterial.GetTexture("_Diffuse");
				}
				return diffuseTexture;
			}
		}

		public void OnValidate()
		{
			if (Renderer.sharedMaterial == null)
			{
				Debug.LogError("Renderer for '" + base.gameObject.name + "' is missing shared material!", this);
			}
			if (Renderer.sharedMaterials.Length > 1)
			{
				Debug.LogError("Renderer for '" + base.gameObject.name + "' has more than one shared material!", this);
			}
			if (!GameObjectData.RendererUsesSpriteSheetShader(Renderer))
			{
				Debug.LogError("Renderer for '" + base.gameObject.name + "' is not a spritesheet renderer! Remove the TextureAtlasData component or change the shader.", this);
			}
			if (DiffuseTexture == null)
			{
				Debug.LogError("Renderer material for '" + base.gameObject.name + "' is missing diffuse texture!", this);
			}
			else if (DiffuseTexture.width != DiffuseTexture.height)
			{
				Debug.LogError("Diffuse texture '" + DiffuseTexture.name + "' for '" + base.gameObject.name + "' is not square!", this);
			}
		/*	if (!base.gameObject.isStatic)
			{
				Debug.LogError("Only static game objects can be optimized!", this);
			}*/
		}

		public void Awake()
		{
			base.hideFlags = HideFlags.DontSaveInBuild;
		}
	}
}
