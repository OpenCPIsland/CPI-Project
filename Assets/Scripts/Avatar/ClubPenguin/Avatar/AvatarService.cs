#define ENABLE_PROFILER
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Foundation.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace ClubPenguin.Avatar
{
	public class AvatarService
	{
		public class Request
		{
			public Mesh Mesh;

			public RenderTexture Atlas;

			public bool Finished;
		}

		public const string COMBINED_MESH_SHADER_NAME = "CpRemix/Combined Avatar";

		public const string COMBINED_MESH_TRANSPARENT_SHADER_NAME = "CpRemix/Combined Avatar Alpha";

		public const string BODY_PREVIEW_SHADER_NAME = "CpRemix/Avatar Body Preview";

		public const string BODY_BAKE_SHADER_NAME = "CpRemix/Avatar Body Bake";

		public const string EQUIPMENT_PREVIEW_SHADER_NAME = "CpRemix/Equipment Preview";

		public const string EQUIPMENT_BAKE_SHADER_NAME = "CpRemix/Equipment Bake";

		public const string EQUIPMENT_SCREENSHOT_SHADER_NAME = "CpRemix/Equipment Screenshot";

		public const string GPU_COMBINED_MESH_SHADER_NAME = "CpRemix/GPU Combined Avatar";

		public const string GPU_COMBINED_TRANSPARENT_SHADER_NAME = "CpRemix/GPU Combined Avatar Alpha";

		public static Shader CombinedMeshShader;

		public static Shader BodyBakeShader;

		public static Shader BodyPreviewShader;

		public static Shader EquipmentBakeShader;

		public static Shader EquipmentPreviewShader;

		public static Shader EquipmentScreenshotShader;

		public static Shader GpuCombinedMeshShader;

		public static readonly Color DefaultBodyColor = new Color32(25, 210, 214, byte.MaxValue);

		protected readonly AvatarDefinition[] definitions;

		private readonly Queue<KeyValuePair<Request, IEnumerator>> queue = new Queue<KeyValuePair<Request, IEnumerator>>();

		[RuntimeInitializeOnLoadMethod]
		public static void Initialize()
		{
			CombinedMeshShader = Shader.Find("CpRemix/Combined Avatar");
			BodyBakeShader = Shader.Find("CpRemix/Avatar Body Bake");
			BodyPreviewShader = Shader.Find("CpRemix/Avatar Body Preview");
			EquipmentBakeShader = Shader.Find("CpRemix/Equipment Bake");
			EquipmentPreviewShader = Shader.Find("CpRemix/Equipment Preview");
			EquipmentScreenshotShader = Shader.Find("CpRemix/Equipment Screenshot");
			GpuCombinedMeshShader = Shader.Find("CpRemix/GPU Combined Avatar");
		}

		public AvatarService(AvatarDefinition[] definitions)
		{
			Service.Get<FibreService>().AddFibre("AvatarService:New", 5f, fibre);
			this.definitions = definitions;
		}

		public Request CombineParts(AvatarDefinition definition, BodyColorMaterialProperties bodycolor, List<ViewPart> parts, int maxAtlasDimension)
		{
			Request request = new Request();
			IEnumerator value = partsCombiner(request, definition, bodycolor ?? definition.BodyColor, parts, maxAtlasDimension);
			queue.Enqueue(new KeyValuePair<Request, IEnumerator>(request, value));
			return request;
		}

		public void ForceClearQueue()
		{
			queue.Clear();
		}

		public void CancelAllRequests()
		{
			throw new NotImplementedException("CancelAllRequests has not been implemented yet");
		}

		public AvatarDefinition GetDefinitionByName(string name)
		{
			for (int i = 0; i < definitions.Length; i++)
			{
				if (definitions[i].name == name)
				{
					return definitions[i];
				}
			}
			return null;
		}

		private static IEnumerator partsCombiner(Request request, AvatarDefinition definition, BodyColorMaterialProperties bodycolor, List<ViewPart> parts, int maxAtlasDimension)
		{
			int meshCount = parts.Count;
			Mesh[] meshes = new Mesh[meshCount];
			int[][] meshBoneIndexes = new int[meshCount][];
			for (int i = 0; i < meshCount; i++)
			{
				string[] boneNames = parts[i].GetBoneNames();
				meshes[i] = parts[i].GetMesh();
				meshBoneIndexes[i] = new int[boneNames.Length];
				for (int j = 0; j < boneNames.Length; j++)
				{
					meshBoneIndexes[i][j] = definition.BoneIndexLookup[boneNames[j]];
				}
			}
			request.Mesh = Combine.CombineSkinnedMeshes(meshes, meshBoneIndexes, definition.BindPose);
			yield return null;
			int curSize;
			Rect[] atlasUVOffsets = Combine.CalculateAtlasLayout(parts, out curSize);
			yield return null;
			int renderTextureSize = Mathf.Min(Mathf.ClosestPowerOfTwo(curSize), maxAtlasDimension);
			request.Atlas = new RenderTexture(renderTextureSize, renderTextureSize, 0, RenderTextureFormat.ARGB32);
			request.Atlas.isPowerOfTwo = true;
			request.Atlas.filterMode = FilterMode.Bilinear;
			request.Atlas.useMipMap = false;
			Combine.BakeTexture(parts, atlasUVOffsets, bodycolor, request.Atlas);
			yield return null;
			Combine.ApplyAtlasUV(meshes, request.Mesh, atlasUVOffsets);
		}

		private IEnumerator<bool> fibre()
		{
			while (true)
			{
				if (queue.Count > 0)
				{
					KeyValuePair<Request, IEnumerator> current = queue.Dequeue();
					int step = 0;
					bool busy;
					do
					{
						try
						{
							Profiler.BeginSample("ApplyCombinedMesh_" + step++);
							busy = current.Value.MoveNext();
							Profiler.EndSample();
						}
						catch (Exception ex)
						{
							Log.LogException(this, ex);
							ComponentExtensions.DestroyIfInstance(current.Key.Mesh);
							ComponentExtensions.DestroyIfInstance(current.Key.Atlas);
							current.Key.Mesh = null;
							current.Key.Atlas = null;
							break;
						}
						yield return true;
					}
					while (busy);
					current.Key.Finished = true;
				}
				else
				{
					yield return false;
				}
			}
		}
	}
}
