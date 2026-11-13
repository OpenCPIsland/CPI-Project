using System;
using UnityEngine;

namespace DevonLocalization.Core
{
	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public class LocalizationTokenAttribute : PropertyAttribute
	{
		public string InitialText;
	}
}
