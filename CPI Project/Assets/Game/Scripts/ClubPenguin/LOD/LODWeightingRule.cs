using UnityEngine;

namespace ClubPenguin.LOD
{
	public abstract class LODWeightingRule : MonoBehaviour
	{
		protected LODRequest request;

		public float Weighting
		{
			get;
			private set;
		}

		public void Awake()
		{
			request = GetComponent<LODRequest>();
			request.AddWeightingRule(this);
		}

		public virtual void Setup()
		{
		}

		public virtual void Reset()
		{
			Weighting = 0f;
		}

		public virtual void OnDisable()
		{
			Weighting = 0f;
		}

		public virtual void OnDestroy()
		{
			if (request != null)
			{
				request.RemoveWeightingRule(this);
				request = null;
			}
		}

		public float CalculateWeighting()
		{
			Weighting = UpdateWeighting();
			return Weighting;
		}

		protected abstract float UpdateWeighting();
	}
}
