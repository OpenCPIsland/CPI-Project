using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Game.PartyGames
{
	public class DanceBattleScoreBar : MonoBehaviour
	{
		public Image Bar;

		public float BounceTime = 0.5f;

		public float BounceAmount = 0.1f;

		public float ScoreFillTime = 0.5f;

		public Color HighlightColor;

		private float currentBarValue = 0f;

		private float currentBounceValue;

		private Color defaultColor;

		private Hashtable bounceUpTweenData;

		private Hashtable bounceDownTweenData;

		private void Awake()
		{
			defaultColor = Bar.color;
			initBounceAnimData();
		}

		private void Update()
		{
			Bar.fillAmount = currentBarValue + currentBounceValue;
		}

		private void initBounceAnimData()
		{
			bounceUpTweenData = iTween.Hash("name", "BounceUp", "from", 0f, "to", BounceAmount, "time", BounceTime * 0.2f, "easetype", iTween.EaseType.easeInQuad, "onupdate", "updateBounce", "onupdatetarget", base.gameObject, "oncomplete", "bounceUpComplete", "oncompletetarget", base.gameObject);
			bounceDownTweenData = iTween.Hash("name", "BounceDown", "from", BounceAmount, "to", 0f, "time", BounceTime * 0.8f, "easetype", iTween.EaseType.easeInQuad, "onupdate", "updateBounce", "onupdatetarget", base.gameObject, "oncomplete", "bounceDownComplete", "oncompletetarget", base.gameObject);
		}

		public void StartBounceAnim()
		{
			currentBarValue = 0f;
			if (!base.gameObject.IsDestroyed() && bounceUpTweenData != null)
			{
				iTween.ValueTo(base.gameObject, bounceUpTweenData);
			}
		}

		public void StopBounceAnim()
		{
			iTween.StopByName(base.gameObject, "BounceUp");
			iTween.StopByName(base.gameObject, "BounceDown");
			currentBounceValue = 0f;
		}

		public void TurnBarOff()
		{
			Bar.color = defaultColor;
			iTween.Stop(base.gameObject);
			currentBarValue = Bar.fillAmount;
			currentBounceValue = 0f;
			Hashtable args = iTween.Hash("name", "StopBar", "from", Bar.fillAmount, "to", 0f, "time", 1f, "onupdate", "updateValue", "onupdatetarget", base.gameObject);
			iTween.ValueTo(base.gameObject, args);
		}

		public void SetBarValue(float value)
		{
			Bar.color = HighlightColor;
			iTween.Stop(base.gameObject);
			Hashtable args = iTween.Hash("name", "ChangeValue", "from", currentBarValue, "to", value, "delay", 1f, "time", ScoreFillTime, "onupdate", "updateValue", "onupdatetarget", base.gameObject, "oncomplete", "barValueComplete", "onCompeltetarget", base.gameObject);
			iTween.ValueTo(base.gameObject, args);
		}

		private void updateBounce(float value)
		{
			currentBounceValue = value;
		}

		private void updateValue(float value)
		{
			currentBarValue = value;
		}

		private void bounceUpComplete()
		{
			iTween.ValueTo(base.gameObject, bounceDownTweenData);
		}

		private void bounceDownComplete()
		{
			iTween.ValueTo(base.gameObject, bounceUpTweenData);
		}

		private void barValueComplete()
		{
			Bar.color = defaultColor;
		}
	}
}
