using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.BlobShadows
{
	public class BlobShadowRenderer : MonoBehaviour
	{
		private const string BLOB_SHADOW_CAM_POSITION_PROP = "_ShadowPlaneWorldPos";

		private const string SHADOW_BOX_DIMENSION_PROP = "_ShadowPlaneDim";

		private const string RENDER_TEX_DIMENSION_PROP = "_ShadowTextureDim";

		private const string BLOB_SHADOW_TEX_PROP = "_BlobShadowTex";

		private const string BLOB_SHADOWS_ACTIVE_PROP = "_BlobShadowsActive";

		private const string BLOB_SHADOW_CAM_VP = "_blobShadowCamVp";

		private const float HEIGHT_OFFSET = 5f;

		private const float MAIN_CAM_TO_CENTER_RATIO = 0.45f;

		private int BLOB_SHADOW_CAM_POSITION_PROP_ID = 0;

		private int BLOB_SHADOW_CAM_VP_ID = 0;

		public float ShadowBoxDimension = 32f;

		public int RenderTextureDimension = 128;

		public Texture2D ShadowTexture = null;

		[HideInInspector]
		public List<BlobShadowCaster> ShadowCasters = new List<BlobShadowCaster>();

		public bool BlobShadowsSupported = true;

		private Transform transformRef;

		private RenderTexture shadowRenderTexture;

		private Camera prevMainCam;

		private Transform mainCamTrans;

		private Dictionary<Material, Material> replacementMats;

		private EventDispatcher eventDispatcher;

		private Matrix4x4 projectionMatrix;

		private Vector3[] startingVertices;

		private Vector2[] startingUv;

		private Vector3[] shadowVertices;

		private Vector2[] shadowUv;

		private int[] shadowTriangles;

		private Mesh shadowMesh;

		private Material shadowMaterial;

		private int shadowCasterCount;

		private Vector4 blobShadowPosition;

		public bool IsShadowsVisible
		{
			get;
			private set;
		}

		private void Awake()
		{
			if (UnityEngine.Object.FindObjectsOfType<BlobShadowRenderer>().Length > 1)
			{
				throw new Exception("A scene should only contain 1 BlobShadowRenderer.");
			}
			SceneRefs.Set(this);
		}

		private void Start()
		{
			if (BlobShadowsSupported)
			{
				startingVertices = new Vector3[4]
				{
					new Vector3(0.5f, 0f, 0.5f),
					new Vector3(0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, -0.5f),
					new Vector3(-0.5f, 0f, 0.5f)
				};
				startingUv = new Vector2[4]
				{
					new Vector2(0f, 0f),
					new Vector2(0f, 1f),
					new Vector2(1f, 1f),
					new Vector2(1f, 0f)
				};
				shadowVertices = new Vector3[0];
				shadowUv = new Vector2[0];
				shadowTriangles = new int[0];
				shadowMesh = new Mesh();
				shadowMaterial = new Material(Shader.Find("CpRemix/BlobShadows/ShadowGeoShader"));
				shadowMaterial.SetTexture("_MainTex", ShadowTexture);
				IsShadowsVisible = true;
				transformRef = base.transform;
				projectionMatrix = Matrix4x4.Ortho(-0.5f, 0.5f, -0.5f, 0.5f, 0.3f, 1000f);
				Vector4 row = projectionMatrix.GetRow(0);
				row.x /= ShadowBoxDimension;
				projectionMatrix.SetRow(0, row);
				row = projectionMatrix.GetRow(1);
				row.y /= ShadowBoxDimension;
				projectionMatrix.SetRow(1, row);
				string graphicsDeviceVersion = SystemInfo.graphicsDeviceVersion;
				if (Application.platform == RuntimePlatform.WindowsEditor || graphicsDeviceVersion.IndexOf("OpenGL") <= -1)
				{
					for (int i = 0; i < 4; i++)
					{
						projectionMatrix[1, i] = 0f - projectionMatrix[1, i];
						projectionMatrix[2, i] = projectionMatrix[2, i] * 0.5f + projectionMatrix[3, i] * 0.5f;
					}
				}
				blobShadowPosition = default(Vector4);
				BLOB_SHADOW_CAM_POSITION_PROP_ID = Shader.PropertyToID("_ShadowPlaneWorldPos");
				BLOB_SHADOW_CAM_VP_ID = Shader.PropertyToID("_blobShadowCamVp");
				setupRenderTexture();
				enforceDownwardLook();
				setupShadowReceiverMaterials();
			}
			eventDispatcher = Service.Get<EventDispatcher>();
			eventDispatcher.AddListener<BlobShadowEvents.DisableBlobShadows>(onDisableBlobShadows);
			eventDispatcher.AddListener<BlobShadowEvents.EnableBlobShadows>(onEnableBlobShadows);
		}

		private void setupExistingShadowCasters()
		{
			BlobShadowCaster[] array = UnityEngine.Object.FindObjectsOfType<BlobShadowCaster>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetBlobShadowCam(this);
				if (!ShadowCasters.Contains(array[i]))
				{
					ShadowCasters.Add(array[i]);
				}
			}
		}

		private void setupRenderTexture()
		{
			RenderTextureFormat format = RenderTextureFormat.ARGBHalf;
			if (!SystemInfo.SupportsRenderTextureFormat(format))
			{
				format = RenderTextureFormat.ARGB32;
			}
			shadowRenderTexture = new RenderTexture(RenderTextureDimension, RenderTextureDimension, 0, format);
			shadowRenderTexture.name = "Blob Shadow RenderTexture: " + base.name;
			shadowRenderTexture.useMipMap = false;
			shadowRenderTexture.Create();
		}

		private void enforceDownwardLook()
		{
			transformRef.up = Vector3.forward;
			transformRef.forward = Vector3.down;
		}

		public void RenderBlobs()
		{
			if (!BlobShadowsSupported)
			{
				return;
			}
			Graphics.SetRenderTarget(shadowRenderTexture);
			GL.Clear(false, true, Color.white);
			GL.LoadOrtho();
			GL.LoadIdentity();
			Matrix4x4 worldToLocalMatrix = transformRef.worldToLocalMatrix;
			Vector4 row = worldToLocalMatrix.GetRow(2);
			row.y = 0f - row.y;
			row.w = 0f - row.w;
			worldToLocalMatrix.SetRow(2, row);
			Matrix4x4 value = projectionMatrix * worldToLocalMatrix;
			shadowMaterial.SetMatrix(BLOB_SHADOW_CAM_VP_ID, value);
			shadowMaterial.SetPass(0);
			int num = 0;
			int count = ShadowCasters.Count;
			for (int i = 0; i < count; i++)
			{
				if (ShadowCasters[i].GeoVisible)
				{
					num++;
				}
			}
			if (num != shadowCasterCount)
			{
				Array.Resize(ref shadowVertices, num * 4);
				Array.Resize(ref shadowUv, num * 4);
				Array.Resize(ref shadowTriangles, num * 6);
				for (int i = shadowCasterCount; i < num; i++)
				{
					int num2 = i * 4;
					int num3 = i * 6;
					for (int j = 0; j < 4; j++)
					{
						shadowUv[num2 + j] = startingUv[j];
					}
					shadowTriangles[num3] = num2;
					shadowTriangles[num3 + 1] = num2 + 1;
					shadowTriangles[num3 + 2] = num2 + 2;
					shadowTriangles[num3 + 3] = num2;
					shadowTriangles[num3 + 4] = num2 + 2;
					shadowTriangles[num3 + 5] = num2 + 3;
				}
				shadowCasterCount = num;
				shadowMesh.Clear();
				shadowMesh.vertices = shadowVertices;
				shadowMesh.uv = shadowUv;
				shadowMesh.triangles = shadowTriangles;
			}
			int num4 = 0;
			for (int i = 0; i < count; i++)
			{
				if (ShadowCasters[i].GeoVisible)
				{
					int num5 = num4 * 4;
					Quaternion rotation = ShadowCasters[i].transform.rotation;
					Vector3 position = ShadowCasters[i].transform.position;
					float scaleX = ShadowCasters[i].ScaleX;
					float scaleZ = ShadowCasters[i].ScaleZ;
					for (int j = 0; j < 4; j++)
					{
						shadowVertices[num5 + j] = new Vector3(startingVertices[j].x * scaleX, startingVertices[j].y, startingVertices[j].z * scaleZ);
						shadowVertices[num5 + j] = rotation * shadowVertices[num5 + j] + position;
					}
					num4++;
				}
			}
			shadowMesh.vertices = shadowVertices;
			Graphics.DrawMeshNow(shadowMesh, Matrix4x4.identity);
			Graphics.SetRenderTarget(null);
		}

		private void Update()
		{
			if (!BlobShadowsSupported)
			{
				return;
			}
			Camera main = Camera.main;
			if (main == null)
			{
				return;
			}
			if (prevMainCam != main)
			{
				main.gameObject.AddComponent<BlobShadowCameraController>();
				mainCamTrans = main.transform;
				prevMainCam = main;
			}
			Vector3 vector = updatePosition(mainCamTrans);
			bool flag = vector != transformRef.position;
			if (flag)
			{
				if (flag)
				{
					updateShaderBlobCamPos(vector);
				}
				transformRef.position = vector;
			}
		}

		private void updateShaderBlobCamPos(Vector3 newCamPosition)
		{
			blobShadowPosition.Set(newCamPosition.x, newCamPosition.y, newCamPosition.z, 0f);
			Shader.SetGlobalVector(BLOB_SHADOW_CAM_POSITION_PROP_ID, blobShadowPosition);
		}

		private Vector3 updatePosition(Transform mainCamTrans)
		{
			Vector3 position = mainCamTrans.position;
			Vector3 forward = mainCamTrans.forward;
			Vector2 vector = new Vector2(forward.x, forward.z);
			vector.Normalize();
			position.y += 5f;
			position.x += vector.x * ShadowBoxDimension * 0.45f;
			position.z += vector.y * ShadowBoxDimension * 0.45f;
			return position;
		}

		private void setupShadowReceiverMaterials()
		{
			Shader.SetGlobalFloat("_ShadowPlaneDim", ShadowBoxDimension);
			Shader.SetGlobalFloat("_ShadowTextureDim", RenderTextureDimension);
			replacementMats = new Dictionary<Material, Material>();
			Renderer[] array = UnityEngine.Object.FindObjectsOfType<Renderer>();
			foreach (Renderer renderer in array)
			{
				Material sharedMaterial = renderer.sharedMaterial;
				if (sharedMaterial != null && sharedMaterial.HasProperty("_BlobShadowTex"))
				{
					Material material = null;
					if (!replacementMats.ContainsKey(sharedMaterial))
					{
						material = new Material(sharedMaterial);
						material.name = sharedMaterial.name + "_runtimeReplacement";
						material.SetTexture("_BlobShadowTex", shadowRenderTexture);
						replacementMats.Add(sharedMaterial, material);
					}
					else
					{
						material = replacementMats[sharedMaterial];
					}
					renderer.sharedMaterial = material;
				}
			}
		}

		private bool onEnableBlobShadows(BlobShadowEvents.EnableBlobShadows evt)
		{
			IsShadowsVisible = true;
			return false;
		}

		private bool onDisableBlobShadows(BlobShadowEvents.DisableBlobShadows evt)
		{
			IsShadowsVisible = false;
			return false;
		}

		private void OnDestroy()
		{
			eventDispatcher.RemoveListener<BlobShadowEvents.DisableBlobShadows>(onDisableBlobShadows);
			eventDispatcher.RemoveListener<BlobShadowEvents.EnableBlobShadows>(onEnableBlobShadows);
			if (replacementMats != null)
			{
				foreach (KeyValuePair<Material, Material> replacementMat in replacementMats)
				{
					UnityEngine.Object.Destroy(replacementMat.Value);
				}
			}
		}
	}
}
