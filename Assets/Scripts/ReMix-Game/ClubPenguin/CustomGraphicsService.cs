using ClubPenguin.Configuration;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class CustomGraphicsService
	{
		public int AntiAliasSamplesMedium = 4;

		public int AntiAliasSamplesHigh = 4;

		public readonly Dictionary<string, float> SupportedAspectRatios = new Dictionary<string, float>
		{
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
				"4:3",
				1.33333337f
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
				QualityLevel.Low
			},
			{
				"Mobile_High",
				QualityLevel.Low
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
				QualityLevel.Low
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
			SetAntialiasing(AntiAliasLevel);
			SetCameraPostEffects(CameraPostEnabled);
			SetLodPenguinQualityLevel(LodPenguinQualityLevel);
			SetFullscreen(Screen.fullScreen);
			SetAspectRatio((float)Screen.width / (float)Screen.height);
			if (AspectAlias == string.Empty)
			{
				SetAspectRatio(1.77777779f);
			}
		}

		private void getDefaultGraphicsValues(string qualityName, out QualityLevel graphicsLevel, out QualityLevel lodPenguinQualityLevel, out int antiAlias, out bool cameraPost)
		{
			if (QualityLevelByConditionalTier.TryGetValue(qualityName, out graphicsLevel))
			{
				switch (graphicsLevel)
				{
				case QualityLevel.Low:
					antiAlias = 0;
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
					antiAlias = 0;
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
					SetAntialiasing(0);
					SetCameraPostEffects(false);
					break;
				}
				SetLodPenguinQualityLevel(value);
				GraphicsLevel.SetValue(value);
			}
			else
			{
				setQualityLevel("Default");
				SetAntialiasing(0);
				SetCameraPostEffects(false);
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
			QualityConditionalTier.SetValue(qualityName);
		}

		public void SetAntialiasing(int samples)
		{
			QualitySettings.antiAliasing = samples;
			AntiAliasLevel.SetValue(samples);
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
			if (Mathf.Abs(ratio - 1.33333337f) < 0.01f)
			{
				return "4:3";
			}
			return string.Empty;
		}
	}
}
