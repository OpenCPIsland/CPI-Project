using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_EnvironmentManager : MonoBehaviour
	{
		private mg_jr_ScrollingSpeed m_speed;

		private Dictionary<mg_jr_EnvironmentID, mg_jr_Environment> m_environments = new Dictionary<mg_jr_EnvironmentID, mg_jr_Environment>();

		public mg_jr_Environment CurrentEnvironment
		{
			get;
			private set;
		}

		private void Start()
		{
			mg_jr_GameLogic gameLogic = MinigameManager.GetActive<mg_JetpackReboot>().GameLogic;
			m_speed = gameLogic.ScrollingSpeed;
			mg_jr_Environment mg_jr_Environment = AddNewEnvironment(new mg_jr_EnvironmentID(EnvironmentType.FOREST, EnvironmentVariant.DEFAULT));
			CurrentEnvironment = m_environments[mg_jr_Environment.Id];
			CurrentEnvironment.gameObject.SetActive(true);
			AddNewEnvironment(new mg_jr_EnvironmentID(EnvironmentType.FOREST, EnvironmentVariant.NIGHT));
			AddNewEnvironment(new mg_jr_EnvironmentID(EnvironmentType.CAVE, EnvironmentVariant.DEFAULT));
			AddNewEnvironment(new mg_jr_EnvironmentID(EnvironmentType.TOWN, EnvironmentVariant.DEFAULT));
			AddNewEnvironment(new mg_jr_EnvironmentID(EnvironmentType.TOWN, EnvironmentVariant.NIGHT));
			AddNewEnvironment(new mg_jr_EnvironmentID(EnvironmentType.WATER, EnvironmentVariant.DEFAULT));
			AddNewEnvironment(new mg_jr_EnvironmentID(EnvironmentType.WATER, EnvironmentVariant.NIGHT));
		}

		private mg_jr_Environment AddNewEnvironment(mg_jr_EnvironmentID _id)
		{
			mg_jr_Environment mg_jr_Environment = mg_jr_Environment.CreateEnvironment(_id.Type, _id.Variant, m_speed);
			mg_jr_Environment.transform.parent = base.transform;
			mg_jr_Environment.gameObject.SetActive(false);
			m_environments.Add(_id, mg_jr_Environment);
			return mg_jr_Environment;
		}

		public void ChangeEnvironment()
		{
			mg_jr_EnvironmentID id = CurrentEnvironment.Id;
			Assert.NotNull(id, "Environment id should never be null");
			List<mg_jr_EnvironmentID> list = new List<mg_jr_EnvironmentID>(m_environments.Keys);
			list.Remove(new mg_jr_EnvironmentID(id.Type, EnvironmentVariant.DEFAULT));
			list.Remove(new mg_jr_EnvironmentID(id.Type, EnvironmentVariant.NIGHT));
			mg_jr_EnvironmentID environmentId = list[Random.Range(0, list.Count)];
			ChangeEnvironment(environmentId);
		}

		public void ChangeEnvironment(mg_jr_EnvironmentID _environmentId)
		{
			Assert.NotNull(_environmentId, "Environment id can't be null");
			if (!(CurrentEnvironment != null) || !(_environmentId == CurrentEnvironment.Id))
			{
				if (m_environments.ContainsKey(_environmentId))
				{
					CurrentEnvironment.gameObject.SetActive(false);
					CurrentEnvironment = m_environments[_environmentId];
					CurrentEnvironment.gameObject.SetActive(true);
				}
				else
				{
					DisneyMobile.CoreUnitySystems.Logger.LogWarning(this, "No environment found for id, environment remaina the same");
				}
			}
		}
	}
}
