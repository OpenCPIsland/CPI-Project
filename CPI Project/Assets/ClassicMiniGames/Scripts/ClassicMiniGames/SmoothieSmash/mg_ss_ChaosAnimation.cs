using MinigameFramework;
using System.Collections.Generic;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_ChaosAnimation
	{
		private mg_ss_PlayerObject m_playerObject;

		private List<mg_ss_GoldenAppleRotationObject> m_apples;

		public void Initialize(mg_ss_PlayerObject p_playerObject)
		{
			m_apples = new List<mg_ss_GoldenAppleRotationObject>();
			m_playerObject = p_playerObject;
		}

		public void Reset()
		{
			foreach (mg_ss_GoldenAppleRotationObject apple in m_apples)
			{
				Object.Destroy(apple.gameObject);
			}
			m_apples.Clear();
		}

		public void FlyApplesFly(mg_SmoothieSmash p_minigame)
		{
			Camera mainCamera = MinigameManager.GetActive().MainCamera;
			Vector2 start = new Vector2(0f - mainCamera.aspect * mainCamera.orthographicSize, mainCamera.orthographicSize);
			start.x /= p_minigame.transform.lossyScale.x;
			start.x -= -0.714f;
			start.y /= p_minigame.transform.lossyScale.y;
			start.y -= 1.16f;
			mg_ss_GoldenApple_FlyInfo p_flyInfo = default(mg_ss_GoldenApple_FlyInfo);
			p_flyInfo.Start = start;
			p_flyInfo.Target = m_playerObject.transform.position;
			p_flyInfo.Target.y -= 0.2f * p_minigame.transform.lossyScale.y;
			p_flyInfo.TotalFlyTime = 1f;
			p_flyInfo.RemainingFlyTime = 1f;
			mg_ss_GoldenApple_RotateInfo p_rotateInfo = default(mg_ss_GoldenApple_RotateInfo);
			p_rotateInfo.StartingAngle = 4.712389f;
			p_rotateInfo.CurrentAngle = p_rotateInfo.StartingAngle;
			p_rotateInfo.RotateTime = 1f;
			p_rotateInfo.MaxTurns = 1f;
			p_rotateInfo.Radius = 1.5f;
			p_rotateInfo.Offset = 0.2f * p_minigame.transform.lossyScale.y;
			float num = 0.2f;
			for (int i = 0; i < 5; i++)
			{
				GameObject instancedResource = p_minigame.Resources.GetInstancedResource(mg_ss_EResourceList.GAME_GOLDEN_APPLE_ROTATE);
				MinigameSpriteHelper.AssignParentTransform(instancedResource, p_minigame.transform);
				mg_ss_GoldenAppleRotationObject component = instancedResource.GetComponent<mg_ss_GoldenAppleRotationObject>();
				component.Fly(p_flyInfo);
				component.RotateAround(p_rotateInfo);
				component.Delay((float)i * num);
				m_apples.Add(component);
			}
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			Vector2 p_target = m_playerObject.transform.position;
			p_target.y += 1.5f;
			foreach (mg_ss_GoldenAppleRotationObject apple in m_apples)
			{
				apple.MinigameUpdate(p_deltaTime, p_target);
			}
		}
	}
}
