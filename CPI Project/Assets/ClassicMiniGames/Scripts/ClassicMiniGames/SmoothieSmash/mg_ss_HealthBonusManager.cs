using MinigameFramework;
using System.Collections;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_HealthBonusManager : MonoBehaviour
	{
		private mg_ss_GameLogic_Survival m_logic;

		public void Initialize(mg_ss_GameLogic_Survival p_logic)
		{
			m_logic = p_logic;
		}

		public void OnFruitCollision(mg_ss_Item_FruitObject p_fruit)
		{
			int p_health = 2;
			if (p_fruit.CollideState == mg_ss_ECollideState.SMASH)
			{
				p_health = 4;
			}
			m_logic.AddHealth(p_health);
			if (!p_fruit.ChaosItem)
			{
				ShowHeartIcons(p_fruit);
			}
		}

		private void ShowHeartIcons(mg_ss_Item_FruitObject p_fruit)
		{
			StartCoroutine(AddHeartIcon(p_fruit, 0f));
			if (p_fruit.CollideState == mg_ss_ECollideState.SMASH)
			{
				float num = 0.2f;
				for (int i = 1; i < 3; i++)
				{
					StartCoroutine(AddHeartIcon(p_fruit, num));
					num += 0.2f;
				}
			}
		}

		private IEnumerator AddHeartIcon(mg_ss_Item_FruitObject p_fruit, float p_delay)
		{
			yield return new WaitForSeconds(p_delay);
			GameObject heartIcon = m_logic.Minigame.Resources.GetInstancedResource(mg_ss_EResourceList.GAME_HEART_ICON);
			MinigameSpriteHelper.AssignParentTransform(heartIcon, base.transform);
			heartIcon.GetComponent<mg_ss_HealthIcon>().Initialize(p_fruit, base.transform);
		}
	}
}
