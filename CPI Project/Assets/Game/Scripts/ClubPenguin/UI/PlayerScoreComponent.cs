using ClubPenguin.Core;
using ClubPenguin.Rewards;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class PlayerScoreComponent : MonoBehaviour
	{
		public Text ScoreText;

		public Text ScoreTextShadow;

		public PrefabContentKey CoinParticlePrefabContentKey;

		public PrefabContentKey XPParticlePrefabContentKey;

		private RectTransform rectTransform;

		private long playerId;

		private GameObject playerScoreObject;

		private PlayerScoreEvents.ParticleType particleType;

		private Color xpTintColor;

		private string score;

		private bool scoreShown = false;

		private void Start()
		{
			rectTransform = (base.transform as RectTransform);
		}

		public void Init(long playerId, GameObject playerScoreObject, string score, PlayerScoreEvents.ParticleType particleType, Color xpTintColor)
		{
			this.playerId = playerId;
			this.playerScoreObject = playerScoreObject;
			this.score = score;
			this.particleType = particleType;
			this.xpTintColor = xpTintColor;
			ScoreText.text = "";
			ScoreTextShadow.text = "";
		}

		public void ShowScore()
		{
			if (!scoreShown)
			{
				scoreShown = true;
				ScoreText.text = score;
				ScoreTextShadow.text = score;
				if (Service.Get<CPDataEntityCollection>().IsLocalPlayer(playerId))
				{
					loadParticleEffect();
				}
				animateText();
			}
		}

		private void animateText()
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("amount", new Vector3(2f, 1f, 2f));
			hashtable.Add("time", 1f);
			hashtable.Add("oncomplete", "onPunchScaleComplete");
			iTween.PunchScale(base.gameObject, hashtable);
		}

		private void onPunchScaleComplete()
		{
			float num = 0.7f;
			Hashtable args = iTween.Hash("from", rectTransform.localPosition.y, "to", rectTransform.localPosition.y + 100f, "time", num, "onupdate", "updatePosition", "oncomplete", "onAnimationComplete", "easetype", iTween.EaseType.spring, "name", "MoveAnim");
			iTween.ValueTo(base.gameObject, args);
			ScoreText.CrossFadeAlpha(0f, num, true);
			ScoreTextShadow.CrossFadeAlpha(0f, num, true);
		}

		private void updatePosition(float value)
		{
			Vector3 localPosition = rectTransform.localPosition;
			localPosition.y = value;
			rectTransform.localPosition = localPosition;
		}

		private void loadParticleEffect()
		{
			switch (particleType)
			{
			case PlayerScoreEvents.ParticleType.Coins:
				Content.LoadAsync(onParticleLoaded, CoinParticlePrefabContentKey);
				break;
			case PlayerScoreEvents.ParticleType.XP:
				Content.LoadAsync(onParticleLoaded, XPParticlePrefabContentKey);
				break;
			}
		}

		private void onParticleLoaded(string path, GameObject prefab)
		{
			if (base.gameObject == null || playerScoreObject == null)
			{
				return;
			}
			GameObject gameObject = Object.Instantiate(prefab);
			gameObject.transform.SetParent(playerScoreObject.transform, false);
			SkinParticleForXPMascot[] componentsInChildren = gameObject.GetComponentsInChildren<SkinParticleForXPMascot>();
			if (componentsInChildren != null)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].SetXPMascotColor(xpTintColor);
				}
			}
		}

		private void onAnimationComplete()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerScoreEvents.RemovePlayerScore(playerId, playerScoreObject));
		}
	}
}
