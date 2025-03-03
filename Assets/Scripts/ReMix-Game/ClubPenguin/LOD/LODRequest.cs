using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.LOD
{
	public class LODRequest : MonoBehaviour
	{
		[HideInInspector]
		public LODRequestData Data;

		[SerializeField]
		private GameObject lodGameObjectReference;

		private List<LODWeightingRule> weightingRules = new List<LODWeightingRule>();

		public float TotalWeighting = 0f;

		public GameObject LODGameObject
		{
			get
			{
				return lodGameObjectReference;
			}
			set
			{
				if (lodGameObjectReference != value)
				{
					if (value != null)
					{
						lodGameObjectReference = value;
						Data.OnGameObjectGenerated(lodGameObjectReference);
					}
					else
					{
						reset();
						Data.OnGameObjectRevoked(lodGameObjectReference);
						lodGameObjectReference = value;
					}
				}
			}
		}

		public bool CanBeSpawned
		{
			get
			{
				return !ObjectGenerated && !BeingProcessed && !IsPaused && TotalWeighting >= 0f;
			}
		}

		public bool CanBeRevoked
		{
			get
			{
				return ObjectGenerated && !BeingProcessed;
			}
		}

		public bool ObjectGenerated
		{
			get
			{
				return lodGameObjectReference != null;
			}
		}

		public bool BeingProcessed
		{
			get;
			set;
		}

		public bool IsPaused
		{
			get;
			set;
		}

		public void AddWeightingRule(LODWeightingRule rule)
		{
			weightingRules.Add(rule);
		}

		public void RemoveWeightingRule(LODWeightingRule rule)
		{
			weightingRules.Remove(rule);
		}

		public void CalculateWeighting()
		{
			TotalWeighting = 0f;
			int count = weightingRules.Count;
			for (int i = 0; i < count; i++)
			{
				TotalWeighting += weightingRules[i].CalculateWeighting();
			}
		}

		public void Setup()
		{
			int count = weightingRules.Count;
			for (int i = 0; i < count; i++)
			{
				weightingRules[i].Setup();
			}
		}

		public void OnDisable()
		{
			LODGameObject = null;
			BeingProcessed = false;
			IsPaused = false;
			TotalWeighting = 0f;
		}

		private void reset()
		{
			int count = weightingRules.Count;
			for (int i = 0; i < count; i++)
			{
				weightingRules[i].Reset();
			}
		}
	}
}
