using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class CustomizerBackgroundFade : MonoBehaviour
	{
		public Transform rootTransform = null;

		public Material[] fadeMaterials = null;

		private Material[] _adjMaterials = null;

		private Color[] _originalColors = null;

		private readonly string COLOR_PARAMATER = "_TintColor";

		private Color fadeColor = Color.white;

		private bool isFaded = false;

		private EventChannel eventChannel;

		private void Start()
		{
			setupMaterials();
			setupListeners();
		}

		private void setupMaterials()
		{
			_originalColors = new Color[fadeMaterials.Length];
			_adjMaterials = new Material[fadeMaterials.Length];
			for (int i = 0; i < fadeMaterials.Length; i++)
			{
				_adjMaterials[i] = new Material(fadeMaterials[i]);
				_originalColors[i] = _adjMaterials[i].GetColor(COLOR_PARAMATER);
			}
			Renderer[] componentsInChildren = rootTransform.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				for (int j = 0; j < fadeMaterials.Length; j++)
				{
					if (componentsInChildren[i].sharedMaterial == fadeMaterials[j])
					{
						componentsInChildren[i].sharedMaterial = _adjMaterials[j];
						break;
					}
				}
			}
		}

		private void setupListeners()
		{
			eventChannel = new EventChannel(CustomizationContext.EventBus);
			eventChannel.AddListener<CustomizerEffectsEvents.FadeBackground>(onBackgroundFade);
		}

		private bool onBackgroundFade(CustomizerEffectsEvents.FadeBackground evt)
		{
			if (isFaded != evt.DoFade)
			{
				isFaded = evt.DoFade;
				float num = evt.DoFade ? 0f : 1f;
				float num2 = evt.DoFade ? 1f : 0f;
				if (evt.DoFade)
				{
					fadeColor = new Color(evt.FadeAmount, evt.FadeAmount, evt.FadeAmount);
				}
				iTween.ValueTo(base.gameObject, iTween.Hash("from", num, "to", num2, "time", evt.Duration, "easetype", iTween.EaseType.easeOutExpo, "onupdatetarget", base.gameObject, "onupdate", "updateLightValue"));
			}
			return false;
		}

		private void updateLightValue(float value)
		{
			for (int i = 0; i < _adjMaterials.Length; i++)
			{
				Color value2 = Color.Lerp(_originalColors[i], fadeColor, value);
				_adjMaterials[i].SetColor(COLOR_PARAMATER, value2);
			}
		}

		private void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
		}
	}
}
