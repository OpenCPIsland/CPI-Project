using Disney.Kelowna.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeachBallTargetController : MonoBehaviour
{
	public enum TargetType
	{
		Positive,
		Negative,
		Shielded
	}

	public enum TargetFace
	{
		Low,
		Medium,
		High,
		Negative,
		Shield
	}

	[Serializable]
	public class TargetDefinition
	{
		public TargetType type;

		public TargetFace face;

		public int pointValue;

		public int blockerLevel;
	}

	public List<GameObject> hiddenFloatingScoreTextList = new List<GameObject>();

	private Vector3 ANIMATION_OFFSET = new Vector3(0f, 2f, 0f);

	public float minVisibleTime = 0.25f;

	public float maxVisibleTime = 5f;

	private int score;

	public int goal;

	private int scoreTarget;

	public int goalTarget;

	private float startTime;

	public float totalTime;

	private float roundTime;

	private float exitDelay = 0.4f;

	public TextMesh scoreText;

	public TextMesh goalText;

	public TextMesh timerText;

	public TargetDefinition[] definitions;

	private void OnEnable()
	{
		BeachBallTarget.OnTargetHit += onTargetHit;
	}

	private void OnDisable()
	{
		BeachBallTarget.OnTargetHit -= onTargetHit;
	}

	private void Awake()
	{
		startTime = Time.time;
		initTargets();
		updateScreen();
	}

	private void initTargets()
	{
		BeachBallTarget[] componentsInChildren = GetComponentsInChildren<BeachBallTarget>();
		BeachBallTarget[] array = componentsInChildren;
		foreach (BeachBallTarget beachBallTarget in array)
		{
			CoroutineRunner.Start(hideVisibleTarget(beachBallTarget, 0f), beachBallTarget, "HideBeachBallTarget");
		}
	}

	private void Update()
	{
		updateTimer();
		updateScore();
		updateGoal();
	}

	private void updateTimer()
	{
		roundTime = totalTime - (Time.time - startTime);
		if (timerText != null)
		{
			timerText.text = string.Format("{0:0}:{1:00}", Mathf.Floor(roundTime / 60f), roundTime % 60f);
		}
	}

	private void updateScore()
	{
		if (scoreTarget > score)
		{
			int num = scoreTarget - score;
			int num2 = (int)Mathf.Clamp((float)num * 0.25f, 1f, num);
			score += num2;
			updateScreen();
		}
		else
		{
			checkForGoal();
		}
	}

	private void updateGoal()
	{
		if (goalTarget > goal)
		{
			int num = goalTarget - goal;
			int num2 = (int)Mathf.Clamp((float)num * 0.25f, 1f, num);
			goal += num2;
			updateScreen();
		}
	}

	private void updateScreen()
	{
		if (scoreText != null)
		{
			scoreText.text = score.ToString();
		}
		if (goalText != null)
		{
			goalText.text = goal.ToString();
		}
	}

	private void checkForGoal()
	{
		if (score >= goal)
		{
			goalTarget += goal * 2;
		}
	}

	private void onTargetHit(BeachBallTarget.TargetDefinition definition, BeachBall ball, BeachBallTarget target)
	{
		switch (definition.type)
		{
		case TargetType.Positive:
			scoreTarget += definition.pointValue;
			target.ScorePopUp.InitFloatingScoreText(target.transform, definition.pointValue);
			CoroutineRunner.StopAllForOwner(target);
			CoroutineRunner.Start(hideVisibleTarget(target, exitDelay), target, "HideBeachBallTarget");
			punchScale(target);
			break;
		case TargetType.Negative:
			CoroutineRunner.StopAllForOwner(target);
			CoroutineRunner.Start(hideVisibleTarget(target, exitDelay), target, "HideBeachBallTarget");
			punchScale(target);
			break;
		case TargetType.Shielded:
			if (definition.blockerLevel == 0)
			{
				scoreTarget += definition.pointValue;
				target.ScorePopUp.InitFloatingScoreText(target.transform, definition.pointValue);
				CoroutineRunner.StopAllForOwner(target);
				CoroutineRunner.Start(hideVisibleTarget(target, exitDelay), target, "HideBeachBallTarget");
			}
			break;
		}
	}

	private void punchScale(BeachBallTarget target)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("amount", new Vector3(0.65f, 0.65f, 0.65f));
		hashtable.Add("time", exitDelay);
		iTween.PunchScale(target.gameObject, hashtable);
	}

	private IEnumerator hideVisibleTarget(BeachBallTarget target, float delay)
	{
		yield return new WaitForSeconds(delay);
		target.EnableColliders(false);
		animateOut(target);
	}

	private void animateOut(BeachBallTarget target)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("position", target.OriginPosition - (ANIMATION_OFFSET + new Vector3(0f, target.OriginPosition.y, 0f)));
		hashtable.Add("time", 0.5f);
		hashtable.Add("islocal", true);
		hashtable.Add("easetype", "spring");
		hashtable.Add("oncompletetarget", base.gameObject);
		hashtable.Add("oncomplete", "onAnimatedOut");
		hashtable.Add("oncompleteparams", target);
		iTween.MoveTo(target.gameObject, hashtable);
	}

	private void onAnimatedOut(BeachBallTarget target)
	{
		CoroutineRunner.Start(showHiddenTarget(target, UnityEngine.Random.Range(minVisibleTime, maxVisibleTime)), target, "ShowBeachBallTarget");
	}

	private IEnumerator showHiddenTarget(BeachBallTarget target, float delay)
	{
		yield return new WaitForSeconds(delay);
		target.EnableColliders(true);
		SetRandomTargetType(target);
		animateIn(target);
	}

	private void SetRandomTargetType(BeachBallTarget target)
	{
		int num = UnityEngine.Random.Range(0, definitions.Length);
		TargetDefinition defintion = definitions[num];
		target.SetDefintion(defintion);
	}

	private void animateIn(BeachBallTarget target)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("position", target.OriginPosition);
		hashtable.Add("time", 0.5f);
		hashtable.Add("islocal", true);
		hashtable.Add("easetype", "spring");
		hashtable.Add("oncompletetarget", base.gameObject);
		hashtable.Add("oncomplete", "onAnimatedIn");
		hashtable.Add("oncompleteparams", target);
		iTween.MoveTo(target.gameObject, hashtable);
	}

	private void onAnimatedIn(BeachBallTarget target)
	{
		CoroutineRunner.Start(hideVisibleTarget(target, UnityEngine.Random.Range(minVisibleTime, maxVisibleTime)), target, "HideBeachBallTarget");
	}
}
