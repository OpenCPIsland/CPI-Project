using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Performance
{
	public abstract class PerformanceResponder : MonoBehaviour
	{
		private Transform _transformRef;

		public float DetailLevel
		{
			get;
			protected set;
		}

		public Transform TransformRef
		{
			get
			{
				if (_transformRef == null)
				{
					_transformRef = base.transform;
				}
				return _transformRef;
			}
		}

		public abstract PerformanceResponderType GetPerformanceResponderType();

		protected virtual void Awake()
		{
			float normalizedDetailLevel = Service.Get<PerformanceManagerService>().AddResponder(this);
			SetNormalizedDetailLevel(normalizedDetailLevel);
		}

		protected virtual void OnDestroy()
		{
			Service.Get<PerformanceManagerService>().RemoveResponder(this);
		}

		public void SetNormalizedDetailLevel(float normalizedDetailLevel)
		{
			float num = Mathf.Clamp(normalizedDetailLevel, 0f, 1f);
			if (num != DetailLevel)
			{
				DetailLevel = num;
				onDetailLevelChanged(num);
			}
		}

		protected abstract void onDetailLevelChanged(float newDetailLevel);
	}
}
