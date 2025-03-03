using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ClubPenguin.Configuration
{
	public abstract class ConditionalTier<T> : ScriptableObject
	{
		public ConditionDefinition Condition;

		public string AnalyticsName = "";

		[FormerlySerializedAs("Value")]
		public T StaticValue;

		public virtual T DynamicValue
		{
			get
			{
				return StaticValue;
			}
			internal set
			{
				StaticValue = value;
			}
		}

		public event Action<T> EDynamicValueChanged;

		protected void DispatchDynamicChanged()
		{
			if (this.EDynamicValueChanged != null)
			{
				this.EDynamicValueChanged(DynamicValue);
			}
		}

		public override string ToString()
		{
			return DynamicValue.ToString();
		}
	}
}
