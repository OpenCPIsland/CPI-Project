using ClubPenguin.Avatar;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class ClothingDesignerOutlinerChild : MonoBehaviour
	{
		public const int NUM_CHANNELS_OUTLINEABLE = 3;

		private Material outlineMaterial;

		private SkinnedMeshRenderer smr;

		private int selectedChannel = -1;

		public ClothingDesignerOutliner Outliner
		{
			get;
			internal set;
		}

		public AvatarViewDistinctChild Avdc
		{
			get;
			internal set;
		}

		private void OnEnable()
		{
			Avdc.OnReady += onReady;
		}

		private void OnDisable()
		{
			Avdc.OnReady -= onReady;
		}

		public void OnDestroy()
		{
			Object.Destroy(outlineMaterial);
			outlineMaterial = null;
		}

		private void onReady(AvatarBaseAsync aba)
		{
			bool flag = Avdc.ViewPart.HasMaterialProperties(typeof(EquipmentMaterialProperties));
			if (outlineMaterial != null)
			{
				Object.Destroy(outlineMaterial);
			}
			SkinnedMeshRenderer component = Avdc.GetComponent<SkinnedMeshRenderer>();
			smr = (smr ?? base.gameObject.AddComponent<SkinnedMeshRenderer>());
			smr.bones = component.bones;
			smr.rootBone = component.rootBone;
			smr.sharedMesh = component.sharedMesh;
			if (flag)
			{
				Texture2D whiteTexture = Texture2D.whiteTexture;
				outlineMaterial = new Material(component.sharedMaterial);
				outlineMaterial.shader = AvatarService.EquipmentScreenshotShader;
				outlineMaterial.SetTexture(ShaderParams.DIFFUSE_TEX, whiteTexture);
				for (int i = 0; i < 3; i++)
				{
					outlineMaterial.SetTexture(ShaderParams.DECAL_TEX[i], whiteTexture);
				}
				for (int i = 3; i < 6; i++)
				{
					outlineMaterial.SetTexture(ShaderParams.DECAL_TEX[i], null);
				}
				outlineMaterial.SetTexture(ShaderParams.DETAIL_MATCAPMASK_EMISSIVE_TEX, whiteTexture);
				outlineMaterial.SetColor(ShaderParams.BODY_RED_CHANNEL_COLOR, Color.black);
				outlineMaterial.SetColor(ShaderParams.BODY_GREEN_CHANNEL_COLOR, Color.black);
				outlineMaterial.SetColor(ShaderParams.BODY_BLUE_CHANNEL_COLOR, Color.black);
				smr.material = outlineMaterial;
				updateMaterial();
			}
			else
			{
				smr.material = Outliner.BlackMaterial;
			}
		}

		public void Update()
		{
			if (outlineMaterial != null && Outliner.SelectedChannel != selectedChannel)
			{
				selectedChannel = Outliner.SelectedChannel;
				updateMaterial();
			}
		}

		private void updateMaterial()
		{
			for (int i = 0; i < 3; i++)
			{
				Color value = (i == selectedChannel) ? Color.white : Color.black;
				outlineMaterial.SetColor(ShaderParams.DECAL_COLOR[i], value);
			}
		}
	}
}
