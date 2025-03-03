using Disney.Manimal.Common.Util;
using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	[Serializable]
	public class DateUnityWrapper : ISerializationCallbackReceiver
	{
		[SerializeField]
		private int day;

		[SerializeField]
		private int month;

		[SerializeField]
		private int year;

		public long TimeStampInMilliseconds;

		public DateTime Date
		{
			get
			{
				return TimeStampInMilliseconds.MsToDateTime();
			}
			set
			{
				TimeStampInMilliseconds = value.GetTimeInMilliseconds();
			}
		}

		public void OnAfterDeserialize()
		{
			TimeStampInMilliseconds = new DateTime(year, month, day).GetTimeInMilliseconds();
		}

		public void OnBeforeSerialize()
		{
			DateTime dateTime = TimeStampInMilliseconds.MsToDateTime();
			day = dateTime.Day;
			month = dateTime.Month;
			year = dateTime.Year;
		}

		public static implicit operator DateTime(DateUnityWrapper dateUnityWrapper)
		{
			return dateUnityWrapper.TimeStampInMilliseconds.MsToDateTime();
		}

		public static implicit operator DateUnityWrapper(DateTime dateTime)
		{
			DateUnityWrapper dateUnityWrapper = new DateUnityWrapper();
			dateUnityWrapper.TimeStampInMilliseconds = dateTime.GetTimeInMilliseconds();
			return dateUnityWrapper;
		}
	}
}
