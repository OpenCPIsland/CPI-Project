using MinigameFramework;
using UnityEngine;

namespace PuffleRoundup
{
	public class mg_pr_SpawnController : MonoBehaviour
	{
		private GameObject PuffleWhite;

		private GameObject PuffleYellow;

		private GameObject PuffleOrange;

		private GameObject PuffleRed;

		private GameObject PufflePink;

		private GameObject PufflePurple;

		private GameObject PuffleBlue;

		private GameObject PuffleGreen;

		private GameObject PuffleBrown;

		private GameObject PuffleBlack;

		private GameObject Rock1;

		private GameObject Rock2;

		private GameObject Stump;

		private GameObject penBoundsGO;

		private GameObject m_PuffleContainer;

		private GameObject m_ObsticleContainer;

		private Component[] m_puffles;

		private Component[] m_obsticles;

		private mg_PuffleRoundup Minigame;

		private void Awake()
		{
			m_PuffleContainer = MinigameSpriteHelper.CreateContainer("mg_pr_PuffleContainer");
			m_ObsticleContainer = MinigameSpriteHelper.CreateContainer("mg_pr_ObsticleContainer");
			Minigame = MinigameManager.GetActive<mg_PuffleRoundup>();
			penBoundsGO = Minigame.transform.Find("mg_pr_GameContainer/mg_pr_pf_GameBG(Clone)/mg_pr_pf_Pen").gameObject;
			GameObject baseContainer = GetComponent<mg_pr_GameLogic>().BaseContainer;
			MinigameSpriteHelper.AssignParent(m_PuffleContainer, baseContainer);
			MinigameSpriteHelper.AssignParent(m_ObsticleContainer, baseContainer);
		}

		public void SpawnPuffles()
		{
			int i = 0;
			int num = 10;
			bool flag = false;
			int num2 = 0;
			Vector3 vector = default(Vector3);
			for (; i < num; i++)
			{
				flag = false;
				while (!flag)
				{
					vector.x = Random.Range(-5f, 5f);
					vector.y = Random.Range(-4f, 3f);
					flag = !penBoundsGO.GetComponent<SpriteRenderer>().bounds.Contains(vector);
					num2++;
					if (num2 > 100)
					{
						return;
					}
				}
				switch (Random.Range(1, 11))
				{
				case 1:
					PuffleWhite = Minigame.Resources.GetInstancedResource(mg_pr_EResourceList.GAME_ASSET_PUFFLE_WHITE);
					MinigameSpriteHelper.SetSpriteLayer(PuffleWhite, 50);
					MinigameSpriteHelper.AssignParent(PuffleWhite, m_PuffleContainer);
					PuffleWhite.transform.position = vector;
					break;
				case 2:
					PuffleYellow = Minigame.Resources.GetInstancedResource(mg_pr_EResourceList.GAME_ASSET_PUFFLE_YELLOW);
					MinigameSpriteHelper.SetSpriteLayer(PuffleYellow, 50);
					MinigameSpriteHelper.AssignParent(PuffleYellow, m_PuffleContainer);
					PuffleYellow.transform.position = vector;
					break;
				case 3:
					PuffleOrange = Minigame.Resources.GetInstancedResource(mg_pr_EResourceList.GAME_ASSET_PUFFLE_ORANGE);
					MinigameSpriteHelper.SetSpriteLayer(PuffleOrange, 50);
					MinigameSpriteHelper.AssignParent(PuffleOrange, m_PuffleContainer);
					PuffleOrange.transform.position = vector;
					break;
				case 4:
					PuffleRed = Minigame.Resources.GetInstancedResource(mg_pr_EResourceList.GAME_ASSET_PUFFLE_RED);
					MinigameSpriteHelper.SetSpriteLayer(PuffleRed, 50);
					MinigameSpriteHelper.AssignParent(PuffleRed, m_PuffleContainer);
					PuffleRed.transform.position = vector;
					break;
				case 5:
					PufflePink = Minigame.Resources.GetInstancedResource(mg_pr_EResourceList.GAME_ASSET_PUFFLE_PINK);
					MinigameSpriteHelper.SetSpriteLayer(PufflePink, 50);
					MinigameSpriteHelper.AssignParent(PufflePink, m_PuffleContainer);
					PufflePink.transform.position = vector;
					break;
				case 6:
					PufflePurple = Minigame.Resources.GetInstancedResource(mg_pr_EResourceList.GAME_ASSET_PUFFLE_PURPLE);
					MinigameSpriteHelper.SetSpriteLayer(PufflePurple, 50);
					MinigameSpriteHelper.AssignParent(PufflePurple, m_PuffleContainer);
					PufflePurple.transform.position = vector;
					break;
				case 7:
					PuffleBlue = Minigame.Resources.GetInstancedResource(mg_pr_EResourceList.GAME_ASSET_PUFFLE_BLUE);
					MinigameSpriteHelper.SetSpriteLayer(PuffleBlue, 50);
					MinigameSpriteHelper.AssignParent(PuffleBlue, m_PuffleContainer);
					PuffleBlue.transform.position = vector;
					break;
				case 8:
					PuffleGreen = Minigame.Resources.GetInstancedResource(mg_pr_EResourceList.GAME_ASSET_PUFFLE_GREEN);
					MinigameSpriteHelper.SetSpriteLayer(PuffleGreen, 50);
					MinigameSpriteHelper.AssignParent(PuffleGreen, m_PuffleContainer);
					PuffleGreen.transform.position = vector;
					break;
				case 9:
					PuffleBrown = Minigame.Resources.GetInstancedResource(mg_pr_EResourceList.GAME_ASSET_PUFFLE_BROWN);
					MinigameSpriteHelper.SetSpriteLayer(PuffleBrown, 50);
					MinigameSpriteHelper.AssignParent(PuffleBrown, m_PuffleContainer);
					PuffleBrown.transform.position = vector;
					break;
				case 10:
					PuffleBlack = Minigame.Resources.GetInstancedResource(mg_pr_EResourceList.GAME_ASSET_PUFFLE_BLACK);
					MinigameSpriteHelper.SetSpriteLayer(PuffleBlack, 50);
					MinigameSpriteHelper.AssignParent(PuffleBlack, m_PuffleContainer);
					PuffleBlack.transform.position = vector;
					break;
				}
			}
		}

