using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.SpecialEvents
{
	public class ScheduledFog : MonoBehaviour
	{
		[Header("-- Fog settings -----")]
		public ScheduledFogData StartFogData;

		public ScheduledFogData TargetFogData;

		public AnimationCurve AnimCurve;

		public float TotalTime = 1f;

		public int UpdateOnFrame = 1;

		public Material MaterialSkyBox;

		public Color StartSkyTint;

		public Color TargetSkyTint;

		private float elapsedTime = 0f;

		private float normalizedTime = 0f;

		private bool isComplete = false;

		private ScheduledFogData startFogData;

		private ScheduledFogData targetFogData;

		private Color startSkyTint;

		private Color targetSkyTint;

		private EventChannel eventChannel;

		private void Awake()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<ScheduledCoreEvents.ShowFog>(onShowFog);
			eventChannel.AddListener<ScheduledCoreEvents.HideFog>(onHideFog);
		}

		private bool onShowFog(ScheduledCoreEvents.ShowFog evt)
		{
			ChangeFog();
			return false;
		}

		private bool onHideFog(ScheduledCoreEvents.HideFog evt)
		{
			RestoreFog();
			return false;
		}

		public void ChangeFog()
		{
			startFogAdjustment(getFogData(), TargetFogData, getTintColor(MaterialSkyBox), TargetSkyTint);
		}

		public void RestoreFog()
		{
			startFogAdjustment(getFogData(), StartFogData, getTintColor(MaterialSkyBox), StartSkyTint);
		}

		private void startFogAdjustment(ScheduledFogData startData, ScheduledFogData endData, Color startSky, Color endSky)
		{
			startFogData = startData;
			targetFogData = endData;
			startSkyTint = startSky;
			targetSkyTint = endSky;
			elapsedTime = 0f;
			normalizedTime = 0f;
			isComplete = false;
		}

		private void Update()
		{
			if (targetFogData == null || !targetFogData.FogEnabled || !(TotalTime > 0f))
			{
				return;
			}
			if (UpdateOnFrame < 1)
			{
				UpdateOnFrame = 1;
			}
			if (Time.frameCount % UpdateOnFrame != 0)
			{
				return;
			}
			if (elapsedTime <= TotalTime)
			{
				normalizedTime = elapsedTime / TotalTime;
				float t = normalizedTime;
				if (AnimCurve != null && AnimCurve.keys.Length > 1)
				{
					t = AnimCurve.Evaluate(normalizedTime);
				}
				Color fogColor = Color.Lerp(startFogData.Color, targetFogData.Color, t);
				float fogDensity = Mathf.Lerp(startFogData.Density, targetFogData.Density, t);
				float fogStartDistance = Mathf.Lerp(startFogData.StartDistance, targetFogData.StartDistance, t);
				float fogEndDistance = Mathf.Lerp(startFogData.EndDistance, targetFogData.EndDistance, t);
				RenderSettings.fogColor = fogColor;
				RenderSettings.fogDensity = fogDensity;
				RenderSettings.fogStartDistance = fogStartDistance;
				RenderSettings.fogEndDistance = fogEndDistance;
				Color value = Color.Lerp(startSkyTint, targetSkyTint, t);
				MaterialSkyBox.SetColor("_TintColor", value);
				elapsedTime += Time.deltaTime * (float)UpdateOnFrame;
			}
			else if (!isComplete)
			{
				RenderSettings.fogColor = targetFogData.Color;
				RenderSettings.fogDensity = targetFogData.Density;
				RenderSettings.fogStartDistance = targetFogData.StartDistance;
				RenderSettings.fogEndDistance = targetFogData.EndDistance;
				isComplete = true;
				MaterialSkyBox.SetColor("_TintColor", targetSkyTint);
			}
		}

		private void OnDestroy()
		{
			setFogData(StartFogData);
			eventChannel.RemoveAllListeners();
		}

		private ScheduledFogData getFogData()
		{
			ScheduledFogData scheduledFogData = new ScheduledFogData();
			scheduledFogData.FogEnabled = RenderSettings.fog;
			scheduledFogData.Color = RenderSettings.fogColor;
			scheduledFogData.Density = RenderSettings.fogDensity;
			scheduledFogData.FogMode = RenderSettings.fogMode;
			scheduledFogData.StartDistance = RenderSettings.fogStartDistance;
			scheduledFogData.EndDistance = RenderSettings.fogEndDistance;
			return scheduledFogData;
		}

		private void setFogData(ScheduledFogData fogData)
		{
			RenderSettings.fog = fogData.FogEnabled;
			RenderSettings.fogColor = fogData.Color;
			RenderSettings.fogDensity = fogData.Density;
			RenderSettings.fogMode = fogData.FogMode;
			RenderSettings.fogStartDistance = fogData.StartDistance;
			RenderSettings.fogEndDistance = fogData.EndDistance;
		}

		private Color getTintColor(Material material)
		{
			return material.GetColor("_TintColor");
		}
	}
}
