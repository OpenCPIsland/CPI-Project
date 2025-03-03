using Disney.LaunchPadFramework;
using UnityEngine;

namespace ClubPenguin.Halloween
{
	public class CandleFlickerController : MonoBehaviour
	{
		public int UpdateOnFrame = 6;

		public CandleFlickerData[] FlickerData;

		private int dataLength;

		private Color[] origAdditiveColor;

		private Color flickerColor;

		private float[] animCurveTime;

		private void Start()
		{
			dataLength = FlickerData.Length;
			origAdditiveColor = new Color[dataLength];
			animCurveTime = new float[dataLength];
			for (int i = 0; i < dataLength; i++)
			{
				CandleFlickerData candleFlickerData = FlickerData[i];
				if (candleFlickerData.PumpkinMaterial != null)
				{
					origAdditiveColor[i] = candleFlickerData.PumpkinMaterial.GetColor("_AdditiveColor");
				}
				else
				{
					Log.LogError(this, string.Format("Error: Missing a material on {0}", base.gameObject.GetPath()));
				}
				animCurveTime[i] = Random.Range(0f, candleFlickerData.AnimTime);
			}
		}

		private void Update()
		{
			if (UpdateOnFrame < 1)
			{
				UpdateOnFrame = 1;
			}
			if (Time.frameCount % UpdateOnFrame != 0)
			{
				return;
			}
			for (int i = 0; i < dataLength; i++)
			{
				CandleFlickerData candleFlickerData = FlickerData[i];
				float num = animCurveTime[i] + candleFlickerData.AnimStep;
				if (num < 0f)
				{
					num = candleFlickerData.AnimTime;
				}
				else if (num > candleFlickerData.AnimTime)
				{
					num = 0f;
				}
				animCurveTime[i] = num;
				float num2 = candleFlickerData.flickerCurve.Evaluate(num);
				flickerColor.r = Mathf.Clamp(origAdditiveColor[i].r + candleFlickerData.BaseColor.r * num2, 0f, 1f);
				flickerColor.g = Mathf.Clamp(origAdditiveColor[i].g + candleFlickerData.BaseColor.g * num2, 0f, 1f);
				flickerColor.b = Mathf.Clamp(origAdditiveColor[i].b + candleFlickerData.BaseColor.b * num2, 0f, 1f);
				candleFlickerData.PumpkinMaterial.SetColor("_AdditiveColor", flickerColor);
			}
		}

		private void OnDestroy()
		{
			restoreMaterials();
		}

		private void restoreMaterials()
		{
			for (int i = 0; i < dataLength; i++)
			{
				CandleFlickerData candleFlickerData = FlickerData[i];
				candleFlickerData.PumpkinMaterial.SetColor("_AdditiveColor", origAdditiveColor[i]);
			}
		}
	}
}
