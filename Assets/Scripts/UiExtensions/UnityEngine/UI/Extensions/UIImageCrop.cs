namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Effects/Extensions/UIImageCrop")]
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public class UIImageCrop : MonoBehaviour
	{
		private MaskableGraphic mGraphic;

		private Material mat;

		private int XCropProperty;

		private int YCropProperty;

		public float XCrop = 0f;

		public float YCrop = 0f;

		private void Start()
		{
			SetMaterial();
		}

		public void SetMaterial()
		{
			mGraphic = GetComponent<MaskableGraphic>();
			XCropProperty = Shader.PropertyToID("_XCrop");
			YCropProperty = Shader.PropertyToID("_YCrop");
			if (mGraphic != null)
			{
				if (mGraphic.material == null || mGraphic.material.name == "Default UI Material")
				{
					mGraphic.material = new Material(Shader.Find("UI Extensions/UI Image Crop"));
				}
				mat = mGraphic.material;
			}
			else
			{
				Debug.LogError("Please attach component to a Graphical UI component");
			}
		}

		public void OnValidate()
		{
			SetMaterial();
			SetXCrop(XCrop);
			SetYCrop(YCrop);
		}

		public void SetXCrop(float xcrop)
		{
			XCrop = Mathf.Clamp01(xcrop);
			mat.SetFloat(XCropProperty, XCrop);
		}

		public void SetYCrop(float ycrop)
		{
			YCrop = Mathf.Clamp01(ycrop);
			mat.SetFloat(YCropProperty, YCrop);
		}
	}
}
