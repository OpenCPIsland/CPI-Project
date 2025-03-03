using MinigameFramework;
using UnityEngine;

namespace PuffleRoundup
{
	public class mg_pr_GameLogic : MonoBehaviour
	{
		private const int MAX_PUFFLES = 10;

		private GameObject m_background;

		private mg_pr_UICaughtCount m_caughtGo;

		private mg_pr_UIEscapedCount m_escapedGo;

		public int caught;

		public int escaped;

		public GameObject BaseContainer
		{
			get;
			private set;
		}

		protected void Awake()
		{
			BaseContainer = MinigameSpriteHelper.CreateContainer("mg_pr_GameContainer");
			mg_PuffleRoundup active = MinigameManager.GetActive<mg_PuffleRoundup>();
			m_background = active.Resources.GetInstancedResource(mg_pr_EResourceList.GAME_ASSET_BACKGROUND);
			MinigameSpriteHelper.SetSpriteLayer(m_background, -100);
			Vector3 localScale = m_background.transform.localScale;
			MinigameSpriteHelper.FitSpriteToScreen(active.MainCamera, m_background, false);
			MinigameSpriteHelper.AssignParent(m_background, BaseContainer);
			BaseContainer.transform.localScale = m_background.transform.localScale;
			m_background.transform.localScale = localScale;
			BaseContainer.transform.parent = active.transform;
			m_caughtGo = GetComponentInChildren<mg_pr_UICaughtCount>();
			m_escapedGo = GetComponentInChildren<mg_pr_UIEscapedCount>();
		}

		private void Update()
		{
			m_caughtGo.m_caught = caught;
			m_escapedGo.m_escaped = escaped;
			if (escaped >= 10)
			{
				SendMessage("CompleteRound");
			}
			else if (caught > 0 && caught + escaped >= 10)
			{
				SendMessage("CompleteRound");
			}
		}
	}
}