		public void ClearPuffles()
		{
			m_puffles = m_PuffleContainer.GetComponentsInChildren<mg_pr_PuffleController>();
			Component[] puffles = m_puffles;
			for (int i = 0; i < puffles.Length; i++)
			{
				mg_pr_PuffleController mg_pr_PuffleController = (mg_pr_PuffleController)puffles[i];
				Object.Destroy(mg_pr_PuffleController.gameObject);
			}
		}

		public void SpawnObsticles(int numToSpawn)
		{
			int i = 0;
			bool flag = false;
			int num = 0;
			Vector3 vector = default(Vector3);
			for (; i < numToSpawn; i++)
			{
				flag = false;
				while (!flag)
				{
					vector.x = Random.Range(-5f, 5f);
					vector.y = Random.Range(-4f, 3f);
					flag = !penBoundsGO.GetComponent<SpriteRenderer>().bounds.Contains(vector);
					num++;
					if (num > 100)
					{
						return;
					}
				}
				switch (Random.Range(1, 4))
				{
				case 1:
					Rock1 = Minigame.Resources.GetInstancedResource(mg_pr_EResourceList.GAME_ASSET_OBSTICLE_ROCK1);
					MinigameSpriteHelper.SetSpriteLayer(Rock1, 0);
					MinigameSpriteHelper.AssignParent(Rock1, m_ObsticleContainer);
					Rock1.transform.position = vector;
					break;
				case 2:
					Rock2 = Minigame.Resources.GetInstancedResource(mg_pr_EResourceList.GAME_ASSET_OBSTICLE_ROCK2);
					MinigameSpriteHelper.SetSpriteLayer(Rock2, 0);
					MinigameSpriteHelper.AssignParent(Rock2, m_ObsticleContainer);
					Rock2.transform.position = vector;
					break;
				case 3:
					Stump = Minigame.Resources.GetInstancedResource(mg_pr_EResourceList.GAME_ASSET_OBSTICLE_STUMP);
					MinigameSpriteHelper.SetSpriteLayer(Stump, 0);
					MinigameSpriteHelper.AssignParent(Stump, m_ObsticleContainer);
					Stump.transform.position = vector;
					break;
				}
			}
		}

		public void ClearObsticles()
		{
			m_obsticles = m_ObsticleContainer.GetComponentsInChildren<mg_pr_ObsticleClass>();
			Component[] obsticles = m_obsticles;
			for (int i = 0; i < obsticles.Length; i++)
			{
				mg_pr_ObsticleClass mg_pr_ObsticleClass = (mg_pr_ObsticleClass)obsticles[i];
				Object.Destroy(mg_pr_ObsticleClass.gameObject);
			}
		}
	}
}
