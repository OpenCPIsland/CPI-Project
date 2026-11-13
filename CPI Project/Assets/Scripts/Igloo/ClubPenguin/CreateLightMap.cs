#define UNITY_ASSERTIONS
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin
{
	public class CreateLightMap : MonoBehaviour
	{
		private const float SHADOW_BIAS = 0.001f;

		public Shader blurShader;

		public Shader lightmapCreationShader;

		[Header("Renderer that will receive the lightmap")]
		public MeshRenderer target;

		[Range(0f, 2f)]
		public int numberBlurPasses = 1;

		[Header("Lightmap size (POT)")]
		public int textureSize = 256;

		private RenderTexture renderTex;

		private Mesh mesh;

		private static Material blurMaterial = null;

		private static Material lightmapCreationMaterial = null;

		private static int lightmapFieldID;

		private static int shadowCamFieldID;

		private static int shadowMapFieldID;

		private static int brightnessFieldID;

		private static int biasFieldID;

		private static int offsetsFieldID;

		private static int platformFieldID;

		private static int platformDepthFieldID;

		private static Vector4 platformRotationValues;

		private static Vector2 platformDepthValues;

		private void Awake()
		{
			platformRotationValues = new Vector4(1f, 0f, 0f, -1f);
			platformDepthValues = new Vector2(1f, -1f);
			Assert.IsNotNull(blurShader);
			Assert.IsFalse(base.gameObject.isStatic, string.Format("GameObject {0} cannot be marked as static because it is used to create the dynamic lightmap.", base.name));
			storeFieldIDHashes();
			if (!blurMaterial)
			{
				blurMaterial = new Material(blurShader);
				blurMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			if (!lightmapCreationMaterial)
			{
				lightmapCreationMaterial = new Material(lightmapCreationShader);
				lightmapCreationMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			Assert.IsNotNull(target);
			Assert.IsNotNull(target.GetComponent<MeshFilter>());
			Assert.IsTrue(target.sharedMaterial.HasProperty(lightmapFieldID), "Target material does not support dynamic lightmap");
			mesh = target.GetComponent<MeshFilter>().sharedMesh;
			renderTex = createRenderTexture();
		}

		public void Clear(Color clearColor)
		{
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = renderTex;
			GL.Clear(false, true, clearColor);
			RenderTexture.active = active;
		}

		public void RenderSinglePass(Matrix4x4 vp, RenderTexture depthMap, int currentPass, int numPasses)
		{
			if (currentPass == 0)
			{
				Graphics.SetRenderTarget(renderTex);
				GL.Clear(true, true, Color.black);
			}
			else
			{
				Graphics.SetRenderTarget(renderTex);
			}
			float brightness = 1f / (float)numPasses;
			setCreationUniforms(vp, depthMap, brightness);
			renderSingleMesh(mesh, lightmapCreationMaterial);
			if (currentPass + 1 >= numPasses)
			{
				performBlur(renderTex);
			}
		}

		private RenderTexture createRenderTexture()
		{
			RenderTexture renderTexture = target.sharedMaterial.GetTexture(lightmapFieldID) as RenderTexture;
			if (!renderTexture)
			{
				renderTexture = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.R8);
				Material[] sharedMaterials = target.sharedMaterials;
				for (int i = 0; i < sharedMaterials.Length; i++)
				{
					sharedMaterials[i].SetTexture(lightmapFieldID, renderTexture);
				}
			}
			return renderTexture;
		}

		private void setCreationUniforms(Matrix4x4 vp, RenderTexture depthMap, float brightness)
		{
			Matrix4x4 value = vp * target.transform.localToWorldMatrix;
			lightmapCreationMaterial.SetMatrix(shadowCamFieldID, value);
			lightmapCreationMaterial.SetTexture(shadowMapFieldID, depthMap);
			lightmapCreationMaterial.SetFloat(brightnessFieldID, brightness);
			lightmapCreationMaterial.SetFloat(biasFieldID, 0.001f);
			lightmapCreationMaterial.SetVector(platformFieldID, platformRotationValues);
			lightmapCreationMaterial.SetVector(platformDepthFieldID, platformDepthValues);
		}

		private void renderSingleMesh(Mesh mesh, Material mat)
		{
			GL.PushMatrix();
			GL.Viewport(new Rect(0f, 0f, renderTex.width, renderTex.height));
			GL.LoadProjectionMatrix(Matrix4x4.identity);
			GL.modelview = Matrix4x4.identity;
			mat.SetPass(0);
			Graphics.DrawMeshNow(mesh, Matrix4x4.identity);
			GL.PopMatrix();
		}

		private void performBlur(RenderTexture finalRenderTex)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(finalRenderTex.width, finalRenderTex.height, 0, finalRenderTex.format);
			for (int i = 0; i < numberBlurPasses; i++)
			{
				float num = (float)(i + 1) / (float)textureSize;
				temporary.DiscardContents();
				blurMaterial.SetVector(offsetsFieldID, new Vector4(num, 0f, 0f, 0f));
				Graphics.Blit(finalRenderTex, temporary, blurMaterial);
				finalRenderTex.DiscardContents();
				blurMaterial.SetVector(offsetsFieldID, new Vector4(0f, num, 0f, 0f));
				Graphics.Blit(temporary, finalRenderTex, blurMaterial);
			}
			RenderTexture.ReleaseTemporary(temporary);
		}

		private static void storeFieldIDHashes()
		{
			lightmapFieldID = Shader.PropertyToID("_Lightmap");
			shadowCamFieldID = Shader.PropertyToID("_ShadowCamVP");
			shadowMapFieldID = Shader.PropertyToID("_ShadowMap");
			brightnessFieldID = Shader.PropertyToID("_Brightness");
			biasFieldID = Shader.PropertyToID("_Bias");
			offsetsFieldID = Shader.PropertyToID("offsets");
			platformFieldID = Shader.PropertyToID("_PlatformRotationValues");
			platformDepthFieldID = Shader.PropertyToID("_PlatformDepthValues");
		}
	}
}
