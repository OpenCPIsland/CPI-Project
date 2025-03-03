using ClubPenguin.Core;
using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.Manimal.Common.Util;
using Disney.MobileNetwork;
using Fabric;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Halloween
{
	public class ProgressOverTime : MonoBehaviour
	{
		public GameObject scenePrefab;

		public Image FillMeter;

		public GameObject FillMeterComplete;

		public Text ProgressText;

		public string InProgressToken;

		public string ProgressDoneToken;

		public float MinValue = 0f;

		public float MaxValue = 1.667f;

		public float FinalValue = 1.79f;

		public ScheduledEventDateDefinitionKey DateDefinitionKey;

		public int UpdateIntervalMinutes;

		[Tooltip("Range to randomize updates, will be between -value and value")]
		public int UpdateRandomSeconds = 15;

		[Tooltip("Particles to be shown when meter increases. Ignored if blank")]
		public GameObject IncreaseEffect;

		public Vector3 EffectOffset = Vector3.zero;

		[Tooltip("Audio to play when meter increases. Ignored if blank")]
		public string AudioIncreaseEffect;

		private ScheduledEventDateDefinition DateDefinition;

		private INetworkServicesManager network;

		private Localizer localizer;

		private ParticleSystem partSys;

		private void Start()
		{
			network = Service.Get<INetworkServicesManager>();
			localizer = Service.Get<Localizer>();
			DateDefinition = Service.Get<IGameData>().Get<Dictionary<int, ScheduledEventDateDefinition>>()[DateDefinition.Id];
			CheckWithinDateRange();
		}

		private void OnDestroy()
		{
			CancelInvoke("updateProgressRepeating");
		}

		private void CheckWithinDateRange()
		{
			if (Service.Get<ContentSchedulerService>().IsDuringScheduleEventDates(DateDefinition))
			{
				if (ProgressText != null)
				{
					ProgressText.text = localizer.GetTokenTranslation(InProgressToken);
				}
				if (IncreaseEffect != null)
				{
					Vector3 localPosition = Vector3.zero;
					if (EffectOffset != Vector3.zero)
					{
						localPosition = EffectOffset;
					}
					GameObject gameObject = UnityEngine.Object.Instantiate(IncreaseEffect, base.gameObject.transform);
					gameObject.transform.localPosition = localPosition;
					if (gameObject != null)
					{
						partSys = gameObject.GetComponent<ParticleSystem>();
					}
				}
				if (UpdateIntervalMinutes > 0)
				{
					updateProgressRepeating();
				}
				else
				{
					updateProgress();
				}
			}
			else if (Service.Get<ContentSchedulerService>().IsAfterScheduleEventDates(DateDefinition))
			{
				if (ProgressText != null)
				{
					ProgressText.text = localizer.GetTokenTranslation(ProgressDoneToken);
				}
				if (FillMeterComplete != null)
				{
					FillMeterComplete.SetActive(true);
				}
				updateProgress(FinalValue);
				CancelInvoke("updateProgressRepeating");
			}
		}

		private void updateProgressRepeating()
		{
			updateProgress();
			Invoke("updateProgressRepeating", (float)UpdateIntervalMinutes * 60f + UnityEngine.Random.Range((float)(-UpdateRandomSeconds), (float)UpdateRandomSeconds));
		}

		private void updateProgress(float manualPos = 0f)
		{
			if (scenePrefab != null)
			{
				Vector3 localPosition = scenePrefab.transform.localPosition;
				if (manualPos < 0f || manualPos > 0f)
				{
					localPosition.y = manualPos;
				}
				else
				{
					localPosition.y = CalculateProgress();
				}
				scenePrefab.transform.localPosition = localPosition;
				if (partSys != null)
				{
					partSys.Play();
				}
				playAudioEvent(AudioIncreaseEffect, base.gameObject);
			}
			if (FillMeter != null)
			{
				FillMeter.fillAmount = CalculateProgress();
			}
		}

		public float CalculateProgress()
		{
			DateTime dateTime = Service.Get<ContentSchedulerService>().ScheduledEventDate();
			DateTime serverTime = getServerTime();
			DateTime d = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, serverTime.Hour, serverTime.Minute, serverTime.Second);
			DateTime date = DateDefinition.Dates.StartDate.Date;
			DateTime date2 = DateDefinition.Dates.EndDate.Date;
			double num;
			if (d.Day > date2.Day)
			{
				num = FinalValue;
			}
			else
			{
				double totalMinutes = (date2 - date).TotalMinutes;
				double num2 = (UpdateIntervalMinutes <= 0) ? totalMinutes : (totalMinutes / (double)UpdateIntervalMinutes);
				double totalMinutes2 = (d - date).TotalMinutes;
				float num3 = Mathf.Abs(MaxValue - MinValue);
				double num4 = (double)num3 / num2;
				double num5 = (UpdateIntervalMinutes <= 0) ? totalMinutes2 : (totalMinutes2 / (double)UpdateIntervalMinutes);
				num = num5 * num4;
			}
			return (float)num;
		}

		private DateTime getServerTime()
		{
			if (network != null)
			{
				long gameTimeMilliseconds = network.GameTimeMilliseconds;
				if (gameTimeMilliseconds > 0)
				{
					return gameTimeMilliseconds.MsToDateTime();
				}
			}
			return DateTime.MinValue;
		}

		private void playAudioEvent(string audioEventName, GameObject anchorObj = null)
		{
			if (!string.IsNullOrEmpty(audioEventName))
			{
				if (anchorObj != null)
				{
					EventManager.Instance.PostEvent(audioEventName, EventAction.PlaySound, anchorObj);
				}
				else
				{
					EventManager.Instance.PostEvent(audioEventName, EventAction.PlaySound);
				}
			}
		}
	}
}
