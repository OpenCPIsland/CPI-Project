using MinigameFramework;
using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public abstract class mg_jr_Boss : MonoBehaviour
	{
		public delegate void OnBossComplete();

		protected mg_JetpackReboot m_miniGame;

		protected mg_jr_Warning m_warning;

		protected virtual void Awake()
		{
			m_miniGame = MinigameManager.GetActive<mg_JetpackReboot>();
			Assert.NotNull(m_miniGame, "mini game not found");
			m_warning = GetComponent<mg_jr_Warning>();
			if (m_warning == null)
			{
				m_warning = base.gameObject.AddComponent<mg_jr_Warning>();
			}
		}

		public abstract void StartBossBattle(OnBossComplete _completionCallback);
	}
}
