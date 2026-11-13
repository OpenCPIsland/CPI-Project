using UnityEngine;

namespace ClubPenguin.WorldEditor.Optimization
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(MeshFilter), typeof(Renderer))]
	public class GameObjectData : MonoBehaviour
	{
		public const string WORLD_OBJECT_SHADER_NAME = "CpRemix/World/WorldObject";

		public const string TERRAIN_3_SHADER_NAME = "CpRemix/World/Terrain 3 Tile";

		public const string TERRAIN_2_SHADER_NAME = "CpRemix/World/Terrain 2 Tile";

		public const string TERRAIN_1_SHADER_NAME = "CpRemix/World/Terrain 1 Tile";

		public const string SNOW_SHADER_NAME = "CpRemix/World/Snow Ramp";

		public const string DIFFUSE_TEXTURE_NAME = "_Diffuse";

		public const string MAIN_TEXTURE_NAME = "_MainTex";

		public const string OSCILLATION_SHADER_NAME = "CpRemix/World/Wave Osc Unlit (Vertex Alpha)";

		public const string OSCILLATION_DEPTH_SHADER_NAME = "CpRemix/World/Wave Osc Depth (Vertex Alpha)";

		[Range(0f, 2f)]
		public float LightmapWeight = 1f;

		[Header("Should not be edited by user")]
		public float MinLightmapDistance = float.MaxValue;

		public Mesh OriginalMesh;

		public bool[] VertexVisibilities;

		public bool[] TriangleVisibilities;

		private MeshFilter meshFilter;

		public MeshFilter MeshFilter
		{
			get
			{
				if (meshFilter == null)
				{
					meshFilter = GetComponent<MeshFilter>();
				}
				return meshFilter;
			}
		}

		public static bool RendererUsesCullableShader(Renderer renderer)
		{
			string name = renderer.sharedMaterial.shader.name;
			int result;
			switch (name)
			{
			default:
				result = ((name == "CpRemix/World/Wave Osc Depth (Vertex Alpha)") ? 1 : 0);
				break;
			case "CpRemix/World/WorldObject":
			case "CpRemix/World/Terrain 3 Tile":
			case "CpRemix/World/Terrain 2 Tile":
			case "CpRemix/World/Terrain 1 Tile":
			case "CpRemix/World/Snow Ramp":
			case "CpRemix/World/Wave Osc Unlit (Vertex Alpha)":
				result = 1;
				break;
			}
			return (byte)result != 0;
		}

		public static bool RendererUsesSpriteSheetShader(Renderer renderer)
		{
			return renderer.sharedMaterial.shader.name == "CpRemix/World/WorldObject";
		}

		public void OnValidate()
		{
			/*if (MeshFilter.sharedMesh == null)
			{
				Debug.LogError("Mesh filter for '" + base.gameObject.name + "' is missing shared mesh!", this);
			}
			if (!base.gameObject.isStatic)
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
