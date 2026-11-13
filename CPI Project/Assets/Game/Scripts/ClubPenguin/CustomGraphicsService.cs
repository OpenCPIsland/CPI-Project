using ClubPenguin.Configuration;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ClubPenguin
{
	public class CustomGraphicsService
	{
		public const string RenderScalePlayerPrefsKey = "cp.RenderScale";
		private const float DefaultRenderScale = 1f;
		private const float MinimumRenderScale = 0.1f;
		private const float MaximumRenderScale = 3f;
		private const float RenderScaleStep = 0.1f;

		public int AntiAliasSamplesLow = 2;

		public int AntiAliasSamplesMedium = 4;

		public int AntiAliasSamplesHigh = 8;

        public readonly Dictionary<string, float> SupportedAspectRatios = new Dictionary<string, float>
		{
			{
				"32:9",
				3.55555558f
			},
			{
				"12:5",
				2.4f
			},
			{
				"21:9",
				2.33333325f
			},
			{
				"16:9",
				1.77777779f
			},
			{
				"5:3",
				1.66666663f
			},
			{
				"16:10",
				1.6f
			},
			{
				"14:9",
				1.55555558f
			},
			{
				"16:10.4",
				1.54789555f
			},
			{
				"3:2",
				1.5f
			},
			{
				"4:3",
				1.33333337f
			},
			{
				"5:4",
				1.25f
			}
	};

        public readonly Dictionary<string, QualityLevel> QualityLevelByConditionalTier = new Dictionary<string, QualityLevel>
		{
			{
				"Mobile_Low",
				QualityLevel.Low
			},
			{
				"Mobile_Medium",
				QualityLevel.Medium
			},
			{
				"Mobile_High",
				QualityLevel.High
			},
			{
				"Standalone_Low",
				QualityLevel.Low
			},
			{
				"Standalone_Medium",
				QualityLevel.Medium
			},
			{
				"Standalone_High",
				QualityLevel.High
			},
			{
				"Default",
				QualityLevel.High
			}
		};

		public static float AspectRatio
		{
			get;
			private set;
		}

		public static string AspectAlias
		{
			get;
			private set;
		}

		public CacheableType<string> QualityConditionalTier
		{
			get;
			private set;
		}

		public CacheableType<QualityLevel> GraphicsLevel
		{
			get;
			private set;
		}

		public CacheableType<QualityLevel> LodPenguinQualityLevel
		{
			get;
			private set;
		}

		public CacheableType<int> AntiAliasLevel
		{
			get;
			private set;
		}

		public CacheableType<bool> CameraPostEnabled
		{
			get;
			private set;
		}

		public void Init()
		{
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
			Texture.SetGlobalAnisotropicFilteringLimits(16, 16);
			string text = Service.Get<ConditionalConfiguration>().Get("Unity.QualitySetting.property", "Mobile_Low");
			QualityLevel graphicsLevel;
			QualityLevel lodPenguinQualityLevel;
			int antiAlias;
			bool cameraPost;
			getDefaultGraphicsValues(text, out graphicsLevel, out lodPenguinQualityLevel, out antiAlias, out cameraPost);
			QualityConditionalTier = new CacheableType<string>("cp.QualityConditionalTier", text);
			GraphicsLevel = new CacheableType<QualityLevel>("cp.GraphicsLevel", graphicsLevel);
			LodPenguinQualityLevel = new CacheableType<QualityLevel>("cp.LodPenguinQualityLevel", lodPenguinQualityLevel);
			AntiAliasLevel = new CacheableType<int>("cp.AntiAliasEnabled", antiAlias);
			CameraPostEnabled = new CacheableType<bool>("cp.CameraPostenabled", cameraPost);
			GameSettings gameSettings = Service.Get<GameSettings>();
			gameSettings.RegisterSetting(QualityConditionalTier, true);
			gameSettings.RegisterSetting(GraphicsLevel, true);
			gameSettings.RegisterSetting(LodPenguinQualityLevel, true);
			gameSettings.RegisterSetting(AntiAliasLevel, true);
			gameSettings.RegisterSetting(CameraPostEnabled, true);
			int savedAntiAlias = AntiAliasLevel;
			SetAntialiasing((savedAntiAlias > 0) ? antiAlias : 0);
			SetCameraPostEffects(CameraPostEnabled);
			SetLodPenguinQualityLevel(LodPenguinQualityLevel);
			SetFullscreen(Screen.fullScreen);
			SetAspectRatio((float)Screen.width / (float)Screen.height);
			if (AspectAlias == string.Empty)
			{
				SetAspectRatio(1.77777779f);
			}
			ApplyRenderScale(PlayerPrefs.GetFloat(RenderScalePlayerPrefsKey, DefaultRenderScale), false);
		}

		public float RenderScale
		{
			get { return PlayerPrefs.GetFloat(RenderScalePlayerPrefsKey, DefaultRenderScale); }
		}

		public static float[] GetRenderScaleOptions()
		{
			int optionCount = Mathf.RoundToInt((MaximumRenderScale - MinimumRenderScale) / RenderScaleStep) + 1;
			float[] options = new float[optionCount];
			for (int i = 0; i < optionCount; i++)
			{
				options[i] = MinimumRenderScale + i * RenderScaleStep;
			}
			return options;
		}

		public void SetRenderScale(float scale)
		{
			ApplyRenderScale(scale, true);
		}

		private static void ApplyRenderScale(float scale, bool save)
		{
			float nearestScale = DefaultRenderScale;
			float nearestDistance = float.MaxValue;
			float[] renderScaleOptions = GetRenderScaleOptions();
			for (int i = 0; i < renderScaleOptions.Length; i++)
			{
				float distance = Mathf.Abs(renderScaleOptions[i] - scale);
				if (distance < nearestDistance)
				{
					nearestDistance = distance;
					nearestScale = renderScaleOptions[i];
				}
			}

			UniversalRenderPipelineAsset renderPipeline = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
			if (renderPipeline != null)
			{
				renderPipeline.renderScale = nearestScale;
			}
			if (save)
			{
				PlayerPrefs.SetFloat(RenderScalePlayerPrefsKey, nearestScale);
				PlayerPrefs.Save();
			}
		}

		private void getDefaultGraphicsValues(string qualityName, out QualityLevel graphicsLevel, out QualityLevel lodPenguinQualityLevel, out int antiAlias, out bool cameraPost)
		{
			if (QualityLevelByConditionalTier.TryGetValue(qualityName, out graphicsLevel))
			{
				switch (graphicsLevel)
				{
				case QualityLevel.Low:
                    antiAlias = AntiAliasSamplesLow;
                    cameraPost = false;
					break;
				case QualityLevel.Medium:
					antiAlias = AntiAliasSamplesMedium;
					cameraPost = false;
					break;
				case QualityLevel.High:
					antiAlias = AntiAliasSamplesHigh;
					cameraPost = true;
					break;
				default:
                    antiAlias = AntiAliasSamplesHigh;
                    cameraPost = false;
					break;
				}
				lodPenguinQualityLevel = graphicsLevel;
			}
			else
			{
				graphicsLevel = QualityLevel.High;
				lodPenguinQualityLevel = graphicsLevel;
				antiAlias = AntiAliasSamplesHigh;
				cameraPost = true;
			}
		}

		public void SetGraphicsLevel(string qualityName)
		{
			QualityLevel value;
			if (QualityLevelByConditionalTier.TryGetValue(qualityName, out value))
			{
				setQualityLevel(qualityName);
				switch (value)
				{
				case QualityLevel.High:
					SetAntialiasing(AntiAliasSamplesHigh);
					SetCameraPostEffects(true);
					break;
				case QualityLevel.Medium:
					SetAntialiasing(AntiAliasSamplesMedium);
					SetCameraPostEffects(false);
					break;
				case QualityLevel.Low:
                        SetAntialiasing(AntiAliasSamplesLow);
                        SetCameraPostEffects(false);
					break;
				}
				SetLodPenguinQualityLevel(value);
				GraphicsLevel.SetValue(value);
			}
			else
			{
				setQualityLevel("Default");
                SetAntialiasing(AntiAliasSamplesHigh);
                SetCameraPostEffects(true);
			}
		}

		private void setQualityLevel(string qualityName)
		{
			int num = 0;
			while (true)
			{
				if (num < QualitySettings.names.Length)
				{
					if (QualitySettings.names[num].Equals(qualityName, StringComparison.OrdinalIgnoreCase))
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			QualitySettings.SetQualityLevel(num, true);
			ApplyRenderScale(PlayerPrefs.GetFloat(RenderScalePlayerPrefsKey, DefaultRenderScale), false);
			QualityConditionalTier.SetValue(qualityName);
		}

		public void SetAntialiasing(int samples)
		{
			int requiredAntiAliasing = 8;
#if UNITY_ANDROID || UNITY_IOS
			requiredAntiAliasing = 4;
#endif
			int antiAliasing = (samples <= 0) ? 0 : ((samples == AntiAliasSamplesLow) ? AntiAliasSamplesLow : requiredAntiAliasing);
			QualitySettings.antiAliasing = antiAliasing;
			UniversalRenderPipelineAsset renderPipeline = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
			if (renderPipeline != null)
			{
				renderPipeline.msaaSampleCount = (antiAliasing > 0) ? antiAliasing : 1;
			}
			AntiAliasLevel.SetValue(antiAliasing);
		}

		public void SetCameraPostEffects(bool value)
		{
			CameraPostEnabled.SetValue(value);
		}

		public void SetLodPenguinQualityLevel(QualityLevel level)
		{
			LodPenguinQualityLevel.SetValue(level);
		}

		public void SetFullscreen(bool value)
		{
			Screen.fullScreen = value;
			if (!value)
			{
				TryFitWindowedScreen(Screen.width, Screen.height);
			}
		}

		public bool TryFitWindowedScreen(int width, int height)
		{
			int num = (int)((float)Screen.currentResolution.width * 0.99f);
			int num2 = (int)((float)Screen.currentResolution.height * 0.94f);
			int width2 = Math.Min(width, num);
			int height2 = Math.Min(height, num2);
			if (width > num || height > num2)
			{
				DisplayResolutionManager.SetRawResolution(width2, height2, false);
				return false;
			}
			return true;
		}

		public void SetAspectRatio(float ratio)
		{
			AspectRatio = ratio;
			AspectAlias = GetAspectAliasFromRatio(ratio);
		}

        public string GetAspectAliasFromRatio(float ratio)
        {
            if (Mathf.Abs(ratio - 3.55555558f) < 0.01f)
            {
                return "32:9";
            }
            if (Mathf.Abs(ratio - 2.4f) < 0.01f)
            {
                return "12:5";
            }
            if (Mathf.Abs(ratio - 2.33333325f) < 0.01f)
            {
                return "21:9";
            }
            if (Mathf.Abs(ratio - 1.77777779f) < 0.01f)
            {
                return "16:9";
            }
            if (Mathf.Abs(ratio - 1.66666663f) < 0.01f)
            {
                return "5:3";
            }
            if (Mathf.Abs(ratio - 1.6f) < 0.01f)
            {
                return "16:10";
            }
            if (Mathf.Abs(ratio - 1.55555558f) < 0.01f)
            {
                return "14:9";
            }
            if (Mathf.Abs(ratio - 1.54789555f) < 0.01f)
            {
                return "16:10.4";
            }
            if (Mathf.Abs(ratio - 1.5f) < 0.01f)
            {
                return "3:2";
            }
            if (Mathf.Abs(ratio - 1.33333337f) < 0.01f)
            {
                return "4:3";
            }
            if (Mathf.Abs(ratio - 1.25f) < 0.01f)
            {
                return "5:4";
            }
            return string.Empty;
        }
    }
}
