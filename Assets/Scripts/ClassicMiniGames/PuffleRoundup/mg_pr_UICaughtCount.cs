using UnityEngine;
using UnityEngine.UI;

namespace PuffleRoundup
{
	public class mg_pr_UICaughtCount : MonoBehaviour
	{
		private Text m_label;

		public int m_caught;

		private void Start()
		{
			m_label = GetComponent<Text>();
		}

		public void Update()
		{
			m_label.text = string.Concat(m_caught);
		}
	}
}
