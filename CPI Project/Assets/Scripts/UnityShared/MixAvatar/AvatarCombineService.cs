using System;
using System.Collections.Generic;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using Foundation.Unity;

namespace MixAvatar
{
	public class AvatarCombineService
	{
		public const float TimeSliceMs = 10f;

		public int AtlasMaxDimension = 4096;

		private readonly Queue<CombineRequest> combineRequests = new Queue<CombineRequest>();

		public AvatarCombineService()
		{
			Service.Get<FibreService>().AddFibre("AvatarCombineService", 10f, combineFibre);
		}

		public void Combine(CombineRequest request)
		{
			combineRequests.Enqueue(request);
		}

		public void ClearCombineQueue()
		{
			combineRequests.Clear();
		}

		private IEnumerator<bool> combineFibre()
		{
			while (true)
			{
				if (combineRequests.Count > 0)
				{
					CombineRequest request = combineRequests.Dequeue();
					if (request.TargetBonesGameObject != null && request.TargetMeshGameObject != null)
					{
						try
						{
							request.CombinedSkinMeshRenderer = AvatarUtils.CombineMeshToTargetGameObject(request.MeshCombineData, request.TargetMeshGameObject, request.TargetBonesGameObject, out request.NumVerticesPerSubMesh, out request.SubMeshes, out request.RootBone);
						}
						catch (Exception ex)
						{
							Log.LogException(this, ex);
						}
						yield return true;
						int renderTextureSize = calcOffsetsAndSize(request);
						yield return true;
                        RenderTexture atlasRenderTexture = new RenderTexture(renderTextureSize, renderTextureSize, 0, RenderTextureFormat.ARGB32)
                        {
						#if UNITY_ANDROID || UNITY_IOS
                            antiAliasing = 4,
						#else
							antiAliasing = 8,
						#endif
                            isPowerOfTwo = true,
                            filterMode = FilterMode.Trilinear,
                            anisoLevel = 16,
                            useMipMap = (request.UseMipMaps == MipMapping.Off)
                        };
                        yield return true;
						RenderTexture prevRt = RenderTexture.active;
						RenderTexture.active = atlasRenderTexture;
						GL.Clear(true, true, new Color32(0, 0, 0, 0));
						for (int j = 0; j < request.SubMeshes.Count; j++)
						{
							RenderTexture.active = prevRt;
							yield return true;
							RenderTexture.active = atlasRenderTexture;
							atlasRenderTexture.DiscardContents();
							try
							{
								request.TextureBaker.BakeEquipmentMaterialToAtlas(request.SubMeshes[j].sharedMaterial, request.AtlasData.AtlasUVOffsets[j], atlasRenderTexture);
							}
							catch (Exception ex2)
							{
								Log.LogException(this, ex2);
							}
						}
						request.TextureBaker.UnloadBlitMaterials();
						RenderTexture.active = prevRt;
						request.AtlasData.AtlasMaterial.mainTexture = atlasRenderTexture;
						yield return true;
						try
						{
							MeshUtils.OffsetUVsToAtlas(request.CombinedSkinMeshRenderer, request.NumVerticesPerSubMesh, request.AtlasData.AtlasUVOffsets);
							request.CombinedSkinMeshRenderer.sharedMaterial = request.AtlasData.AtlasMaterial;
						}
						catch (Exception ex3)
						{
							Log.LogException(this, ex3);
						}
					}
					request.Finished = true;
				}
				else
				{
					yield return false;
				}
			}
		}

		private int calcOffsetsAndSize(CombineRequest combineRequest)
		{
			int num = 0;
			try
			{
				Rect[] array = new Rect[combineRequest.SubMeshes.Count];
				for (int i = 0; i < combineRequest.SubMeshes.Count; i++)
				{
					int largestTextureDimensionInMaterial = combineRequest.TextureBaker.GetLargestTextureDimensionInMaterial(combineRequest.SubMeshes[i].sharedMaterial);
					array[i] = new Rect(0f, 0f, largestTextureDimensionInMaterial, largestTextureDimensionInMaterial);
				}
				num = RectanglePack.Pack(array, 0);
				for (int j = 0; j < array.Length; j++)
				{
					Rect rect = new Rect(array[j].x / (float)num, array[j].y / (float)num, array[j].width / (float)num, array[j].height / (float)num);
					array[j] = rect;
				}
				Material atlasMaterial = new Material(combineRequest.CombinedMeshShader);
				atlasMaterial.enableInstancing = true;
				combineRequest.AtlasData = new CombinedMeshAtlasData(array, atlasMaterial);
			}
			catch (Exception ex)
			{
				Log.LogException(this, ex);
			}
			return Mathf.Min(Mathf.ClosestPowerOfTwo(num), AtlasMaxDimension);
		}
	}
}
