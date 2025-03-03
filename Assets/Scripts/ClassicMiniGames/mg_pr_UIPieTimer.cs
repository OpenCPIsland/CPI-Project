using UnityEngine;
using UnityEngine.UI;

public class mg_pr_UIPieTimer : MonoBehaviour
{
	private GameObject m_circle1;

	private GameObject m_circle2;

	private GameObject m_circle3;

	private GameObject m_circle4;

	private GameObject m_circle5;

	private GameObject m_circle6;

	private float m_currentTime;

	private float m_segmentSize;

	public float m_gameTime;

	private void Awake()
	{
		m_circle1 = base.transform.Find("mg_pr_pf_Circle1").gameObject;
		m_circle2 = base.transform.Find("mg_pr_pf_Circle2").gameObject;
		m_circle3 = base.transform.Find("mg_pr_pf_Circle3").gameObject;
		m_circle4 = base.transform.Find("mg_pr_pf_Circle4").gameObject;
		m_circle5 = base.transform.Find("mg_pr_pf_Circle5").gameObject;
		m_circle6 = base.transform.Find("mg_pr_pf_Circle6").gameObject;
	}

	public void StartTimer()
	{
		m_segmentSize = 360f / m_gameTime;
		m_circle1.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
		m_circle2.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		m_circle3.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
		m_circle4.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
		m_circle1.SetActive(true);
		m_circle2.SetActive(true);
		m_circle3.SetActive(true);
		m_circle4.SetActive(false);
		m_circle5.SetActive(true);
		m_currentTime = m_gameTime;
		InvokeRepeating("AnimatePie", 1f, 1f);
	}

	private void AnimatePie()
	{
		m_currentTime -= 1f;
		if (m_currentTime > 0f)
		{
			m_circle1.transform.Rotate(0f, 0f, 0f - m_segmentSize);
			m_circle2.transform.Rotate(0f, 0f, 0f - m_segmentSize);
		}
		if (m_currentTime <= m_gameTime * 0.5f)
		{
			m_circle2.SetActive(false);
			m_circle3.SetActive(false);
			m_circle4.SetActive(true);
			m_circle5.SetActive(false);
		}
		if (m_currentTime <= 0f)
		{
			m_currentTime = 0f;
			m_circle1.SetActive(false);
			m_circle4.SetActive(false);
			CancelInvoke();
		}
	}

	public void StopTimer()
	{
		CancelInvoke();
	}

	private void Update()
	{
		if (m_currentTime > 30f)
		{
			m_circle3.GetComponent<Image>().color = new Color(1f, 0.98f, 0.416f);
			m_circle4.GetComponent<Image>().color = new Color(1f, 0.98f, 0.416f);
			m_circle6.GetComponent<Image>().color = new Color(1f, 0.98f, 0.416f);
		}
		else if (m_currentTime > 20f && m_currentTime <= 30f)
		{
			m_circle3.GetComponent<Image>().color = new Color(0.65f, 0.643f, 0.019f);
			m_circle4.GetComponent<Image>().color = new Color(0.65f, 0.643f, 0.019f);
			m_circle6.GetComponent<Image>().color = new Color(0.65f, 0.643f, 0.019f);
		}
		else if (m_currentTime > 10f && m_currentTime <= 20f)
		{
			m_circle3.GetComponent<Image>().color = new Color(0.901f, 0.411f, 0.16f);
			m_circle4.GetComponent<Image>().color = new Color(0.901f, 0.411f, 0.16f);
			m_circle6.GetComponent<Image>().color = new Color(0.901f, 0.411f, 0.16f);
		}
		else if (m_currentTime <= 10f)
		{
			m_circle3.GetComponent<Image>().color = new Color(1f, 0f, 0f);
			m_circle4.GetComponent<Image>().color = new Color(1f, 0f, 0f);
			m_circle6.GetComponent<Image>().color = new Color(1f, 0f, 0f);
		}
	}
}
