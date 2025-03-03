#define ENABLE_PROFILER
using Disney.LaunchPadFramework;
using Foundation.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace ClubPenguin.DCE
{
	public class DceService
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

		public static readonly Shader CombinedMeshShader = Shader.Find("CpRemix/Combined Avatar");

		public static readonly Shader BodyBakeShader = Shader.Find("CpRemix/Avatar Body Bake");

		public static readonly Shader BodyPreviewShader = Shader.Find("CpRemix/Avatar Body Preview");

		public static readonly Shader EquipmentBakeShader = Shader.Find("CpRemix/Equipment Bake");

		public static readonly Shader EquipmentPreviewShader = Shader.Find("CpRemix/Equipment Preview");

		public static readonly Shader EquipmentScreenshotShader = Shader.Find("CpRemix/Equipment Screenshot");

		public static readonly Shader GpuCombinedMeshShader = Shader.Find("CpRemix/GPU Combined Avatar");

		private readonly Queue<KeyValuePair<Request, IEnumerator>> queue = new Queue<KeyValuePair<Request, IEnumerator>>();

		public Request CombineParts(List<ViewPart> parts, int maxAtlasDimension)
		{
			Request request = new Request();
			IEnumerator value = partsCombiner(request, parts, maxAtlasDimension);
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

		private static IEnumerator partsCombiner(Request request, List<ViewPart> parts, int maxAtlasDimension)
		{
			yield return null;
			int curSize;
			Combine.CalculateAtlasLayout(parts, out curSize);
			yield return null;
			int renderTextureSize = Mathf.Min(Mathf.ClosestPowerOfTwo(curSize), maxAtlasDimension);
			request.Atlas = new RenderTexture(renderTextureSize, renderTextureSize, 0, RenderTextureFormat.ARGB32);
			request.Atlas.isPowerOfTwo = true;
			request.Atlas.filterMode = FilterMode.Bilinear;
			request.Atlas.useMipMap = false;
			yield return null;
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
