using UnityEngine;
using UnityEngine.UI;

public class mg_pr_UIEscapedCount : MonoBehaviour
{
	private Text m_label;

	public int m_escaped;

	private void Start()
	{
		m_label = GetComponent<Text>();
	}

	public void Update()
	{
		m_label.text = string.Concat(m_escaped);
	}
}
