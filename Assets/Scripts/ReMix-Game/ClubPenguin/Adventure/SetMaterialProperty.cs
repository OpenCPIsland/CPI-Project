using System;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	public class SetMaterialProperty : MonoBehaviour
	{
		private Material tintMaterial;

		private string tintMaterialProperty;

		private AnimationCurve animCurve;

		private Color tintColor;

		private float animTime = 0f;

		private float animTimeMax = 0f;

		public event Action OnComplete;

		public void ChangeProperty(Material tintMaterial, string tintMaterialProperty, Color tintColor, AnimationCurve animCurve = null)
		{
			this.tintMaterial = tintMaterial;
			this.tintMaterialProperty = tintMaterialProperty;
			this.tintColor = tintColor;
			this.animCurve = animCurve;
			animTime = 0f;
			if (animCurve != null && animCurve.length > 0)
			{
				animTimeMax = animCurve[animCurve.length - 1].time;
			}
			else
			{
				animTimeMax = 0f;
			}
			DoSetMaterialProperty();
		}

		private void Update()
		{
			DoSetMaterialProperty();
		}

		private void DoSetMaterialProperty()
		{
			float num = animCurve.Evaluate(animTime);
			Color value = tintColor;
			value.r *= num;
			value.g *= num;
			value.b *= num;
			tintMaterial.SetColor(tintMaterialProperty, value);
			animTime += Time.deltaTime;
			if (animTime >= animTimeMax)
			{
				if (this.OnComplete != null)
				{
					this.OnComplete();
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
