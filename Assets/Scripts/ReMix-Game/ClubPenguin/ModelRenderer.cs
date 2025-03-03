using ClubPenguin.Cinematography;
using System;
using UnityEngine;

namespace ClubPenguin
{
	public class ModelRenderer
	{
		public enum OrthographicOption
		{
			Enabled,
			Disabled
		}

		public enum CameraFramingObjectOption
		{
			InCamera,
			NotInCamera
		}

		public const string ICON_RENDER_LAYER = "IconRender";

		private const int MODEL_RENDERER_MAX_COUNT = 64;

		private const string MODEL_RENDERER_PARENT_NAME = "ModelRenderers";

		private readonly Vector3 RENDER_POSITION_OFFSET = new Vector3(-10f, 0f, 0f);

		private static Transform modelRendererParentTransform;

		private static int modelRendererCount;

		private static ModelRenderer[] modelRenderers;

		private ModelRendererConfig config;

		private int index;

		private int iconRenderLayer;

		private RenderTexture renderTexture;

		private Transform modelRendererTransform;

		private Camera renderCamera;

		private Transform previousParent;

		public RenderTexture RenderTexture
		{
			get
			{
				return renderTexture;
			}
		}

		public Texture2D Image
		{
			get
			{
				bool flag = false;
				RenderTexture.active = renderTexture;
				Texture2D texture2D = new Texture2D((int)config.TextureDimensions.x, (int)config.TextureDimensions.y, TextureFormat.ARGB32, flag);
				texture2D.ReadPixels(new Rect(0f, 0f, (int)config.TextureDimensions.x, (int)config.TextureDimensions.y), 0, 0);
				texture2D.Apply(flag);
				RenderTexture.active = null;
				renderTexture.DiscardContents();
				return texture2D;
			}
		}

		public ModelRenderer(ModelRendererConfig config)
		{
			this.config = config;
			init();
		}

		private void init()
		{
			iconRenderLayer = LayerMask.NameToLayer("IconRender");
			previousParent = config.ObjectToRenderTransform.parent;
			modelRendererTransform = createModelRenderGameObject();
			renderCamera = createCamera();
			renderTexture = createRenderTexture();
			renderTexture.name = "Model Render Texture: " + config.ObjectToRenderTransform.name;
			positionObjectToRender();
			if (config.FrameObjectInCamera)
			{
				placeObjectSoItIsVisibleInCamera();
			}
			renderCamera.targetTexture = renderTexture;
			renderCamera.Render();
			modelRendererCount++;
		}

		public void SetModelRotation(Quaternion rotation)
		{
			config.ObjectToRenderTransform.rotation = rotation;
		}

		public Quaternion GetModelRotation()
		{
			return config.ObjectToRenderTransform.rotation;
		}

		public void Destroy()
		{
			if (!config.AutoDestroyObjectToRender)
			{
				config.ObjectToRenderTransform.SetParent(previousParent);
			}
			if (modelRendererTransform != null)
			{
				UnityEngine.Object.Destroy(modelRendererTransform.gameObject);
			}
			modelRenderers[index] = null;
			modelRendererCount--;
			if (modelRendererParentTransform != null && modelRendererCount <= 0)
			{
				UnityEngine.Object.Destroy(modelRendererParentTransform.gameObject);
				modelRendererParentTransform = null;
			}
			if (renderTexture != null)
			{
				UnityEngine.Object.Destroy(renderTexture);
			}
		}

		private RenderTexture createRenderTexture()
		{
			return new RenderTexture((int)config.TextureDimensions.x, (int)config.TextureDimensions.y, 24, RenderTextureFormat.ARGB32);
		}

		private Transform createModelRenderGameObject()
		{
			GameObject gameObject = new GameObject();
			gameObject.name = modelRendererCount + " " + config.ObjectToRenderTransform.gameObject.name;
			if (modelRendererParentTransform == null)
			{
				createModelRendererParent();
			}
			Transform transform = gameObject.transform;
			transform.SetParent(modelRendererParentTransform);
			index = getPositionIndex();
			modelRenderers[index] = this;
			transform.localPosition = findLocalRenderPosition(index);
			return transform;
		}

		private Camera createCamera()
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "ModelRenderCamera";
			gameObject.layer = iconRenderLayer;
			Camera camera = gameObject.AddComponent<Camera>();
			camera.aspect = config.TextureDimensions.x / config.TextureDimensions.y;
			camera.clearFlags = CameraClearFlags.Color;
			camera.backgroundColor = new Color(1f, 1f, 1f, 0f);
			camera.orthographic = config.IsOrthographic;
			camera.fieldOfView = config.FieldOfView;
			camera.useOcclusionCulling = config.UseOcclusionCulling;
			CameraCullingMaskHelper.SetSingleLayer(camera, "IconRender");
			if (config.UseSolidBackground)
			{
				camera.backgroundColor = config.CameraBackgroundColor;
			}
			gameObject.transform.SetParent(modelRendererTransform);
			gameObject.transform.localPosition = Vector3.zero;
			return camera;
		}

		public void RotateCamera(Vector3 cameraRotation)
		{
			renderCamera.transform.Rotate(cameraRotation);
		}

		private void positionObjectToRender()
		{
			config.ObjectToRenderTransform.SetParent(modelRendererTransform);
			config.ObjectToRenderTransform.localPosition = config.ObjectCameraOffset;
			if (config.IsOrthographic)
			{
				float d = renderCamera.orthographicSize * 2f;
				config.ObjectToRenderTransform.localScale = Vector3.one * d;
			}
		}

		private void placeObjectSoItIsVisibleInCamera()
		{
			config.ObjectToRenderTransform.Rotate(new Vector3(0f, 210f, 0f));
			Renderer[] componentsInChildren = config.ObjectToRenderTransform.GetComponentsInChildren<Renderer>();
			Bounds bounds = componentsInChildren[0].bounds;
			componentsInChildren[0].gameObject.layer = iconRenderLayer;
			for (int i = 1; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.layer = iconRenderLayer;
				bounds.Encapsulate(componentsInChildren[i].bounds);
			}
			Vector3 center = bounds.center;
			Vector3 min = bounds.min;
			Vector3 max = bounds.max;
			float x = Mathf.Abs(center.z - renderCamera.transform.position.z);
			float y = Mathf.Abs(max.x - center.x);
			float y2 = Mathf.Abs(max.y - center.y);
			float a = 2f * Mathf.Atan2(y, x) * 180f / (float)Math.PI;
			float b = 2f * Mathf.Atan2(y2, x) * 180f / (float)Math.PI;
			renderCamera.fieldOfView = Mathf.Max(a, b);
			Vector3 worldPosition = default(Vector3);
			worldPosition.x = min.x + (max.x - min.x) / 2f;
			worldPosition.y = min.y + (max.y - min.y) / 2f;
			worldPosition.z = min.z + (max.z - min.z) / 2f;
			renderCamera.transform.LookAt(worldPosition);
		}

		private Vector3 findLocalRenderPosition(int positionIndex)
		{
			return RENDER_POSITION_OFFSET * (positionIndex + 1);
		}

		private int getPositionIndex()
		{
			if (modelRenderers == null)
			{
				modelRenderers = new ModelRenderer[64];
			}
			for (int i = 0; i < modelRenderers.Length; i++)
			{
				if (modelRenderers[i] == null)
				{
					return i;
				}
			}
			throw new InvalidOperationException("No model renderers available. Consider increasing the size");
		}

		private void createModelRendererParent()
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "ModelRenderers";
			modelRendererParentTransform = gameObject.transform;
			modelRendererParentTransform.position = Vector3.zero;
		}
	}
}
