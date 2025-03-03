using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	[Serializable]
	public class TimeSpanUnityWrapper : ISerializationCallbackReceiver
	{
		[SerializeField]
		private int days;

		[SerializeField]
		private int hours;

		[SerializeField]
		private int minutes;

		[SerializeField]
		private int seconds;

		public TimeSpan TimeSpan = default(TimeSpan);

		public void OnAfterDeserialize()
		{
			TimeSpan = new TimeSpan(days, hours, minutes, seconds);
		}

		public void OnBeforeSerialize()
		{
			days = TimeSpan.Days;
			hours = TimeSpan.Hours;
			minutes = TimeSpan.Minutes;
			seconds = TimeSpan.Seconds;
		}
	}
}
