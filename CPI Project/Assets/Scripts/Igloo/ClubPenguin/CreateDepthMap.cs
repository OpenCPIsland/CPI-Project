#define UNITY_ASSERTIONS
using Disney.LaunchPadFramework;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin
{
	[RequireComponent(typeof(Camera))]
	public class CreateDepthMap : MonoBehaviour
	{
		private const int TEXTURE_BIT_DEPTH = 16;

		public Shader depthOnlyShader;

		public Light directionalLight;

		public BoxCollider worldBoundingBox;

		[Header("Objects in the scene that need lightmaps")]
		public CreateLightMap[] lightMapTargets;

		[Header("Temporary depth map size (POT)")]
		public int textureSize = 2048;

		[Range(0f, 20f)]
		public float maxShadowAngle = 20f;

		public int numberPasses = 10;

		private Camera depthCam;

		private RenderTexture renderTex;

		private Vector3[] worldBoxVerts;

		private void Awake()
		{
			Assert.IsNotNull(lightMapTargets);
			Assert.IsNotNull(worldBoundingBox);
			Assert.IsNotNull(directionalLight);
			Assert.IsNotNull(depthOnlyShader);
			Assert.IsTrue(lightMapTargets.Length > 0);
			Assert.IsTrue(worldBoundingBox.center == Vector3.zero, "BoxCollider with non-zero origin is not supported!");
			depthCam = GetComponent<Camera>();
			depthCam.enabled = false;
			depthCam.SetReplacementShader(depthOnlyShader, "RenderType");
			worldBoxVerts = getOriginalWorldBounds();
		}

		private void OnDestroy()
		{
			if (depthCam != null)
			{
				depthCam.targetTexture = null;
			}
			if (renderTex != null)
			{
				renderTex.Release();
				Object.Destroy(renderTex);
			}
		}

		public void Clear(Color clearColor)
		{
			int num = lightMapTargets.Length;
			for (int i = 0; i < num; i++)
			{
				CreateLightMap createLightMap = lightMapTargets[i];
				createLightMap.Clear(clearColor);
			}
		}

		public void Render(bool destroyOnComplete)
		{
			if (depthCam == null)
			{
				Log.LogError(this, "The Depth Camera has been destroyed. Depth map not created.");
				return;
			}
			createRenderTexture();
			Quaternion localRotation = directionalLight.transform.localRotation;
			float num = 0f;
			if (numberPasses > 1)
			{
				num = maxShadowAngle / (float)(numberPasses - 1);
				rotateLight((0f - num) * (float)(numberPasses - 1) / 2f);
			}
			for (int i = 0; i < numberPasses; i++)
			{
				frameWorld();
				depthCam.Render();
				Matrix4x4 lhs = Matrix4x4.TRS(Vector3.one * 0.5f, Quaternion.identity, Vector3.one * 0.5f);
				Matrix4x4 rhs = depthCam.projectionMatrix * depthCam.worldToCameraMatrix;
				int num2 = lightMapTargets.Length;
				for (int j = 0; j < num2; j++)
				{
					CreateLightMap createLightMap = lightMapTargets[j];
					createLightMap.RenderSinglePass(lhs * rhs, renderTex, i, numberPasses);
				}
				rotateLight(num);
			}
			directionalLight.transform.localRotation = localRotation;
			if (destroyOnComplete)
			{
				int num2 = lightMapTargets.Length;
				for (int j = 0; j < num2; j++)
				{
					CreateLightMap createLightMap = lightMapTargets[j];
					Object.Destroy(createLightMap);
				}
				Object.Destroy(base.gameObject);
			}
		}

		private void createRenderTexture()
		{
			if (renderTex == null)
			{
				renderTex = new RenderTexture(textureSize, textureSize, 16, RenderTextureFormat.Depth);
				depthCam.targetTexture = renderTex;
			}
		}

		private void rotateLight(float angle)
		{
			directionalLight.transform.Rotate(Vector3.up, angle, Space.Self);
		}

		private Vector3[] getOriginalWorldBounds()
		{
			Vector3[] array = new Vector3[8];
			for (int i = 0; i < 8; i++)
			{
				Vector3 vector = worldBoundingBox.size * 0.5f;
				Vector3 scale = new Vector3(((i & 1) == 0) ? 1 : (-1), ((i & 2) == 0) ? 1 : (-1), ((i & 4) == 0) ? 1 : (-1));
				vector.Scale(scale);
				array[i] = worldBoundingBox.transform.TransformVector(vector);
			}
			return array;
		}

		private void frameWorld()
		{
			Matrix4x4 inverse = Matrix4x4.TRS(Vector3.zero, directionalLight.transform.rotation, Vector3.one).inverse;
			Bounds bounds = default(Bounds);
			for (int i = 0; i < worldBoxVerts.Length; i++)
			{
				Vector3 point = worldBoxVerts[i];
				bounds.Encapsulate(inverse.MultiplyPoint3x4(point));
			}
			depthCam.transform.rotation = directionalLight.transform.rotation;
			Vector3 forward = depthCam.transform.forward;
			float nearClipPlane = depthCam.nearClipPlane;
			float d = bounds.extents.z + nearClipPlane;
			Vector3 position = worldBoundingBox.transform.position - forward * d;
			depthCam.transform.position = position;
			depthCam.farClipPlane = nearClipPlane + bounds.size.z;
			float x = bounds.size.x;
			float y = bounds.size.y;
			depthCam.orthographicSize = 0.5f * y;
			depthCam.aspect = x / y;
		}
	}
}
