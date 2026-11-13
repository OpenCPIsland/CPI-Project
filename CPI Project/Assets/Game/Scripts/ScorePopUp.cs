using System.Collections;
using UnityEngine;

public class ScorePopUp : MonoBehaviour
{
	public float timeDelay;

	public float animationDuration;

	public float height;

	public iTween.EaseType easeType = iTween.EaseType.spring;

	private TextMesh[] pointMeshes;

	private Transform parent;

	private void Awake()
	{
		parent = base.transform.parent;
		getReferences();
	}

	private void getReferences()
	{
		pointMeshes = GetComponentsInChildren<TextMesh>();
	}

	public void InitFloatingScoreText(Transform target, int pointValue)
	{
		base.gameObject.SetActive(true);
		base.transform.position = target.position;
		base.transform.rotation = target.rotation;
		base.transform.SetParent(null);
		for (int i = 0; i < pointMeshes.Length; i++)
		{
			pointMeshes[i].text = pointValue.ToString();
		}
		floatScoreText();
	}

	private void floatScoreText()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("position", base.transform.localPosition + new Vector3(0f, height, 0f));
		hashtable.Add("time", animationDuration);
		hashtable.Add("delay", timeDelay);
		hashtable.Add("islocal", true);
		hashtable.Add("easetype", easeType);
		hashtable.Add("oncompletetarget", base.gameObject);
		hashtable.Add("oncomplete", "onFloatingScoreTextComplete");
		hashtable.Add("oncompleteparams", base.gameObject);
		iTween.MoveTo(base.gameObject, hashtable);
	}

	private void onFloatingScoreTextComplete()
	{
		base.transform.SetParent(parent);
		base.gameObject.SetActive(false);
	}
}
