using UnityEngine;
using UnityEngine.UI;

public class mg_pr_UITimerCount : MonoBehaviour
{
	public float m_timer;

	private Text m_label;

	private void Start()
	{
		m_label = GetComponent<Text>();
	}

	public void Update()
	{
		m_label.text = (m_timer.ToString("n0") ?? "");
	}
}
