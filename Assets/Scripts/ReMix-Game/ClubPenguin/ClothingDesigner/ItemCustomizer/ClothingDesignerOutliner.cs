using ClubPenguin.Avatar;
using ClubPenguin.Core;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	[RequireComponent(typeof(AvatarViewDistinct))]
	public class ClothingDesignerOutliner : MonoBehaviour
	{
		private const int MAX_MOBILE_RENDER_SIZE = 512;

		private const int MAX_STANDALONE_RENDER_SIZE = 512;

		private const int ANTI_ALIASING_2 = 2;

		public int MaxRenderTextureSize;

		public AnimationCurve OutlinerAnimationCurve;

		public Color DarkenedSwatchColor = Color.grey;

		public string OutlineLayerName;

		public Material BlackMaterial;

		private Camera referenceCamera;

		private Camera outlineCamera;

		private ClothingOutlinerImageEffect outlinerEffect;

		private RenderTexture outlineRtt;

		private readonly List<ClothingDesignerOutlinerChild> children = new List<ClothingDesignerOutlinerChild>();

		public int SelectedChannel
		{
			get;
			set;
		}

		public int OutlineLayer
		{
			get;
			private set;
		}

		public void Awake()
		{
			AvatarViewDistinct component = GetComponent<AvatarViewDistinct>();
			component.OnChildAdded += onChildAdded;
			SelectedChannel = -1;
			OutlineLayer = LayerMask.NameToLayer(OutlineLayerName);
		}

		private void onChildAdded(AvatarViewDistinctChild child)
		{
			GameObject gameObject = new GameObject(child.name + "Outline");
			gameObject.transform.SetParent(child.transform, false);
			gameObject.layer = OutlineLayer;
			gameObject.SetActive(false);
			ClothingDesignerOutlinerChild clothingDesignerOutlinerChild = gameObject.AddComponent<ClothingDesignerOutlinerChild>();
			clothingDesignerOutlinerChild.Avdc = child;
			clothingDesignerOutlinerChild.Outliner = this;
			gameObject.SetActive(true);
			children.Add(clothingDesignerOutlinerChild);
		}

		public void Init(Camera referenceCamera)
		{
			this.referenceCamera = referenceCamera;
			setupCamera();
		}

		private void Update()
		{
			if (referenceCamera != null)
			{
				outlineCamera.fieldOfView = referenceCamera.fieldOfView;
			}
		}

		private void setupCamera()
		{
			if (outlineCamera == null)
			{
				GameObject gameObject = new GameObject("OutlineCamera");
				gameObject.layer = OutlineLayer;
				outlineCamera = gameObject.AddComponent<Camera>();
				outlineCamera.transform.SetParent(referenceCamera.transform, false);
				outlineCamera.transform.localPosition = Vector3.zero;
				outlineCamera.transform.localRotation = Quaternion.identity;
				AudioListener component = outlineCamera.GetComponent<AudioListener>();
				if (component != null)
				{
					Object.Destroy(component);
				}
			}
			outlineCamera.CopyFrom(referenceCamera);
			outlineCamera.rect = new Rect(0f, 0f, 1f, 1f);
			int num = 1 << OutlineLayer;
			outlineCamera.cullingMask = num;
			referenceCamera.cullingMask &= ~num;
			outlineCamera.clearFlags = CameraClearFlags.Color;
			outlineCamera.backgroundColor = Color.black;
			bool flag = PlatformUtils.GetPlatformType() == PlatformType.Standalone;
			MaxRenderTextureSize = (flag ? 512 : 512);
			int num2 = MaxRenderTextureSize;
			int num3 = MaxRenderTextureSize;
			if (referenceCamera.aspect > 1f)
			{
				num3 = Mathf.RoundToInt((float)num2 / referenceCamera.aspect);
			}
			else
			{
				num2 = Mathf.RoundToInt((float)num3 * referenceCamera.aspect);
			}
			outlineRtt = new RenderTexture(num2, num3, 24);
			if (flag)
			{
				outlineRtt.antiAliasing = 2;
			}
			outlineCamera.targetTexture = outlineRtt;
			ClothingOutlinerImageEffect component2 = referenceCamera.gameObject.GetComponent<ClothingOutlinerImageEffect>();
			outlinerEffect = (component2 ?? referenceCamera.gameObject.AddComponent<ClothingOutlinerImageEffect>());
			outlinerEffect.OutlineTexture = outlineRtt;
			outlinerEffect.AnimationCurve = OutlinerAnimationCurve;
		}

		public void OnEnable()
		{
			if (outlinerEffect != null)
			{
				outlinerEffect.enabled = true;
			}
		}

		public void OnDisable()
		{
			if (outlinerEffect != null)
			{
				outlinerEffect.enabled = false;
			}
		}

		private void OnDestroy()
		{
			if (outlineRtt != null)
			{
				Object.Destroy(outlineRtt);
			}
			if (outlineCamera != null)
			{
				Object.Destroy(outlineCamera.gameObject);
			}
			if (outlinerEffect != null)
			{
				Object.Destroy(outlinerEffect);
			}
		}
	}
}
