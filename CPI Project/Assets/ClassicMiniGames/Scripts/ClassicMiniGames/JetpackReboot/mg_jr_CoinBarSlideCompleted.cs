using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_CoinBarSlideCompleted : MonoBehaviour
	{
		private mg_jr_ResultScreen m_resultScreen = null;

		private void Start()
		{
			m_resultScreen = GetComponentInParent<mg_jr_ResultScreen>();
			m_resultScreen.OnCoinDiplaySlideInComplete();
		}

		private void OnSlideCompleted()
		{
			m_resultScreen.OnCoinDiplaySlideInComplete();
		}
	}
}
